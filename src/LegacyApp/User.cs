using System;

namespace LegacyApp;

public record User(
    Client Client,
    DateOnly DateOfBirth,
    string EmailAddress,
    string FirstName,
    string LastName,
    bool HasCreditLimit,
    int? CreditLimit
);
