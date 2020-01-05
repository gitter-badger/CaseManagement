﻿using CaseManagement.CMMN.CaseInstance.Commands;
using CaseManagement.CMMN.Domains;
using System.Threading.Tasks;

namespace CaseManagement.CMMN.CaseInstance.CommandHandlers
{
    public interface ICreateCaseInstanceCommandHandler
    {
        Task<CMMNWorkflowInstance> Handle(CreateCaseInstanceCommand command);
    }
}