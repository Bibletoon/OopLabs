using System;
using Banks.Banks;
using Banks.Clients;

namespace Banks.Builders
{
    public class ClientBuilder
    {
        private string _name;
        private string _surname;
        private string _address;
        private uint _passportNumber;

        public ClientBuilder(string name, string surname)
        {
            _name = name;
            _surname = surname;
        }

        public ClientBuilder SetAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Invalid address");
            _address = address;
            return this;
        }

        public ClientBuilder SetPassportNumber(uint passportNumber)
        {
            _passportNumber = passportNumber;
            return this;
        }

        public Client Build()
        {
            return new Client(_name, _surname, _address, _passportNumber);
        }
    }
}