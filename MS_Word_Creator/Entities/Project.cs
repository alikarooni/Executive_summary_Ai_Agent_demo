using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Word_Creator.Entities
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectType { get; set; }
        public List<string> ProjectFiles { get; set; } = new();
        public List<DocumentParagraphs> DocumentParts { get; set; } = new();
    }
}
