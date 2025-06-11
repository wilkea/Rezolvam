using System.Diagnostics;
using System.Security.Claims;
using AdminMVC.Models;
using AdminMVC.ViewModel.Common;
using AdminMVC.ViewModel.Reports;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;

        public ReportController(ILogger<ReportController> logger, IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
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
                var pagedResult = new PagedViewModel<ReportViewModel>
                {
                    Items = dtoResult.Items.Select(r => _mapper.Map<ReportViewModel>(r)).ToList(),
                    TotalCount = dtoResult.TotalCount,
                    PageIndex = dtoResult.PageIndex,
                    PageSize = dtoResult.PageSize,
                    TotalPages = dtoResult.TotalPages,
                    HasPreviousPage = dtoResult.HasPreviousPage,
                    HasNextPage = dtoResult.HasNextPage
                };
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
        public IActionResult Details(Guid id)
        {
            // This action would typically return a view with details of a specific report.
            return View(id);
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
                var command = _mapper.Map<CreateReportCommand>(model);
                command.UserId = GetCurrentUserId(); // Use current user ID or default for anonymous

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