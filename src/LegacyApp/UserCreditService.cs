using System;
using System.Collections.Generic;
using System.Threading;

namespace LegacyApp
{
    public interface IUserCreditService
    {
        CreditLimit? GetCreditLimit(string lastName, ClientType clientType);
    }

    internal sealed class UserCreditService : IUserCreditService, IDisposable
    {
        /// <summary>
        /// Simulating database
        /// </summary>
        private readonly Dictionary<string, int> _database =
            new()
            {
                { "Kowalski", 200 },
                { "Malewski", 20000 },
                { "Smith", 10000 },
                { "Doe", 3000 },
                { "Kwiatkowski", 1000 }
            };

        public void Dispose()
        {
            //Simulating disposing of resources
        }

        /// <summary>
        /// This method is simulating contact with remote service which is used to get info about someone's credit limit
        /// </summary>
        /// <returns>Client's credit limit</returns>
        public CreditLimit? GetCreditLimit(string lastName, ClientType clientType)
        {
            int randomWaitingTime = new Random().Next(3000);
            Thread.Sleep(randomWaitingTime);
            return clientType switch
            {
                ClientType.Regular => new(true, GetFromDatabase(lastName)),
                ClientType.Important => new(false, GetFromDatabase(lastName)),
                ClientType.VeryImportant => null,
                _ => throw new ArgumentOutOfRangeException(nameof(clientType))
            };

            int GetFromDatabase(string lastName)
            {
                if (_database.TryGetValue(lastName, out int value))
                {
                    return value;
                }

                // I absolutely hate exceptions... but this is just refactor...
                throw new ArgumentException($"Client {lastName} does not exist");
            }
        }
    }
}
