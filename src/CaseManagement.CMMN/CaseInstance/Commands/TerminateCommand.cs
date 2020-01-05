﻿namespace CaseManagement.CMMN.CaseInstance.Commands
{
    public class TerminateCommand
    {
        public TerminateCommand(string caseInstanceId, string caseInstanceElementId)
        {
            CaseInstanceId = caseInstanceId;
            CaseInstanceElementId = caseInstanceElementId;
        }

        public string CaseInstanceId { get; set; }
        public string CaseInstanceElementId { get; set; }
    }
}
