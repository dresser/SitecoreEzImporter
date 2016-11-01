using System.Collections.Generic;
using Sitecore.Data;

namespace EzImporter.Map
{
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