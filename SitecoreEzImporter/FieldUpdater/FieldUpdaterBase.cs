using System;

namespace EzImporter.FieldUpdater
{
    public abstract class FieldUpdaterBase : IFieldUpdater
    {
        public void UpdateField(Sitecore.Data.Fields.Field field, string importValue)
        {
            throw new NotImplementedException();
        }
    }
}