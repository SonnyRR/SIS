using System;
using System.Linq;
using Panda.Data;
using Panda.Data.Enums;
using Panda.Models;

namespace Panda.Services
{
    public class PackageService : IPackageService
    {
        private readonly PandaDbContext context;
        private readonly IUserService userService;
        private readonly IReceiptsService receiptsService;

        public PackageService(PandaDbContext context, IUserService userService, IReceiptsService receiptsService)
        {
            this.context = context;
            this.userService = userService;
            this.receiptsService = receiptsService;
        }

        public string Create(string description, decimal weight, string shippingAddress, string recipientName)
        {
            var userId = this.userService.GetUserIdByUsername(recipientName);

            var package = new Package()
            {
                Description = description,
                Weight = weight,
                ShippingAddress = shippingAddress,
                RecipientId = userId
            };

            this.context.Add(package);
            this.context.SaveChanges();

            return package.Id;
        }

        public IQueryable<Package> GetPackagesByStatus(PackageStatus status)
        {
            return this.context.Packages.Where(p => p.PackageStatus == status);
        }
        
        public void DeliverPackage(string id)
        {
            var packageToBeDelivered = this.context.Packages.FirstOrDefault(pkg => pkg.Id == id);
            packageToBeDelivered.PackageStatus = PackageStatus.Delivered;
            this.context.SaveChanges();

            this.receiptsService.CreateReceipt(packageToBeDelivered);
        }

    }
}
