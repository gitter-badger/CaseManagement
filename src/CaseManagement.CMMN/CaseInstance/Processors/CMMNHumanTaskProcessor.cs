﻿using CaseManagement.CMMN.Domains;
using CaseManagement.CMMN.Extensions;
using CaseManagement.Workflow.Domains;
using CaseManagement.Workflow.Domains.Process;
using CaseManagement.Workflow.Engine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CaseManagement.CMMN.CaseInstance.Processors
{
    public class CMMNHumanTaskProcessor : BaseCMMNTaskProcessor
    {
        public override string ProcessFlowElementType => Enum.GetName(typeof(CMMNPlanItemDefinitionTypes), CMMNPlanItemDefinitionTypes.HumanTask).ToLowerInvariant();

        public override async Task Run(WorkflowHandlerContext context, CancellationToken token)
        {
            var pf = context.ProcessFlowInstance;
            var cmmnPlanItem = context.GetCMMNPlanItem();
            var humanTask = context.GetCMMNHumanTask();
            var formInstance = pf.GetFormInstance(context.CurrentElement.Id);
            if (formInstance != null && formInstance.Status == ProcessFlowInstanceElementFormStatus.Complete)
            {
                pf.CompletePlanItem(cmmnPlanItem);
                await context.Complete(token);
                return;
            }

            if (humanTask.IsBlocking)
            {
                pf.BlockElement(cmmnPlanItem);
                return;
            }

            pf.CompletePlanItem(cmmnPlanItem);
            await context.Complete(token);
            return;
        }
    }
}
