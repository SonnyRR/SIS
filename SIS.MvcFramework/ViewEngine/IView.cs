namespace SIS.MvcFramework.ViewEngine
{
    using SIS.MvcFramework.Identity;

    public interface IView
    {
        string GetHtml(object model, Principal user);
    }
}
