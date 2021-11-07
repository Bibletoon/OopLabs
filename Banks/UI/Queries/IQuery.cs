namespace Banks.UI.Queries
{
    public interface IQuery<T>
    {
        T Execute();
    }
}