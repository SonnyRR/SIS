using SIS.MvcFramework.Attributes.Validation;

namespace SULS.App.ViewModels.Submissions
{
    public class SubmissionCreateInputModel
    {
        private const string CODE_LENGTH_RANGE_MSG = "Code must be between 30 and 800 characters long!";

        [RequiredSis]
        public string ProblemId { get; set; }

        [RequiredSis]
        [StringLengthSis(30,800, CODE_LENGTH_RANGE_MSG)]
        public string Code { get; set; }
    }
}
