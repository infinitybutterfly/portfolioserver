using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfoilio_Server.Data;
using Portfoilio_Server.Models;
//using PortfolioApi.Data;
using PortfolioApi.DTOs;
//using PortfolioApi.Models;

namespace PortfolioApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly PortfolioDbContext _context;

        public ProjectsController(PortfolioDbContext context)
        {
            _context = context;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        //{
        //    return await _context.Projects.ToListAsync();
        //}

        [HttpGet]
        public async Task<ActionResult> GetProjects(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string? filter = null,
            [FromQuery] string[]? languages = null,
            [FromQuery] string[]? types = null,
            [FromQuery] string sort = "id-desc")
        {
            var query = _context.Projects.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
                query = query.Where(p => p.Status.ToLower() == filter.ToLower());

            if (languages != null && languages.Any())
            {
                var lowerLangs = languages.Select(l => l.ToLower()).ToList();
                query = query.Where(p => lowerLangs.Contains(p.Language.ToLower()));
            }

            if (types != null && types.Any())
            {
                var lowerTypes = types.Select(t => t.ToLower()).ToList();
                query = query.Where(p => lowerTypes.Contains(p.Type.ToLower()));
            }

            query = sort switch
            {
                "id-asc" => query.OrderBy(p => p.Id),
                "name-asc" => query.OrderBy(p => p.Name),
                "name-desc" => query.OrderByDescending(p => p.Name),
                "language-asc" => query.OrderBy(p => p.Language),
                "type-asc" => query.OrderBy(p => p.Type),
                "startDate-desc" => query.OrderByDescending(p => p.StartDate),
                "startDate-asc" => query.OrderBy(p => p.StartDate),
                _ => query.OrderByDescending(p => p.Id)
            };

            var totalRecords = await query.CountAsync();
            List<Project> projects;
            int totalPages = 1;

            if (limit > 0)
            {
                totalPages = (int)Math.Ceiling(totalRecords / (double)limit);
                projects = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();
            }
            else
            {
                // If React asks for Limit 0 (Show All)
                projects = await query.ToListAsync();
            }

            return Ok(new
            {
                data = projects,
                currentPage = page,
                totalPages = totalPages,
                totalRecords = totalRecords
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Project>> CreateProject([FromBody] ProjectInputDto dto)
        {
            var project = new Project
            {
                Name = dto.Name,
                Url = dto.Url,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Language = dto.Language,
                Type = dto.Type,
                Status = dto.Status,
                Description = dto.Description
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(project);

            //return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectInputDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound(new { message = "Project not found" });

            project.Name = dto.Name;
            project.Url = dto.Url;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.Language = dto.Language;
            project.Type = dto.Type;
            project.Status = dto.Status;
            project.Description = dto.Description;

            await _context.SaveChangesAsync();
            return Ok(project);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound(new { message = "Project not found" });

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Project deleted successfully" });
        }
    }
}