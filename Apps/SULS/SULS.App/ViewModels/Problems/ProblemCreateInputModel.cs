using SIS.MvcFramework.Attributes.Validation;

namespace SULS.App.ViewModels.Problems
{
    public class ProblemCreateInputModel
    {

        private const string PROBLEM_NAME_LENGTH_MSG = "Problem name must be between 5 and 20 characters long!";
        private const string PROBLEM_POINTS_RANGE_MSG = "Problem points must range between 50 and 300 included!";

        [RequiredSis]
        [StringLengthSis(5, 20, PROBLEM_NAME_LENGTH_MSG)]
        public string Name { get; set; }

        [RequiredSis]
        [RangeSis(50, 300, PROBLEM_POINTS_RANGE_MSG)]
        public int Points { get; set; }
    }
}
