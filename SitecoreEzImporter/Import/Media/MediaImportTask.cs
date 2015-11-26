using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using EzImporter.Extensions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace EzImporter.Import.Media
{
    public class MediaImportTask
    {
        protected StringBuilder Log { get; set; }
        protected MediaImportTaskArgs Args { get; set; }
        /// <summary>
        /// Full file name
        /// </summary>
        private const string FileNameField = "FileName";
        /// <summary>
        /// The name for the media item 
        /// </summary>
        private const string MediaItemNameField = "MediaItemNameField";
        private const string MediaIdField = "MediaId";
        private const string FilePathField = "FilePath";
        private const string IsValidFileField = "IsValid";
        private const string AltTextField = "AltText";
        private static readonly ID MediaFolderTemplateId = new ID("{FE5DD826-48C6-436D-B87A-7C4210C7413B}");

        public string Run(MediaImportTaskArgs args)
        {
            Log = new StringBuilder();
            Args = args;
            if (!ValidateArgs())
            {
                Log.AppendLine("Invalid import map definition");
                return Log.ToString();
            }
            var dataTable = new DataTable();
            ReadMapInfo(ref dataTable);
            ExtractZipFile();
            ParseExtractedFileNames(ref dataTable);
            CreateMediaItems(ref dataTable);
            UpdateContentItems(dataTable);
            return Log.ToString();
        }

        private bool ValidateArgs()
        {
            return Args != null &&
                Args.MediaImportMap != null &&
                Args.MediaImportMap.IsValid;
        }

        protected void ReadMapInfo(ref DataTable dataTable)
        {
            foreach (var field in Args.MediaImportMap.MappingFields)
            {
                dataTable.Columns.Add(field, typeof(string));
            }
            dataTable.Columns.Add(FileNameField, typeof(string));
            dataTable.Columns.Add(MediaItemNameField, typeof(string));
            dataTable.Columns.Add(MediaIdField, typeof(ID));
            dataTable.Columns.Add(FilePathField, typeof(string));
            dataTable.Columns.Add(IsValidFileField, typeof(bool));
            dataTable.Columns.Add(AltTextField, typeof(string));
        }

        protected void ExtractZipFile()
        {
            if (Directory.Exists(Args.ExtractionFolder))
            {
                Directory.Delete(Args.ExtractionFolder, true);
            }
            System.IO.Compression.ZipFile.ExtractToDirectory(Args.ZipFileName, Args.ExtractionFolder);
        }

        protected void ParseExtractedFileNames(ref DataTable dataTable)
        {
            FindFiles(ref dataTable, Args.ExtractionFolder);
        }

        private void FindFiles(ref DataTable dataTable, string directory)
        {
            foreach (var subDirectory in Directory.GetDirectories(directory))
            {
                FindFiles(ref dataTable, subDirectory);
            }
            foreach (var file in Directory.GetFiles(directory))
            {
                dataTable.Rows.Add(ParseFileName(file, dataTable));
            }
        }

        private DataRow ParseFileName(string fileName, DataTable dataTable)
        {
            var row = dataTable.NewRow();
            var shortFileName = GetNameWithoutExtension(fileName);
            var properties = shortFileName.Split(MediaImportMap.FileNameFormatDelimiter, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Args.MediaImportMap.MappingFields.Length; i++)
            {
                row[Args.MediaImportMap.MappingFields[i]] = i < properties.Length ? properties[i] : "";
            }
            if (Args.MediaImportMap.UseFileNameForMediaItem)
            {
                row[MediaItemNameField] = shortFileName;
            }
            else
            {
                var mediaItemNameFragments = new List<string>();
                foreach (var nameMappingField in Args.MediaImportMap.MediaNameMappingFields)
                {
                    var value = dataTable.Columns[nameMappingField] != null ? Convert.ToString(row[nameMappingField]) : nameMappingField;
                    mediaItemNameFragments.Add(value);
                }
                row[MediaItemNameField] = string.Join(MediaImportMap.FileNameWordDelimiter, mediaItemNameFragments);
            }
            row[FileNameField] = fileName;
            row[FilePathField] = GetRelativePath(fileName, Args.ExtractionFolder);
            row[IsValidFileField] = properties.Length != Args.MediaImportMap.MappingFields.Length;
            var altTextFragments = new List<string>();
            foreach (var altTextMappingField in Args.MediaImportMap.AltTextMappingFields)
            {
                var value = dataTable.Columns[altTextMappingField] != null ? Convert.ToString(row[altTextMappingField]) : altTextMappingField;
                altTextFragments.Add(value);
            }
            row[AltTextField] = string.Join(" ", altTextFragments).Trim();
            Log.AppendFormat("{0}/{1} extracted from zip file.{2}", row[FilePathField], GetShortName(fileName), Environment.NewLine);
            return row;
        }

        private string GetShortName(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            return fileInfo.Name;
        }

        private string GetNameWithoutExtension(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            var i = fileInfo.Name.LastIndexOf(fileInfo.Extension);
            return fileInfo.Name.Substring(0, i);
        }

        private string GetRelativePath(string fileName, string extractFolder)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.FullName.StartsWith(extractFolder))
            {
                throw new ArgumentException("Invalid extractFolder for given fileName");
            }
            return fileInfo.Directory.FullName.Substring(extractFolder.Length).Replace(@"\", @"/").TrimEnd(new[] { '/' });
        }

        protected void CreateMediaItems(ref DataTable dataTable)
        {
            var rootMediaItem = Args.Database.GetItem(Args.RootMediaItemId);
            var rootPath = rootMediaItem.Paths.ContentPath; //won't end in '/'
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                var destinationPath = rootPath + Sitecore.StringUtil.EnsurePrefix('/', row[FilePathField].ToString());
                var fileName = row[FileNameField].ToString();
                var mediaItemName = Utils.GetValidItemName(row[MediaItemNameField]);
                string altText = row[AltTextField].ToString();
                var mediaItem = AddFile(fileName, destinationPath, mediaItemName, altText);
                if (mediaItem != null)
                {
                    row[MediaIdField] = mediaItem.ID;
                }
            }
        }

        private MediaItem AddFile(string fileName, string destinationPath, string mediaItemName, string altText)
        {
            //ensure that the containing folder exists
            var folderOptions = new MediaCreatorOptions
            {
                KeepExisting = true,
                Destination = destinationPath,
                Database = Args.Database
            };
            MediaManager.Creator.CreateFromFolder(destinationPath, folderOptions);

            //create the file
            var fileOptions = new MediaCreatorOptions
            {
                FileBased = false,
                IncludeExtensionInItemName = false,
                KeepExisting = false,
                Versioned = false,
                Destination = destinationPath + "/" + mediaItemName,
                Database = Args.Database,
                AlternateText = altText
            };
            MediaItem mediaItem = MediaManager.Creator.CreateFromFile(fileName, fileOptions);
            Log.AppendLine("Created: " + mediaItem.Path);
            return mediaItem;
        }

        protected void UpdateContentItems(DataTable dataTable)
        {
            var idColumn = new[] { Args.MediaImportMap.ItemIdProperty };
            var itemIds = dataTable.GroupBy(idColumn);
            var query = string.Format("//*[@@id='{0}']//*[@@templateid='{1}']", Args.RootDataItemId, Args.MediaImportMap.TemplateId);
            var queryItems = Args.Database.SelectItems(query);
            for (var i = 0; i < itemIds.Rows.Count; i++)
            {
                var itemId = itemIds.Rows[i][Args.MediaImportMap.ItemIdProperty].ToString();
                var item = queryItems.FirstOrDefault(itm => string.Equals(itm[Args.MediaImportMap.ItemIdProperty], itemId, StringComparison.CurrentCultureIgnoreCase));
                if (item != null)
                {
                    var selectedRows = dataTable.AsEnumerable().Where(r => r[Args.MediaImportMap.ItemIdProperty].ToString() == itemId);
                    using (new EditContext(item, true, false))
                    {
                        foreach (var row in selectedRows)
                        {
                            var imageFieldName = row[Args.MediaImportMap.ImageFieldProperty].ToString();
                            if (string.IsNullOrEmpty(imageFieldName))
                            {
                                Log.AppendLine("No image field name found, skipping image attachment.");
                                continue;
                            }
                            var imageField = item.Fields[imageFieldName];
                            if (imageField != null)
                            {
                                var altText = "test";
                                imageField.Value = string.Format("<image mediaid=\"{0}\" alt=\"{1}\"/>", row[MediaIdField], altText);
                            }

                        }
                    }
                }
                else
                {
                    Log.AppendFormat("No item found with id of '{0}', skipping image attachment.", itemId);
                }
            }
        }

    }
}