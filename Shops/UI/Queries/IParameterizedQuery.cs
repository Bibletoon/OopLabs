namespace Shops.UI.Queries
{
    public interface IParameterizedQuery<T, TArgument>
    {
        T Execute(TArgument argument);
    }
}