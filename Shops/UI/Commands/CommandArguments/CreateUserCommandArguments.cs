namespace Shops.UI.Commands.CommandArguments
{
    public class CreateUserCommandArguments
    {
        public CreateUserCommandArguments(string name, int money)
        {
            Name = name;
            Money = money;
        }

        public string Name { get; }
        public int Money { get; }
    }
}