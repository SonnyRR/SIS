using System;
using System.Collections.Generic;
using System.Linq;
using Panda.Data;
using Panda.Models;

namespace Panda.Services
{
    public class ReceiptsService : IReceiptsService
    {
        private readonly PandaDbContext context;

        public ReceiptsService(PandaDbContext context)
        {
            this.context = context;
        }

        public void CreateReceipt(Package package)
        {
            var receipt = new Receipt()
            {
                Fee = package.Weight * 2.67M,
                RecipientId = package.RecipientId,
                PackageId = package.Id,
                IssuedOn = DateTime.UtcNow
            };

            this.context.Add(receipt);
            this.context.SaveChanges();
        }

        public IQueryable<Receipt> GetReceiptsForUser(string userId)
        {
            var receipts = this.context.Receipts.Where(r => r.Recipient.Id == userId);
            return receipts;
        }
    }
}
