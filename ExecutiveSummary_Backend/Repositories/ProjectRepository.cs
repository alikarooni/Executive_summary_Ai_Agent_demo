using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MS_Word_Creator.Entities;
using MS_Word_Creator.Services;

namespace MS_Word_Creator.Repositories
{
    public interface IProjectRepository
    {
        Task<int> AddProject(string projectName, string projectType, IFormFileCollection formFiles);
        Task<Project> GetProject(int projectId);

        void AddDocumentPart(int projectId, string paragraph, bool hasToGetUpdated, bool hasDrawing, bool isPartOfATable, string gpt_Reponse);
        DocumentParagraphs GetDocumentPart(int projectId, int partId);
        List<DocumentParagraphs> GetDocumentParts(int projectId);
        List<DocumentParagraphs> GetMustBeSentParts(int projectId);
        void UpdateResponses(int projectId, List<string> responses);
    }

    public class ProjectRepository : IProjectRepository
    {
        public List<Project> Projects = new List<Project>();

        public ProjectRepository() { }

        public async Task<int> AddProject(string projectName, string projectType, IFormFileCollection formFiles)
        {
            List<string> savedFiles = new List<string>();
            var filesPath = Path.Combine(Directory.GetCurrentDirectory(), "ProjectFiles", projectName);

            if (!Directory.Exists(filesPath))
                Directory.CreateDirectory(filesPath);

            foreach (var file in formFiles)
            {
                if (file.Length > 0)
                {
                    var filePath = Path.Combine(filesPath, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    savedFiles.Add(filePath);
                }
            }

            Projects.Add(new Project
            {
                ProjectId = 123,
                ProjectName = projectName,
                ProjectType = projectType,
                ProjectFiles = savedFiles
            });

            int projectId = 123;
            return projectId;
        }

        public async Task<Project> GetProject(int projectId)
        {
            await Task.Delay(0);
            return Projects.FirstOrDefault(x => x.ProjectId == projectId);
        }


        public void AddDocumentPart(int projectId, string paragraph, bool hasToGetUpdated, bool hasDrawing, bool isPartOfATable, string gpt_Reponse)
        {
            var project = Projects.First(x => x.ProjectId == projectId);
            project.DocumentParts.Add(new DocumentParagraphs
            {
                Paragraph = paragraph,
                HasToGetUpdated = hasToGetUpdated,
                HasDrawing = hasDrawing,
                IsTableRow = isPartOfATable,
                GPT_Reponse = gpt_Reponse
            });
        }

        public DocumentParagraphs GetDocumentPart(int projectId, int partId)
        {
            var project = Projects.First(x => x.ProjectId == projectId);
            return project.DocumentParts.ElementAt(partId);
        }
        
        public List<DocumentParagraphs> GetDocumentParts(int projectId)
        {
            var project = Projects.First(x => x.ProjectId == projectId);
            return project.DocumentParts;
        }

        public List<DocumentParagraphs> GetMustBeSentParts(int projectId)
        {
            var project = Projects.First(x => x.ProjectId == projectId);
            return project.DocumentParts.Where(x => x.HasDrawing != true).Where(x => x.HasToGetUpdated == true).ToList();
        }

        public void UpdateResponses(int projectId, List<string> responses)
        {
            try
            {
                List<DocumentParagraphs> mustBeSentParts = GetMustBeSentParts(projectId);
                for (int i = 0; i < mustBeSentParts.Count(); i++)
                {
                    mustBeSentParts[i].GPT_Reponse = responses[i];
                }
            }
            catch { }
        }
    }
}
