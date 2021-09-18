using System;
using Shops.Tools;

namespace Shops.Models
{
    public class User
    {
        public User(string name, int money)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            if (name == string.Empty)
                throw new ShopsException("Name can't be empty");
            Name = name;

            if (money <= 0)
                throw new ShopsException("Money can't be zero or less");
            Money = money;
        }

        public string Name { get; init; }
        public int Money { get; private set; }

        internal void PayMoney(int amount)
        {
            if (amount > Money)
                throw new ShopsException("User has not enough money to pay");

            Money -= amount;
        }
    }
}