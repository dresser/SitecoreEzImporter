define(["sitecore"], function(Sitecore) {
    var DataImporter = Sitecore.Definitions.App.extend({
        uploadedFiles: [],

        initialize: function() {
            this.on("upload-fileUploaded", this.FileUploaded, this);
            $.ajax({
                url: "/sitecore/api/EzImporter/DefaultSettings",
                type: "GET",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                context: this,
                success: function (data) {
                    this.ExistingItemHandling.viewModel.selectedValue(data.ExistingItemHandling);
                    this.ExistingItemHandling.viewModel.rebind();
                    this.InvalidLinkHandling.viewModel.selectedItem(data.InvalidLinkHandling);
                    this.CsvDelimiter.viewModel.text(data.CsvDelimiter);
                    this.MultipleValuesImportSeparator.viewModel.text(data.MultipleValuesSeparator);
                    console.log(data);
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
                    url: "/sitecore/api/EzImporter/Import",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    context: this,
                    success: function() {

                    },
                    data: JSON.stringify(item)
                });
            }

            this.ProgressIndicator.viewModel.hide();
        }
    });

    return DataImporter;
});