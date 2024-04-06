using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LegacyApp
{
    // Zakładam że po prawdziwym refactorze ustawiłbym kontener IoC i wystawił interfejs pod rejestracje do niego, więc implementacje są internal
    public sealed partial class UserService(
        IUserCreditService userCreditService,
        IClientRepository clientRepository,
        IUserDataAccess userDataAccess,
        IDateTimeProvider dateTimeProvider
    ) : IDisposable // Only for purpose of handling legacy service instantiation
    {
        private readonly IUserCreditService userCreditService = userCreditService;
        private readonly List<IDisposable> _compositeDisposables = []; // Przyzwyczajenie do klasy System.Reactive.Disposables.CompositeDisposable daje się we znaki...

        [Obsolete("Legacy left for compatibility")]
        public UserService()
            : this(
                new UserCreditService(),
                new ClientRepository(),
                new UserDataAccessWrapper(),
                new DateTimeProvider()
            )
        {
            _compositeDisposables.Add((UserCreditService)userCreditService);
        }

        [GeneratedRegex("(?<local>.+)@(?<subdomain>.+)\\.(?<tld>.+)")]
        private static partial Regex EmailRegex();

        [Obsolete("Legacy left for compatibility")]
        public bool AddUser(
            string firstName,
            string lastName,
            string email,
            DateTime dateOfBirth,
            int clientId
        )
        {
            var emailMatch = EmailRegex().Match(email);
            if (!emailMatch.Success)
            {
                return false;
            }
            var emailRecord = new Email(
                emailMatch.Groups["local"].Value,
                new Domain(emailMatch.Groups["subdomain"].Value, emailMatch.Groups["tld"].Value)
            );
            return AddUser(
                firstName,
                lastName,
                emailRecord,
                DateOnly.FromDateTime(dateOfBirth),
                clientId
            )
                is not null;
        }

        public User? AddUser(
            string firstName,
            string lastName,
            Email email,
            DateOnly dateOfBirth,
            int clientId
        )
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return null;
            }

            var now = dateTimeProvider.Now;
            int age = now.Year - dateOfBirth.Year;
            if (
                now.Month < dateOfBirth.Month
                || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)
            )
                age--;

            if (age < 21)
            {
                return null;
            }

            var client = clientRepository.GetById(clientId);

            if (client is null)
            {
                throw new ArgumentException("There is no client with given id", nameof(clientId));
            }

            var user = new User(client, dateOfBirth, email, firstName, lastName, null);

            if (client.Type == ClientType.VeryImportant) { }
            else if (client.Type == ClientType.Important)
            {
                int creditLimit = userCreditService.GetCreditLimit(
                    user.LastName,
                    user.DateOfBirth.ToDateTime(default)
                );
                creditLimit *= 2;
                user = user with { CreditLimit = new(false, creditLimit) };
            }
            else
            {
                int creditLimit = userCreditService.GetCreditLimit(
                    user.LastName,
                    user.DateOfBirth.ToDateTime(default)
                );
                user = user with { CreditLimit = new(true, creditLimit) };
            }

            if (user.CreditLimit is { Enforced: true, Value: < 500m })
            {
                return null;
            }

            userDataAccess.AddUser(user);
            return user;
        }

        public void Dispose() => _compositeDisposables.ForEach(disposable => disposable.Dispose());
    }
}
