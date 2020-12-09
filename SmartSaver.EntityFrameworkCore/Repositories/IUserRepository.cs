namespace SmartSaver.EntityFrameworkCore.Repositories
{
    public interface IUserRepository
    {
        public bool DoesUsernameExist(string username);
        public bool DoesEmailExist(string email);
        public T GetId<T>(string email);
    }
}
