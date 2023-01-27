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
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using DemoApplication.Contracts.Email;

namespace DemoApplication.Services.Concretes
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private User _currentUser;

        public UserService(
            DataContext dataContext,
            IHttpContextAccessor httpContextAccessor,
            IEmailService emailService)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
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
            var model = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (model is null || !BCrypt.Net.BCrypt.Verify(password, model.Password))
            {
                return false;
            }
            return true;
        }



        public async Task SignInAsync(int id,string? role = null)
        {
            var claims = new List<Claim>
            {
                new Claim(CustomClaimNames.ID, id.ToString())
            };
            if (role is not null)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
        }

        public async Task SignInAsync(string? email, string? password,string? role = null)
        {
            //var user = await _dataContext.Users.FirstAsync(u => u.Email == email && u.Password == password);
            //await SignInAsync(user.Id, role);
            //var user = await _dataContext.Users.FirstAsync(u => u.Email == email);
            //bool verified = BCrypt.Net.BCrypt.Verify(password, user.Password);
            //if (verified == true)
            //{
            //    await SignInAsync(user.Id,role);
            //}

            var user = await _dataContext.Users.FirstAsync(u => u.Email == email);

            if (user is not null && BCrypt.Net.BCrypt.Verify(password, user.Password) && user.IsActive == true)
            {
                await SignInAsync(user.Id, role);
            }
        }


        public async Task SignOutAsync()
        {
            await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
            
        public async Task CreateAsync(RegisterViewModel model)
        {
     

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
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


            await SendActivationUrl(user);

            await _dataContext.SaveChangesAsync();



            async Task SendActivationUrl(User user) 
            {

                string token = GenerateActivationToken();
                string activationUrl = GenerateActivationUrl(token);
                DateTime expireDate = DateTime.Now.AddMinutes(1);

                var userActivation = new UserActivation
                {
                    User = user,
                    ActivationUrl = activationUrl,
                    ActivationToken = token,
                    ExpiredDate = expireDate,
                };

                await _dataContext.UserActivations.AddAsync(userActivation);

                var body = EmailMessages.Body.ACTIVATION_MESSAGE.Replace(EmailMessageKeyword.ACTIVATION_URL, activationUrl);

                var subject = EmailMessages.Subject.ACTIVATION_MESSAGE;

                _emailService.Send(new MessageDto(user.Email,subject,body));
            }
        }
        private string GenerateActivationToken()
        {
            return Guid.NewGuid().ToString();
        }

        private string GenerateActivationUrl(string token) 
        {
            return $"https://localhost:7026/authentication/activate/{token}";
        }
    }
}
