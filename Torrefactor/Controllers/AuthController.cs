using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Torrefactor.DAL;
using Torrefactor.Models;
using Torrefactor.Models.Auth;
using Torrefactor.Services;

namespace Torrefactor.Controllers
{
	[Route("api/auth")]
	public class AuthController : Controller
	{
		public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, InviteRepository inviteRepository, Config config)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_inviteRepository = inviteRepository;
			_config = config;
		}

		[Route("user")]
		[HttpGet]
		public UserModel GetCurrentUser()
		{
			if (!User.Identity.IsAuthenticated)
				return null;

			return new UserModel(User.IsAdmin(_config), User.Identity.Name);
		}

		[Route("sign-in")]
		[HttpPost]
		public async Task<object> SignIn([FromBody] SignInModel model)
		{
			var res = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
			return new
			{
				Success = res.Succeeded
			};
		}
		
		[Route("sign-out")]
		[HttpPost]
		public async Task SignOut()
		{
			await _signInManager.SignOutAsync();
		}

		[Route("invite")]
		[HttpGet, Authorize]
		public async Task<IReadOnlyCollection<Invite>> GetPendingInviteRequests()
		{
			return await _inviteRepository.GetInState(InviteState.Pending);
		} 

		[Route("invite")]
		[HttpPost]
		public async Task CreateInvite([FromBody] InviteModel model)
		{
			await _inviteRepository.Insert(new []
			{
				new Invite(model.Email, model.Name),
			});
		}

		[Route("invite/state")]
		[HttpPost]
		public async Task ApproveOrDecline(string id, bool isApproved)
		{
			var invite = await _inviteRepository.Get(ObjectId.Parse(id));

			if (isApproved)
			{
				invite.Approve();
			}
			else
			{
				invite.Decline();
			}

			await _inviteRepository.Update(invite);
		}

		[Route("register")]
		[HttpPost]
		public async Task Register([FromBody] RegistrationModel model)
		{
			var invite = await _inviteRepository.Get(model.Token);
			if (invite == null)
				throw new InvalidOperationException($"Unknown token {model.Token}");

			if (invite.State != InviteState.Approved)
				throw new InvalidOperationException($"Invite {invite.Id} is not approved");

			var result = await _userManager.CreateAsync(
				new ApplicationUser {UserName = invite.Email, Email = invite.Email},
				model.Password);
			
			if (!result.Succeeded)
				throw new InvalidOperationException(
					$"Unexpected errors {string.Join(",", result.Errors.Select(e => $"{e.Code} - {e.Description}"))}");
			
			invite.ResetToken();
			await _inviteRepository.Update(invite);
			
			await _signInManager.PasswordSignInAsync(invite.Email, model.Password, true, false);
		}
		
		private readonly InviteRepository _inviteRepository;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly Config _config;
	}
}