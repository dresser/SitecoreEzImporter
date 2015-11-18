using Sitecore.Data.Fields;

namespace EzImporter.FieldUpdater
{
    public class TextFieldUpdater : IFieldUpdater
    {
        public void UpdateField(Field field, string importValue)
        {
            field.Value = importValue;
        }
    }
}