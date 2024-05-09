using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MS_Word_Creator.Entities;
using MS_Word_Creator.Repositories;

namespace MS_Word_Creator.Controllers;

[ApiController]
[Route("[controller]")]
public class DesignController : ControllerBase
{
    //private readonly IDocumentPartsRepository _documentPartsRepository;
    //public DesignController(IDocumentPartsRepository documentPartsRepository)
    //{
    //    _documentPartsRepository = documentPartsRepository;
    //}

    //[HttpPost]
    //public async Task<IActionResult> GetDocumentParts()
    //{
    //    return Ok(new { DocumentPartsRepository = _documentPartsRepository.GetDocumentParts() });
    //}
}
