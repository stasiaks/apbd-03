using System;

namespace LegacyApp;

public record User(
    Client Client,
    DateOnly DateOfBirth,
    Email EmailAddress,
    string FirstName,
    string LastName,
    CreditLimit? CreditLimit
);

public record CreditLimit(bool Enforced, decimal Value);
