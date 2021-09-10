using Shops.Tools;

namespace Shops.Models
{
    public class Person
    {
        public Person(string name, int money)
        {
            Name = name;
            Money = money;
        }

        public string Name { get; init; }
        public long Money { get; private set; }

        internal void PayMoney(long amount)
        {
            if (amount > Money)
                throw new ShopsException("Person has not enough money to pay");

            Money -= amount;
        }
    }
}