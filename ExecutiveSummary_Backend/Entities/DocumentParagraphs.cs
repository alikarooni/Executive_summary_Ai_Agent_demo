using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Word_Creator.Entities
{
    public class DocumentParagraphs
    {
        public string Paragraph { get; set; } = string.Empty;
        public bool HasToGetUpdated { get; set; }
        public bool HasDrawing { get; set; }
        public bool IsTableRow { get; set; }
        public string GPT_Reponse { get; set; } = string.Empty;
    }
}
