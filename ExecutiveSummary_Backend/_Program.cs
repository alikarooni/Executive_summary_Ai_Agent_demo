using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;

//class Program
//{
//    static void Main(string[] args)
//    {
//        string sourcePath = @"C:\Users\ali.karooni\Documents\_My_Projects\Executive summary demo\MS_Word_Creator\docs\Savills_TEDD_Report_Client_v2.docx";
//        string destinationPath = @"C:\Users\ali.karooni\Documents\_My_Projects\Executive summary demo\MS_Word_Creator\docs\new_document.docx";

//        CopyAndModifyWordDocument(sourcePath, destinationPath);
//    }

//    private static void CopyAndModifyWordDocument(string sourcePath, string destinationPath)
//    {
//        // Ensure the destination directory exists
//        var destinationDirectory = Path.GetDirectoryName(destinationPath);
//        if (!Directory.Exists(destinationDirectory))
//        {
//            Directory.CreateDirectory(destinationDirectory);
//        }

//        // Open the source document for reading
//        using (WordprocessingDocument sourceDocument = WordprocessingDocument.Open(sourcePath, false))
//        {
//            // Create a new document for the destination
//            using (WordprocessingDocument destinationDocument = WordprocessingDocument.Create(destinationPath, sourceDocument.DocumentType))
//            {
//                // Copy parts from source to destination
//                foreach (var part in sourceDocument.Parts)
//                {
//                    OpenXmlPart destinationPart = destinationDocument.AddPart(part.OpenXmlPart, part.RelationshipId);

//                    // If the part is a main document part, modify its content
//                    if (destinationPart is MainDocumentPart)
//                    {
//                        ModifyContent((MainDocumentPart)destinationPart);
//                    }
//                }
//            }
//        }
//    }

//    private static void ModifyContent(MainDocumentPart mainPart)
//    {
//        // Modify paragraphs
//        ModifyParagraphs(mainPart);

//        // Modify tables
//        ModifyTables(mainPart);
//    }

//    private static void ModifyParagraphs(MainDocumentPart mainPart)
//    {
//        foreach (Paragraph paragraph in mainPart.Document.Body.Elements<Paragraph>())
//        {
//            foreach (var run in paragraph.Elements<Run>())
//            {
//                foreach (var text in run.Elements<Text>())
//                {
//                    text.Text = "Modified: " + text.Text;
//                }
//            }
//        }
//    }

//    private static void ModifyTables(MainDocumentPart mainPart)
//    {
//        foreach (Table table in mainPart.Document.Body.Elements<Table>())
//        {
//            foreach (TableRow row in table.Elements<TableRow>())
//            {
//                foreach (TableCell cell in row.Elements<TableCell>())
//                {
//                    // Here you can modify the cell as needed
//                    string cellText = GetCellText(cell);
//                    SetCellText(cell, "Modified: " + cellText);
//                }
//            }
//        }
//    }

//    private static string GetCellText(TableCell cell)
//    {
//        var sb = new System.Text.StringBuilder();
//        foreach (var para in cell.Elements<Paragraph>())
//        {
//            foreach (var run in para.Elements<Run>())
//            {
//                foreach (var text in run.Elements<Text>())
//                {
//                    sb.Append(text.Text);
//                }
//            }
//        }
//        return sb.ToString();
//    }

//    private static void SetCellText(TableCell cell, string text)
//    {
//        cell.RemoveAllChildren<Paragraph>();
//        var para = new Paragraph();
//        var run = new Run();
//        run.Append(new Text(text));
//        para.Append(run);
//        cell.Append(para);
//    }
//}
