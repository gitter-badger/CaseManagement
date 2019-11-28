﻿using CaseManagement.CMMN.Domains;
using CaseManagement.Workflow.Domains;
using CaseManagement.Workflow.Domains.Process;
using CaseManagement.Workflow.Engine;
using CaseManagement.Workflow.Infrastructure;
using CaseManagement.Workflow.Infrastructure.Services;
using CaseManagement.Workflow.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CaseManagement.CMMN.CaseInstance.Processors
{
    public class CMMNPlanItemProcessor : BaseProcessFlowElementProcessor
    {
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IFormQueryRepository _formQueryRepository;
        
        public CMMNPlanItemProcessor(IBackgroundTaskQueue backgroundTaskQueue, IFormQueryRepository formQueryRepository)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _formQueryRepository = formQueryRepository;
        }

        public override Type ProcessFlowElementType => typeof(CMMNPlanItem);

        protected override Task<bool> HandleProcessFlowInstance(ProcessFlowInstanceElement pfe, ProcessFlowInstanceExecutionContext context)
        {
            var planItem = (CMMNPlanItem)pfe;
            var processTask = planItem.PlanItemDefinition as CMMNProcessTask;
            if (processTask != null)
            {
                return HandleTask(planItem, processTask, context, HandleProcessTask);
            }

            var humanTask = planItem.PlanItemDefinition as CMMNHumanTask;
            if (humanTask != null)
            {
                return HandleTask(planItem, humanTask, context, HandleHumanTask);
            }

            return Task.FromResult(true);
        }

        protected async Task<bool> HandleTask<T>(CMMNPlanItem planItem, T task, ProcessFlowInstanceExecutionContext context, Func<CMMNPlanItem, T, ProcessFlowInstanceExecutionContext, Task<bool>> callback) where T : CMMNTask
        {
            if (planItem.ExitCriterions.Any() && planItem.ExitCriterions.Any(s => CheckCriterion(s, context)))
            {
                planItem.Terminate();
                return true;
            }

            if (task.State == CMMNTaskStates.Available)
            {
                if (planItem.EntryCriterions.Any() && !planItem.EntryCriterions.Any(s => CheckCriterion(s, context)))
                {
                    return true;
                }

                if (planItem.PlanItemControl != null)
                {
                    var manualActivationRule = planItem.PlanItemControl as CMMNManualActivationRule;
                    if (manualActivationRule != null)
                    {
                        // Note : at the moment the ContextRef is ignored.
                        if (ExpressionParser.IsValid(manualActivationRule.Expression.Body, context))
                        {
                            planItem.Enable();
                            return false;
                        }
                    }
                }

                planItem.Start();
            }

            if (task.State == CMMNTaskStates.Active)
            {
                return await callback(planItem, task, context);
            }

            return true;
        }

        protected async Task<bool> HandleProcessTask(CMMNPlanItem planItem, CMMNProcessTask processTask, ProcessFlowInstanceExecutionContext context)
        {
            var instance = (WorkflowTaskDelegate)Activator.CreateInstance(Type.GetType(processTask.AssemblyQualifiedName));
            if (processTask.IsBlocking)
            {
                await instance.Handle(context);
            }
            else
            {
                _backgroundTaskQueue.QueueBackgroundWorkItem((token) => instance.Handle(context));
            }

            planItem.Complete();
            return true;
        }

        protected async Task<bool> HandleHumanTask(CMMNPlanItem planItem, CMMNHumanTask humanTask, ProcessFlowInstanceExecutionContext context)
        {
            if (planItem.FormInstance != null && planItem.FormInstance.Status == ProcessFlowInstanceElementFormStatus.Complete)
            {
                planItem.Complete();
                return true;
            }

            var form = await _formQueryRepository.FindFormById(humanTask.FormId);
            planItem.SetFormInstance(form);
            if (humanTask.IsBlocking)
            {
                return false;
            }

            planItem.Complete();
            return true;
        }

        private bool CheckCriterion(CMMNCriterion sCriterion, ProcessFlowInstanceExecutionContext context)
        {
            foreach (var onPart in sCriterion.SEntry.OnParts)
            {
                if (onPart is CMMNPlanItemOnPart)
                {
                    if (!string.IsNullOrWhiteSpace(onPart.SourceRef))
                    {
                        var elt = context.GetPlanItem(onPart.SourceRef);
                        if (elt == null || elt.Events.Last().Transition != onPart.StandardEvent)
                        {
                            return false;
                        }
                    }
                }
            }

            if (sCriterion.SEntry.IfPart != null)
            {
                return ExpressionParser.IsValid(sCriterion.SEntry.IfPart.Condition, context);
            }

            return true;
        }
    }
}