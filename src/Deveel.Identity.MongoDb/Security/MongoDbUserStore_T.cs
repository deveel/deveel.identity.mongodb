// Copyright 2022 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Deveel.Security {
	public class MongoDbUserStore<TUser> : MongoDbStoreBase<TUser>,
		IQueryableUserStore<TUser>,
		IProtectedUserStore<TUser>,
		IUserPasswordStore<TUser>,
		IUserEmailStore<TUser>,
		IUserPhoneNumberStore<TUser>,
		IUserLockoutStore<TUser>,
		IUserLoginStore<TUser>,
		IUserRoleStore<TUser>,
		IUserSecurityStampStore<TUser>,
		IUserTwoFactorStore<TUser>,
		IUserTwoFactorRecoveryCodeStore<TUser>,
		IUserClaimStore<TUser>,
		IUserAuthenticationTokenStore<TUser>,
		IUserAuthenticatorKeyStore<TUser>
		where TUser : MongoUser {
		public MongoDbUserStore(IOptions<MongoDbStoreOptions> options, ILogger<MongoDbUserStore<TUser>> logger)
			: base(options, logger) {
		}

		public MongoDbUserStore(IOptions<MongoDbStoreOptions> options)
			: base(options) {
		}

		public MongoDbUserStore(MongoDbStoreOptions options, ILogger<MongoDbUserStore<TUser>> logger)
			: base(options, logger) {
		}

		public MongoDbUserStore(MongoDbStoreOptions options)
			: base(options) {
		}

		protected override string CollectionName => Options.UsersCollection;

		public virtual IQueryable<TUser> Users => GetQueryableStore();

		private IQueryable<TUser> GetQueryableStore() {
			IQueryable<TUser> query = Collection.AsQueryable<TUser>();

			if (Options.HasTenantSet) {
				query = query.Where(x => x.TenantId == Options.TenantId);
			}

			return query;
		}

		private async Task<IList<TUser>> FindAllAsync(FilterDefinition<TUser> filter, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			try {
				Trace("Trying to find all users in");

				var options = new FindOptions<TUser, TUser>();
				var result = await Collection.FindAsync(NormalizeFilter(filter), options, cancellationToken);

				var users = await result.ToListAsync(cancellationToken);

				if (users.Count == 0) {
					Trace("None user was found in");
				} else if (users.Count == 1) {
					Trace("One user was found in");
				} else {
					Trace("{UserCount} users were found in", users.Count);
				}

				return users;
			} catch (Exception ex) {
				Error(ex, "It was not possible to retrieve the users in");

				throw new MongoException("Error while getting users for a given filter", ex);
			}
		}

		protected async Task<TUser> FindUserAsync(FilterDefinition<TUser> filter, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			try {
				Trace("Trying to find a user in");

				var options = new FindOptions<TUser, TUser> { Limit = 1 };
				var result = await Collection.FindAsync(NormalizeFilter(filter), options, cancellationToken);

				var user = await result.FirstOrDefaultAsync(cancellationToken);

				if (user != null) {
					Trace("A user was found in");

				} else {
					Trace("None user was found in");
				}

				return user;
			} catch (Exception ex) {
				Error(ex, "The user could not be looked up in");

				throw new MongoException("Error while looking up for the user", ex);
			}
		}

		protected Task<TUser> FindUserAsync(Expression<Func<TUser, bool>> exp, CancellationToken cancellationToken)
			=> FindUserAsync(new ExpressionFilterDefinition<TUser>(exp), cancellationToken);

		public virtual Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.NormalizedName, cancellationToken);

		public virtual Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.Id == ObjectId.Empty ? (string)null : user.Id.ToString(), cancellationToken);

		public virtual Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.Name, cancellationToken);

		public virtual Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
			=> SetAsync(() => user.NormalizedName = normalizedName, cancellationToken);

		public virtual Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
			=> SetAsync(() => user.Name = userName, cancellationToken);

		#region Core CRUD

		public virtual async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			try {
				Trace("Creating a new user in");

				user.CreatedAt = DateTimeOffset.UtcNow;

				if (Options.HasTenantSet)
					user.TenantId = Options.TenantId;

				var options = new InsertOneOptions { BypassDocumentValidation = true };
				await Collection.InsertOneAsync(user, options, cancellationToken);

				Trace("New user with ID '{UserId}' created in", user.Id);

				return IdentityResult.Success;
			} catch (Exception ex) {
				Error(ex, "Could not create a new user in");

				return IdentityResult.Failed(new IdentityError {
					Code = MongoDbStoreErrorCodes.UnknownError,
					Description = "The storage system failed persisting the user"
				});
			}
		}

		public virtual async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken) {
			if (user is null)
				throw new ArgumentNullException(nameof(user));

			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			try {
				Trace("Deleting the user with ID '{UserId}' from", user.Id);

				var result = await Collection.DeleteOneAsync(IdFilter(user.Id), cancellationToken);
				if (result.DeletedCount == 0) {
					Warning("Inconsistent delete: user with ID '{UserId}' was not removed from", user.Id);

					return IdentityResult.Failed(new IdentityError {
						Code = MongoDbStoreErrorCodes.UserNotFound,
						Description = "The user was not deleted from the storage"
					});
				} else if (result.DeletedCount == 1) {
					Trace("User with ID '{UserId}' was successfully deleted from", user.Id);
				} else if (result.DeletedCount > 1) {
					Trace("Incosistent delete: more than one user deleted while trying to remove the user with ID '{UserId}' from", user.Id);
				}

				return IdentityResult.Success;
			} catch (Exception ex) {
				Error(ex, "The user '{UserId}' was not deleted from", user.Id);

				return IdentityResult.Failed(new IdentityError {
					Code = MongoDbStoreErrorCodes.UnknownError,
					Description = "Could not delete the user from the storage system"
				});
			}
		}

		public virtual async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken) {
			if (!ObjectId.TryParse(userId, out var id))
				throw new ArgumentException($"The provided user ID {userId} is not in a valid 24-digits format");

			Trace("Trying to find a user with ID '{UserId}' in", userId);

			return await FindUserAsync(x => x.Id == id, cancellationToken);
		}

		public virtual Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) {
			Trace("Trying to find a user named '{UserName}' in", normalizedUserName);

			return FindUserAsync(x => x.NormalizedName == normalizedUserName, cancellationToken);
		}

		public virtual async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			try {
				Trace("Updating the user with ID {UserId} in", user.Id);

				var options = new ReplaceOptions { IsUpsert = false };
				var result = await Collection.ReplaceOneAsync(IdFilter(user.Id), user, options, cancellationToken);

				if (result.MatchedCount == 0) {
					Warning("The user with ID {UserId} was not found in", user.Id);

					return IdentityResult.Failed(new IdentityError {
						Code = MongoDbStoreErrorCodes.UserNotFound,
						Description = $"The user with ID {user.Id} was not found and could not be updated"
					});
				}

				if (result.ModifiedCount == 0) {
					Warning("The user with ID '{UserId}' was not modified in", user.Id);

					return IdentityResult.Failed(new IdentityError {
						Code = MongoDbStoreErrorCodes.UserNotModified,
						Description = $"The user with ID {user.Id} was not updated"
					});
				}

				Trace("The user with ID '{UserId}' was successfully updated in", user.Id);

				return IdentityResult.Success;
			} catch (Exception ex) {
				Logger.LogError(ex, "The user with ID {UserId} was not updated in the collection {CollectionName} of database {DatabaseName} because of an unknown error",
					user.Id, CollectionName, DatabaseName);

				return IdentityResult.Failed(new IdentityError {
					Code = MongoDbStoreErrorCodes.UnknownError,
					Description = "Could not update the user in the storage system"
				});
			}
		}

		#endregion

		#region Email

		public virtual Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
			=> SetAsync(() => user.Email = email, cancellationToken);

		public virtual Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.Email, cancellationToken);

		public virtual Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.EmailConfirmed, cancellationToken);

		public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
			=> SetAsync(() => user.EmailConfirmed = confirmed, cancellationToken);

		public virtual Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) {
			Trace("Trying to find a user for e-mail '{Email}' in", normalizedEmail);

			return FindUserAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);
		}

		public virtual Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.NormalizedEmail, cancellationToken);

		public virtual Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
			=> SetAsync(() => user.NormalizedEmail = normalizedEmail, cancellationToken);

		#endregion

		#region Phone Number

		public virtual Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
			=> SetAsync(() => user.Phone = phoneNumber, cancellationToken);

		public virtual Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.Phone, cancellationToken);

		public virtual Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.PhoneConfirmed, cancellationToken);

		public virtual Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
			=> SetAsync(() => user.PhoneConfirmed = confirmed, cancellationToken);

		#endregion

		#region Lockout

		public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.LockoutEnd, cancellationToken);

		public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
			=> SetAsync(() => user.LockoutEnd = lockoutEnd, cancellationToken);

		public virtual Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => {
				var count = user.AccessFailedCount ?? 0;
				user.AccessFailedCount = ++count;

				return count;
			}, cancellationToken);

		public virtual Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
			=> SetAsync(() => user.AccessFailedCount = null, cancellationToken);

		public virtual Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.AccessFailedCount ?? 0, cancellationToken);

		public virtual Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.LockoutEnabled, cancellationToken);

		public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
			=> SetAsync(() => user.LockoutEnabled = enabled, cancellationToken);

		#endregion

		#region Logins

		public virtual Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
			=> SetAsync(() => {
				if (user.Logins == null)
					user.Logins = new List<MongoUserLogin>();

				if (!user.Logins.Any(x => x
					.Provider == login.LoginProvider &&
					x.LoginKey == login.ProviderKey)) {
					user.Logins.Add(new MongoUserLogin {
						LoginKey = login.ProviderKey,
						Provider = login.LoginProvider,
						ProviderDisplayName = login.ProviderDisplayName
					});
				}
			}, cancellationToken);

		public virtual Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
			=> SetAsync(() => {
				var login = user.Logins?.FirstOrDefault(x =>
				x.Provider == loginProvider &&
				x.LoginKey == providerKey);

				if (login != null && user.Logins != null)
					user.Logins.Remove(login);
			}, cancellationToken);

		public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => {
				return (IList<UserLoginInfo>)user.Logins?
					.Select(x => new UserLoginInfo(x.Provider, x.LoginKey, x.ProviderDisplayName))
					.ToList();
			}, cancellationToken);

		public virtual Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
			Trace("Trying to find a user for the login '{LoginProvider}'", loginProvider);

			var filter = Builders<TUser>.Filter.ElemMatch(user => user.Logins,
				login => login.Provider == loginProvider && login.LoginKey == providerKey);

			return FindUserAsync(filter, cancellationToken);
		}

		public virtual Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
			=> SetAsync(() => {
				var oldToken = user.Tokens?.Find(x => x.Provider == loginProvider && x.TokenName == name);
				if (oldToken != null)
					user.Tokens.Remove(oldToken);

				if (user.Tokens == null)
					user.Tokens = new List<MongoUserToken>();

				user.Tokens.Add(new MongoUserToken {
					Provider = loginProvider,
					Token = value,
					TokenName = name
				});
			}, cancellationToken);

		public virtual Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
			=> SetAsync(() => {
				var token = user.Tokens?.Find(x => x.Provider == loginProvider && x.TokenName == name);
				if (token != null)
					user.Tokens.Remove(token);
			}, cancellationToken);

		public virtual Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
			=> GetAsync(() => {
				var token = user.Tokens?.Find(x => x.Provider == loginProvider && x.TokenName == name);
				if (token != null)
					return token.Token;

				return null;
			}, cancellationToken);

		#endregion

		#region Roles

		public virtual Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
			=> SetAsync(() => {
				if (user.Roles == null)
					user.Roles = new List<string>();

				if (!user.Roles.Contains(roleName))
					user.Roles.Add(roleName);
			}, cancellationToken);

		public virtual Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
			=> SetAsync(() => {
				if (user.Roles != null && user.Roles.Contains(roleName))
					user.Roles.Remove(roleName);
			}, cancellationToken);

		public virtual Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => (IList<string>)user.Roles, cancellationToken);

		public virtual Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
			=> GetAsync(() => user.Roles?.Contains(roleName) ?? false, cancellationToken);

		public virtual Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) {
			Trace("Trying to retrieve all users with role '{RoleName}' in", roleName);

			var filter = Builders<TUser>.Filter.AnyEq(x => x.Roles, roleName);

			return FindAllAsync(filter, cancellationToken);
		}

		#endregion

		#region Security Stamp

		public virtual Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
			=> SetAsync(() => user.SecurityStamp = stamp, cancellationToken);

		public virtual Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.SecurityStamp, cancellationToken);

		#endregion


		#region Two Factors

		public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
			=> SetAsync(() => user.TwoFactorsEnabled = enabled, cancellationToken);

		public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.TwoFactorsEnabled, cancellationToken);

		public virtual Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
			=> SetAsync(() => user.RecoveryCodes = new List<string>(recoveryCodes), cancellationToken);

		public virtual Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
			=> GetAsync(() => user.RecoveryCodes?.Remove(code) ?? false, cancellationToken);

		public virtual Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.RecoveryCodes?.Count ?? 0, cancellationToken);

		#endregion

		#region Claims

		public virtual Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => (IList<Claim>)user.Claims?.Select(x => x.ToClaim()).ToList(), cancellationToken);

		public virtual Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
			=> SetAsync(() => {
				if (user.Claims == null)
					user.Claims = new List<MongoClaim>();

				foreach (var claim in claims) {
					var oldClaim = user.Claims.Find(x => x.Type == claim.Type);
					if (oldClaim == null)
						user.Claims.Add(MongoClaim.Create(claim));
				}
			}, cancellationToken);

		public virtual Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
			=> SetAsync(() => {
				var oldClaim = user.Claims?.Find(c => c.Type == claim.Type);
				if (oldClaim != null) {
					user.Claims.Remove(oldClaim);
				}

				user.Claims.Add(MongoClaim.Create(newClaim));
			}, cancellationToken);

		public virtual Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
			=> SetAsync(() => {
				foreach (var claim in claims) {
					var oldClaim = user.Claims?.Find(c => c.Type == claim.Type);
					if (oldClaim != null) {
						user.Claims.Remove(oldClaim);
					}
				}
			}, cancellationToken);

		public virtual Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken) {
			Trace("Trying to find all users for claim '{ClaimType}' with value '{ClaimValue}' in", claim.Type, claim.Value);

			var filter = Builders<TUser>.Filter.ElemMatch(user => user.Claims,
				c => c.Type == claim.Type && c.Value == claim.Value);

			return FindAllAsync(filter, cancellationToken);
		}

		#endregion

		#region Password

		public virtual Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
			=> SetAsync(() => user.PasswordHash = passwordHash, cancellationToken);

		public virtual Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.PasswordHash, cancellationToken);

		public virtual Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => !String.IsNullOrWhiteSpace(user.PasswordHash), cancellationToken);

		#endregion

		public virtual Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
			=> SetAsync(() => user.AuthenticationKey = key, cancellationToken);

		public virtual Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
			=> GetAsync(() => user.AuthenticationKey, cancellationToken);
	}
}
