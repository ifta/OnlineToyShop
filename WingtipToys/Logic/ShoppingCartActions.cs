﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
    public class ShoppingCartActions : IDisposable
    {
        public string ShoppingCartId { get; set; }

        private ProductContext _db = new ProductContext();

        public const string CartSessionKey = "CartId";

        public void AddToCart(int id)
        {
            //Retrieve the product from database
            ShoppingCartId = GetCartId();

            var cartItem = _db.ShoppingCartItems.SingleOrDefault( c => c.CartId == ShoppingCartId && c.ProductId == id);

            if(cartItem == null)
            {
                //Create a new cart item If no cart Item exist
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = id,
                    Product = _db.Products.SingleOrDefault(p => p.ProductID == id),
                    Quantity = 1,
                    DateCreated = DateTime.Now
                };
                _db.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                //If the item exist ,
                // then add 1 cart item in list
                cartItem.Quantity++;
            }
            _db.SaveChanges();
        } 
        //End of AddToCart function

        public void Dispose() 
        {
            if (_db != null)
            {
                _db.Dispose();
                _db = null;
            }
        }

        public string GetCartId() 
        {
            if (HttpContext.Current.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session[CartSessionKey] = HttpContext.Current.User.Identity.Name;
                }
                else 
                {
                    //Generate a new Random GUID using System.Guid Class
                    Guid tempCartId = Guid.NewGuid();
                    HttpContext.Current.Session[CartSessionKey] = tempCartId.ToString();
                }
            }

            return HttpContext.Current.Session[CartSessionKey].ToString();
        }
        // End of GetCartId function

        public List<CartItem> GetCartItems() 
        {
            ShoppingCartId = GetCartId();

            return _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId).ToList();
        }
    }
}