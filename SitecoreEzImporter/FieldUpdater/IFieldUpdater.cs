using Sitecore.Data.Fields;

namespace EzImporter.FieldUpdater
{
    public interface IFieldUpdater
    {
        void UpdateField(Field field, string importValue);
    }
}
