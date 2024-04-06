using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LegacyApp
{
    public interface IClientRepository
    {
        Client? GetById(int clientId);
    }

    internal class ClientRepository : IClientRepository
    {
        /// <summary>
        /// This collection is used to simulate remote database
        /// </summary>
        public static readonly Dictionary<int, Client> Database = new List<Client>()
        {
            new(1, "Kowalski", "Warszawa, Złota 12", "kowalski@wp.pl", ClientType.Regular),
            new(
                2,
                "Malewski",
                "Warszawa, Koszykowa 86",
                "malewski@gmail.pl",
                ClientType.VeryImportant
            ),
            new(3, "Smith", "Warszawa, Kolorowa 22", "smith@gmail.pl", ClientType.Important),
            new(4, "Doe", "Warszawa, Koszykowa 32", "doe@gmail.pl", ClientType.Important),
            new(5, "Kwiatkowski", "Warszawa, Złota 52", "kwiatkowski@wp.pl", ClientType.Regular),
            new(
                6,
                "Andrzejewicz",
                "Warszawa, Koszykowa 52",
                "andrzejewicz@wp.pl",
                ClientType.Regular
            )
        }.ToDictionary(x => x.Id);

        public ClientRepository() { }

        /// <summary>
        /// Simulating fetching a client from remote database
        /// </summary>
        /// <returns>Returning client object or <c>null</c> if client doesn't exist</returns>
        public Client? GetById(int clientId)
        {
            int randomWaitTime = new Random().Next(2000);
            Thread.Sleep(randomWaitTime);

            return Database.GetValueOrDefault(clientId);
        }
    }
}
