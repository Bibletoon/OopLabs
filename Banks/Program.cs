using System;
using Banks.Accounts.AccountConfigurations;
using Banks.Accounts.AccountConfigurations.Plans;
using Banks.Banks;
using Banks.Builders;
using Banks.Data;
using Banks.Tools;
using Banks.UI;
using Microsoft.EntityFrameworkCore;
using Terminal.Gui;

namespace Banks
{
    public static class Program
    {
        private static void Main()
        {
            new ApplicationManager().Run();
        }

        public class DateTimeProvider : IDateTimeProvider
        {
            public DateTime DateTime { get; set; } = DateTime.Now;

            public DateTime Now() => DateTime;
        }
    }
}