﻿using AutoMapper;
using CartApi.Data.ValueObject;
using CartApi.Model;
using CartApi.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace CartApi.Repository;

public class CartRepository : ICartRepository
{
    private readonly MySqlContext _context;
    private IMapper _mapper;

    public CartRepository(MySqlContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<bool> ApplyCoupon(string userId, string couponCode)
    {
        var header = await _context.CartHeaders
                    .FirstOrDefaultAsync(c => c.UserId == userId);
        if (header != null)
        {
            header.CouponCode = couponCode;
            _context.CartHeaders.Update(header);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> ClearCart(string userId)
    {
        var cartHeader = await _context.CartHeaders
                        .FirstOrDefaultAsync(c => c.UserId == userId) ?? new CartHeader();
        if (cartHeader != null)
        {
            _context.CartDetails
                .RemoveRange(
                _context.CartDetails.Where(c => c.CartHeaderId == cartHeader.Id));
            _context.CartHeaders.Remove(cartHeader);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<CartVO> FindCartByUserId(string userId)
    {
        Cart cart = new()
        {
            CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId),
        };
        cart.CartDetails = _context.CartDetails
            .Where(c => c.CartHeaderId == cart.CartHeader.Id)
                .Include(c => c.Product);

        if (cart.CartHeader != null)
        {
            return _mapper.Map<CartVO>(cart);
        }
        else
        {
            return new CartVO();
        }
    }

    public async Task<bool> RemoveCoupon(string userId)
    {
        var header = await _context.CartHeaders
                    .FirstOrDefaultAsync(c => c.UserId == userId);
        if (header != null)
        {
            header.CouponCode = "";
            _context.CartHeaders.Update(header);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> RemoveFromCart(long cartDetailsId)
    {
        try
        {
            CartDetail cartDetail = await _context.CartDetails.FirstOrDefaultAsync(c => c.Id == cartDetailsId);

            int total = _context.CartDetails
                .Where(c => c.CartHeaderId == cartDetail.CartHeaderId).Count();

            _context.CartDetails.Remove(cartDetail);

            if (total == 1)
            {
                var cartHeaderToRemove = await _context.CartHeaders
                    .FirstOrDefaultAsync(c => c.Id == cartDetail.CartHeaderId);
                _context.CartHeaders.Remove(cartHeaderToRemove);
            }
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<CartVO> SaveOrUpdateCart(CartVO vo)
    {
        Cart cart = _mapper.Map<Cart>(vo);

        //Checks if the product is already saved in the database if it does not exist then save
        var product = await _context.Products.FirstOrDefaultAsync(
            p => p.Id == vo.CartDetails.FirstOrDefault().ProductId);

        if (product == null)
        {
            _context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
            await _context.SaveChangesAsync();
        }

        //Check if CartHeader is null

        var cartHeader = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(
            c => c.UserId == cart.CartHeader.UserId);

        if (cartHeader == null)
        {
            //Create CartHeader and CartDetails
            _context.CartHeaders.Add(cart.CartHeader);
            await _context.SaveChangesAsync();
            cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
            cart.CartDetails.FirstOrDefault().Product = null;
            _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
            await _context.SaveChangesAsync();
        }
        else
        {
            //If CartHeader is not null
            //Check if CartDetails has same product
            var cartDetail = await _context.CartDetails.FirstOrDefaultAsync(
                p => p.ProductId == vo.CartDetails.FirstOrDefault().ProductId &&
                p.CartHeaderId == cartHeader.Id);

            if (cartDetail == null)
            {
                //Create CartDetails
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;
                var currentCartDetails = cart.CartDetails.FirstOrDefault();
                _context.CartDetails.Add(currentCartDetails!);
                await _context.SaveChangesAsync();
            }
            else
            {
                //Update product count and CartDetails
                cart.CartDetails.FirstOrDefault().Product = null;
                cart.CartDetails.FirstOrDefault().Count += cartDetail.Count;
                cart.CartDetails.FirstOrDefault().Id = cartDetail.Id;
                cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetail.CartHeaderId;
                _context.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
        }
        return _mapper.Map<CartVO>(cart);
    }
}
