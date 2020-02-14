﻿using System.Runtime.Serialization;

namespace CaseManagement.CMMN.CaseFile.Commands
{
    [DataContract]
    public class UpdateCaseFileCommand
    {
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "payload")]
        public string Payload { get; set; }
        [DataMember(Name = "performer")]
        public string Performer { get; set; }
    }
}