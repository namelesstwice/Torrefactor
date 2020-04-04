using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Torrefactor.Models;
using Torrefactor.Models.Auth;
using Torrefactor.Services;

namespace Torrefactor.Controllers
{
	[Route("api/auth")]
	public class AuthController : Controller
	{
		public AuthController(
			SignInManager<ApplicationUser> signInManager, 
			UserManager<ApplicationUser> userManager,
			Config config)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_config = config;
		}

		[Route("users/current")]
		[HttpGet]
		public async Task<UserModel> GetCurrentUser()
		{
			if (!User.Identity.IsAuthenticated)
				return null;

			var user = await _userManager.FindByEmailAsync(User.Identity.Name);
			return new UserModel(User.IsAdmin(), user.DisplayName, user.Email, user.Id);
		}

		[Route("sign-in")]
		[HttpPost]
		public async Task<object> SignIn([FromBody] SignInModel model)
		{
			var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

			if (!result.Succeeded)
				return new {Success = false};
			
			var user = await _userManager.FindByEmailAsync(model.Email);
			
			var token = GenerateJwtToken(user.Email, user);
			return new
			{
				Success = true,
				AccessToken = token
			};
		}
		
		[Route("sign-out")]
		[HttpPost]
		public async Task SignOut()
		{
			await _signInManager.SignOutAsync();
		}

		[Route("users/not-confirmed")]
		[HttpGet, Authorize(Roles = "admin")]
		public IReadOnlyCollection<UserModel> GetPendingInviteRequests()
		{
			return _userManager.Users
				.Where(_ => !_.EmailConfirmed)
				.Select(u => new UserModel(false, u.DisplayName, u.Email, u.Id))
				.ToList();
		}

		[Route("users/{id}/confirmation-state")]
		[HttpPut, Authorize(Roles = "admin")]
		public async Task ApproveOrDecline(string id, bool isApproved)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (await _userManager.IsEmailConfirmedAsync(user))
				throw new InvalidOperationException("User is already confirmed");

			if (!isApproved)
			{
				await _userManager.DeleteAsync(user);
				return;
			}

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			await _userManager.ConfirmEmailAsync(user, token);
		}

		[Route("register")]
		[HttpPost]
		public async Task Register([FromBody] RegistrationModel model)
		{
			var isAdmin = AuthExtensions.IsAdminEmail(model.Email, _config);
			
			var result = await _userManager.CreateAsync(
				new ApplicationUser
				{
					UserName = model.Email,
					Email = model.Email,
					DisplayName = model.Name, 
					EmailConfirmed = isAdmin,
					Roles = isAdmin ? new List<string> { "admin" } : new List<string>()
				},
				model.Password);
			
			if (!result.Succeeded)
				throw new InvalidOperationException(
					$"Unexpected errors {string.Join(",", result.Errors.Select(e => $"{e.Code} - {e.Description}"))}");

			await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
		}

		private string GenerateJwtToken(string email, ApplicationUser user)
		{
			var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.Name, user.UserName));
			claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddDays(30);

			var token = new JwtSecurityToken(
				null,
				null,
				claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
		
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly Config _config;
	}
}