﻿using System;

namespace CaseManagement.CMMN.Domains
{
    [Serializable]
    public class CasePlanElementInstanceTransitionHistory : ICloneable
    {
        public DateTime ExecutionDateTime { get; set; }
        public CMMNTransitions Transition { get; set; }

        public object Clone()
        {
            return new CasePlanElementInstanceTransitionHistory
            {
                ExecutionDateTime = ExecutionDateTime,
                Transition = Transition
            };
        }
    }
}
