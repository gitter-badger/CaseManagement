﻿using CaseManagement.CMMN.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CaseManagement.CMMN.CasePlanInstance.Results
{
    public class CasePlanInstanceResult
    {
        public string Id { get; set; }
        public string CasePlanId { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
        public Dictionary<string, string> ExecutionContext { get; set; }
        public ICollection<CasePlanInstanceFileItemResult> Files { get; set; }
        public ICollection<CasePlanInstanceRoleResult> Roles { get; set; }
        public ICollection<CasePlanItemInstanceResult> Children { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public class CasePlanInstanceFileItemResult
        {
            public string CasePlanElementInstanceId { get; set; }
            public string CaseFileItemType { get; set; }
            public string ExternalValue { get; set; }

            public static CasePlanInstanceFileItemResult ToDTO(CasePlanInstanceFileItem result)
            {
                return new CasePlanInstanceFileItemResult
                {
                    CaseFileItemType = result.CaseFileItemType,
                    CasePlanElementInstanceId = result.CasePlanElementInstanceId,
                    ExternalValue = result.ExternalValue
                };
            }
        }

        public class CasePlanInstanceRoleResult
        {
            public string Id { get; set; }
            public string Name { get; set; }
            
            public static CasePlanInstanceRoleResult ToDto(CasePlanInstanceRole role)
            {
                return new CasePlanInstanceRoleResult
                {
                    Id = role.Id,
                    Name = role.Name
                };
            }
        }

        public class CasePlanItemInstanceResult
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string State { get; set; }
            public ICollection<TransitionHistoryResult> TransitionHistories { get; set; }

            public static CasePlanItemInstanceResult ToDto(BaseCasePlanItemInstance casePlanItemInstance)
            {
                string stateStr = null;
                if (casePlanItemInstance is BaseTaskOrStageElementInstance)
                {
                    var state = ((BaseTaskOrStageElementInstance)casePlanItemInstance).State;
                    stateStr = state == null ? null : Enum.GetName(typeof(TaskStageStates), state);
                }

                if (casePlanItemInstance is BaseMilestoneOrTimerElementInstance)
                {
                    var state = ((BaseMilestoneOrTimerElementInstance)casePlanItemInstance).State;
                    stateStr = state == null ? null : Enum.GetName(typeof(MilestoneEventStates), state);
                }

                return new CasePlanItemInstanceResult
                {
                    Id = casePlanItemInstance.Id,
                    Name = casePlanItemInstance.Name,
                    State = stateStr,
                    Type = Enum.GetName(typeof(CasePlanElementInstanceTypes), casePlanItemInstance.Type).ToUpperInvariant(),
                    TransitionHistories = casePlanItemInstance.TransitionHistories.Select(_ => TransitionHistoryResult.ToDto(_)).ToList()
                };
            }
        }

        public class TransitionHistoryResult
        {
            public string Transition { get; set; }
            public DateTime ExecutionDateTime { get; set; }
            public static TransitionHistoryResult ToDto(CasePlanElementInstanceTransitionHistory history)
            {
                return new TransitionHistoryResult
                {
                    ExecutionDateTime = history.ExecutionDateTime,
                    Transition = Enum.GetName(typeof(CMMNTransitions), history.Transition)
                };
            }
        }

        public static CasePlanInstanceResult ToDto(CasePlanInstanceAggregate casePlanInstance)
        {
            return new CasePlanInstanceResult
            {
                Id = casePlanInstance.AggregateId,
                CasePlanId = casePlanInstance.CasePlanId,
                Name = casePlanInstance.Name,
                State = casePlanInstance.State == null ? string.Empty : Enum.GetName(typeof(CaseStates), casePlanInstance.State),
                Roles = casePlanInstance.Roles.Select(_ => CasePlanInstanceRoleResult.ToDto(_)).ToList(),
                CreateDateTime = casePlanInstance.CreateDateTime,
                UpdateDateTime = casePlanInstance.UpdateDateTime,
                Children = casePlanInstance.GetFlatListCasePlanItems().Select(_ => CasePlanItemInstanceResult.ToDto(_)).ToList(),
                Files = casePlanInstance.Files.Select(_ => CasePlanInstanceFileItemResult.ToDTO(_)).ToList(),
                ExecutionContext = casePlanInstance.ExecutionContext == null ? new Dictionary<string, string>() : casePlanInstance.ExecutionContext.Variables.ToDictionary(k => k.Key, k => k.Value)
            };
        }
    }
}
