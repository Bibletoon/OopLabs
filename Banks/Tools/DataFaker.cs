using Banks.Accounts;
using Banks.Banks;
using Banks.Builders;

namespace Banks.Tools
{
    public class DataFaker
    {
        private Bank _bank;

        public DataFaker(Bank bank)
        {
            _bank = bank;
        }

        public void CreateData()
        {
            var client = new ClientBuilder("Name", "Surname").Build();
            var clientTwo = new ClientBuilder("Eman", "Emanrus").SetAddress("Address").SetPassportNumber(1337).Build();

            _bank.CreateDebitAccount(client, 1000);
            _bank.CreateDepositAccount(client, 5000);
            _bank.CreateCreditAccount(clientTwo, 777);
        }
    }
}