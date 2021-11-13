using Banks.Clients;

namespace Banks.UI.Commands.CommandArguments
{
    public class CreateAccountCommandArguments
    {
        public CreateAccountCommandArguments(Client client, decimal startBalance)
        {
            Client = client;
            StartBalance = startBalance;
        }

        public Client Client { get; }
        public decimal StartBalance { get; }
    }
}