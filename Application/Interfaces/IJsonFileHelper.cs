namespace HackDemoAPI.Application.Interfaces
{
    public interface IJsonFileHelper
    {
        Task<List<T>> ReadAsync<T>(string filePath, CancellationToken cancellationToken);

        Task WriteAsync<T>(string filePath, List<T> data, CancellationToken cancellationToken);
    }
}
