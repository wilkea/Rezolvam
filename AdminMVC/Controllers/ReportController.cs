using AdminMVC.ViewModel;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rezolvam.Application.Report.Commands;
using rezolvam.Application.Report.Queries;


namespace AdminMVC.Controllers
{
    [Authorize(Roles = "admin")]
    public class ReportController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ReportController(IMediator mediator, IMapper mapper)
        {
            _mapper = mapper;
            _mediator = mediator;

        }
        public async Task<IActionResult> Index(GetReportsPagedQuery query)
        {
            Console.WriteLine("Handler intrat");
            var reports = await _mediator.Send(query);
            return View(reports);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ReportViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ReportViewModel model)
        {
            // Check if we're getting the data
            if (model == null)
            {
                ModelState.AddModelError("", "Model is null");
                return View(new ReportViewModel());
            }

            // If model validation passes, proceed with creation
            if (ModelState.IsValid)
            {
                try
                {
                    var command = new CreateReportCommand
                    {
                        Title = model.Title ?? string.Empty,
                        Description = model.Description ?? string.Empty,
                        PhotoUrl = model.PhotoUrl ?? string.Empty
                    };

                    var id = await _mediator.Send(command);
                    TempData["SuccessMessage"] = "Report created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating report: {ex.Message}");
                    return View(model);
                }
            }

            // If we get here, something failed, redisplay form
            return View(model);

        }


        [HttpPost("Details/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var report = await _mediator.Send(new GetReportDetailsQuery { Id = id });
            if (report == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<ReportViewModel>(report);
            return View(model);
        }
    }
}