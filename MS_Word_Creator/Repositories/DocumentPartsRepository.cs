using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MS_Word_Creator.Entities;

namespace MS_Word_Creator.Repositories
{
    //public interface IDocumentPartsRepository
    //{
    //    void AddDocumentPart(string text, bool mustBeSent, bool isDrawing, string response);
    //    List<DocumentPart> GetDocumentParts();
    //    List<DocumentPart> GetMustBeSentParts();
    //    void UpdateResponses(List<string> responses);
    //}

    //public class DocumentPartsRepository : IDocumentPartsRepository
    //{
    //    public List<DocumentPart> DocumentParts = new List<DocumentPart>();

    //    public DocumentPartsRepository() { }

    //    public void AddDocumentPart(string text, bool mustBeSent, bool isDrawing, string response)
    //    {
    //        DocumentParts.Add(new DocumentPart
    //        {
    //            Text = text,
    //            MustBeSent = mustBeSent,
    //            IsDrawing = isDrawing,
    //            Reponse = response
    //        });
    //    }

    //    public List<DocumentPart> GetDocumentParts()
    //    { 
    //        return DocumentParts; 
    //    }

    //    public List<DocumentPart> GetMustBeSentParts()
    //    {
    //        return DocumentParts.Where(x => x.IsDrawing != true).Where(x => x.MustBeSent == true).ToList();
    //    }

    //    public void UpdateResponses(List<string> responses)
    //    {
    //        try
    //        {
    //            List<DocumentPart> mustBeSentParts = GetMustBeSentParts();
    //            for (int i = 0; i < mustBeSentParts.Count(); i++)
    //            {
    //                mustBeSentParts[i].Reponse = responses[i];
    //            }
    //        }
    //        catch { }
    //    }
    //}
}
