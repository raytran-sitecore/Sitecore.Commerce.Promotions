﻿using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Rules;
using System;
using System.Linq;

namespace Feature.Carts.Engine
{
    [EntityIdentifier(nameof(CartItemQuantityXForQuantityYAction))]
    public class CartItemQuantityXForQuantityYAction : CartTargetItemId, ICartLineAction
    {
        public IRuleValue<int> QuantityX { get; set; }

        public IRuleValue<int> QuantityY { get; set; }
        
        public void Execute(IRuleExecutionContext context)
        {
            var commerceContext = context.Fact<CommerceContext>();
            var cart = commerceContext?.GetObject<Cart>();
            var totals = commerceContext?.GetObject<CartTotals>();
            if (cart == null || !cart.Lines.Any() || totals == null || !totals.Lines.Any())
            {
                return;
            }

            var quantityX = QuantityX.Yield(context);
            var quantityY = QuantityY.Yield(context);
            if (quantityX <= 0 || quantityY <= 0 || quantityX <= quantityY)
            {
                return;
            }
            
            var lines = this.MatchingLines(context).ToList();
            lines = lines.Where(l => l.Quantity >= quantityX).ToList();

            if (!lines.Any())
            {
                return;
            }

            var propertiesModel = commerceContext.GetObject<PropertiesModel>();
            var discount = commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount;

            foreach (var line in lines)
            {
                var timesQualified = Math.Floor(line.Quantity / quantityX);
                var discountValue = (quantityX - quantityY) * line.UnitListPrice.Amount * timesQualified;

                if (commerceContext.GetPolicy<GlobalPricingPolicy>().ShouldRoundPriceCalc)
                {
                    discountValue = decimal.Round(
                            discountValue,
                            commerceContext.GetPolicy<GlobalPricingPolicy>().RoundDigits,
                            commerceContext.GetPolicy<GlobalPricingPolicy>().MidPointRoundUp ?
                                MidpointRounding.AwayFromZero :
                                MidpointRounding.ToEven
                        );
                }

                discountValue *= decimal.MinusOne;
                line.Adjustments.Add(new CartLineLevelAwardedAdjustment()
                {
                    Name = (propertiesModel?.GetPropertyValue("PromotionText") as string ?? discount),
                    DisplayName = (propertiesModel?.GetPropertyValue("PromotionCartText") as string ?? discount),
                    Adjustment = new Money(commerceContext.CurrentCurrency(), discountValue),
                    AdjustmentType = discount,
                    IsTaxable = false,
                    AwardingBlock = nameof(CartItemQuantityXForQuantityYAction)
                });
                totals.Lines[line.Id].SubTotal.Amount = totals.Lines[line.Id].SubTotal.Amount + discountValue;
                line.GetComponent<MessagesComponent>().AddMessage(
                    commerceContext.GetPolicy<KnownMessageCodePolicy>().Promotions,
                    $"PromotionApplied: {propertiesModel?.GetPropertyValue("PromotionId") ?? nameof(CartItemQuantityXForQuantityYAction)}");
            }
        }
    }
}
