using Panda.Models;
using System.Collections.Generic;
using System.Linq;

namespace Panda.Services
{
    public interface IReceiptsService
    {
        IQueryable<Receipt> GetReceiptsForUser(string username);

        void CreateReceipt(Package package);
    }
}
