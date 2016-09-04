using System.Web.Hosting;
using EzImporter.Configuration;
using EzImporter.Pipelines.ImportItems;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Tasks;
using System;
using System.IO;

namespace EzImporter.Tasks
{
    public class Import
    {
        public void Run(Item[] items, CommandItem command, ScheduleItem schedule)
        {
            var importCommand = new ImportCommandItem(command.InnerItem);
            Log.Info(
                "EzImporter.Tasks.Import.Run() Starting Import: "
                + " fileName=" + importCommand.FileName
                + " lang=" + importCommand.TargetLanguage
                + " location=" + importCommand.ImportLocationId
                + " importMap=" + importCommand.ImportMapId
                + " database=" + importCommand.Database.Name
                + " csvDelimiter=" + importCommand.CsvDelimiter
                + " ExistingItemHandling=" + importCommand.ExistingItemHandling
                + " InvalidLinkHandling=" + importCommand.InvalidLinkHandling
                + " MultipleValuesImportSeparator=" + importCommand.MultipleValuesImportSeparator
                + " TreePathValuesImportSeparator=" + importCommand.TreePathValuesImportSeparator, this);

            var options = Factory.GetDefaultImportOptions();
            if (importCommand.CsvDelimiter != null)
            {
                options.CsvDelimiter = new[] {importCommand.CsvDelimiter};
            }
            if (importCommand.ExistingItemHandling != null)
            {
                options.ExistingItemHandling = (ExistingItemHandling)
                    Enum.Parse(typeof (ExistingItemHandling), importCommand.ExistingItemHandling);
            }
            if (importCommand.InvalidLinkHandling != null)
            {
                options.InvalidLinkHandling = (InvalidLinkHandling)
                    Enum.Parse(typeof (InvalidLinkHandling), importCommand.InvalidLinkHandling);
            }
            if (importCommand.MultipleValuesImportSeparator != null)
            {
                options.MultipleValuesImportSeparator = importCommand.MultipleValuesImportSeparator;
            }
            if (importCommand.TreePathValuesImportSeparator != null)
            {
                options.TreePathValuesImportSeparator = importCommand.TreePathValuesImportSeparator;
            }
            if (string.IsNullOrWhiteSpace(importCommand.FileName))
            {
                Log.Error(
                    "EzImporter.Tasks.Import.Run() - Import Error: File not specified",
                    this);
                return;
            }
            string fileName;
            if (File.Exists(importCommand.FileName))
            {
                fileName = importCommand.FileName;
            }
            else
            {
                fileName = HostingEnvironment.MapPath(importCommand.FileName);
                if (!File.Exists(fileName))
                {
                    Log.Error(
                        "EzImporter.Tasks.Import.Run() - Import Error: File not found (" + importCommand.FileName + ")",
                        this);
                    return;
                }
            }
            var extension = GetFileExtension(fileName);
            if (extension == null)
            {
                Log.Error(
                    "EzImporter.Tasks.Import.Run() - Import Error: Unknown file extension (" + importCommand.FileName +
                    ")", this);
                return;
            }
            var stream = new StreamReader(fileName);
            var args = new ImportItemsArgs
            {
                Database = importCommand.ImportDatabase,
                FirstRowAsColumnNames = importCommand.FirstRowAsColumnNames,
                FileExtension = extension,
                FileStream = stream.BaseStream,
                RootItemId = importCommand.ImportLocationId,
                TargetLanguage = importCommand.TargetLanguage,
                Map = Map.Factory.BuildMapInfo(importCommand.ImportMapId),
                ImportOptions = options
            };
            try
            {
                CorePipeline.Run("importItems", args);
                Log.Info("EzImporter.Tasks.Import.Run() Import Finished " + args.Statistics, this);
            }
            catch (Exception ex)
            {
                Log.Error("EzImporter.Tasks.Import.Run() - Import Error: " + ex, this);
                throw;
            }
        }

        private string GetFileExtension(string fileName)
        {
            var index = fileName.LastIndexOf(".");
            if (index > -1 && index < (fileName.Length - 1))
            {
                return fileName.Substring(index + 1);
            }
            return "";
        }
    }
}