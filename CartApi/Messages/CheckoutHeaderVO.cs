﻿using CartApi.Data.ValueObject;
using MessageBus;

namespace CartApi.Messages;

public class CheckoutHeaderVO : BaseMessage
{
    public string? UserId { get; set; }
    public string CouponCode { get; set; } = "";
    public decimal PurshaseAmount { get; set; }
    public decimal DiscountTotal { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateTime { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? CardNumber { get; set; }
    public string? CVV { get; set; }
    public string? ExpireMothYear { get; set; }
    public int CartTotalItens { get; set; }
    public IEnumerable<CartDetailVO>? CartDetails { get; set; }
}
