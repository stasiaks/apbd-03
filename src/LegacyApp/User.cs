using System;

namespace LegacyApp;

public record User(
    Client Client,
    DateOnly DateOfBirth,
    Email EmailAddress,
    string FirstName,
    string LastName,
    bool IsExemptFromCreditLimitMinimum,
    int? CreditLimit
);
