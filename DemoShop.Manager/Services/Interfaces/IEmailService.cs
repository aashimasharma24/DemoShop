using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmationEmailAsync(int userId, int orderId);
    }
}
