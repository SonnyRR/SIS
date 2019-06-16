using Panda.Data.Enums;
using Panda.Models;
using System.Linq;

namespace Panda.Services
{
    public interface IPackageService
    {
        string Create(string description, decimal weight, string shippingAddress, string recipientName);

        IQueryable<Package> GetPackagesByStatus(PackageStatus status);

        void DeliverPackage(string id);
    }
}
