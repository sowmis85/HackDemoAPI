using HackDemoAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HackDemoAPI.Infrastructure
{
    public class JsonFileHelper : IJsonFileHelper
    {
        public async Task<List<T>> ReadAsync<T>(string filePath, CancellationToken cancellationToken)
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            var json = await File.ReadAllTextAsync(
                filePath,
                cancellationToken);
            try
            {
                return JsonSerializer.Deserialize<List<T>>(json)
                    ?? new List<T>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);

                throw;
            }
        }

        public async Task WriteAsync<T>(string filePath, List<T> data, CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            await File.WriteAllTextAsync(
                filePath,
                json,
                cancellationToken);
        }
    }
}
