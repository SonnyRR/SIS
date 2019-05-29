namespace SIS.HTTP.Sessions
{
    public interface IHttpSession
    {
        string Id { get; }

        object GetParameter(string name);

        bool ContainsParameter(string name);

        bool IsNew { get; set; }

        void AddParameter(string name, object parameter);

        void ClearParameters();
    }
}
