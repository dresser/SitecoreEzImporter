<%@ Page Language="C#" Debug="true" AutoEventWireup="true" Inherits="Sitecore.sitecore.admin.AdminPage" %>
<%@ Import Namespace="EzImporter" %>
<%@ Import Namespace="EzImporter.Import.Item" %>
<%@ Import Namespace="EzImporter.Import.Media" %>
<%@ Import Namespace="EzImporter.Map" %>
<%@ Import Namespace="EzImporter.Pipelines.ImportItems" %>
<%@ Import Namespace="Sitecore.Configuration" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="Sitecore.Pipelines" %>

<script runat="server">
    protected override void OnInit(EventArgs args)
    {
        CheckSecurity(true);
        base.OnInit(args);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!Page.IsPostBack)
        {
            var db = Sitecore.Configuration.Factory.GetDatabase("master");
            var settings = EzImporter.Configuration.Settings.GetConfigurationSettings();
            var siteNodes = db.SelectItems(settings.RootItemQuery);
            ddlSites.DataSource = siteNodes.Select(n => new { n.Name, n.ID });
            ddlSites.DataBind();

            var languages = db.SelectItems("/sitecore/system/Languages//*");
            ddlLanguages.DataSource = languages;
            ddlLanguages.DataBind();

            var mediaFolders = db.SelectItems("/sitecore/media library/Master//*[@@name='Products']");
            ddlMediaFolder.DataSource = mediaFolders.Select(mf => new { mf.Parent.Name, mf.ID });
            ddlMediaFolder.DataBind();

            ddlDataImportMap.DataSource = db.SelectItems(settings.MapsLocation);
            ddlDataImportMap.DataBind();

            ddlMediaImportMap.DataSource = db.SelectItems("/sitecore/system/Modules/EzImporter//*[@@templatename='MediaImportMap']");
            ddlMediaImportMap.DataBind();
        }
    }

    protected void importItems_Click(object sender, EventArgs e)
    {
        homePanel.Visible = false;
        dataPanel.Visible = true;
    }

    protected void importMedia_Click(object sender, EventArgs e)
    {
        homePanel.Visible = false;
        mediaPanel.Visible = true;
    }

    protected void dataUploadNew_Click(object sender, EventArgs e)
    {
        dataUploadPanel.Visible = true;
        dataPanel.Visible = false;
    }

    private void CreateDirectoryIfNotFound(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    protected void uploadData_Click(object sender, EventArgs e)
    {
        var settings = EzImporter.Configuration.Settings.GetConfigurationSettings();
        var importDir = Server.MapPath(settings.ImportDirectory);
        CreateDirectoryIfNotFound(importDir);
        importDir = importDir + @"\" + settings.ImportItemsSubDirectory;
        CreateDirectoryIfNotFound(importDir);
        csvFileName.Value = importDir + @"\" + csvFile.FileName;
        csvFile.PostedFile.SaveAs(csvFileName.Value);

        dataUploadPanel.Visible = false;
        dataPanel.Visible = true;
        selectedDataFile.Text = csvFile.FileName;
    }

    protected void dataSelectExisting_Click(object sender, EventArgs e)
    {
        var settings = EzImporter.Configuration.Settings.GetConfigurationSettings();
        var importDir = Server.MapPath(settings.ImportDirectory);
        CreateDirectoryIfNotFound(importDir);
        importDir = Server.MapPath(settings.ImportDirectory + @"\" + settings.ImportItemsSubDirectory);
        CreateDirectoryIfNotFound(importDir);
        var importDirectory = new DirectoryInfo(importDir);
        existingFiles.Items.AddRange(importDirectory.GetFiles().Select(f => new ListItem(f.Name)).ToArray());
        dataSelectExistingPanel.Visible = true;
        dataPanel.Visible = false;
    }

    protected void dataSelectExistingContinue_Click(object sender, EventArgs e)
    {
        var settings = EzImporter.Configuration.Settings.GetConfigurationSettings();
        var importDir = Server.MapPath(settings.ImportDirectory + @"\" + settings.ImportItemsSubDirectory);
        csvFileName.Value = importDir + @"\" + existingFiles.SelectedItem.Text;
        selectedDataFile.Text = existingFiles.SelectedItem.Text;
        dataSelectExistingPanel.Visible = false;
        dataPanel.Visible = true;
    }

    protected void dataSelectExistingCancel_Click(object sender, EventArgs e)
    {
        dataSelectExistingPanel.Visible = false;
        dataPanel.Visible = true;
    }

    protected void dataPanelNext_Click(object sender, EventArgs e)
    {
        dataPanel.Visible = false;
        processDataPanel.Visible = true;
    }

    protected void mediaUploadNew_Click(object sender, EventArgs e)
    {
        mediaPanel.Visible = false;
        mediaUploadPanel.Visible = true;
    }

    protected void mediaSelectExisting_Click(object sender, EventArgs e)
    {
        var settings = EzImporter.Configuration.Settings.GetConfigurationSettings();
        var importDir = Server.MapPath(settings.ImportDirectory + @"\" + settings.ImportMediaSubDirectory);
        CreateDirectoryIfNotFound(importDir);
        var importDirectory = new DirectoryInfo(importDir);
        existingMediaFiles.Items.AddRange(importDirectory.GetFiles().Select(f => new ListItem(f.Name)).ToArray());
        mediaPanel.Visible = false;
        mediaSelectExistingPanel.Visible = true;
    }

    protected void mediaPanelNext_Click(object sender, EventArgs e)
    {
        mediaPanel.Visible = false;
        processMediaPanel.Visible = true;
    }

    protected void uploadMedia_Click(object sender, EventArgs e)
    {
        var settings = EzImporter.Configuration.Settings.GetConfigurationSettings();
        var importDir = Server.MapPath(settings.ImportDirectory);
        CreateDirectoryIfNotFound(importDir);
        importDir = importDir + @"\" + settings.ImportMediaSubDirectory;
        CreateDirectoryIfNotFound(importDir);
        imagesZipFileName.Value = importDir + @"\" + imagesZip.FileName;
        imagesZip.PostedFile.SaveAs(imagesZipFileName.Value);
        selectedMediaFile.Text = imagesZip.FileName;
        
        mediaUploadPanel.Visible = false;
        mediaPanel.Visible = true;
    }

    protected void uploadMediaCancel_Click(object sender, EventArgs e)
    {
        mediaUploadPanel.Visible = false;
        mediaPanel.Visible = true;
    }

    protected void mediaSelectExistingCancel_Click(object sender, EventArgs e)
    {
        mediaSelectExistingPanel.Visible = false;
        mediaPanel.Visible = true;
    }

    protected void mediaSelectExistingContinue_Click(object sender, EventArgs e)
    {
        var settings = EzImporter.Configuration.Settings.GetConfigurationSettings();
        var importDir = Server.MapPath(settings.ImportDirectory + @"\" + settings.ImportMediaSubDirectory);
        imagesZipFileName.Value = importDir + @"\" + existingMediaFiles.SelectedItem.Text;
        selectedMediaFile.Text = existingMediaFiles.SelectedItem.Text;
        mediaSelectExistingPanel.Visible = false;
        mediaPanel.Visible = true;
    }

    private void processData_OnClick(object sender, EventArgs e)
    {
        var args = new ImportItemsArgs
        {
            Database = Sitecore.Configuration.Factory.GetDatabase("master"),
            //FileName = csvFileName.Value,
            RootItemId = new ID(ddlSites.SelectedValue),
            TargetLanguage = Sitecore.Globalization.Language.Parse(ddlLanguages.SelectedValue),
            Map = EzImporter.Map.Factory.BuildMapInfo(new ID(ddlDataImportMap.SelectedValue)),
            ImportOptions = EzImporter.Configuration.Factory.GetDefaultImportOptions()
        };
        CorePipeline.Run("importItems", args);
        //output.Text = args.Statistics.Log.ToString();
    }

    private void processImages_Click(object sender, EventArgs e)
    {
        var settings = EzImporter.Configuration.Settings.GetConfigurationSettings();
        var database = Sitecore.Configuration.Factory.GetDatabase("master");
        var args = new MediaImportTaskArgs
        {
            ZipFileName = imagesZipFileName.Value,
            ExtractionFolder = Server.MapPath(settings.ImportDirectory + "/ExtractedFiles"),
            Database = database,
            RootMediaItemId = new ID(ddlMediaFolder.SelectedValue),
            RootDataItemId = new ID(ddlSites.SelectedValue),
            TargetLanguage = Sitecore.Globalization.Language.Parse(ddlLanguages.SelectedValue),
            MediaImportMap = EzImporter.Map.Factory.GetMediaImportMap(new ID(ddlMediaImportMap.SelectedValue), database)
        };
        var task = new MediaImportTask();
        var result = task.Run(args);
        imageImportOutput.Text = result.Replace(Environment.NewLine, "<br/>");
    }

</script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>EzImporter</h1>
        <asp:Panel ID="homePanel" runat="server" Visible="True">
            <p>
                <asp:Button ID="importItems" OnClick="importItems_Click" Text="Import Items" runat="server"/>
            </p>
            <p>
                <asp:Button ID="importMedia" OnClick="importMedia_Click" Text="Import Media" runat="server"/>
            </p>
        </asp:Panel>
        <asp:Panel ID="dataPanel" Visible="false" runat="server">
            <p>Data Panel</p>
            <p>
                <asp:Label AssociatedControlID="selectedDataFile" Text="Data File" runat="server" />
                <asp:TextBox ID="selectedDataFile" runat="server"/>
            </p>
            <p>
                <asp:Button Text="Select Existing" OnClick="dataSelectExisting_Click" runat="server"/>
                <asp:Button Text="Upload New" OnClick="dataUploadNew_Click" runat="server"/>
            </p>
            <p>
                <asp:Button OnClick="dataPanelNext_Click" Text="Next" runat="server" />
            </p>            
        </asp:Panel>
        <asp:Panel ID="dataUploadPanel" Visible="false" runat="server">
            <p>Upload Panel</p>
            <p>
                <asp:Label AssociatedControlID="csvFile" Text="Data (Supported format: CSV, XLS, XLSX)" runat="server" />
            </p>
            <p>
                <asp:FileUpload ID="csvFile" AllowMultiple="false" runat="server" />
            </p>            
            <asp:Button ID="upload" OnClick="uploadData_Click" Text="Upload" runat="server" />
        </asp:Panel>
        <asp:Panel ID="dataSelectExistingPanel" Visible="false" runat="server">
            <p>Select Existing data Panel</p>
            <p>
                <asp:Label AssociatedControlID="existingFiles" Text="Existing Files:" runat="server" />
                <asp:ListBox ID="existingFiles" SelectionMode="Single" runat="server">
                </asp:ListBox>
            </p>            
            <asp:Button OnClick="dataSelectExistingContinue_Click" Text="Select" runat="server" />
            <asp:Button OnClick="dataSelectExistingCancel_Click" Text="Back" runat="server" />
        </asp:Panel>
        <asp:Panel ID="processDataPanel" Visible="false" runat="server">
            <asp:Label AssociatedControlID="ddlLanguages" Text="Select target language" runat="server" />
            <asp:DropDownList ID="ddlLanguages" DataTextField="Name" DataValueField="Name" runat="server" />

            <asp:HiddenField ID="csvFileName" runat="server" />
            <asp:HiddenField ID="imagesZipFileName" runat="server" />
            <div class="dataImporter">
                <h2>Product Data</h2>

                <p>
                    <asp:Label AssociatedControlID="ddlDataImportMap" Text="Select Data Import Map" runat="server" />
                    <asp:DropDownList ID="ddlDataImportMap" DataTextField="Name" DataValueField="ID" runat="server" />
                </p>
                <p>
                    <asp:Label AssociatedControlID="ddlSites" Text="Select node to import data to" runat="server" />
                    <asp:DropDownList ID="ddlSites" DataTextField="Name" DataValueField="ID" runat="server" />
                </p>
                <asp:Button ID="process" OnClick="processData_OnClick" Text="Import" runat="server"/>
                <div>
                    <p><asp:Literal ID="output" runat="server"/></p>
                </div>
            </div>
        </asp:Panel>
        <asp:Panel ID="mediaPanel" Visible="false" runat="server">
            <p>Media</p>
            <p>
                <asp:Label AssociatedControlID="selectedMediaFile" Text="Media File" runat="server" />
                <asp:TextBox ID="selectedMediaFile" runat="server"/>
            </p>
            <p>
                <asp:Button Text="Select Existing" OnClick="mediaSelectExisting_Click" runat="server"/>
                <asp:Button Text="Upload New" OnClick="mediaUploadNew_Click" runat="server"/>
            </p>
            <p>
                <asp:Button OnClick="mediaPanelNext_Click" Text="Next" runat="server" />
            </p>            
        </asp:Panel>
        <asp:Panel ID="mediaUploadPanel" Visible="false" runat="server">           
            <p>
                <asp:Label AssociatedControlID="imagesZip" Text="Media (ZIP)" runat="server" />
            </p>
            <p>
                <asp:FileUpload ID="imagesZip" AllowMultiple="false" runat="server" />
            </p>
            <asp:Button ID="uploadMedia" OnClick="uploadMedia_Click" Text="Upload" runat="server" />
            <asp:Button OnClick="uploadMediaCancel_Click" Text="Back" runat="server" />
        </asp:Panel>
        <asp:Panel ID="mediaSelectExistingPanel" Visible="false" runat="server">
            <p>Select Existing Media</p>
            <p>
                <asp:Label AssociatedControlID="existingMediaFiles" Text="Existing Files:" runat="server" />
                <asp:ListBox ID="existingMediaFiles" SelectionMode="Single" runat="server">
                </asp:ListBox>
            </p>            
            <asp:Button OnClick="mediaSelectExistingContinue_Click" Text="Select" runat="server" />
            <asp:Button OnClick="mediaSelectExistingCancel_Click" Text="Back" runat="server" />
        </asp:Panel>
        <asp:Panel ID="processMediaPanel" Visible="False" runat="server">
            <div class="imageImporter">
                <h2>Image Data</h2>

                <p>
                    <asp:Label ID="Label1" AssociatedControlID="ddlMediaImportMap" Text="Select Image Import Map" runat="server" />
                    <asp:DropDownList ID="ddlMediaImportMap" DataTextField="Name" DataValueField="ID" runat="server" />
                </p>
                <p>
                    <asp:Label ID="Label3" AssociatedControlID="ddlMediaFolder" Text="Select folder to import images to" runat="server" />
                    <asp:DropDownList ID="ddlMediaFolder" DataTextField="Name" DataValueField="ID" runat="server" />
                </p>
                <asp:Button ID="processImages" OnClick="processImages_Click" Text="Import" runat="server" />
                <div>
                    <p><asp:Literal ID="imageImportOutput" runat="server"/></p>
                </div>
            </div>
        </asp:Panel>
    </div>
    </form>
</body>
</html>
