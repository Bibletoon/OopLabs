using System;

namespace Banks.Clients
{
    public class Client
    {
        private string _address;
        private uint? _passportNumber;

        internal Client(string name, string surname, string address, uint? passportNumber)
        {
            Name = name;
            Surname = surname;
            _address = address;
            _passportNumber = passportNumber;
        }

        private Client()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string Address { get; internal set; }

        public uint? PassportNumber { get; internal set; }

        public bool IsConfirmed => _passportNumber is not null && !string.IsNullOrWhiteSpace(_address);

        public void Notify(string message)
        {
        }
    }
}