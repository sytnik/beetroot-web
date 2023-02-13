using BlazorServerMud.DAO;
using BlazorServerMud.Logic;
using Microsoft.JSInterop;

namespace BlazorServerMud.Pages;

public sealed partial class Purchases
{
    private Manager _manager;
    private List<Product> _allProducts;
    private Purchase _purchase = new();

    private decimal _amount => CalculateAmount(_purchase);

    private decimal CalculateAmount(Purchase purchase)
    {
        decimal sum = 0;
        if (_manager is not null && purchase is not null)
            foreach (var purchaseProduct in purchase.PurchaseProducts)
                sum += purchaseProduct.Product.Price * purchaseProduct.Counter;
        return sum;
    }

    protected override async Task OnInitializedAsync()
    {
        _allProducts = await Repository.GetAllProducts();
        await GetPurchases();
        _purchase = new Purchase
        {
            Id = await Repository.GetNewPurchaseId(), ManagerId = _manager.Id,
            PurchaseProducts = new List<PurchaseProduct>()
        };
        foreach (var product in _allProducts)
        {
            _purchase.PurchaseProducts.Add(new PurchaseProduct
                {PurchaseId = _purchase.Id, ProductId = product.Id, Counter = 0, Product = product});
        }
    }

    private async Task GetPurchases()
    {
        var manager = await Repository.GetManager(Authentication);
        if (manager is not null) _manager = await Repository.GetManagerWithData(manager.Id);
    }

    private async Task OnSubmitPurchase()
    {
        await Repository.SubmitPurchase(_purchase);
        await GetPurchases();
    }

    private async Task OnDeletePurchase(int id)
    {
        if (!await Js.InvokeAsync<bool>("confirm", $"Are you sure to delete purchase {id}?")) return;
        await Repository.DeletePurchase(id);
        await GetPurchases();
    }
}