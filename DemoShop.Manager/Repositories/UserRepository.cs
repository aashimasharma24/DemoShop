using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories.Interfaces;

namespace DemoShop.Manager.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DemoShopDbContext _context;

        public UserRepository(DemoShopDbContext context)
        {
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User GetByGUID(String guid)
        {
            return _context.Users.FirstOrDefault(x => x.Guid == guid);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Delete(User user)
        {
            _context.Users.Remove(user);
        }
    }
}
