using System;

namespace LegacyApp
{
    public sealed class UserService(
        IUserCreditService userCreditService,
        IClientRepository clientRepository
    )
    {
        [Obsolete("Legacy left for compatibility")]
        public UserService()
            : this(new UserCreditService(), new ClientRepository()) { }

        [Obsolete("Legacy left for compatibility")]
        public bool AddUser(
            string firstName,
            string lastName,
            string email,
            DateTime dateOfBirth,
            int clientId
        )
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return false;
            }

            if (!email.Contains('@') && !email.Contains('.'))
            {
                return false;
            }

            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (
                now.Month < dateOfBirth.Month
                || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)
            )
                age--;

            if (age < 21)
            {
                return false;
            }

            var client = clientRepository.GetById(clientId);

            var user = new User(
                client,
                DateOnly.FromDateTime(dateOfBirth),
                email,
                firstName,
                lastName,
                true,
                null
            );

            if (client.Type == "VeryImportantClient")
            {
                user = user with { HasCreditLimit = false };
            }
            else if (client.Type == "ImportantClient")
            {
                int creditLimit = userCreditService.GetCreditLimit(
                    user.LastName,
                    user.DateOfBirth.ToDateTime(default)
                );
                creditLimit *= 2;
                user = user with { HasCreditLimit = false, CreditLimit = creditLimit };
            }
            else
            {
                int creditLimit = userCreditService.GetCreditLimit(
                    user.LastName,
                    user.DateOfBirth.ToDateTime(default)
                );
                user = user with { HasCreditLimit = false, CreditLimit = creditLimit };
            }

            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                return false;
            }

            UserDataAccess.AddUser(user);
            return true;
        }
    }
}
