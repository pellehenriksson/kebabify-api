namespace Kebabify.Api.Services
{
    public interface IStorageService
    {
        Task Persist(string input, string result);
    }
}
