
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CouponApi.Data.ValueObject;

public class CouponVO
{
    public long Id { get; set; }
    public string CouponCode { get; set; }
    public decimal DiscountAmount { get; set; }
}
