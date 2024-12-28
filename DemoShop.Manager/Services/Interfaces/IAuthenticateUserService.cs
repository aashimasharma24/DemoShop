namespace DemoShop.Manager.Services.Interfaces
{
    public interface IAuthenticateUserService
    {
        string Authenticate(string username, string password);
    }
}
