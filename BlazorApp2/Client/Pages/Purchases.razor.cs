using System.Net.Http.Json;
using BlazorApp2.Server.Logic;
using Microsoft.AspNetCore.Components;

namespace BlazorApp2.Client.Pages;

public sealed partial class Purchases
{
    [Inject] public HttpClient Client { get; set; }
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
        _allProducts = await Client.GetFromJsonAsync<List<Product>>("getallproducts");
        await GetPurchases();
        _purchase = new Purchase
        {
            Id = await Client.GetFromJsonAsync<int>("GetNewPurchaseId"), ManagerId = _manager.Id,
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
        var manager = await Client.GetFromJsonAsync<Manager>("getmanager");
        if (manager is not null)
        {
            _manager = await Client.GetFromJsonAsync<Manager>($"getmanagerwithdata?id={manager.Id}");
        }
    }

    private async Task OnSubmitPurchase()
    {
        await Client.PostAsJsonAsync("SubmitPurchase", _purchase);
        await GetPurchases();
    }

    private async Task OnDeletePurchase(int id)
    {
        if (!await Js.InvokeAsync<bool>("confirm", $"Are you sure to delete purchase {id}?")) return;
        await Http.DeleteAsync($"DeletePurchase?id={id}");
        await GetPurchases();
    }
}