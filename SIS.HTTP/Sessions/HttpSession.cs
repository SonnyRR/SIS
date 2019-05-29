namespace SIS.HTTP.Sessions
{
    using System.Collections.Generic;

    using SIS.HTTP.Common;

    public class HttpSession : IHttpSession
    {

        private Dictionary<string, object> parameters;

        public HttpSession(string id)
        {
            CoreValidator.ThrowIfNullOrEmpty(id, nameof(id));

            this.parameters = new Dictionary<string, object>();
            this.Id = id;
        }

        public string Id { get; }

        public void AddParameter(string name, object parameter)
        {
            CoreValidator.ThrowIfNullOrEmpty(name, nameof(name));
            CoreValidator.ThrowIfNull(parameter, nameof(parameter));

            this.parameters[name] = parameter;
        }

        public void ClearParameters()
        {
            this.parameters.Clear();
        }

        public bool ContainsParameter(string name)
        {
            CoreValidator.ThrowIfNullOrEmpty(name, nameof(name));

            return this.parameters.ContainsKey(name);
        }

        public object GetParameter(string name)
        {
            CoreValidator.ThrowIfNullOrEmpty(name, nameof(name));

            return this.parameters?[name];
        }
    }
}
