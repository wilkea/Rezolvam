using System.Security.Claims;
using AdminMVC.ViewModel;
using AdminMVC.ViewModel.Common;
using AdminMVC.ViewModel.Reports;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rezolvam.Application.Commands.Report;
using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using rezolvam.Application.Interfaces;
using rezolvam.Application.Queries.Report;
using rezolvam.Domain.Reports.StatusChanges.Enums;
using Rezolvam.Application.DTOs.Common;


namespace AdminMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminController> _logger;
        private readonly IFileStorageService _fileStorage;

        public AdminController(
            IMediator mediator,
            IMapper mapper,
            ILogger<AdminController> logger,
            IFileStorageService fileStorage)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
            _fileStorage = fileStorage;
        }

        public async Task<IActionResult> Index([FromQuery] PaginationRequest pagination)
        {
            try
            {
                var query = new GetReportsRequiringAdminActionQuery
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
                ViewBag.AvailableStatuses = Enum.GetValues<ReportStatus>();

                return View(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reports");
                TempData["Error"] = "Unable to load reports at this time. Please try again later.";

                var emptyResult = new PagedViewModel<ReportViewModel>
                {
                    Items = new List<ReportViewModel>(),
                    TotalCount = 0,
                    PageIndex = pagination.ValidatedPageIndex,
                    PageSize = pagination.ValidatedPageSize,
                    TotalPages = 0,
                    HasPreviousPage = false,
                    HasNextPage = false
                };

                return View(emptyResult);
            }
        }

        [HttpGet("Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
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

                var viewModel = _mapper.Map<ReportDetailViewModel>(reportDetailDto);

                viewModel.Comments = viewModel.Comments
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
                ViewBag.AvailableStatuses = Enum.GetValues<ReportStatus>()
                    .Where(s => s != viewModel.Status)
                    .Select(s => new { Value = (int)s, Text = s.ToString() })
                    .ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading report details for ID: {ReportId}", id);
                TempData["Error"] = "Error loading report details. Please try again.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost("UpdateStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid reportId, ReportStatus newStatus, string? reason)
        {
            try
            {
                var command = new UpdateReportStatusCommand
                {
                    ReportId = reportId,
                    AdminId = GetCurrentUserId(),
                    NewStatus = newStatus,
                    Reason = reason
                };

                await _mediator.Send(command);
                TempData["Success"] = "Report status updated successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating report status {ReportId}", reportId);
                TempData["Error"] = "Failed to update report status.";
            }

            return RedirectToAction(nameof(Details), new { id = reportId });
        }
        [HttpPost("AddComment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAdminComment(Guid reportId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Comment message cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = reportId });
            }

            try
            {
                var command = new AddAdminCommentCommand
                {
                    ReportId = reportId,
                    Message = message,
                    AdminId = GetCurrentUserId()
                };

                await _mediator.Send(command);
                TempData["Success"] = "Admin comment added successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding admin comment to report {ReportId}", reportId);
                TempData["Error"] = "Unable to add comment. Please try again later.";
            }

            return RedirectToAction(nameof(Details), new { id = reportId });
        }

        [HttpPost("VerifyReport")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyReport(Guid reportId)
        {
            try
            {
                var command = new VerifyReportCommand
                {
                    ReportId = reportId,
                    AdminId = GetCurrentUserId()
                };

                await _mediator.Send(command);
                TempData["Success"] = "Report verified successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying report {ReportId}", reportId);
                TempData["Error"] = "Unable to verify report. Please try again later.";
            }

            return RedirectToAction(nameof(Details), new { id = reportId });
        }

        [HttpPost("RejectReport")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectReport(Guid reportId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "Rejection reason is required.";
                return RedirectToAction(nameof(Details), new { id = reportId });
            }

            try
            {
                var command = new RejectReportCommand
                {
                    ReportId = reportId,
                    AdminId = GetCurrentUserId(),
                    Reason = reason
                };

                await _mediator.Send(command);
                TempData["Success"] = "Report rejected successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting report {ReportId}", reportId);
                TempData["Error"] = "Unable to reject report. Please try again later.";
            }

            return RedirectToAction(nameof(Details), new { id = reportId });
        }

        [HttpPost("AddPhoto")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoto(Guid reportId, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
            {
                TempData["Error"] = "Please select a photo to upload.";
                return RedirectToAction(nameof(Details), new { id = reportId });
            }

            try
            {
                // Save the photo to local storage
                var photoUrl = await _fileStorage.SaveFileAsync(photo, "uploads/reports");

                var command = new AddPhotoCommand
                {
                    ReportId = reportId,
                    PhotoUrl = photoUrl,
                    UserId = GetCurrentUserId()
                };

                await _mediator.Send(command);
                TempData["Success"] = "Photo added successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding photo to report {ReportId}", reportId);
                TempData["Error"] = "Unable to add photo. Please try again later.";
            }

            return RedirectToAction(nameof(Details), new { id = reportId });
        }

        [HttpPost("RemovePhoto")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePhoto(Guid reportId, Guid photoId)
        {
            try
            {
                var query = new GetReportByIdQuery
                {
                    ReportId = reportId,
                    UserId = GetCurrentUserId(),
                    IsAdmin = true
                };

                var report = await _mediator.Send(query);
                var photo = report?.Photos.FirstOrDefault(p => p.Id == photoId);

                if (photo != null)
                {
                    var command = new RemovePhotoCommand
                    {
                        ReportId = reportId,
                        PhotoId = photoId,
                        UserId = GetCurrentUserId()
                    };

                    await _mediator.Send(command);

                    await _fileStorage.DeleteFileAsync(photo.PhotoUrl, "uploads/reports");
                    TempData["Success"] = "Photo removed successfully.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing photo {PhotoId} from report {ReportId}", photoId, reportId);
                TempData["Error"] = "Unable to remove photo. Please try again later.";
            }

            return RedirectToAction(nameof(Details), new { id = reportId });
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