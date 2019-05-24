namespace SIS.HTTP.Sessions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using SIS.HTTP.Sessions.Contracts;

    public class HttpSession : IHttpSession
    {

        private Dictionary<string, object> parameters;

        public HttpSession(string id)
        {
            this.parameters = new Dictionary<string, object>();
        }


        public string Id { get; }

        public void AddParameter(string name, object parameter)
        {
            this.parameters.Add(name, parameter);
        }

        public void ClearParameters()
        {
            this.parameters.Clear();
        }

        public bool ContainsParameter(string name)
        {
            return this.parameters.ContainsKey(name);
        }

        public object GetParameter(string name)
        {
            return this.parameters[name];
        }
    }
}
