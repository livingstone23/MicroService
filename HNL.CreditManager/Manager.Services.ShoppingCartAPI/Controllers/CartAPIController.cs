//using Manager.MessageBus;
//using Manager.Services.ShoppingCartAPI.Messages;
//using Manager.Services.ShoppingCartAPI.RabbitMQSender;

using Manager.Services.ShoppingCartAPI.Models.Dto;
using Manager.Services.ShoppingCartAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartAPIController : Controller
    {
        private readonly ICartRepository _cartRepository;
        protected ResponseDto _response;
        //private readonly ICouponRepository _couponRepository;

        //private readonly IMessageBus _messageBus;
        //private readonly IRabbitMQCartMessageSender _rabbitMQCartMessageSender;

        public CartAPIController(ICartRepository cartRepository)//, ICouponRepository couponRepository)
             //, IRabbitMQCartMessageSender rabbitMQCartMessageSender, IMessageBus messageBus)
        {
            _cartRepository = cartRepository;
            this._response = new ResponseDto();
            //_couponRepository = couponRepository;

            //_rabbitMQCartMessageSender = rabbitMQCartMessageSender;
            //_messageBus = messageBus;

        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _response.Result = cartDto;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            try
            {
                CartDto cartDt = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = cartDt;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CartDto cartDto)
        {
            try
            {
                CartDto cartDt = await _cartRepository.CreateUpdateCart(cartDto);
                _response.Result = cartDt;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody]int cartId)
        {
            try
            {
                bool isSuccess = await _cartRepository.RemoveFromCart(cartId);
                _response.Result = isSuccess;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        //[HttpPost("ApplyCoupon")]
        //public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        //{
        //    try
        //    {
        //        bool isSuccess = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId,
        //            cartDto.CartHeader.CouponCode);
        //        _response.Result = isSuccess;
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages = new List<string>() { ex.ToString() };
        //    }
        //    return _response;
        //}

        //[HttpPost("RemoveCoupon")]
        //public async Task<object> RemoveCoupon([FromBody] string userId)
        //{
        //    try
        //    {
        //        bool isSuccess = await _cartRepository.RemoveCoupon(userId);
        //        _response.Result = isSuccess;
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages = new List<string>() { ex.ToString() };
        //    }
        //    return _response;
        //}

        //[HttpPost("Checkout")]
        //public async Task<object> Checkout(CheckoutHeaderDto checkoutHeader)
        //{
        //    try
        //    {
        //        CartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeader.UserId);
        //        if(cartDto == null)
        //        {
        //            return BadRequest();
        //        }

        //        if (!string.IsNullOrEmpty(checkoutHeader.CouponCode))
        //        {
        //            CouponDto coupon = await _couponRepository.GetCoupon(checkoutHeader.CouponCode);
        //            if (checkoutHeader.DiscountTotal != coupon.DiscountAmount)
        //            {
        //                _response.IsSuccess = false;
        //                _response.ErrorMessages = new List<string>() { "Coupon Price has changed, please confirm" };
        //                _response.DisplayMessage = "Coupon Price has changed, please confirm";
        //                return _response;
        //            }
        //        }

        //        checkoutHeader.CartDetails = cartDto.CartDetails;
        //        //logic to add message to process order.
        //        await _messageBus.PublishMessage(checkoutHeader, "checkoutqueue");

        //        ////rabbitMQ
        //        //_rabbitMQCartMessageSender.SendMessage(checkoutHeader, "checkoutqueue");
        //        await _cartRepository.ClearCart(checkoutHeader.UserId);
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.ErrorMessages = new List<string>() { ex.ToString() };
        //    }
        //    return _response;
        //}
    }
}
