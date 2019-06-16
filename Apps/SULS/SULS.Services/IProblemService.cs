namespace SULS.Services
{    
    using System.Collections.Generic;
    using System.Linq;

    using SULS.Models;

    public interface IProblemService
    {
        // This interface got too long...
        void Create(string name, int points);

        IQueryable<Problem> GetAllProblems();

        Problem GetProblemById(string id);

        ICollection<Submission> GetProblemSubmissions(string id);

        bool DoesProblemExist(string problemId);
    }
}
