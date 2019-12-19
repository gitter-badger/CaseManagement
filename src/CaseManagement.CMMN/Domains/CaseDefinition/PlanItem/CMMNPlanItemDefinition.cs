﻿using System.Collections.Generic;

namespace CaseManagement.CMMN.Domains
{
    public class CMMNPlanItemDefinition
    {
        public CMMNPlanItemDefinition(string id, string name)
        {
            Id = id;
            Name = name;
            EntryCriterions = new List<CMMNCriterion>();
            ExitCriterions = new List<CMMNCriterion>();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Reference to the corresponding PlanItemDefinition object. [1...1]
        /// </summary>
        public CMMNPlanItemDefinitionTypes PlanItemDefinitionType { get; set; }
        public CMMNTask PlanItemDefinitionTask { get; set; }
        public CMMNHumanTask PlanItemDefinitionHumanTask { get; set; }
        public CMMNProcessTask PlanItemDefinitionProcessTask { get; set; }
        public CMMNTimerEventListener PlanItemDefinitionTimerEventListener { get; set; }
        public CMMNMilestone PlanItemMilestone { get; set; }

        /// <summary>
        /// The PlanItemControl controls aspects of the behavior of instances of the PlanItem object. [0...1]
        /// </summary>
        public CMMNActivationRuleTypes? ActivationRule { get; set; }
        public CMMNManualActivationRule ManualActivationRule { get; set; }
        public CMMNRepetitionRule RepetitionRule { get; set; }

        /// <summary>
        /// Zero or more EntryCriterion for that PlanItem. [0...*].
        /// An EntryCriterion represents the condition for a PlanItem to become available.
        /// </summary>
        public ICollection<CMMNCriterion> EntryCriterions { get; set; }
        /// <summary>
        /// An ExitCriterion represents the condition for a PlanItem to terminate. [0...*]
        /// </summary>
        public ICollection<CMMNCriterion> ExitCriterions { get; set; }

        public void SetManualActivationRule(CMMNManualActivationRule activationRule)
        {
            ManualActivationRule = activationRule;
            ActivationRule = CMMNActivationRuleTypes.ManualActivation;
        }

        public void SetRepetitionRule(CMMNRepetitionRule repetitionRule)
        {
            RepetitionRule = repetitionRule;
            ActivationRule = CMMNActivationRuleTypes.Repetition;
        }

        public static CMMNPlanItemDefinition New(string id, string name, CMMNHumanTask humanTask)
        {
            var result = new CMMNPlanItemDefinition(id, name)
            {
                PlanItemDefinitionHumanTask = humanTask,
                PlanItemDefinitionType = CMMNPlanItemDefinitionTypes.HumanTask
            };
            return result;
        }

        public static CMMNPlanItemDefinition New(string id, string name, CMMNMilestone milestone)
        {
            var result = new CMMNPlanItemDefinition(id, name)
            {
                PlanItemMilestone = milestone,
                PlanItemDefinitionType = CMMNPlanItemDefinitionTypes.Milestone
            };
            return result;
        }

        public static CMMNPlanItemDefinition New(string id, string name, CMMNProcessTask processTask)
        {
            var result = new CMMNPlanItemDefinition(id, name)
            {
                PlanItemDefinitionProcessTask = processTask,
                PlanItemDefinitionType = CMMNPlanItemDefinitionTypes.ProcessTask
            };
            return result;
        }

        public static CMMNPlanItemDefinition New(string id, string name, CMMNTask task)
        {
            var result = new CMMNPlanItemDefinition(id, name)
            {
                PlanItemDefinitionTask = task,
                PlanItemDefinitionType = CMMNPlanItemDefinitionTypes.Task
            };
            return result;
        }

        public static CMMNPlanItemDefinition New(string id, string name, CMMNTimerEventListener timer)
        {
            var result = new CMMNPlanItemDefinition(id, name)
            {
                PlanItemDefinitionTimerEventListener = timer,
                PlanItemDefinitionType = CMMNPlanItemDefinitionTypes.TimerEventListener
            };
            return result;
        }

        /*
        public void Create()
        {
            Handle(CMMNPlanItemTransitions.Create);
            Version++;
            TransitionHistories.Add(new CMMNPlanItemStateHistory(DateTime.UtcNow, CMMNPlanItemTransitions.Create, Version));
        }

        public void Enable()
        {
            Handle(CMMNPlanItemTransitions.Enable);
            TransitionHistories.Add(new CMMNPlanItemStateHistory(DateTime.UtcNow, CMMNPlanItemTransitions.Enable, Version));
        }

        public void ManualStart()
        {
            Handle(CMMNPlanItemTransitions.ManualStart);
            TransitionHistories.Add(new CMMNPlanItemStateHistory(DateTime.UtcNow, CMMNPlanItemTransitions.ManualStart, Version));
        }

        public void Occur()
        {
            Handle(CMMNPlanItemTransitions.Occur);
            TransitionHistories.Add(new CMMNPlanItemStateHistory(DateTime.UtcNow, CMMNPlanItemTransitions.Occur, Version));
        }

        public void Start()
        {
            Handle(CMMNPlanItemTransitions.Start);
            TransitionHistories.Add(new CMMNPlanItemStateHistory(DateTime.UtcNow, CMMNPlanItemTransitions.Start, Version));
        }

        public void Disable()
        {
            Handle(CMMNPlanItemTransitions.Disable);
            TransitionHistories.Add(new CMMNPlanItemStateHistory(DateTime.UtcNow, CMMNPlanItemTransitions.Disable, Version));
        }

        public void Complete()
        {
            Handle(CMMNPlanItemTransitions.Complete);
            TransitionHistories.Add(new CMMNPlanItemStateHistory(DateTime.UtcNow, CMMNPlanItemTransitions.Complete, Version));
        }

        public void Terminate()
        {
            Handle(CMMNPlanItemTransitions.Terminate);
            TransitionHistories.Add(new CMMNPlanItemStateHistory(DateTime.UtcNow, CMMNPlanItemTransitions.Terminate, Version));
        }

        private void Handle(CMMNPlanItemTransitions transition)
        {
            switch(PlanItemDefinitionType)
            {
                case CMMNPlanItemDefinitionTypes.HumanTask:
                    PlanItemDefinitionHumanTask.Handle(transition);
                    break;
                case CMMNPlanItemDefinitionTypes.ProcessTask:
                    PlanItemDefinitionProcessTask.Handle(transition);
                    break;
                case CMMNPlanItemDefinitionTypes.Task:
                    PlanItemDefinitionTask.Handle(transition);
                    break;
                case CMMNPlanItemDefinitionTypes.TimerEventListener:
                    PlanItemDefinitionTimerEventListener.Handle(transition);
                    break;
                case CMMNPlanItemDefinitionTypes.Milestone:
                    PlanItemMilestone.Handle(transition);
                    break;
            }
        }


        public override void HandleLaunch()
        {
            Create();
        }

        public override void HandleEvent(string state)
        {
            var stateEnum = (CMMNPlanItemTransitions)Enum.Parse(typeof(CMMNPlanItemTransitions), state);
            switch(stateEnum)
            {
                case CMMNPlanItemTransitions.Create:
                    Create();
                    break;
                case CMMNPlanItemTransitions.Enable:
                    Enable();
                    break;
                case CMMNPlanItemTransitions.ManualStart:
                    ManualStart();
                    break;
                case CMMNPlanItemTransitions.Occur:
                    Occur();
                    break;
                case CMMNPlanItemTransitions.Start:
                    Start();
                    break;
                case CMMNPlanItemTransitions.Disable:
                    Disable();
                    break;
                case CMMNPlanItemTransitions.Complete:
                    Complete();
                    break;
                case CMMNPlanItemTransitions.Terminate:
                    Terminate();
                    break;
            }
        }

        public override object Clone()
        {
            return new CMMNPlanItemDefinition(Id, Name)
            {
                Status = Status,
                ActivationRule = ActivationRule,
                FormInstance = FormInstance == null ? null : (FormInstanceAggregate)FormInstance.Clone(),
                ManualActivationRule = ManualActivationRule == null ? null : (CMMNManualActivationRule)ManualActivationRule.Clone(),
                PlanItemDefinitionHumanTask = PlanItemDefinitionHumanTask == null ? null : (CMMNHumanTask)PlanItemDefinitionHumanTask.Clone(),
                PlanItemDefinitionProcessTask = PlanItemDefinitionProcessTask == null ? null : (CMMNProcessTask)PlanItemDefinitionProcessTask.Clone(),
                PlanItemDefinitionTask = PlanItemDefinitionTask == null ? null : (CMMNTask)PlanItemDefinitionTask.Clone(),
                PlanItemDefinitionTimerEventListener = PlanItemDefinitionTimerEventListener == null ? null : (CMMNTimerEventListener)PlanItemDefinitionTimerEventListener.Clone(),
                PlanItemDefinitionType = PlanItemDefinitionType,
                EntryCriterions = EntryCriterions.Select(e => (CMMNCriterion)e.Clone()).ToList(),
                ExitCriterions = ExitCriterions.Select(e => (CMMNCriterion)e.Clone()).ToList(),
                RepetitionRule = RepetitionRule == null ? null : (CMMNRepetitionRule)RepetitionRule.Clone(),
                TransitionHistories = TransitionHistories.Select(e => (CMMNPlanItemStateHistory)e.Clone()).ToList(),
                PlanItemMilestone = PlanItemMilestone == null ? null : (CMMNMilestone)PlanItemMilestone.Clone(),
                Version = Version
            };
        }
        */
    }
}