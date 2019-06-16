namespace SULS.Services
{
    public interface ISubmissionService
    {
        void Create(string code, string problemId, string userId);

        void Delete(string submissionId);
    }
}
