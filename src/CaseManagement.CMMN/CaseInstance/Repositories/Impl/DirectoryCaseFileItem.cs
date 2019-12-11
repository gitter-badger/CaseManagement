﻿using System.Collections.Generic;
using System.IO;

namespace CaseManagement.CMMN.CaseInstance.Repositories
{
    public class DirectoryCaseFileItem : CaseFileItem
    {
        public DirectoryCaseFileItem(string id) : base(id) { }

        public override IEnumerable<CaseFileItem> GetChildren()
        {
            foreach(var file in Directory.EnumerateFiles(Id))
            {
                yield return new FileCaseFileItem(file);
            }

            foreach(var directory in Directory.EnumerateDirectories(Id))
            {
                yield return new DirectoryCaseFileItem(directory);
            }
        }

        public override string ReadContent()
        {
            return string.Empty;
        }
    }
}