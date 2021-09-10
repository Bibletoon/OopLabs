namespace Shops.Models
{
    public class Product
    {
        internal Product(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; init; }
        public string Name { get; init; }

        public Lot ToLot(int count, int price) => new Lot(this, count, price);

        public ProductOrder ToOrder(int count) => new ProductOrder(this, count);
    }
}