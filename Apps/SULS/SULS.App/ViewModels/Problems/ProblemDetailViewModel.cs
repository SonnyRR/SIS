using SULS.App.ViewModels.Submissions;
using System.Collections.Generic;

namespace SULS.App.ViewModels.Problems
{
    public class ProblemDetailViewModel
    {
        public string Name { get; set; }

        public ICollection<SubmissionDetailViewModel> Submissions { get; set; }
    }
}
