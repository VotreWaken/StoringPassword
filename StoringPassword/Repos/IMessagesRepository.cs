using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoringPassword.Models;

namespace StoringPassword.Repos
{
    public interface IMessagesRepository
    {
        Task<List<Message>> GetAllMessages();
        Task<List<Message>> GetUserMessages(User userSpecification);

        IQueryable<User> GetUserByLogin(string? login);

        Task<Message?> GetMessage(int? id);

        Task CreateMessage(Message msg);

        void UpdateMessage(Message msg);

        Task DeleteMessage(int id);

        Task SaveChanges();
        Task<Message?> GetMessageDetails(int? id);
        Task<bool> MessageExists(int id);
    }
}
