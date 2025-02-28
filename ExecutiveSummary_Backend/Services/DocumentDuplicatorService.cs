using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using MS_Word_Creator.Entities;
using MS_Word_Creator.Repositories;
using Drawing = DocumentFormat.OpenXml.Spreadsheet.Drawing;
using Picture = DocumentFormat.OpenXml.Wordprocessing.Picture;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace MS_Word_Creator.Services;


public interface IDocumentDuplicatorService
{
    Task CopyAndModifyWordDocument(int projectId);
}

public class DocumentDuplicatorService : IDocumentDuplicatorService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IChatGptService _chatGptSercice;
    private readonly char[] _updatableElements = { '[', ']', '*' };
    private bool isDrawing = false;
    private int _projectId = 0;

    private enum elementEnum
    {
        Paragraph,
        Table,
        None
    }

    public DocumentDuplicatorService(IChatGptService chatGptService, IProjectRepository projectRepository)
    {
        _chatGptSercice = chatGptService;
        _projectRepository = projectRepository;
    }

    public async Task CopyAndModifyWordDocument(int projectId)
    {
        _projectId = projectId;
        string projectName = _projectRepository.GetProject(projectId).Result.ProjectName;
        string templateFile = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "Savills_TEDD_Report_Client_v2.docx");
        string reportFile = Path.Combine(Directory.GetCurrentDirectory(), "ProjectFiles", projectName, "Report.docx");

        // Ensure the destination directory exists
        var destinationDirectory = Path.GetDirectoryName(reportFile);
        if (!Directory.Exists(destinationDirectory))
        {
            if (destinationDirectory != null) Directory.CreateDirectory(destinationDirectory);
        }

        ReadDocument(templateFile);

        var data = _projectRepository.GetDocumentParts(_projectId).ToList();//.Where(x => x.IsDrawing == false).ToList(); //.Where(x => x.IsDrawing == false)
        List<string> response = await _chatGptSercice.Request(_projectRepository.GetMustBeSentParts(_projectId).Select(x => x.Paragraph).ToList());
        _projectRepository.UpdateResponses(_projectId, response);

        UpdateDocument(templateFile, reportFile);
    }

    private void UpdateDocument(string sourcePath, string destinationPath)
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
                UpdateContent((MainDocumentPart)destinationPart);
            }
        }
    }

    private void UpdateContent(MainDocumentPart mainPart)
    {
        var elementIndex = -1;

        foreach (var element in mainPart.Document.Body.Elements())
        {
            if (element is Paragraph paragraph)
            {
                elementIndex += 1;
                var part = _projectRepository.GetDocumentPart(_projectId, elementIndex);

                if (part.HasToGetUpdated)
                {
                    SetParagraphText(paragraph, part.GPT_Reponse);
                }
            }
            else if (element is Table table)
            {
                foreach (TableRow row in table.Elements<TableRow>())
                {
                    elementIndex += 1;
                    var part = _projectRepository.GetDocumentPart(_projectId, elementIndex);
                    if (part.HasToGetUpdated)
                    {
                        SetRowText(row, part.GPT_Reponse);
                    }
                }
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
        foreach (var element in mainPart.Document.Body.Elements())
        {
            if (element is Paragraph paragraph)
            {
                var paragraphText = GetParagraphText(paragraph);
                _projectRepository.AddDocumentPart(_projectId,
                    paragraphText,
                    HasUpdatableElelent(paragraphText),
                    ParagraphHasDrawing(paragraph),
                    false,
                    string.Empty
                );
            }
            else if (element is Table table)
            {
                //var columns = table.Elements<TableGrid>().FirstOrDefault().Elements<GridColumn>().Count();
                foreach (TableRow row in table.Elements<TableRow>())
                {
                    string csvRow = GetRowText(row);
                    _projectRepository.AddDocumentPart(_projectId,
                        csvRow,
                        HasUpdatableElelent(csvRow),
                        RowHasDrawing(row),
                        true,
                        string.Empty
                    );
                }
            }
        }
    }

    private bool ParagraphHasDrawing(Paragraph paragraph)
    {
        foreach (var element in paragraph.Elements())
        {
            if (element is Run run)
            {
                // is picture
                if (run.Elements<DocumentFormat.OpenXml.Wordprocessing.Drawing>().Count() > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }
    
    private bool RowHasDrawing(TableRow row)
    {
        foreach (var cell in row.Elements())
        {
            if (cell is TableCell tableCell)
            {
                if (CellHasDrawing(tableCell))
                    return true;
            }
            else if (cell is SdtCell sdtCell)
            {
            }
        }

        return false;
    }

    private bool CellHasDrawing(TableCell cell)
    {
        foreach (var element in cell.Elements())
        {
            if (element is Paragraph paragraph)
            {
                if(ParagraphHasDrawing(paragraph))
                    return true;
            }
            else if (element is Table table)
            {

            }
        }

        return false;
    }

    private string GetParagraphText(Paragraph paragraph)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var element in paragraph.Elements())
        {
            if (element is Run run)
            {
                foreach (var text in run.Elements<Text>())
                {
                    if (HasUpdatableElelent(text.Text))
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

    private string GetRowText(TableRow row)
    {
        string csvRow = "";
        foreach (var cell in row.Elements())
        {
            if (cell is TableCell tableCell)
            {
                string cellText = GetTableCellText(tableCell);
                csvRow += "^" + cellText;
            }
            else if (cell is SdtCell sdtCell)
            {
                string cellText = GetSdtCellText(sdtCell);

                csvRow += "^" + cellText;
            }
        }

        return csvRow;
    }

    private string GetSdtCellText(SdtCell cell)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var sdtContentCell in cell.Elements<SdtContentCell>())
        {
            foreach (var paragraph in sdtContentCell.Elements<TableCell>().SingleOrDefault().Elements<Paragraph>())
            {
                var paragraphText = GetParagraphText(paragraph);
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

    private void SetRowText(TableRow row, string rowText)
    {
        int iCell = -1;
        var cellTexts = rowText.Split("^");

        foreach (var cell in row.Elements())
        {
            if (cell is TableCell tableCell)
            {
                iCell += 1;
                SetTableCellText(tableCell, cellTexts[iCell]);
            }
            else if (cell is SdtCell sdtCell)
            {
                iCell += 1;                
                SetSdtCellText(sdtCell, cellTexts[iCell]);
            }
        }
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

    private void SetTableCellText(TableCell cell, string gptResponse)
    {
        //for now, we replace all paragraphs with only one
        foreach (var paragraph in cell.Elements<Paragraph>().Skip(1).ToList())
        {
            paragraph.Remove();
        }

        // Check if the cell already contains a paragraph
        Paragraph para = cell.Elements<Paragraph>().FirstOrDefault();
        SetParagraphText(para, gptResponse);

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

    private bool HasUpdatableElelent(string text)
    {
        foreach (char ch in _updatableElements)
        { 
            if(text.Contains(ch))
                return true;
        }

        return false;
    }
}
