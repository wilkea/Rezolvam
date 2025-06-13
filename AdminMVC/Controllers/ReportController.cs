using System.Diagnostics;
using System.Security.Claims;
using AdminMVC.Models;
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

namespace AdminMVC.Controllers
{
    [Route("Reports")]
    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;

        public ReportController(
            ILogger<ReportController> logger,
            IMediator mediator,
            IMapper mapper,
            IFileStorageService fileStorage)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _fileStorage = fileStorage;
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
                var statisticsDto = await _mediator.Send(new GetReportStatisticsQuery());

                var pagedResult = new PagedViewModel<ReportViewModel>
                {
                    Items = dtoResult.Items.Select(r => _mapper.Map<ReportViewModel>(r)).ToList(),
                    TotalCount = dtoResult.TotalCount,
                    PageIndex = dtoResult.PageIndex,
                    PageSize = dtoResult.PageSize,
                    TotalPages = dtoResult.TotalPages,
                    HasPreviousPage = dtoResult.HasPreviousPage,
                    HasNextPage = dtoResult.HasNextPage,
                    ExtraData = _mapper.Map<ReportStatisticsViewModel>(statisticsDto)
                };

                ViewBag.AvailableStatuses = new[] { "Open", "InProgress", "Resolved" };

                return View(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading reports");
                TempData["Error"] = "Unable to load public reports at this time.";
                return View(new PagedViewModel<ReportViewModel>
                {
                    Items = new List<ReportViewModel>(),
                    TotalCount = 0,
                    PageIndex = pagination.ValidatedPageIndex,
                    PageSize = pagination.ValidatedPageSize,
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
                var userId = TryGetCurrentUserId(); // Guid?
                var isAdmin = User.IsInRole("Admin");

                var dto = await _mediator.Send(new GetReportByIdQuery
                {
                    ReportId = id,
                    UserId = userId ?? Guid.Empty, // fallback
                    IsAdmin = isAdmin
                });

                var vm = _mapper.Map<ReportDetailViewModel>(dto);
                vm.Comments = vm.Comments
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
                vm.IsAdmin = isAdmin;
                if (userId == vm.SubmittedById)
                {
                    vm.IsOwner = true;
                }
                else
                {
                    vm.IsOwner = false;
                }
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading report details for ID: {ReportId}", id);
                return RedirectToAction("Error");
            }
        }


        [Authorize]
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.ArePhotosValid(out string errorMessage))
            {
                ModelState.AddModelError("Photos", errorMessage);
                return View(model);
            }

            try
            {
                var command = _mapper.Map<CreateReportCommand>(model);
                command.UserId = GetCurrentUserId();
                command.Photos = model.Photos;
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

        [HttpPost("UpdateStatus")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddComment(Guid reportId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["Error"] = "Comment message cannot be empty.";
                return RedirectToAction(nameof(Details), new { id = reportId });
            }

            try
            {
                var userId = GetCurrentUserId();
                var command = new AddCitizenCommentCommand
                {
                    ReportId = reportId,
                    Message = message,
                    UserId = userId
                };

                await _mediator.Send(command);
                TempData["Success"] = "Comment added successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment to report {ReportId}", reportId);
                TempData["Error"] = "Unable to add comment. Please try again later.";
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
                    IsAdmin = User.IsInRole("Admin")
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

        [Route("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
        private Guid? TryGetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : (Guid?)null;
        }
    }
}