using CartApi.Model.Base;


namespace CartApi.Data.ValueObject;

public class CartHeaderVO 
{
    public long Id { get; set; }
    public string UserId { get; set; }
    public string CouponCode { get; set; }
}
