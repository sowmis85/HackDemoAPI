using Azure.Core;
using HackDemoAPI.Application.DTO.Responses;
using HackDemoAPI.Application.Interfaces;
using HackDemoAPI.Application.Interfaces.IRepository;
using HackDemoAPI.Common;
using HackDemoAPI.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace HackDemoAPI.Infrastructure
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private readonly IJsonFileHelper _jsonFileHelper;
        private readonly string _filePath;
        private readonly string _eventsFilePath;

        public RegistrationRepository(
            IWebHostEnvironment environment,
            IJsonFileHelper jsonFileHelper)
        {
            _jsonFileHelper = jsonFileHelper;

            _filePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "registrations.json");

            // Get the events file path
            _eventsFilePath = Path.Combine(
                environment.ContentRootPath,
                "Data",
                "events.json");
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
                    _filePath, cancellationToken);

            var existingRegistration = registrations.FirstOrDefault(x =>
                x.RegistrationId == registration.RegistrationId);

            if (existingRegistration == null)
            {
                return;
            }
            existingRegistration.Status = registration.Status;
            existingRegistration.RegistrationId = registration.RegistrationId;

            await _jsonFileHelper.WriteAsync(
                _filePath,
                registrations,
                cancellationToken);
        }

        public async Task<IEnumerable<UserEventRegisResponse>> GetRegistrationsByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            var registrations = await _jsonFileHelper.ReadAsync<Registration>(
                 _filePath,
                 cancellationToken);

            // Read events from file
            var events = await _jsonFileHelper.ReadAsync<Event>(
                _eventsFilePath,
                cancellationToken);

            // Create event lookup for efficient joining
            var eventLookup = events.ToDictionary(x => x.EventId);

            // Filter user registrations and join with event details
            var userRegistrations = registrations
                .Where(x => x.CreatedBy.Equals(userId, StringComparison.OrdinalIgnoreCase))
                .Select(registration =>
                {
                    var eventExists = eventLookup.TryGetValue(registration.EventId, out var @event);

                    return new UserEventRegisResponse
                    {
                        RegistrationId = registration.RegistrationId,
                        EventId = registration.EventId,
                        AttendeeName=registration.AttendeeName,
                        EventName = eventExists ? @event!.Title : "Event Not Found",
                        EventDate = eventExists ? @event!.StartDate.DateTime : registration.CreatedOn,
                        Location = eventExists ? @event!.Location : string.Empty,
                        Status = registration.Status
                    };
                })
                .OrderByDescending(x => x.EventDate)
                .ToList();
            return userRegistrations;
        }
    }
}
