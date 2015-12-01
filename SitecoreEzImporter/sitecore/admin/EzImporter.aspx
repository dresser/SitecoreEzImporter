<%@ Page Language="C#" Debug="true" AutoEventWireup="true" Inherits="Sitecore.sitecore.admin.AdminPage" %>
<%@ Import Namespace="EzImporter" %>
<%@ Import Namespace="EzImporter.Import.Item" %>
<%@ Import Namespace="EzImporter.Import.Media" %>
<%@ Import Namespace="Sitecore.Configuration" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="System.IO" %>

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

            var siteNodes = db.SelectItems(EzImporter.Settings.RootItemQuery);
            ddlSites.DataSource = siteNodes.Select(n => new { n.Name, n.ID });
            ddlSites.DataBind();

            var languages = db.SelectItems("/sitecore/system/Languages//*");
            ddlLanguages.DataSource = languages;
            ddlLanguages.DataBind();

            var mediaFolders = db.SelectItems("/sitecore/media library/Master//*[@@name='Products']");
            ddlMediaFolder.DataSource = mediaFolders.Select(mf => new { mf.Parent.Name, mf.ID });
            ddlMediaFolder.DataBind();

            ddlDataImportMap.DataSource = db.SelectItems(EzImporter.Settings.MapsLocation); // "/sitecore/system/Modules/EzImporter//*[@@templatename='ImportMap']");
            ddlDataImportMap.DataBind();

            ddlMediaImportMap.DataSource = db.SelectItems("/sitecore/system/Modules/EzImporter//*[@@templatename='MediaImportMap']");
            ddlMediaImportMap.DataBind();
        }
    }

    protected void importItems_Click(object sender, EventArgs e)
    {
        homePanel.Visible = false;
        dataUploadPanel.Visible = true;
    }

    protected void importMedia_Click(object sender, EventArgs e)
    {
        homePanel.Visible = false;
        mediaUploadPanel.Visible = true;
    }

    protected void uploadData_Click(object sender, EventArgs e)
    {
        var importDir = Server.MapPath(EzImporter.Settings.ImportDirectory);
        if (!Directory.Exists(importDir))
        {
            Directory.CreateDirectory(importDir);
        }
        importDir = importDir + @"\Items";
        if (!Directory.Exists(importDir))
        {
            Directory.CreateDirectory(importDir);
        }
        csvFileName.Value = importDir + @"\" + csvFile.FileName;
        csvFile.PostedFile.SaveAs(csvFileName.Value);

        dataUploadPanel.Visible = false;
        processDataPanel.Visible = true;
    }

    protected void uploadMedia_Click(object sender, EventArgs e)
    {
        var importDir = Server.MapPath(EzImporter.Settings.ImportDirectory);
        if (!Directory.Exists(importDir))
        {
            Directory.CreateDirectory(importDir);
        }
        importDir = importDir + @"\Media";
        if (!Directory.Exists(importDir))
        {
            Directory.CreateDirectory(importDir);
        }
        imagesZipFileName.Value = importDir + @"\" + imagesZip.FileName;
        imagesZip.PostedFile.SaveAs(imagesZipFileName.Value);

        mediaUploadPanel.Visible = false;
        processMediaPanel.Visible = true;
    }

    private string GetExtension(string fileName)
    {
        if (fileName == null)
        {
            return null;
        }
        var i = fileName.LastIndexOf(".");
        if (i == -1 || i == fileName.Length - 1)
        {
            return null;
        }
        return fileName.Substring(i);
    }

    private void processData_OnClick(object sender, EventArgs e)
    {
        var args = new ItemImportTaskArgs
        {
            Database = Sitecore.Configuration.Factory.GetDatabase("master"),
            FileName = csvFileName.Value,
            RootItemId = new ID(ddlSites.SelectedValue),
            TargetLanguage = Sitecore.Globalization.Language.Parse(ddlLanguages.SelectedValue),
            Map = ItemImportMap.BuildMapInfo(new ID(ddlDataImportMap.SelectedValue)),
            ExistingItemHandling = EzImporter.Settings.ExistingItemHandling
        };
        var task = new ItemImportTask();
        var result = task.Run(args);
        output.Text = result.Replace(Environment.NewLine, "<br/>");
    }

    private void processImages_Click(object sender, EventArgs e)
    {
        var database=Sitecore.Configuration.Factory.GetDatabase("master");
        var args = new MediaImportTaskArgs
        {
            ZipFileName = imagesZipFileName.Value,
            ExtractionFolder = Server.MapPath(EzImporter.Settings.ImportDirectory + "/ExtractedFiles"),
            Database = database,
            RootMediaItemId = new ID(ddlMediaFolder.SelectedValue),
            RootDataItemId = new ID(ddlSites.SelectedValue),
            TargetLanguage = Sitecore.Globalization.Language.Parse(ddlLanguages.SelectedValue),
            MediaImportMap = new MediaImportMap(new ID(ddlMediaImportMap.SelectedValue), database)
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
        <asp:Panel ID="dataUploadPanel" Visible="false" runat="server">
            <p>
                <asp:Label AssociatedControlID="csvFile" Text="Product Data (Supported format: CSV)" runat="server" />
            </p>
            <p>
                <asp:FileUpload ID="csvFile" AllowMultiple="false" runat="server" />
            </p>            
            <asp:Button ID="upload" OnClick="uploadData_Click" Text="Upload" runat="server" />
        </asp:Panel>
        <asp:Panel ID="mediaUploadPanel" Visible="false" runat="server">           
            <p>
                <asp:Label ID="Label2" AssociatedControlID="imagesZip" Text="Media (ZIP)" runat="server" />
            </p>
            <p>
                <asp:FileUpload ID="imagesZip" AllowMultiple="false" runat="server" />
            </p>
            <asp:Button ID="uploadMedia" OnClick="uploadMedia_Click" Text="Upload" runat="server" />
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
