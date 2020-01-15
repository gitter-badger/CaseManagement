﻿using CaseManagement.CMMN.CaseInstance.Exceptions;
using CaseManagement.CMMN.CaseInstance.Processors.Listeners;
using CaseManagement.CMMN.Domains;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static CaseManagement.CMMN.CaseInstance.Processors.Listeners.CMMNCriterionListener;

namespace CaseManagement.CMMN.CaseInstance.Processors
{
    public abstract class BaseCMMNTaskProcessor : IProcessor
    {
        public abstract CaseElementTypes Type { get; }

        public Task<ProcessorParameter> Handle(ProcessorParameter parameter, CancellationToken token)
        {
            var task = new Task<ProcessorParameter>(() =>
            {
                var cancellationTokenSource = new CancellationTokenSource();
                return HandleTask(parameter, cancellationTokenSource).Result;
            }, token, TaskCreationOptions.LongRunning);
            task.Start();
            return task;
        }

        protected abstract Task Run(ProcessorParameter parameter, CancellationToken token);
        protected abstract void Unsubscribe();

        private async Task<ProcessorParameter> HandleTask(ProcessorParameter parameter, CancellationTokenSource tokenSource)
        {
            CMMNPlanItemTransitionListener.EventListener parentTerminateEvtListener = null;
            CMMNPlanItemTransitionListener.EventListener parentReactivateEvtListener = null;
            CMMNPlanItemTransitionListener.EventListener parentSuspendEvtListener = null;
            CMMNPlanItemTransitionListener.EventListener parentResumeEvtListener = null;
            CMMNPlanItemTransitionListener.EventListener resumeEvtListener = null;
            CMMNPlanItemTransitionListener.EventListener suspendEvtListener = null;
            CMMNPlanItemTransitionListener.EventListener terminateEvtListener = null;
            KeyValuePair<Task, CriterionListener>? kvp = null;
            try
            {
                bool isTerminate = false;
                bool continueExecution = true;
                parentTerminateEvtListener = CMMNPlanItemTransitionListener.Listen(parameter, CMMNTransitions.ParentTerminate, () =>
                {
                    isTerminate = true;
                    continueExecution = false;
                    tokenSource.Cancel();
                });
                var planItemDefinition = parameter.CaseDefinition.GetElement(parameter.CaseElementInstance.CaseElementDefinitionId);
                CMMNCriterionListener.ListenEntryCriterias(parameter, tokenSource.Token);
                var isManuallyActivated = CMMNManualActivationListener.Listen(parameter, tokenSource.Token);
                if (!isManuallyActivated)
                {
                    parameter.CaseInstance.MakeTransitionStart(parameter.CaseElementInstance.Id);
                }

                bool isSuspend = false;
                bool isOperationExecuted = false;
                parentReactivateEvtListener = CMMNPlanItemTransitionListener.Listen(parameter, CMMNTransitions.Reactivate, () =>
                {
                    tokenSource = new CancellationTokenSource();
                    isSuspend = false;
                    isOperationExecuted = false;
                });
                parentSuspendEvtListener = CMMNPlanItemTransitionListener.Listen(parameter, CMMNTransitions.ParentSuspend, () =>
                {
                    isSuspend = true;
                    tokenSource.Cancel();
                });
                parentResumeEvtListener = CMMNPlanItemTransitionListener.Listen(parameter, CMMNTransitions.ParentResume, () =>
                {
                    tokenSource = new CancellationTokenSource();
                    isSuspend = false;
                    isOperationExecuted = false;
                });
                resumeEvtListener = CMMNPlanItemTransitionListener.Listen(parameter, CMMNTransitions.Resume, () =>
                {
                    tokenSource = new CancellationTokenSource();
                    isSuspend = false;
                    isOperationExecuted = false;
                });
                suspendEvtListener = CMMNPlanItemTransitionListener.Listen(parameter, CMMNTransitions.Suspend, () =>
                {
                    isSuspend = true;
                    tokenSource.Cancel();
                });
                terminateEvtListener = CMMNPlanItemTransitionListener.Listen(parameter, CMMNTransitions.Terminate, () =>
                {
                    isTerminate = true;
                    continueExecution = false;
                    tokenSource.Cancel();
                });
                try
                {
                    kvp = CMMNCriterionListener.ListenExitCriterias(parameter, tokenSource.Token);
                    if (kvp != null)
                    {
                        kvp.Value.Key.ContinueWith((r) =>
                        {
                            r.Wait();
                            parameter.CaseInstance.MakeTransitionTerminate(parameter.CaseElementInstance.Id);
                        });
                    }
                }
                catch (TerminateCaseInstanceElementException)
                {
                    parameter.CaseInstance.MakeTransitionTerminate(parameter.CaseElementInstance.Id);
                    return parameter;
                }

                while (continueExecution)
                {
                    Thread.Sleep(CMMNConstants.WAIT_INTERVAL_MS);
                    if (isSuspend)
                    {
                        continue;
                    }

                    if (!isOperationExecuted)
                    {
                        isOperationExecuted = true;
                        try
                        {
                            await Run(parameter, tokenSource.Token);
                            tokenSource.Token.ThrowIfCancellationRequested();
                            continueExecution = false;
                        }
                        catch (OperationCanceledException)
                        {
                            if (isTerminate)
                            {
                                continueExecution = false;
                            }

                            Unsubscribe();
                        }
                        catch (Exception)
                        {
                            parameter.CaseInstance.MakeTransitionFault(parameter.CaseElementInstance.Id);
                            // NOTE : If the task doesn't belong to a stage then exit the loop.
                            if (string.IsNullOrWhiteSpace(parameter.CaseElementInstance.ParentId))
                            {
                                continueExecution = false;
                            }
                            else
                            {
                                isSuspend = true;
                            }

                            Unsubscribe();
                        }
                    }
                }

                parentReactivateEvtListener.Unsubscribe();
                parentTerminateEvtListener.Unsubscribe();
                parentSuspendEvtListener.Unsubscribe();
                parentResumeEvtListener.Unsubscribe();
                suspendEvtListener.Unsubscribe();
                terminateEvtListener.Unsubscribe();
                resumeEvtListener.Unsubscribe();
            }
            finally
            {
                if (parentReactivateEvtListener != null)
                {
                    parentReactivateEvtListener.Unsubscribe();
                }

                if (parentTerminateEvtListener != null)
                {
                    parentTerminateEvtListener.Unsubscribe();
                }

                if (parentSuspendEvtListener != null)
                {
                    parentSuspendEvtListener.Unsubscribe();
                }
                
                if (parentResumeEvtListener != null)
                {
                    parentResumeEvtListener.Unsubscribe();
                }

                if (suspendEvtListener != null)
                {
                    suspendEvtListener.Unsubscribe();
                }

                if (terminateEvtListener != null)
                {
                    terminateEvtListener.Unsubscribe();
                }

                if (resumeEvtListener != null)
                {
                    resumeEvtListener.Unsubscribe();
                }

                if (kvp != null)
                {
                    if (kvp.Value.Key.IsCanceled || kvp.Value.Key.IsCompleted || kvp.Value.Key.IsFaulted)
                    {
                        kvp.Value.Key.Dispose();
                    }

                    kvp.Value.Value.Unsubscribe();
                }
            }

            return parameter;
        }
    }
}
