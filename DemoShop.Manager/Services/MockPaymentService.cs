using System;
using DemoShop.Manager.Repositories.Interfaces;

namespace DemoShop.Manager.Services
{
    public class MockPaymentService : IPaymentService
    {
        public Task<bool> ProcessPayment(decimal amount, string paymentMethod, int userId)
        {
            // Simple mock logic: approve payments under $1000, otherwise fail
            if (amount < 1000m) return Task.FromResult(true);
            return Task.FromResult(false);
        }
    }
}