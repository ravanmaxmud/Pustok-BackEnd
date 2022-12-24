using DemoApplication.Areas.Client.ViewModels.Authentication;
using DemoApplication.Contracts.Identity;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Exceptions;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DemoApplication.Areas.Client.Controllers;
using DemoApplication.Areas.Client.ViewModels.Basket;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace DemoApplication.Services.Concretes
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private User _currentUser;

        public UserService(
            DataContext dataContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public User CurrentUser
        {

            get
            {
                if (_currentUser is not null)
                {
                    return _currentUser;
                }

                var idClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimNames.ID);
                if (idClaim == null)
                {
                    throw new IdentityCookieException("Identity cookie not found");
                }
                _currentUser = _dataContext.Users.First(u => u.Id == int.Parse(idClaim.Value));

                return _currentUser;
            }
        }

        public bool IsAuthenticated
        {
            get => _httpContextAccessor.HttpContext!.User.Identity!.IsAuthenticated;
        }


        public string GetCurrentUserFullName()
        {
            return $"{CurrentUser.FirstName} {CurrentUser.LastName}";
        }

        public async Task<bool> CheckPasswordAsync(string? email, string? password)
        {
            var model = await _dataContext.Users.FirstAsync(u => u.Email == email);
            if (model is null || !BCrypt.Net.BCrypt.Verify(password, model.Password))
            {
                return false;
            }
            return true;
        }



        public async Task SignInAsync(int id)
        {
            var claims = new List<Claim>
            {
                new Claim(CustomClaimNames.ID, id.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
        }

        public async Task SignInAsync(string? email, string? password)
        {
            var user = await _dataContext.Users.FirstAsync(u => u.Email == email);
            bool verified = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (verified == true)
            {
                await SignInAsync(user.Id);
            }
        }


        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task CreateAsync(RegisterViewModel model)
        {
            //var hashedPassword = PasswordHasher.MD5Create(model.Password);
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = passwordHash,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            await _dataContext.Users.AddAsync(user);


            var basket = new Basket
            {
                User = user,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _dataContext.Baskets.AddAsync(basket);



            var productCookieValue = _httpContextAccessor.HttpContext!.Request.Cookies["products"];
            if (productCookieValue is not null)
            {
                var productsCookieViewModel = JsonSerializer.Deserialize<List<ProductCookieViewModel>>(productCookieValue);
                foreach (var productCookieViewModel in productsCookieViewModel)
                {
                    var book = await _dataContext.Books.FirstOrDefaultAsync(b => b.Id == productCookieViewModel.Id);
                    var basketProduct = new BasketProduct
                    {
                        Basket = basket,
                        BookId = book!.Id,
                        Quantity = productCookieViewModel.Quantity,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    await _dataContext.BasketProducts.AddAsync(basketProduct);
                }
            }

            await _dataContext.SaveChangesAsync();
        }
    }
}
