namespace SULS.App.Controllers
{
    using SIS.MvcFramework;
    using SIS.MvcFramework.Attributes;
    using SIS.MvcFramework.Attributes.Security;
    using SIS.MvcFramework.Result;
    using SULS.App.ViewModels.Submissions;
    using SULS.Services;

    public class SubmissionsController : Controller
    {
        private readonly ISubmissionService submissionService;
        private readonly IProblemService problemService;

        public SubmissionsController(ISubmissionService submissionService, IProblemService problemService)
        {
            this.submissionService = submissionService;
            this.problemService = problemService;
        }

        [Authorize]
        public IActionResult Create(SubmissionProblemInputModel inputModel)
        {
            var desiredProblem = this.problemService
                .GetProblemById(inputModel.Id);

            var problemViewModel = new SubmissionProblemViewModel()
            {
                Name = desiredProblem.Name,
                ProblemId = desiredProblem.Id
            };

            return this.View(problemViewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(SubmissionCreateInputModel inputModel)
        {
            if (!this.ModelState.IsValid || !this.problemService.DoesProblemExist(inputModel.ProblemId))
            {
                return this.Redirect($"/Submissions/Create?id={inputModel.ProblemId}");
            }

            this.submissionService.Create(inputModel.Code, inputModel.ProblemId, this.User.Id);

            return this.Redirect("/");
        }

        [Authorize]      
        public IActionResult Delete(string id)
        {
            this.submissionService.Delete(id);
            return this.Redirect("/");
        }
    }
}
