using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories.Interfaces;
using DemoShop.Manager.Services.Interfaces;

namespace DemoShop.Manager.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly DemoShopDbContext _context;

        public UserService(IUserRepository repository, DemoShopDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public IEnumerable<User> GetAll()
        {
            return _repository.GetAll();
        }

        public User GetByGUID(String guid)
        {
            return _repository.GetByGUID(guid);
        }

        public User GetByUsername(String username)
        {
            return _repository.GetByUsername(username);
        }

        public void Add(User user)
        {
            _repository.Add(user);
            _context.SaveChanges();
        }

        public void Update(String guid, User user)
        {
            var existingUser = _repository.GetByGUID(guid);
            if (existingUser != null)
            {
                _repository.Update(user);
                _context.SaveChanges();
            }
        }

        public void Delete(String guid)
        {
            var user = _repository.GetByGUID(guid);
            if (user != null)
            {
                _repository.Delete(user);
                _context.SaveChanges();
            }
        }

    }
}
