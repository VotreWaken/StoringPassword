using StoringPassword.Models;

namespace StoringPassword.Repos
{
    public interface IAccountsRepository
    {
        Task<List<User>> GetAllUsers();

        IQueryable<User> GetUsersByLogin(LoginModel user);

        Task<User> CreateAndHashPassword(User user, RegisterModel model);

        Task<bool> IsPasswordCorrect(User user, LoginModel model);

        Task<bool> IsLoginExists(string? login);

        Task AddUserToDb(User user);
    }
}
