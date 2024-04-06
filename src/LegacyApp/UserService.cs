using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LegacyApp;

// Zakładam że po prawdziwym refactorze ustawiłbym kontener IoC i wystawił interfejs pod rejestracje do niego, więc implementacje są internal
public sealed partial class UserService(
    IUserCreditService userCreditService,
    IClientRepository clientRepository,
    IUserDataAccess userDataAccess,
    TimeProvider timeProvider // <3 .NET 8
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
            TimeProvider.System
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

        if (!ValidateAgeRestriction(dateOfBirth))
        {
            return null;
        }

        var client = clientRepository.GetById(clientId);

        var creditLimit = userCreditService.GetCreditLimit(lastName, client.Type);
        var user = new User(client, dateOfBirth, email, firstName, lastName, creditLimit);

        if (user.CreditLimit is { Enforced: true, Value: < 500m })
        {
            return null;
        }

        userDataAccess.AddUser(user);
        return user;
    }

    public void Dispose() => _compositeDisposables.ForEach(disposable => disposable.Dispose());

    private bool ValidateAgeRestriction(DateOnly dateOfBirth)
    {
        var now = timeProvider.GetLocalNow();

        int age = now.Year - dateOfBirth.Year;
        if (
            now.Month < dateOfBirth.Month
            || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)
        )
        {
            age--;
        }

        return age >= 21;
    }
}
