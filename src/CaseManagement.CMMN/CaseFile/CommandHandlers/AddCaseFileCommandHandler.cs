﻿using CaseManagement.CMMN.CaseFile.Commands;
using CaseManagement.CMMN.Domains;
using CaseManagement.CMMN.Infrastructures;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace CaseManagement.CMMN.CaseFile.CommandHandlers
{
    public class AddCaseFileCommandHandler : IAddCaseFileCommandHandler
    {
        private readonly ICommitAggregateHelper _commitAggregateHelper;
        private readonly CMMNServerOptions _options;

        public AddCaseFileCommandHandler(ICommitAggregateHelper commitAggregateHelper, IOptions<CMMNServerOptions> options)
        {
            _commitAggregateHelper = commitAggregateHelper;
            _options = options.Value;
        }

        public async Task<string> Handle(AddCaseFileCommand addCaseFileCommand)
        {
            var payload = addCaseFileCommand.Payload;
            if (string.IsNullOrWhiteSpace(addCaseFileCommand.Payload))
            {
                payload = _options.DefaultCMMNSchema;
            }

            var caseFile = CaseFileAggregate.New(addCaseFileCommand.Name, addCaseFileCommand.Description, 0, addCaseFileCommand.Owner, payload);
            var streamName = CaseFileAggregate.GetStreamName(caseFile.Id);
            await _commitAggregateHelper.Commit(caseFile, streamName, CMMNConstants.QueueNames.CaseFiles);
            return caseFile.Id;
        }
    }
}