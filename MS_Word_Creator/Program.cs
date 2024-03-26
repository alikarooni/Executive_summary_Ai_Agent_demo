using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Reflection.Metadata.Ecma335;

using System.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MS_Word_Creator;
using Newtonsoft.Json;


class Program
{
    // table csv, should_be_sent_to_ChatGPT, 
    private static string _url = @"http://127.0.0.1:62000/api/call_chatgpt";

    static async Task Main(string[] args)
    {
        string sourcePath = @"C:\Users\ali.karooni\Documents\_My_Projects\Executive summary demo\MS_Word_Creator\docs\Savills_TEDD_Report_Client_v2.docx";
        //string sourcePath = @"C:\Users\ali.karooni\Documents\_My_Projects\Executive summary demo\MS_Word_Creator\docs\output.docx";
        string destinationPath = @"C:\Users\ali.karooni\Documents\_My_Projects\Executive summary demo\MS_Word_Creator\docs\new_output.docx";

        var documentDuplicator = new DocumentDuplicator();
        await documentDuplicator.CopyAndModifyWordDocument(sourcePath, destinationPath);

        Console.ReadKey();
    }
}
