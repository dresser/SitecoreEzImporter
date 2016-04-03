using EzImporter.Configuration;
using EzImporter.Import.Item;
using EzImporter.Models;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Services.Core;
using Sitecore.Services.Infrastructure.Web.Http;
using System;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;

namespace EzImporter.Controllers
{
    [ServicesController]
    public class ImportController : ServicesApiController
    {
        [HttpPost]
        public IHttpActionResult Import(ImportModel importModel)
        {
            var args = new ItemImportTaskArgs
            {
                Database = Sitecore.Configuration.Factory.GetDatabase("master"),
                //FileName = csvFileName.Value, //TODO change to use media item instead
                RootItemId = new ID(importModel.ImportLocationId),
                TargetLanguage = Sitecore.Globalization.Language.Parse(importModel.Language),
                Map = Map.Factory.BuildMapInfo(new ID(importModel.MappingId)),
                ImportOptions = new ImportOptions
                {
                    CsvDelimiter = new[] {importModel.CsvDelimiter},
                    ExistingItemHandling =
                        (ExistingItemHandling)
                            Enum.Parse(typeof (ExistingItemHandling), importModel.ExistingItemHandling),
                    InvalidLinkHandling =
                        (InvalidLinkHandling)
                            Enum.Parse(typeof (InvalidLinkHandling), importModel.InvalidLinkHandling),
                    MultipleValuesImportSeparator = importModel.MultipleValuesSeparator,
                    TreePathValuesImportSeparator = "\""
                }
            };
            var task = new ItemImportTask();
            var result = new ImportResultModel {Log = task.Run(args)};
            return new JsonResult<ImportResultModel>(result, new JsonSerializerSettings(), Encoding.UTF8, this);
        }

        [HttpGet]
        public IHttpActionResult DefaultSettings()
        {
            var options = EzImporter.Configuration.Factory.GetDefaultImportOptions();
            var model = new SettingsModel
            {
                CsvDelimiter = options.CsvDelimiter[0],
                ExistingItemHandling = options.ExistingItemHandling.ToString(),
                InvalidLinkHandling = options.InvalidLinkHandling.ToString(),
                MultipleValuesSeparator = options.MultipleValuesImportSeparator
            };
            return new JsonResult<SettingsModel>(model, new JsonSerializerSettings(), Encoding.UTF8, this);
        }
    }
}