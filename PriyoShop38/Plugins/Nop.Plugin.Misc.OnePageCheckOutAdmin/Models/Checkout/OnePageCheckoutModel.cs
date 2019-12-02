﻿using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.ShoppingCart;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Models.Checkout
{
    public partial class OnePageCheckoutModel : BaseNopModel
    {
        public OnePageCheckoutModel()
        {
            ShippingAddresses=new CheckoutShippingAddressModel();
            BillingAddresses = new CheckoutBillingAddressModel();
            ShoppingCart= new ShoppingCartModel();
            CheckoutShippingMethod= new CheckoutShippingMethodModel();
            CheckoutPaymentMethod= new CheckoutPaymentMethodModel();
            AllOrderTotalsModel = new OrderTotalsModel();
        }
        public bool ShippingRequired { get; set; }
        public bool DisableBillingAddressCheckoutStep { get; set; }
        public CheckoutBillingAddressModel BillingAddresses{ get; set; }
        public CheckoutShippingAddressModel ShippingAddresses{ get; set; }
        public ShoppingCartModel ShoppingCart{ get; set; }
        public CheckoutShippingMethodModel CheckoutShippingMethod{ get; set; }
        public CheckoutPaymentMethodModel CheckoutPaymentMethod{ get; set; }
        public OrderTotalsModel AllOrderTotalsModel { get; set; }
        public int SelectedShippingAdressId { get; set; }
        
        public IList<Customer> Customers { get; set; }        
        public int ProductId { get; set; }
        public int SearchTermMinimumLength { get; set; }

    }
}