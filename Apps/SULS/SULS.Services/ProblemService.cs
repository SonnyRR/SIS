namespace SULS.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;

    using SULS.Data;
    using SULS.Models;

    public class ProblemService : IProblemService
    {
        private readonly SULSContext context;

        public ProblemService(SULSContext context)
        {
            this.context = context;
        }

        public void Create(string name, int points)
        {
            Problem currentProblem = new Problem()
            {
                Name = name,
                Points = points
            };

            this.context.Problems.Add(currentProblem);
            this.context.SaveChanges();
        }

        public bool DoesProblemExist(string problemId)
        {
            return this.context.Problems.Any(p => p.Id == problemId);
        }

        // This is bad but time is ticking.
        public IQueryable<Problem> GetAllProblems()
        {
            return this.context.Problems;
        }

        // This too.
        public Problem GetProblemById(string id)
        {
            return this.context
                .Problems
                .Include(p => p.Submissions)
                .ThenInclude(s => s.User)
                .FirstOrDefault(p => p.Id == id);
        }

        public ICollection<Submission> GetProblemSubmissions(string id)
        {
            return this.context
                .Problems
                .FirstOrDefault(p => p.Id == id)?
                .Submissions;
        }
    }
}
