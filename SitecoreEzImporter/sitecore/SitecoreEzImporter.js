define(["sitecore"], function(Sitecore) {
    var DataImporter = Sitecore.Definitions.App.extend({
        uploadedFiles: [],

        initialize: function() {
            this.on("upload-fileUploaded", this.FileUploaded, this);
            $.ajax({
                url: "/sitecore/api/ssc/EzImporter-Controllers/Import/1/DefaultSettings",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                context: this,
                success: function (data) {
                    var app = this;
                    app.ExistingItemHandlingDataSource.on("change:hasItems", function () {
                        app.ExistingItemHandling.set("selectedValue", data.ExistingItemHandling);
                    });
                    app.InvalidLinkHandlingDataSource.on("change:hasItems", function () {
                        app.InvalidLinkHandling.set("selectedValue", data.InvalidLinkHandling);
                    });
                    this.CsvDelimiter.viewModel.text(data.CsvDelimiter);
                    this.MultipleValuesImportSeparator.viewModel.text(data.MultipleValuesSeparator);
                },
            });
        },

        //Called by the main button. Indirectly triggers ImportData once all files uploaded
        UploadFiles: function() {
            this.ProgressIndicator.viewModel.show();

            if (this.SourceFile.viewModel.totalFiles() > 0) {
                this.SourceFile.viewModel.upload();
            } else {
                this.MessageBar.addMessage("warning", "Please select file(s) to import");
                this.ProgressIndicator.viewModel.hide();
            }
        },

        FileUploaded: function(model) {
            this.uploadedFiles.push(model.itemId);
            this.SourceFile.viewModel.refreshNumberFiles();
            if (this.SourceFile.viewModel.globalPercentage() === 100) {
                this.ImportData();
            }
        },

        ImportData: function() {
            var location = this.ImportLocationTreeView.viewModel.selectedItemId();
            var language = this.TargetLanguageCombo.viewModel.selectedItemId();
            var existingItemHandling = this.ExistingItemHandling.viewModel.selectedItemId();
            var invalidLinkHandling = this.InvalidLinkHandling.viewModel.selectedItem().itemName;
            var csvDelimiter = this.CsvDelimiter.viewModel.text();
            var multipleValuesSeparator = this.MultipleValuesImportSeparator.viewModel.text();
            var mappingId = this.ExistingMapping.viewModel.selectedItemId();
            if (language == null) {
                this.MessageBar.addMessage("warning", "Please select language for import");
            }
            if (location == null) {
                this.MessageBar.addMessage("warning", "Please select import location");
            }
            for (var i = 0; i < this.uploadedFiles.length; i++) {

                var item = {
                    MappingId: mappingId,
                    ImportLocationId: location,
                    Language: language,
                    ExistingItemHandling: existingItemHandling,
                    InvalidLinkHandling: invalidLinkHandling,
                    CsvDelimiter: csvDelimiter,
                    MultipleValuesSeparator: multipleValuesSeparator,
                    MediaItemId: this.uploadedFiles[i]
                };
                console.log(item);

                $.ajax({
                    url: "/sitecore/api/ssc/EzImporter-Controllers/Import/1/Import",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    context: this,
                    success: function (data) {
                        if (data.HasError == true) {
                            this.ErrorDialogMessageBar.addMessage("error", data.ErrorMessage);
                            this.ErrorDialog.show();
                        } else {
                            this.LogInfo.viewModel.text(data.Log);
                        }
                    },
                    data: JSON.stringify(item)
                });
            }

            this.ProgressIndicator.viewModel.hide();
        }
    });

    return DataImporter;
});