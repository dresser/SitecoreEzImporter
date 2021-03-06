﻿using EzImporter.Configuration;
using EzImporter.Map;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace EzImporter.Pipelines.ImportItems
{
    [Serializable]
    public class ImportItemsArgs : PipelineArgs
    {
        public string FileExtension { get; set; }
        public Stream FileStream { get; set; }
        public Database Database { get; set; }
        public ID RootItemId { get; set; }
        public Language TargetLanguage { get; set; }
        public ItemImportMap Map { get; set; }
        public IImportOptions ImportOptions { get; set; }
        public ImportStatistics Statistics { get; set; }
        public DataTable ImportData { get; set; }
        public List<ItemDto> ImportItems { get; set; } 
        public string ErrorDetail { get; set; }
        public Stopwatch Timer { get; set; }

        public ImportItemsArgs()
        {
            Statistics = new ImportStatistics();
            ImportData = new DataTable();
            ImportItems = new List<ItemDto>();
            Timer = new Stopwatch();
        }
    }
}
