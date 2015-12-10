using System;
using EzImporter.Configuration;

namespace EzImporter.FieldUpdater
{
    public abstract class FieldUpdaterBase : IFieldUpdater
    {
        public void UpdateField(Sitecore.Data.Fields.Field field, string importValue, IImportOptions importOptions)
        {
            throw new NotImplementedException();
        }
    }
}