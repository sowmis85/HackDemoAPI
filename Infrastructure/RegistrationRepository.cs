using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Application.Interfaces.IRepository;
using HackDemoAPI.Domain.Entities;

namespace HackDemoAPI.Infrastructure
{
    public class RegistrationRepository: IRegistrationRepository
    {
        private readonly IJsonFileHelper _jsonFileHelper;
        private readonly string _filePath;

        public RegistrationRepository(
            IWebHostEnvironment environment,
            IJsonFileHelper jsonFileHelper)
        {
            _jsonFileHelper = jsonFileHelper;

            _filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "registrations.json");
        }

        public async Task CreateAsync(Registration registration, CancellationToken cancellationToken)
        {
            var registrations =
                await _jsonFileHelper.ReadAsync<Registration>(
                    _filePath,
                    cancellationToken);

            registrations.Add(registration);

            await _jsonFileHelper.WriteAsync(
                _filePath,
                registrations,
                cancellationToken);
        }

        public async Task<List<Registration>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _jsonFileHelper.ReadAsync<Registration>(
                _filePath,
                cancellationToken);
        }

        public async Task<Registration?> GetByIdAsync(string registrationId, CancellationToken cancellationToken)
        {
            var registrations = await _jsonFileHelper.ReadAsync<Registration>(_filePath, cancellationToken);

            return registrations.FirstOrDefault(x =>
                x.RegistrationId.Equals(
                    registrationId,
                    StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Registration?> GetByEmailAndEventAsync(string email, string eventId, CancellationToken cancellationToken)
        {
            var registrations = await _jsonFileHelper.ReadAsync<Registration>(
                    _filePath,
                    cancellationToken);

            return registrations.FirstOrDefault(x =>
                x.AttendeeEmail.Equals(
                    email,
                    StringComparison.OrdinalIgnoreCase)
                &&
                x.EventId.Equals(
                    eventId,
                    StringComparison.OrdinalIgnoreCase));
        }

        public async Task UpdateAsync(Registration registration, CancellationToken cancellationToken)
        {
            var registrations =
                await _jsonFileHelper.ReadAsync<Registration>(
                    _filePath,
                    cancellationToken);

            var existing = registrations.FirstOrDefault(x =>
                x.RegistrationId == registration.RegistrationId);

            if (existing == null)
            {
                return;
            }

            var index = registrations.IndexOf(existing);

            registrations[index] = registration;

            await _jsonFileHelper.WriteAsync(
                _filePath,
                registrations,
                cancellationToken);
        }
    }
}
