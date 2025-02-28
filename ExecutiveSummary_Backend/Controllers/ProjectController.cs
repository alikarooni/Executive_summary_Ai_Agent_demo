using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS_Word_Creator.Entities;
using MS_Word_Creator.Repositories;
using MS_Word_Creator.Services;

namespace MS_Word_Creator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectRepository _projectRepository;
    private readonly IDocumentDuplicatorService _documentDuplicatorService;

    public ProjectController(IProjectRepository projectRepository, IDocumentDuplicatorService documentDuplicatorService)
    {
        _projectRepository = projectRepository;
        _documentDuplicatorService = documentDuplicatorService;
    }

    [HttpPost]
    [Route("AddProject")]
    public async Task<IActionResult> AddProject(IFormCollection form)
    {        
        if (form.Files.Count == 0)
            return BadRequest("No files received from the request.");

        string projectName = form["projectName"];
        string projectType = form["projectType"];

        int projectId = await _projectRepository.AddProject(projectName, projectType, form.Files);

        await _documentDuplicatorService.CopyAndModifyWordDocument(projectId);

        return Ok(new { Message = "Form submitted successfully" });
    }

    [HttpGet]
    [Route("GetProject/{projectId}")]
    public async Task<IActionResult> GetProject(int projectId)
    { 
        Project project = await _projectRepository.GetProject(projectId);

        return Ok(new { project = project });
    }

    [HttpGet]
    [Route("GetDocumentParts/{projectId}")]
    public async Task<IActionResult> GetDocumentParts(int projectId)
    {
        return Ok(new { DocumentParts = _projectRepository.GetDocumentParts(projectId) });
    }
}
