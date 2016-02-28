define(["sitecore"], function (Sitecore) {
    var DataImporter = Sitecore.Definitions.App.extend({

        filesUploaded: [],

        initialize: function () {
            this.on("upload-fileUploaded", this.FileUploaded, this);
        },

        UploadFiles: function () {
            this.pi.viewModel.show();

            if (this.SourceFile.viewModel.totalFiles() > 0) {
                this.SourceFile.viewModel.upload();
            } else {
                this.MessageBar.addMessage("warning", "Please select file(s) to import");
                this.pi.viewModel.hide();
            }

        },

        ImportData: function () {
            var location = this.ImportLocationTreeView.viewModel.selectedItemId();
            var language = this.TargetLanguageCombo.viewModel.selectedItemId();
            var existingItemHandling = this.ExistingItemHandling.viewModel.selectedItemId();
            var invalidLinkHandling = this.InvalidLinkHandling.viewModel.selectedItem();
            var csvDelimiter = this.CsvDelimiter.viewModel.value;
            var multipleValuesSeparator = this.MultipleValuesImportSeparator.viewModel.value;

            alert(location + "\n" + language + "\n" + existingItemHandling + "\n" + invalidLinkHandling + "\n" + csvDelimiter + "\n" + multipleValuesSeparator);
            if (language == null) {
                alert('null language');
                this.MessageBar.addMessage("warning", "Please select language for import");
            }

            if (location == null) {
                alert('null import location');
                this.MessageBar.addMessage("warning", "Please select import location");
                alert('null import location');
            }

            for (var i = 0; i < this.filesUploaded.length; i++) {

                //var item = {
                //    TemplateId: template,
                //    ParentId: folder,
                //    MediaItemId: this.filesUploaded[i]
                //};

                alert('button clicked');
                //$.ajax({
                //    url: "/sitecore/api/ssc/MikeRobbins-SitecoreDataImporter-Controllers/Item/1/ImportItems",
                //    type: "POST",
                //    contentType: "application/json; charset=utf-8",
                //    dataType: "json",
                //    context: this,
                //    success: function () {
                //        this.GetImportAudit(this.filesUploaded[i - 1]);
                //    },
                //    data: JSON.stringify(item)
                //});
            }

            this.pi.viewModel.hide();
        },

        FileUploaded: function (model) {

            this.filesUploaded.push(model.itemId);

            this.SourceFile.viewModel.refreshNumberFiles();

            if (this.SourceFile.viewModel.globalPercentage() === 100) {
                this.ImportData();
            }
        }
    });

    return DataImporter;
});