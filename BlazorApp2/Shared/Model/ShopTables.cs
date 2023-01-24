using BlazorApp2.Shared.DAO;

namespace BlazorApp2.Server.Logic;

// customer has many purchases
// product
// purchase has many product, last purchase/with some status is the shopping cart
// purchaseProduct, has product count 

public record Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public List<PurchaseProduct> PurchaseProducts { get; set; }
    public List<Purchase> Purchases { get; set; }
}

public record Purchase
{
    public int Id { get; set; }
    public int ManagerId { get; set; }
    public Manager Manager { get; set; }
    public List<PurchaseProduct> PurchaseProducts { get; set; }
    public List<Product> Products { get; set; }
}

public record PurchaseProduct
{
    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public int Counter { get; set; }
}