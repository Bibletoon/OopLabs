namespace Shops.UI.Commands
{
    public interface IParameterizedCommand<TArgument>
    {
        CommandResult Execute(TArgument argument);
    }
}