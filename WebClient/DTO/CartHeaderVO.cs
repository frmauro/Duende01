

namespace WebClient.DTO;

public class CartHeaderVO 
{
    public long Id { get; set; }
    public string? UserId { get; set; }
    public string CouponCode { get; set; } = "Empty";
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
}
