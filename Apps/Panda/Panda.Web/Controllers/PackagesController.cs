using Panda.Data.Enums;
using Panda.Services;
using Panda.Web.ViewModels.Packages;
using SIS.MvcFramework;
using SIS.MvcFramework.Attributes;
using SIS.MvcFramework.Attributes.Security;
using SIS.MvcFramework.Result;
using System.Linq;

namespace Panda.Web.Controllers
{
    public class PackagesController : Controller
    {

        private readonly IPackageService packageService;
        private readonly IUserService userService;

        public PackagesController(IPackageService packageService, IUserService userService)
        {
            this.packageService = packageService;
            this.userService = userService;
        }

        [Authorize]
        public IActionResult Create()
        {
            var usernames = this.userService.GetUsernames();
            return this.View(usernames);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(PackageCreateInputModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Redirect("/Packages/Create");
            }

            this.packageService.Create(model.Description, model.Weight, model.ShippingAddress, model.RecipientName);
            return this.Redirect("/Packages/Pending");
        }

        [Authorize]
        public IActionResult Pending()
        {
            var packages = this.packageService.GetPackagesByStatus(PackageStatus.Pending)
                .Select(pkg => new PendingPackagesViewModel()
                {
                    Id = pkg.Id,
                    Description = pkg.Description,
                    Weight = pkg.Weight,
                    ShippingAddress = pkg.ShippingAddress,
                    RecipientName = pkg.Recipient.Username
                })
                .ToList();

            return this.View(packages);
        }

        [Authorize]
        public IActionResult Deliver(string id)
        {
            this.packageService.DeliverPackage(id);
            return this.Redirect("/Packages/Delivered");
        }

        [Authorize]
        public IActionResult Delivered()
        {
            var packages = this.packageService.GetPackagesByStatus(PackageStatus.Delivered)
                .Select(pkg => new DeliveredPackagesViewModel()
                {
                   Description =pkg.Description,
                   Weight=pkg.Weight,
                   ShippingAddress=pkg.ShippingAddress,
                   RecipientName = pkg.Recipient.Username,
                   Status = pkg.PackageStatus.ToString(),
                })
                .ToList();

            return this.View(packages);
        }
    }
}
