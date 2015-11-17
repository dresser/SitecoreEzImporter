using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EzImporter
{
    public enum ExistingItemHandling
    {
        Skip = 0,
        AddVersion = 1,
        Update = 2
    }
}