﻿using System.Runtime.Serialization;

namespace CaseManagement.Gateway.Website.CaseFile.Commands
{
    public class UpdateCaseFileCommand
    {
        public string CaseFileId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Payload { get; set; }
        public string IdentityToken { get; set; }
    }
}