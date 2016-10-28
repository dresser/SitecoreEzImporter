using EzImporter.Configuration;
using EzImporter.Models;
using EzImporter.Pipelines.ImportItems;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
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
            var database = Sitecore.Configuration.Factory.GetDatabase("master");
            var languageItem = database.GetItem(importModel.Language);
            var uploadedFile = (MediaItem)database.GetItem(importModel.MediaItemId);
            if (uploadedFile == null)
            {
                return new JsonResult<ImportResultModel>(null, new JsonSerializerSettings(), Encoding.UTF8, this);
            }
            var args = new ImportItemsArgs
            {
                Database = database,
                FileExtension = uploadedFile.Extension.ToLower(),
                FileStream = uploadedFile.GetMediaStream(),
                RootItemId = new ID(importModel.ImportLocationId),
                TargetLanguage = Sitecore.Globalization.Language.Parse(languageItem.Name),
                Map = Map.Factory.BuildMapInfo(new ID(importModel.MappingId)),
                ImportOptions = new ImportOptions
                {
                    CsvDelimiter = new[] { importModel.CsvDelimiter },
                    ExistingItemHandling =
                        (ExistingItemHandling)
                            Enum.Parse(typeof(ExistingItemHandling), importModel.ExistingItemHandling),
                    InvalidLinkHandling =
                        (InvalidLinkHandling)
                            Enum.Parse(typeof(InvalidLinkHandling), importModel.InvalidLinkHandling),
                    MultipleValuesImportSeparator = importModel.MultipleValuesSeparator,
                    TreePathValuesImportSeparator = @"\",
                    FirstRowAsColumnNames = importModel.FirstRowAsColumnNames
                }
            };
            ImportResultModel result;
            try
            {
                Sitecore.Diagnostics.Log.Info(string.Format("EzImporter: mappingId:{0} mediaItemId:{1} firstRowAsColumnNames:{2}", importModel.MappingId, importModel.MediaItemId, args.ImportOptions.FirstRowAsColumnNames), this);
                CorePipeline.Run("importItems", args);
                if (args.Aborted)
                {
                    result = new ImportResultModel { HasError = true, Log = args.Statistics.ToString(), ErrorMessage = args.Message, ErrorDetail = args.ErrorDetail };
                }
                else
                {
                    result = new ImportResultModel { Log = args.Statistics.ToString() };
                }
            }
            catch (Exception ex)
            {
                result = new ImportResultModel { HasError = true, ErrorMessage = ex.Message, ErrorDetail = ex.ToString() };
            }
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
                MultipleValuesSeparator = options.MultipleValuesImportSeparator,
                FirstRowAsColumnNames = options.FirstRowAsColumnNames
            };
            return new JsonResult<SettingsModel>(model, new JsonSerializerSettings(), Encoding.UTF8, this);
        }
    }
}