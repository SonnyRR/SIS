namespace SULS.App.Controllersr
{
    using System.Linq;

    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes;
    using SIS.MvcFramework.Attributes.Security;
    using SIS.MvcFramework.Result;
    using SULS.App.ViewModels.Problems;
    using SULS.App.ViewModels.Submissions;
    using SULS.Services;

    public class ProblemsController : Controller
    {
        private readonly IProblemService problemService;

        public ProblemsController(IProblemService problemService)
        {
            this.problemService = problemService;
        }

        [Authorize]
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(ProblemCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Redirect("/Problems/Create");
            }

            this.problemService.Create(inputModel.Name, inputModel.Points);

            return this.Redirect("/");
        }

        [Authorize]
        public IActionResult Details(string id)
        {
            var desiredProblem = this.problemService
                .GetProblemById(id);

            var problemSubmissions = this.problemService
                .GetProblemSubmissions(id);

            var problemSubmissionsMapped = problemSubmissions
                .Select(ps => new SubmissionDetailViewModel()
                {                    
                    SubmissionId = ps.Id,
                    Username = ps.User.Username,
                    AchievedResult = $"{ps.AchievedResult}/{ps.Problem.Points}",
                    CreatedOn = ps.CreatedOn.ToString("dd/MM/yyyy")
                })
                .ToList();

            var resultViewModel = new ProblemDetailViewModel()
            {
                Name = desiredProblem.Name,
                Submissions = problemSubmissionsMapped
            };

            return this.View(resultViewModel, "Details");
        }
    }
}
