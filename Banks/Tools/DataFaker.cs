using Banks.Accounts;
using Banks.Banks;
using Banks.Builders;

namespace Banks.Tools
{
    public class DataFaker
    {
        private Bank _bank;
        private CentralBank _centralBank;

        public DataFaker(CentralBank centralBank, Bank bank)
        {
            _centralBank = centralBank;
            _bank = bank;
        }

        public void CreateData()
        {
            var client = new ClientBuilder("Name", "Surname").Build();
            var clientTwo = new ClientBuilder("Eman", "Emanrus").SetAddress("Address").SetPassportNumber(1337).Build();

            _centralBank.CreateDebitAccount(_bank.Id, client, 1000);
            _centralBank.CreateDepositAccount(_bank.Id, client, 5000);
            _centralBank.CreateCreditAccount(_bank.Id, clientTwo, 777);
        }
    }
}