using System.Diagnostics;
using System.Security.Claims;
using AdminMVC.Models;
using AdminMVC.ViewModel.Common;
using AdminMVC.ViewModel.Reports;
using AdminMVC.Controllers.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using rezolvam.Application.Commands.Report;
using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using rezolvam.Application.Queries.Report;

namespace AdminMVC.Controllers
{
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;

        public ReportController(ILogger<ReportController> logger, IMediator mediator, IWebHostEnvironment env)
        {
            _mediator = mediator;
            _logger = logger;
            _env = env;
        }
        public async Task<IActionResult> Index([FromQuery] PaginationRequest pagination)
        {
            try
            {
                var query = new GetPublicReportsQuery
                {
                    Request = new PaginationRequest
                    {
                        PageIndex = pagination.ValidatedPageIndex,
                        PageSize = pagination.ValidatedPageSize,
                        SearchTerm = pagination.SearchTerm,
                        StatusFilter = pagination.StatusFilter,
                        SubmitterId = pagination.SubmitterId
                    }
                };
                var dtoResult = await _mediator.Send(query);
                var pagedResult = dtoResult.ToViewModel();
                ViewBag.AvailableStatuses = new[] { "Open", "InProgress", "Resolved" }; // Only show relevant statuses to public


                return View(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading public reports index");
                TempData["Error"] = "Error loading reports. Please try again.";
                return View(new PagedViewModel<ReportViewModel>
                {
                    Items = new List<ReportViewModel>(),
                    TotalCount = 0,
                    PageIndex = pagination.PageIndex,
                    PageSize = pagination.PageSize,
                    TotalPages = 0,
                    HasPreviousPage = false,
                    HasNextPage = false
                });
            }
        }
        [HttpGet("Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var query = new GetReportByIdQuery
                {
                    ReportId = id,
                    UserId = User.Identity?.IsAuthenticated == true ? GetCurrentUserId() : Guid.Empty,
                    IsAdmin = User.IsInRole("Admin")
                };

                var dto = await _mediator.Send(query);
                if (dto == null) return NotFound();

                var model = dto.ToDetailViewModel();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading report details");
                return RedirectToAction("Index");
            }
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (!model.ArePhotosValid(out var photoError))
                {
                    ModelState.AddModelError("Photos", photoError);
                    return View(model);
                }

                var uploadDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var photoUrls = new List<string>();
                foreach (var photo in model.Photos)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
                    var filePath = Path.Combine(uploadDir, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }
                    photoUrls.Add($"/uploads/{fileName}");
                }

                var command = model.ToCommand(photoUrls);
                command.UserId = GetCurrentUserId();

                await _mediator.Send(command);

                TempData["Success"] = "Report created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating report");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the report. Please try again.");
                return View(model);
            }
        }

        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out var guid) ? guid : throw new UnauthorizedAccessException("User ID not found.");
        }

    }
}