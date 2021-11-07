using System;
using Banks.Accounts;

namespace Banks.Commands
{
    public abstract class Command
    {
        public Guid Id { get; internal set; } = Guid.NewGuid();
        public abstract void Execute(BankAccount account);
    }
}