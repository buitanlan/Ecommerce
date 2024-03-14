using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Ecommerce.Public.Orders;
using Ecommerce.Public.Web.Extensions;
using Ecommerce.Public.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ecommerce.Public.Web.Pages.Cart;

public class CheckoutModel(IOrdersAppService ordersAppService) : PageModel
{
    public List<CartItem> CartItems { get; set; }

    public bool? CreateStatus { set; get; }

    [BindProperty] public OrderDto Order { set; get; }

    public void OnGet()
    {
        CartItems = GetCartItems();
    }

    public async Task OnPostAsync()
    {
        if (ModelState.IsValid == false)
        {
        }

        var cartItems = GetCartItems().Select(item => new OrderItemDto
            { Price = item.Product.SellPrice, ProductId = item.Product.Id, Quantity = item.Quantity }).ToList();
        Guid? currentUserId = User.Identity.IsAuthenticated ? User.GetUserId() : null;
        var order = await ordersAppService.CreateAsync(new CreateOrderDto()
        {
            CustomerName = Order.CustomerName,
            CustomerAddress = Order.CustomerAddress,
            CustomerPhoneNumber = Order.CustomerPhoneNumber,
            Items = cartItems,
            CustomerUserId = currentUserId
        });
        CartItems = GetCartItems();

        CreateStatus = order != null;
    }

    private List<CartItem> GetCartItems()
    {
        var cart = HttpContext.Session.GetString(EcommerceConsts.Cart);
        var productCarts = new Dictionary<string, CartItem>();
        if (cart != null)
        {
            productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
        }

        return productCarts.Values.ToList();
    }
}
