using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MS_Word_Creator.Repositories;
using MS_Word_Creator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithOrigins("http://localhost:3000"));
});

builder.Services.AddControllers();
builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue; // In case of large files
    x.MultipartHeadersLengthLimit = int.MaxValue;
});

//Adding services
builder.Services.AddSingleton<IProjectRepository, ProjectRepository>();
builder.Services.AddSingleton<IChatGptService, ChatGptService>();
builder.Services.AddSingleton<IDocumentDuplicatorService, DocumentDuplicatorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors();
app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});

app.MapControllers();  // Map attribute-routed controllers
app.Run();





//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using System.IO;
//using System.Reflection.Metadata.Ecma335;

//using System.Drawing;
//using DocumentFormat.OpenXml.Spreadsheet;
//using Syncfusion.Pdf;
//using Syncfusion.Pdf.Graphics;
//using Syncfusion.Pdf.Parsing;
//using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
//using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
//using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using MS_Word_Creator;
//using Newtonsoft.Json;


//class Program
//{
//    // table csv, should_be_sent_to_ChatGPT, 
//    private static string _url = @"http://127.0.0.1:62000/api/call_chatgpt";

//    static async Task Main(string[] args)
//    {
//        string sourcePath = @"C:\Users\akarooni\Documents\MY_TEMP\Projects\Executive_summary_demo\MS_Word_Creator\docs\Savills_TEDD_Report_Client_v2.docx";
//        //string sourcePath = @"C:\Users\ali.karooni\Documents\_My_Projects\Executive summary demo\MS_Word_Creator\docs\output.docx";
//        string destinationPath = @"C:\Users\akarooni\Documents\MY_TEMP\Projects\Executive_summary_demo\MS_Word_Creator\docs\new_output.docx";

//        var documentDuplicator = new DocumentDuplicator();
//        await documentDuplicator.CopyAndModifyWordDocument(sourcePath, destinationPath);

//        Console.ReadKey();
//    }
//}
