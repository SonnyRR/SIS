using SULS.Data;
using SULS.Models;
using System;
using System.Linq;

namespace SULS.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly SULSContext context;
        private readonly IProblemService problemService;
        private readonly Random random;

        public SubmissionService(SULSContext context, IProblemService problemService, Random random)
        {
            this.context = context;
            this.problemService = problemService;
            this.random = random;
        }

        public void Create(string code, string problemId, string userId)
        {
            var currentSubmissionProblem = this.problemService.GetProblemById(problemId);

            if (currentSubmissionProblem != null)
            {
                Submission currentSubmission = new Submission()
                {
                    UserId = userId,
                    Code = code,
                    ProblemId = problemId,
                    CreatedOn = DateTime.UtcNow,
                    AchievedResult = this.random.Next(0, currentSubmissionProblem.Points)
                };

                this.context.Submissions.Add(currentSubmission);

                this.context.SaveChanges();
            }
        }

        public void Delete(string submissionId)
        {
            var desiredSubmissionToDelete = this.context
                .Submissions
                .FirstOrDefault(s => s.Id == submissionId);

            if (desiredSubmissionToDelete != null)
            {
                this.context.Remove(desiredSubmissionToDelete);
                this.context.SaveChanges();
            }
        }
    }
}
