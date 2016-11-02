using Sitecore.Data;
using System.Collections.Generic;
using System.Diagnostics;

namespace EzImporter.Map
{
    [DebuggerDisplay("Name={Name} Children={Children.Count}")]
    public class ItemDto
    {
        public string Name { get; set; }
        public ID TemplateId { get; set; }
        public Dictionary<string, string> Fields { get; set; }
        public ItemDto Parent { get; set; }
        public List<ItemDto> Children { get; set; }

        public ItemDto(string name)
        {
            Name = name;
            Fields = new Dictionary<string, string>();
            Children = new List<ItemDto>();
        }
    }
}