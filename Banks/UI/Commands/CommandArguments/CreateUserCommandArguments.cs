namespace Banks.UI.Commands.CommandArguments
{
    public class CreateUserCommandArguments
    {
        public CreateUserCommandArguments(string name, string surname, string address, uint? passportNumber)
        {
            Name = name;
            Surname = surname;
            Address = address;
            PassportNumber = passportNumber;
        }

        public string Name { get; }
        public string Surname { get; }
        public string Address { get; }
        public uint? PassportNumber { get; }
    }
}