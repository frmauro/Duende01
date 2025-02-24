﻿using CartApi.Data.ValueObject;

namespace CartApi.Repository;

public interface ICartRepository
{
    Task<CartVO> FindCartByUserId(string userId);
    Task<CartVO> SaveOrUpdateCart(CartVO cart);
    Task<bool> RemoveFromCart(long cartDetailsId);
    Task<bool> ApplyCoupon(string userId, string couponCode);
    Task<bool> RemoveCoupon(string userId);
    Task<bool> ClearCart(string userId);
}
