using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace Deveel.Security {
	class TestPhoneTwoFactorAuthentication<TUser> : IUserTwoFactorTokenProvider<TUser> where TUser : MongoUser {
		public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user) {
			if (manager != null && user != null) {
				return Task.FromResult(true);
			} else {
				return Task.FromResult(false);
			}
		}

		private const string Magic = "O5930E";

		private string GenerateToken(MongoUser user, string purpose) {
			return Magic + user.Phone + purpose + user.Id;
		}

		public Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user) {
			return Task.FromResult(GenerateToken(user, purpose));
		}

		public Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user) {
			return Task.FromResult(token == GenerateToken(user, purpose));
		}

	}
}
