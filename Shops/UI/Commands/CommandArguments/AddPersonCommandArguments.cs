namespace Shops.UI.Commands.CommandArguments
{
    public class AddPersonCommandArguments
    {
        public AddPersonCommandArguments(string name, int money)
        {
            Name = name;
            Money = money;
        }

        public string Name { get; }
        public int Money { get; }
    }
}