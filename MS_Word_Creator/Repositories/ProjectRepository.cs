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

        void AddDocumentPart(int projectId, string text, bool mustBeSent, bool isDrawing, string response);
        List<DocumentPart> GetDocumentParts(int projectId);
        List<DocumentPart> GetMustBeSentParts(int projectId);
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


        public void AddDocumentPart(int projectId, string text, bool mustBeSent, bool isDrawing, string response)
        {
            var project = Projects.First(x => x.ProjectId == projectId);
            project.DocumentParts.Add(new DocumentPart
            {
                Text = text,
                MustBeSent = mustBeSent,
                IsDrawing = isDrawing,
                Reponse = response
            });
        }

        public List<DocumentPart> GetDocumentParts(int projectId)
        {
            var project = Projects.First(x => x.ProjectId == projectId);
            return project.DocumentParts;
        }

        public List<DocumentPart> GetMustBeSentParts(int projectId)
        {
            var project = Projects.First(x => x.ProjectId == projectId);
            return project.DocumentParts.Where(x => x.IsDrawing != true).Where(x => x.MustBeSent == true).ToList();
        }

        public void UpdateResponses(int projectId, List<string> responses)
        {
            try
            {
                List<DocumentPart> mustBeSentParts = GetMustBeSentParts(projectId);
                for (int i = 0; i < mustBeSentParts.Count(); i++)
                {
                    mustBeSentParts[i].Reponse = responses[i];
                }
            }
            catch { }
        }
    }
}
