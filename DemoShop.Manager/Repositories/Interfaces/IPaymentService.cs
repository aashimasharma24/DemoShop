using DemoShop.Core.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoShop.Manager.Repositories.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPayment(decimal amount, string paymentMethod, int userId);
    }
}