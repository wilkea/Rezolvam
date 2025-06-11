using System.Security.Claims;
using AdminMVC.ViewModel;
using AdminMVC.ViewModel.Common;
using AdminMVC.ViewModel.Reports;
using AdminMVC.Controllers.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using rezolvam.Application.Queries.Report;
using rezolvam.Domain.Reports.StatusChanges.Enums;
using Rezolvam.Application.DTOs.Common;


namespace AdminMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IMediator mediator, ILogger<AdminController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index([FromQuery] PaginationRequest pagination)
        {
            try
            {
                var userId = GetCurrentUserId();

                var query = new GetReportsQuery
                {
                    Pagination = pagination
                };

                var dtoResult = await _mediator.Send(query);

                ViewBag.CurrentStatus = query.Status;
                ViewBag.AvailableStatuses = Enum.GetValues<ReportStatus>().Select(s => s.ToString()).ToList();

                var pagedResult = dtoResult.ToViewModel();
                return View(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin reports index");
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
        [HttpGet("RequiringAction")]
        public async Task<IActionResult> RequiringAction([FromQuery] PaginationRequest pagination)
        {
            try
            {
                var userId = GetCurrentUserId();

                // WHY: Separate query for different business requirement (reports requiring action)
                // HOW: Specialized query handler can implement specific business logic for filtering
                var query = new GetReportsRequiringAdminActionQuery
                {
                    Request = pagination
                };

                var result = await _mediator.Send(query);

                return View("Index", result); // Reuse the same view but with filtered data
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reports requiring action");
                TempData["Error"] = "Error loading reports requiring action. Please try again.";
                return RedirectToAction("Index");
            }
        }
        [HttpGet("Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();

                // WHY: Query for detailed report information including admin-specific data
                // HOW: Query handler will map to detailed DTO with status history and permissions
                var query = new GetReportByIdQuery
                {
                    ReportId = id,
                    UserId = userId,
                    IsAdmin = true
                };

                var reportDetailDto = await _mediator.Send(query);

                if (reportDetailDto == null)
                {
                    TempData["Error"] = "Report not found.";
                    return RedirectToAction("Index");
                }

                // WHY: Provide available status options for admin to change report status
                // HOW: Filter out current status to show only valid transition options
                ViewBag.AvailableStatuses = Enum.GetValues<ReportStatus>()
                    .Where(s => s != reportDetailDto.Status)
                    .Select(s => new { Value = (int)s, Text = s.ToString() })
                    .ToList();

                return View(reportDetailDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading report details for ID: {ReportId}", id);
                TempData["Error"] = "Error loading report details. Please try again.";
                return RedirectToAction("Index");
            }
        }
        // [HttpGet("Inspect/{id:guid}")]
        // public async Task<IActionResult> Inspect(Guid id)
        // {
        //     try
        //     {
        //         var userId = GetCurrentUserId();

        //         var query = new GetReportDetailQuery
        //         {
        //             ReportId = id,
        //             UserId = userId,
        //             IsAdmin = true
        //         };

        //         var reportDetailDto = await _mediator.Send(query);

        //         if (reportDetailDto == null)
        //         {
        //             TempData["Error"] = "Report not found.";
        //             return RedirectToAction("Index");
        //         }

        //         return View(reportDetailDto);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error loading report inspection for ID: {ReportId}", id);
        //         TempData["Error"] = "Error loading report for inspection. Please try again.";
        //         return RedirectToAction("Details", new { id });
        //     }
        // }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ReportViewModel());
        }



        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    throw new UnauthorizedAccessException("User ID not found or invalid");
                }
                return userId;
        }
    }

}