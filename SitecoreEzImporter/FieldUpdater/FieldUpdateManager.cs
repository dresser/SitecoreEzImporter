using Sitecore.Data.Fields;

namespace EzImporter.FieldUpdater
{
    public class FieldUpdateManager
    {
        public static void UpdateField(Field field, string importValue)
        {
            IFieldUpdater updater = GetFieldUpdater(field);
            updater.UpdateField(field, importValue);
        }

        private static IFieldUpdater GetFieldUpdater(Field field)
        {
            if (field.Type == "Droplink")
            {
                return new DropLinkFieldUpdater();
            }
            if (field.Type == "Droptree")
            {
                return new DropTreeFieldUpdater();
            }
            if (field.Type == "Checklist")
            {
                return new MultiLinkFieldUpdater();
            }
            return new TextFieldUpdater();
        }
    }
}