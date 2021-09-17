namespace Shops.UI.Queries
{
    public interface IQuery<T>
    {
        T Execute();
    }
}