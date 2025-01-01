﻿using DemoShop.Core.DataObjects;

namespace DemoShop.Manager.Services.Interfaces
{
    public interface IAuthenticateUserService
    {
        string Authenticate(string username, string password);
        void Register(User user);
        void RequestPasswordReset(string email);
        void ResetPassword(string token, string newPassword);
    }
}
