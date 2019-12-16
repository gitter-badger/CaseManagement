﻿using CaseManagement.Workflow.Infrastructure;
using System;
using System.Diagnostics;

namespace CaseManagement.Workflow.Domains.Events
{
    [DebuggerDisplay("Element {ElementId} is unblocked")]
    public class ProcessFlowElementUnblockedEvent : DomainEvent
    {
        public ProcessFlowElementUnblockedEvent(string id, string aggregateId, int version, string elementId, DateTime blockedDateTime) : base(id, aggregateId, version)
        {
            ElementId = elementId;
            BlockedDateTime = blockedDateTime;
        }

        public string ElementId { get; set; }
        public DateTime BlockedDateTime { get; set; }
    }
}
