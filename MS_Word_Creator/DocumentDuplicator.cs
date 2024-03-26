using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Drawing = DocumentFormat.OpenXml.Spreadsheet.Drawing;
using Picture = DocumentFormat.OpenXml.Wordprocessing.Picture;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace MS_Word_Creator;

public class DocumentDuplicator
{
    // text, should be sent, is drawing
    private readonly List<(string, bool, bool)> _dataset = new List<(string, bool, bool)>();
    private readonly AskChatGpt? _askChatGpt = null;
    private bool isDrawing = false;

    private enum elementEnum
    {
        Paragraph,
        Table,
        None
    }

    public DocumentDuplicator()
    {
        _askChatGpt = new AskChatGpt();
    }

    public async Task CopyAndModifyWordDocument(string sourcePath, string destinationPath)
    {
        // Ensure the destination directory exists
        var destinationDirectory = Path.GetDirectoryName(destinationPath);
        if (!Directory.Exists(destinationDirectory))
        {
            if (destinationDirectory != null) Directory.CreateDirectory(destinationDirectory);
        }

        ReadDocument(sourcePath);

        var data = _dataset.Where(x => x.Item3 != true).Where(x => x.Item2 == true);

        List<string> response = await _askChatGpt.Request(data.Select(x=>x.Item1).ToList());

        UpdateDocument(sourcePath, response, destinationPath);
    }

    private void UpdateDocument(string sourcePath, List<string> response, string destinationPath)
    {
        using var sourceDocument = WordprocessingDocument.Open(sourcePath, false);

        // Create a new document for the destination
        using var destinationDocument = WordprocessingDocument.Create(destinationPath, sourceDocument.DocumentType);

        // Copy parts from source to destination
        foreach (var part in sourceDocument.Parts)
        {
            OpenXmlPart destinationPart = destinationDocument.AddPart(part.OpenXmlPart, part.RelationshipId);

            // If the part is a main document part, modify its content
            if (destinationPart is MainDocumentPart)
            {
                UpdateContent((MainDocumentPart)destinationPart, response);
            }
        }
    }

    private void UpdateContent(MainDocumentPart mainPart, List<string> response)
    {
        var lastElement = elementEnum.None;
        var chatGptResponseIndex = -1;
        var datasetIndex = 0;
        var paragraphIndex = 0;

        foreach (var element in mainPart.Document.Body.Elements())
        {
            if (element is Paragraph paragraph)
            {
                if (string.IsNullOrEmpty(GetParagraphText(paragraph)))
                {
                    continue;
                }

                bool paragraphNeedsAnUpdate = XmlElementValueShouldBeUpdated(datasetIndex);
                if (lastElement != elementEnum.Paragraph)
                {
                    // reset
                    paragraphIndex = 0;
                    //datasetIndex += 1;

                    // if next element needs an update
                    if (paragraphNeedsAnUpdate)
                    {
                        chatGptResponseIndex += 1;
                    }
                }

                if (paragraphNeedsAnUpdate)
                {
                    string updatedParagraph = response.ToArray()[chatGptResponseIndex].Split('\n')[paragraphIndex];
                    // index 1 to pass PARAGRAPH cell value. Paragraphs always have 2 cells
                    updatedParagraph = updatedParagraph.Split("^")[1];
                    SetParagraphText(paragraph, updatedParagraph);

                    paragraphIndex += 1;
                }

                lastElement = elementEnum.Paragraph;
            }
            else if (element is Table table)
            {
                if (lastElement == elementEnum.Paragraph)
                {
                    datasetIndex += 1;
                }

                bool tableShouldBeUpdated = XmlElementValueShouldBeUpdated(datasetIndex);
                // if next element needs an update
                if (tableShouldBeUpdated)
                {
                    chatGptResponseIndex += 1;
                }
                else
                {
                    datasetIndex += 1;
                    lastElement = elementEnum.Table;
                    continue;
                }

                int rowIndex = 0;
                string updatedTable = response.ToArray()[chatGptResponseIndex];

                foreach (TableRow row in table.Elements<TableRow>())
                {
                    string updatedRow = updatedTable.Split('\n')[rowIndex];
                    // starting from 1 to pass ROW{i} value
                    int cellIndex = 1;

                    foreach (var cell in row.Elements())
                    {
                        if (tableShouldBeUpdated)
                        {
                            if (cell is TableCell tableCell)
                            {
                                string updatedCell = updatedRow.Split('^')[cellIndex];
                                SetTableCellText(tableCell, updatedCell);
                                cellIndex += 1;
                            }
                            else if (cell is SdtCell sdtCell)
                            {
                                string updatedCell = updatedRow.Split('^')[cellIndex];
                                SetSdtCellText(sdtCell, updatedCell);
                                cellIndex += 1;
                            }
                        }
                    }
                    rowIndex += 1;
                }

                datasetIndex += 1;
                lastElement = elementEnum.Table;
            }
        }
    }

    private void ReadDocument(string sourcePath)
    {
        using var sourceDocument = WordprocessingDocument.Open(sourcePath, false);
        foreach (var part in sourceDocument.Parts)
        {
            if (part.OpenXmlPart is MainDocumentPart openXmlPart)
            {
                CollectInformation(openXmlPart);
            }
        }
    }

    private void CollectInformation(MainDocumentPart mainPart)
    {
        var lastElement = elementEnum.None;
        string paragrapghData = "";
        int i = 1;

        foreach (var element in mainPart.Document.Body.Elements())
        {
            if (element is Paragraph paragraph)
            {
                if (lastElement != elementEnum.Paragraph)
                {
                    if (paragrapghData != "")
                    {
                        _dataset.Add((paragrapghData, ShouldBeSentToChatGpt(paragrapghData), false));
                    }

                    paragrapghData = "";
                    i = 1;
                }
                
                lastElement = elementEnum.Paragraph;

                var paragraphText = GetParagraphText(paragraph);
                if (!string.IsNullOrEmpty(paragraphText))
                {
                    paragraphText = $"PARAGRAPH:^{paragraphText}";
                    paragrapghData += paragraphText + '\n';
                }

                i += 1;
            }
            else if (element is Table table)
            {
                if (lastElement == elementEnum.Paragraph)
                {
                    if (paragrapghData != "")
                    {
                        _dataset.Add((paragrapghData, ShouldBeSentToChatGpt(paragrapghData), false));
                        paragrapghData = "";
                    }
                }

                lastElement = elementEnum.Table;

                string tableData = "";
                string csvRow = "";
                int j = 1;

                //var columns = table.Elements<TableGrid>().FirstOrDefault().Elements<GridColumn>().Count();
                foreach (TableRow row in table.Elements<TableRow>())
                {
                    csvRow = $"ROW_{j}: ";
                    foreach (var cell in row.Elements())
                    {
                        if (cell is TableCell tableCell)
                        {
                            string cellText = GetTableCellText(tableCell);
                            csvRow += "^" + cellText;
                        }
                        else if(cell is SdtCell sdtCell)
                        {
                            string cellText = GetSdtCellText(sdtCell);

                            csvRow += "^" + cellText;
                        }
                    }

                    j += 1;
                    tableData += csvRow + '\n';
                }

                _dataset.Add((tableData, ShouldBeSentToChatGpt(tableData), isDrawing));
                isDrawing = false;
                tableData = "";
            }
        }

        lastElement = elementEnum.None;
    }

    private bool ShouldBeSentToChatGpt(string stringData) => (stringData.Contains("[") && stringData.Contains("]")) || stringData.Contains("***");
    private bool XmlElementValueShouldBeUpdated(int datasetIndex) => _dataset.Select(x => x.Item2).ToArray()[datasetIndex] && (!_dataset.Select(x => x.Item3).ToArray()[datasetIndex]);

    private string GetParagraphText(Paragraph paragraph)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var element in paragraph.Elements())
        {
            if (element is Run run)
            {
                // is picture
                if (run.Elements<DocumentFormat.OpenXml.Wordprocessing.Drawing>().Count() > 0)
                {
                    isDrawing = true;
                }

                foreach (var text in run.Elements<Text>())
                {
                    if ((!text.Text.Contains("[")) && (!text.Text.Contains("]")) && text.Text.Contains("***"))
                    {
                        sb.Append("[" + text.Text + "]");
                    }
                    else
                    {
                        sb.Append(text.Text);
                    }
                }
            }
            else if (element is SdtRun sdtRun)
            {
                foreach (var sdt in sdtRun.Descendants<Run>())
                {
                    foreach (var text in sdt.Elements<Text>())
                    {
                        sb.Append("[" + text.Text + "]");
                    }
                }
            }
        }
        
        return sb.ToString();
    }

    private void SetParagraphText(Paragraph? paragraph, string newText)
    {
        if (paragraph != null)
        {
            // If there is an existing Paragraph, clear its runs
            paragraph.RemoveAllChildren<Run>();
            paragraph.RemoveAllChildren<SdtRun>();

            // Create a new run with the new text
            var run = new Run(new Text(newText));

            // Append the run to the existing Paragraph to preserve Paragraph properties
            paragraph.Append(run);
        }
        else
        {
            // If there are no Paragraph in the cell, create a new Paragraph
            paragraph = new Paragraph();
            var run = new Run(new Text(newText));
            paragraph.Append(run);
        }
    }

    private string GetSdtCellText(SdtCell cell)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var sdtContentCell in cell.Elements<SdtContentCell>())
        {
            foreach (var paragraph in sdtContentCell.Elements<TableCell>().SingleOrDefault().Elements<Paragraph>())
            {
                string paragraphText = GetParagraphText(paragraph);
                if(paragraphText != "")
                    sb.Append("[" + paragraphText + "]");
            }
        }

        return sb.ToString();
    }

    private string GetTableCellText(TableCell cell)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var element in cell.Elements())
        {
            if (element is Paragraph paragraph)
            {
                sb.Append(GetParagraphText(paragraph));
            }
            else if (element is Table table)
            {

            }
        }

        return sb.ToString();
    }

    private void SetTableCellText(TableCell cell, string newText)
    {
        //for now, we replace all paragraphs with only one
        foreach (var paragraph in cell.Elements<Paragraph>().Skip(1).ToList())
        {
            paragraph.Remove();
        }

        // Check if the cell already contains a paragraph
        Paragraph para = cell.Elements<Paragraph>().FirstOrDefault();
        SetParagraphText(para, newText);

        if (para == null)
        {
            cell.Append(para);
        }
    }

    private void SetSdtCellText(SdtCell cell, string newText)
    {
        //for now, we replace all paragraphs with only one
        foreach (var paragraph in cell.Elements<SdtContentCell>().SingleOrDefault().Elements<TableCell>().SingleOrDefault().Elements<Paragraph>().Skip(1))
        {
            paragraph.Remove();
        }

        // Check if the cell already contains a paragraph
        Paragraph para = cell.Elements<SdtContentCell>().SingleOrDefault().Elements<TableCell>().SingleOrDefault().Elements<Paragraph>().FirstOrDefault();
        SetParagraphText(para, newText);

        if (para == null)
        {
            cell.Append(para);
        }
    }
}
