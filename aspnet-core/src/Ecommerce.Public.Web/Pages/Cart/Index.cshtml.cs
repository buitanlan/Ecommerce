using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Ecommerce.Public.Catalog.Products;
using Ecommerce.Public.Web.Extensions;
using Ecommerce.Public.Web.Models;

namespace Ecommerce.Public.Web.Pages.Cart
{
    public class IndexModel(IProductsAppService productsAppService) : PageModel
    {
        [BindProperty]
        public List<CartItem> CartItems { get; set; }
        public async Task OnGetAsync(string action, string id)
        {
            var cart = HttpContext.Session.GetString(EcommerceConsts.Cart);
            var productCarts = new Dictionary<string, CartItem>();
            if(cart != null)
            {
                productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
            }
            if (!string.IsNullOrEmpty(action))
            {
                if (action == "add")
                {
                    var product = await productsAppService.GetAsync(Guid.Parse(id));
                    if (cart == null)
                    {
                        productCarts.Add(id, new CartItem()
                        {
                            Product = product,
                            Quantity = 1
                        });
                        HttpContext.Session.SetString(EcommerceConsts.Cart, JsonSerializer.Serialize(productCarts));
                    }
                    else
                    {
                        productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
                        if (productCarts.ContainsKey(id)){
                            productCarts[id].Quantity += 1;
                        }
                        else
                        {
                            productCarts.Add(id, new CartItem()
                            {
                                Product = product,
                                Quantity = 1
                            });
                        }
                        HttpContext.Session.SetString(EcommerceConsts.Cart, JsonSerializer.Serialize(productCarts));
                    }
                }
                else if(action == "remove")
                {
                    productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
                    if (productCarts.ContainsKey(id))
                    {
                        productCarts.Remove(id);
                    }

                    HttpContext.Session.SetString(EcommerceConsts.Cart, JsonSerializer.Serialize(productCarts));
                }
            }
            CartItems = productCarts.Values.ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var cart = HttpContext.Session.GetString(EcommerceConsts.Cart);
            var productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
            foreach(var item in productCarts)
            {
                var cartItem = CartItems.FirstOrDefault(x => x.Product.Id == item.Value.Product.Id);
                if (cartItem != null)
                {
                    cartItem.Product = await productsAppService.GetAsync(cartItem.Product.Id);
                    item.Value.Quantity = cartItem.Quantity;
                }
            }

            HttpContext.Session.SetString(EcommerceConsts.Cart, JsonSerializer.Serialize(productCarts));
            return Redirect("/shop-cart.html");
        }
    }
}
