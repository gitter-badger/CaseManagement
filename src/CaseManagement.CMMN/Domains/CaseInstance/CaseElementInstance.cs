﻿using CaseManagement.CMMN.Infrastructures;
using CaseManagement.Workflow.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaseManagement.CMMN.Domains
{
    public class CaseElementInstance : ICloneable
    {
        private object _lock;

        public CaseElementInstance(string id, DateTime createDateTime, string workflowElementDefinitionId, CaseElementTypes workflowElementDefinitionType, int version, string parentId)
        {
            Id = id;
            CreateDateTime = createDateTime;
            CaseElementDefinitionId = workflowElementDefinitionId;
            CaseElementDefinitionType = workflowElementDefinitionType;
            Version = version;
            StateHistories = new List<CaseElementInstanceHistory>();
            TransitionHistories = new List<CaseElementInstanceTransitionHistory>();
            ParentId = parentId;
            _lock = new object();

        }

        public string Id { get; set; }
        public int Version { get; set; }
        public DateTime CreateDateTime { get; set; }
        public string CaseElementDefinitionId { get; set; }
        public CaseElementTypes CaseElementDefinitionType { get; set; }
        public string FormInstanceId { get; set; }
        public string State { get; set; }
        public string ParentId { get; set; }
        public ICollection<CaseElementInstanceHistory> StateHistories { get; set; }
        public ICollection<CaseElementInstanceTransitionHistory> TransitionHistories { get; set; }
        public event EventHandler<string> TransitionApplied;

        public void UpdateState(CMMNTransitions transition, DateTime updateDateTime)
        {
            lock (_lock)
            {
                var state = GetState(transition);
                StateHistories.Add(new CaseElementInstanceHistory(state, updateDateTime));
                TransitionHistories.Add(new CaseElementInstanceTransitionHistory(transition, updateDateTime));
                State = state;
                if (TransitionApplied != null)
                {
                    TransitionApplied(this, state);
                }
            }
        }

        public static CaseElementInstance New(CaseElementDefinition workflowElementDefinition)
        {
            return new CaseElementInstance(Guid.NewGuid().ToString(), DateTime.UtcNow, workflowElementDefinition.Id, workflowElementDefinition.Type, 0, null);
        }

        public bool CanBeTerminated()
        {
            lock (_lock)
            {
                switch (CaseElementDefinitionType)
                {
                    case CaseElementTypes.HumanTask:
                    case CaseElementTypes.ProcessTask:
                    case CaseElementTypes.Stage:
                    case CaseElementTypes.Task:
                        if (State == Enum.GetName(typeof(TaskStates), TaskStates.Terminated) || State == Enum.GetName(typeof(TaskStates), TaskStates.Completed))
                        {
                            return false;
                        }

                        return true;
                    case CaseElementTypes.Milestone:
                    case CaseElementTypes.TimerEventListener:
                        if (State == Enum.GetName(typeof(MilestoneStates), MilestoneStates.Completed) || State == Enum.GetName(typeof(MilestoneStates), MilestoneStates.Terminated))
                        {
                            return false;
                        }

                        return true;
                    case CaseElementTypes.CaseFileItem:
                        if (State == Enum.GetName(typeof(CaseFileItemStates), CaseFileItemStates.Discarded))
                        {
                            return false;
                        }

                        return true;
                }

                return false;
            }
        }

        public bool IsAvailable()
        {
            lock (_lock)
            {
                switch (CaseElementDefinitionType)
                {
                    case CaseElementTypes.HumanTask:
                    case CaseElementTypes.ProcessTask:
                    case CaseElementTypes.Stage:
                    case CaseElementTypes.Task:
                        if (State == Enum.GetName(typeof(TaskStates), TaskStates.Available))
                        {
                            return true;
                        }

                        return false;
                    case CaseElementTypes.Milestone:
                    case CaseElementTypes.TimerEventListener:
                        if (State == Enum.GetName(typeof(MilestoneStates), MilestoneStates.Available))
                        {
                            return true;
                        }

                        return false;
                    case CaseElementTypes.CaseFileItem:
                        if (State == Enum.GetName(typeof(CaseFileItemStates), CaseFileItemStates.Available))
                        {
                            return true;
                        }

                        return false;
                }

                return false;
            }
        }

        public bool IsFail()
        {
            lock(_lock)
            {
                switch (CaseElementDefinitionType)
                {
                    case CaseElementTypes.HumanTask:
                    case CaseElementTypes.ProcessTask:
                    case CaseElementTypes.Stage:
                    case CaseElementTypes.Task:
                        if (State == Enum.GetName(typeof(TaskStates), TaskStates.Failed))
                        {
                            return true;
                        }

                        return false;
                }

                return false;
            }
        }

        public bool IsSuspend()
        {
            lock(_lock)
            {
                switch (CaseElementDefinitionType)
                {
                    case CaseElementTypes.HumanTask:
                    case CaseElementTypes.ProcessTask:
                    case CaseElementTypes.Stage:
                    case CaseElementTypes.Task:
                        if (State == Enum.GetName(typeof(TaskStates), TaskStates.Suspended))
                        {
                            return true;
                        }

                        return false;
                    case CaseElementTypes.Milestone:
                    case CaseElementTypes.TimerEventListener:
                        if (State == Enum.GetName(typeof(MilestoneStates), MilestoneStates.Suspended))
                        {
                            return true;
                        }

                        return false;
                }

                return false;
            }
        }

        public bool IsActive()
        {
            lock(_lock)
            {
                switch (CaseElementDefinitionType)
                {
                    case CaseElementTypes.CaseFileItem:
                        if (State == Enum.GetName(typeof(CaseFileItemStates), CaseFileItemStates.Available))
                        {
                            return true;
                        }

                        return false;
                    case CaseElementTypes.HumanTask:
                    case CaseElementTypes.ProcessTask:
                    case CaseElementTypes.Stage:
                    case CaseElementTypes.Task:
                        if (State == Enum.GetName(typeof(TaskStates), TaskStates.Active))
                        {
                            return true;
                        }

                        return false;
                    case CaseElementTypes.Milestone:
                    case CaseElementTypes.TimerEventListener:
                        if (State == Enum.GetName(typeof(MilestoneStates), MilestoneStates.Available))
                        {
                            return true;
                        }

                        return false;
                }

                return false;
            }
        }

        public bool IsComplete()
        {
            lock(_lock)
            {
                switch (CaseElementDefinitionType)
                {
                    case CaseElementTypes.CaseFileItem:
                        if (State == Enum.GetName(typeof(CaseFileItemStates), CaseFileItemStates.Discarded))
                        {
                            return true;
                        }

                        return false;
                    case CaseElementTypes.HumanTask:
                    case CaseElementTypes.ProcessTask:
                    case CaseElementTypes.Stage:
                    case CaseElementTypes.Task:
                        if (State == Enum.GetName(typeof(TaskStates), TaskStates.Completed) || State == Enum.GetName(typeof(TaskStates), TaskStates.Terminated))
                        {
                            return true;
                        }

                        return false;
                    case CaseElementTypes.Milestone:
                    case CaseElementTypes.TimerEventListener:
                        if (State == Enum.GetName(typeof(MilestoneStates), MilestoneStates.Completed) || State == Enum.GetName(typeof(MilestoneStates), MilestoneStates.Terminated))
                        {
                            return true;
                        }

                        return false;
                }

                return false;
            }
        }

        private string GetState(CMMNTransitions planItemTransition)
        {
            switch(CaseElementDefinitionType)
            {
                case CaseElementTypes.HumanTask:
                case CaseElementTypes.Task:
                case CaseElementTypes.ProcessTask:
                case CaseElementTypes.Stage:
                    TaskStates taskState = TaskStates.Available;
                    switch (planItemTransition)
                    {
                        case CMMNTransitions.Create:
                            if (!string.IsNullOrWhiteSpace(State))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is already initialized" }
                                });
                            }

                            taskState = TaskStates.Available;
                            break;
                        case CMMNTransitions.Enable:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Available))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not available" }
                                });
                            }

                            taskState = TaskStates.Enabled;
                            break;
                        case CMMNTransitions.Disable:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Enabled))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not enabled" }
                                });
                            }

                            taskState = TaskStates.Disabled;
                            break;
                        case CMMNTransitions.Reenable:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Disabled))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not disabled" }
                                });
                            }

                            taskState = TaskStates.Enabled;
                            break;
                        case CMMNTransitions.Fault:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Active))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not active" }
                                });
                            }

                            taskState = TaskStates.Failed;
                            break;
                        case CMMNTransitions.Reactivate:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Failed))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not failed" }
                                });
                            }

                            taskState = TaskStates.Active;
                            break;
                        case CMMNTransitions.ManualStart:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Enabled))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not enabled" }
                                });
                            }

                            taskState = TaskStates.Active;
                            break;
                        case CMMNTransitions.Start:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Available))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not available" }
                                });
                            }

                            taskState = TaskStates.Active;
                            break;
                        case CMMNTransitions.Terminate:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Active))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not active" }
                                });
                            }

                            taskState = TaskStates.Terminated;
                            break;
                        case CMMNTransitions.Suspend:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Active))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not active" }
                                });
                            }

                            taskState = TaskStates.Suspended;
                            break;
                        case CMMNTransitions.Resume:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Suspended))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not suspended" }
                                });
                            }

                            taskState = TaskStates.Active;
                            break;
                        case CMMNTransitions.Complete:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Active))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not active" }
                                });
                            }

                            taskState = TaskStates.Completed;
                            break;
                        case CMMNTransitions.ParentTerminate:
                            taskState = TaskStates.Terminated;
                            break;
                        case CMMNTransitions.ParentSuspend:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Active))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not active" }
                                });
                            }

                            taskState = TaskStates.Suspended;
                            break;
                        case CMMNTransitions.ParentResume:
                            if (State != Enum.GetName(typeof(TaskStates), TaskStates.Suspended))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "planitem instance is not suspended" }
                                });
                            }

                            taskState = TaskStates.Active;
                            break;
                    }

                    return Enum.GetName(typeof(TaskStates), taskState);
                case CaseElementTypes.Milestone:
                case CaseElementTypes.TimerEventListener:
                    MilestoneStates milestoneState = MilestoneStates.Available;
                    switch (planItemTransition)
                    {
                        case CMMNTransitions.Create:
                            if (!string.IsNullOrWhiteSpace(State))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "milestone instance is initialized" }
                                });
                            }

                            milestoneState = MilestoneStates.Available;
                            break;
                        case CMMNTransitions.Occur:
                            if (State != Enum.GetName(typeof(MilestoneStates), MilestoneStates.Available))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "milestone instance is not available" }
                                });
                            }

                            milestoneState = MilestoneStates.Completed;
                            break;
                        case CMMNTransitions.Suspend:
                            if (State != Enum.GetName(typeof(MilestoneStates), MilestoneStates.Available))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "milestone instance is not available" }
                                });
                            }

                            milestoneState = MilestoneStates.Suspended;
                            break;
                        case CMMNTransitions.Terminate:
                            if (State != Enum.GetName(typeof(MilestoneStates), MilestoneStates.Available))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "milestone instance is not available" }
                                });
                            }

                            milestoneState = MilestoneStates.Terminated;
                            break;
                        case CMMNTransitions.Resume:
                            if (State != Enum.GetName(typeof(MilestoneStates), MilestoneStates.Suspended))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "milestone instance is not suspended" }
                                });
                            }

                            milestoneState = MilestoneStates.Available;
                            break;
                    }

                    return Enum.GetName(typeof(MilestoneStates), milestoneState);
                case CaseElementTypes.CaseFileItem:
                    CaseFileItemStates caseFileItemState = CaseFileItemStates.Available;
                    switch(planItemTransition)
                    {
                        case CMMNTransitions.Create:
                            if (!string.IsNullOrWhiteSpace(State))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "casefileitem is initialized" }
                                });
                            }

                            caseFileItemState = CaseFileItemStates.Available;
                            break;
                        case CMMNTransitions.Update:
                        case CMMNTransitions.Replace:
                        case CMMNTransitions.AddChild:
                        case CMMNTransitions.RemoveChild:
                        case CMMNTransitions.AddReference:
                        case CMMNTransitions.RemoveReference:
                            if (State != Enum.GetName(typeof(CaseFileItemStates), CaseFileItemStates.Available))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "casefileitem is not available" }
                                });
                            }

                            caseFileItemState = CaseFileItemStates.Available;
                            break;
                        case CMMNTransitions.Delete:
                            if (State != Enum.GetName(typeof(CaseFileItemStates), CaseFileItemStates.Available))
                            {
                                throw new AggregateValidationException(new Dictionary<string, string>
                                {
                                    { "transition", "casefileitem is not available" }
                                });
                            }

                            caseFileItemState = CaseFileItemStates.Discarded;
                            break;
                    }
                    return Enum.GetName(typeof(CaseFileItemStates), caseFileItemState);
            }

            return null;
        }

        public object Clone()
        {
            return new CaseElementInstance(Id, CreateDateTime, CaseElementDefinitionId, CaseElementDefinitionType, Version, ParentId)
            {
                FormInstanceId = FormInstanceId,
                State = State,
                StateHistories = StateHistories == null ? null : StateHistories.Select(s => (CaseElementInstanceHistory)s.Clone()).ToList(),
                TransitionHistories = TransitionHistories == null ? null : TransitionHistories.Select(t => (CaseElementInstanceTransitionHistory)t.Clone()).ToList()
            };
        }
    }
}