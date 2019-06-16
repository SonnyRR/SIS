using Panda.Services;
using Panda.Web.ViewModels.Receipts;
using SIS.MvcFramework;
using SIS.MvcFramework.Attributes.Security;
using SIS.MvcFramework.Result;
using System.Linq;

namespace Panda.Web.Controllers
{
    public class ReceiptsController : Controller
    {
        private readonly IReceiptsService receiptsService;

        public ReceiptsController(IReceiptsService receiptsService)
        {
            this.receiptsService = receiptsService;
        }

        [Authorize]
        public IActionResult Index()
        {
            var receipts = this.receiptsService.GetReceiptsForUser(this.User.Id)
                .Select(r => new ReceiptsViewModel()
                {
                    Id = r.Id,
                    Fee = r.Fee,
                    RecipientName = r.Recipient.Username,
                    IssuedOn = r.IssuedOn.ToString()
                })
                .ToList();

            return this.View(receipts);
        }
    }
}
