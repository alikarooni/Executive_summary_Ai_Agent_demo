using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Word_Creator.Entities
{
    public class DocumentPart
    {
        public string Text { get; set; } = string.Empty;
        public bool MustBeSent { get; set; }
        public bool IsDrawing { get; set; }
        public string Reponse { get; set; } = string.Empty;
    }
}
