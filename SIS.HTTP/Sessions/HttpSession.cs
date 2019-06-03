namespace SIS.HTTP.Sessions
{
    using System.Collections.Generic;

    using SIS.Common;
    using SIS.HTTP.Common;

    public class HttpSession : IHttpSession
    {
        private Dictionary<string, object> parameters;

        public HttpSession(string id)
        {
            id.ThrowIfNullOrEmpty(nameof(id));

            this.parameters = new Dictionary<string, object>();
            this.IsNew = true;
            this.Id = id;
        }

        public string Id { get; }

        public bool IsNew { get; set; }

        public void AddParameter(string name, object parameter)
        {
            name.ThrowIfNullOrEmpty(nameof(name));
            parameter.ThrowIfNull(nameof(parameter));

            this.parameters[name] = parameter;
        }

        public void ClearParameters()
        {
            this.parameters.Clear();
        }

        public bool ContainsParameter(string name)
        {
            name.ThrowIfNullOrEmpty(nameof(name));

            return this.parameters.ContainsKey(name);
        }

        public object GetParameter(string name)
        {
            name.ThrowIfNullOrEmpty(nameof(name));

            return this.parameters?[name];
        }
    }
}
