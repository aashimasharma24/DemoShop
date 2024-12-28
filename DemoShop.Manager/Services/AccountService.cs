using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace DemoShop.Manager.Services
{
    public class AccountService : IAccountService
    {
        private readonly DemoShopDbContext _context;
        public AccountService(DemoShopDbContext context)
        {
            _context = context;
            Log.Information("AccountService initialized.");
        }
        public void Register(User user)
        {
            Log.Information("Registering new user: {Username}", user.Username);
            _context.Users.Add(user);
            _context.SaveChanges();
            Log.Information("User registered successfully: {Username}", user.Username);
        }

    }
}
