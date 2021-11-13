using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Banks;
using Banks.Builders;
using Banks.Data;
using Banks.Tools;

namespace Banks.Clients
{
    public class ClientService
    {
        private BanksDbContext _dbContext;

        public ClientService(BanksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ClientBuilder CreateClientBuilder(string name, string surname)
        {
            return new ClientBuilder(name, surname);
        }

        public void RegisterClient(Client client)
        {
            _dbContext.Clients.Add(client);
            _dbContext.SaveChanges();
        }

        public List<Client> GetClients()
        {
            return _dbContext.Clients.ToList();
        }

        public void ChangeAddress(Client client, string address)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));

            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Can't set empty string");
            client.Address = address;
            _dbContext.Clients.Update(client);
            _dbContext.SaveChanges();
        }

        public void ChangePassportNumber(Client client, uint passportNumber)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            client.PassportNumber = passportNumber;
            _dbContext.Clients.Update(client);
            _dbContext.SaveChanges();
        }
    }
}