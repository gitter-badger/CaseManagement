﻿namespace CaseManagement.CMMN
{
    public static class CMMNConstants
    {
        public const int WAIT_INTERVAL_MS = 20;

        public static class StandardProcessMappingVariables
        {
            public const string CaseInstanceId = "$caseinstanceid$";
        }

        public static class QueueNames
        {
            public const string ExternalEvents = "externalevts";
            public const string CasePlanInstances = "caseplaninstances";
            public const string DomainEvents = "domainevts";
        }

        public static class ExternalTransitionNames
        {
            public const string Terminate = "terminate";
            public const string ManualStart = "manualstart";
            public const string Complete = "complete";
        }

        public static class RouteNames
        {
            public const string CaseFiles = "case-files";
            public const string CasePlans = "case-plans";
            public const string CasePlanInstances = "case-plan-instances";
            public const string CaseProcesses = "case-processes";
            public const string CaseFormInstances = "case-form-instances";
            public const string CaseWorkerTasks = "case-worker-tasks";
            public const string Statistics = "statistics";
            public const string Performances = "performances";
            public const string Forms = "forms";
            public const string Roles = "roles";
        }

        public static class ProcessImplementationTypes
        {
            public const string BMNN20 = "http://www.omg.org/spec/CMMN/ProcessType/BPMN20";
            public const string XPDL2 = "http://www.omg.org/spec/CMMN/ProcessType/XPDL2";
            public const string WSBPEL20 = "http://www.omg.org/spec/CMMN/ProcessType/WSBPEL20";
            public const string WSBPEL1 = "http://www.omg.org/spec/CMMN/ProcessType/WSBPEL1";
            public const string CASEMANAGEMENTCALLBACK = "https://github.com/simpleidserver/CaseManagement/callback";
        }

        public static class ContentManagementTypes
        {
            public const string DIRECTORY = "https://github.com/simpleidserver/casemanagement/directory";
            public const string FILE = "https://github.com/simpleidserver/casemanagement/file";
        }
    }
}
