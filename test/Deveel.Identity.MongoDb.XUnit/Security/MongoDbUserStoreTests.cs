using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using MongoDB.Bson;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Security {
	public class MongoDbUserStoreTests : IClassFixture<MongoDbIdentityTestFixture>, IDisposable {
		public MongoDbUserStoreTests(MongoDbIdentityTestFixture testFixture, ITestOutputHelper outputHelper) {
			TestFixture = testFixture;
			TestFixture.TestCreated(outputHelper);
		}

		public MongoDbIdentityTestFixture TestFixture { get; }

		public TService Service<TService>() => TestFixture.Service<TService>();

		public UserManager<MongoUser> UserManager => Service<UserManager<MongoUser>>();

		[Fact]
		public async Task CreateNewUser() {
			var user = new MongoUser {
				Name = "testUser",
				Email = "test@example.com"
			};

			var result = await UserManager.CreateAsync(user);

			Assert.True(result.Succeeded);
			Assert.NotEqual(ObjectId.Empty, user.Id);
			Assert.NotNull(user.NormalizedName);
			Assert.NotNull(user.NormalizedEmail);
		}

		[Fact]
		public async Task CreateNewUserWithPassword() {
			var user = new MongoUser {
				Name = "testUser",
				Email = "test@example.com"
			};

			const string pass = "sup3r$secRetPas$";

			var result = await UserManager.CreateAsync(user, pass);

			Assert.True(result.Succeeded);
			Assert.NotEqual(ObjectId.Empty, user.Id);
			Assert.NotNull(user.NormalizedName);
			Assert.NotNull(user.NormalizedEmail);
			Assert.NotNull(user.PasswordHash);
		}

		[Fact]
		public async Task ChangePassword() {
			const string pass = "sup3r$secRetPas$";

			var passwordHasher = Service<IPasswordHasher<MongoUser>>();

			var user = await TestFixture.CreateUser("testUser", "test@example.com", u =>
				u.PasswordHash = passwordHasher.HashPassword(u, pass));

			var result = await UserManager.ChangePasswordAsync(user, pass, "newPas$w0rd");

			Assert.True(result.Succeeded);
		}

		[Fact]
		public async Task AddNewPassword() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");
			const string pass = "sup3r$secRetPas$";

			var result = await UserManager.AddPasswordAsync(user, pass);

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotNull(updated.PasswordHash);
		}

		[Fact]
		public async Task DeleteExistingUser() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.DeleteAsync(user);

			Assert.True(result.Succeeded);

			var deleted = await TestFixture.FindUser(user.Id);
			Assert.Null(deleted);
		}

		[Fact]
		public async Task DeleteNotExistingUser() {
			var user = new MongoUser {
				Id = ObjectId.GenerateNewId(),
				Name = "testUser",
				Email = "test@example.com",
			};

			var result = await UserManager.DeleteAsync(user);

			Assert.False(result.Succeeded);
			Assert.NotEmpty(result.Errors);
			Assert.Single(result.Errors);
			Assert.Equal(MongoDbStoreErrorCodes.UserNotFound, result.Errors.First().Code);
		}

		[Fact]
		public async Task UpdateExistingUser() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var tenantId = Guid.NewGuid().ToString("N");
			user.TenantId = tenantId;

			var result = await UserManager.UpdateAsync(user);

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.Equal(tenantId, updated.TenantId);
		}

		[Fact]
		public async Task UpdateNotExistingUser() {
			var user = new MongoUser {
				Id = ObjectId.GenerateNewId(),
				Name = "testUser",
				Email = "test@example.com",
				SecurityStamp = Guid.NewGuid().ToString("N")
			};

			var tenantId = Guid.NewGuid().ToString("N");
			user.TenantId = tenantId;

			var result = await UserManager.UpdateAsync(user);

			Assert.False(result.Succeeded);
			Assert.NotEmpty(result.Errors);
			Assert.Single(result.Errors);
			Assert.Equal(MongoDbStoreErrorCodes.UserNotFound, result.Errors.First().Code);
		}

		[Fact]
		public async Task FindByNameExistingUser() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.FindByNameAsync("testUser");

			Assert.NotNull(result);
			Assert.Equal(user.Id, result.Id);
			Assert.Equal(user.Email, result.Email);
		}

		[Fact]
		public async Task FindById() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.FindByIdAsync(user.Id.ToString());

			Assert.NotNull(result);
			Assert.Equal(user.Id, result.Id);
		}

		[Fact]
		public async Task QueryAllUsers() {
			var user1 = await TestFixture.CreateUser("testUser", "test@example.com");
			var user2 = await TestFixture.CreateUser("test2", "tester2@example.com");

			var users = UserManager.Users.ToList();

			Assert.NotNull(users);
			Assert.NotEmpty(users);
			Assert.Equal(2, users.Count);

			Assert.Equal(user1.Name, users[0].Name);
			Assert.Equal(user2.Name, users[1].Name);
		}

		[Fact]
		public async Task FindByEmailExistingUser() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.FindByEmailAsync("test@example.com");

			Assert.NotNull(result);
			Assert.Equal(user.Id, result.Id);
			Assert.Equal(user.Email, result.Email);
		}

		[Fact]
		public async Task ChangeEmail() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var token = await UserManager.GenerateChangeEmailTokenAsync(user, "test2@example.com");

			var result = await UserManager.ChangeEmailAsync(user, "test2@example.com", token);

			Assert.True(result.Succeeded);
			Assert.Equal("test2@example.com", await UserManager.GetEmailAsync(user));

			var updated = await TestFixture.FindUser(user.Id);

			Assert.Equal("test2@example.com", updated.Email);
		}

		[Fact]
		public async Task ConfirmEmail() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);

			var result = await UserManager.ConfirmEmailAsync(user, token);

			Assert.True(result.Succeeded);
			Assert.True(await UserManager.IsEmailConfirmedAsync(user));

			var updated = await TestFixture.FindUser(user.Id);

			Assert.Equal("test@example.com", updated.Email);
			Assert.True(updated.EmailConfirmed);
		}

		[Fact]
		public async Task GetEmail() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var email = await UserManager.GetEmailAsync(user);

			Assert.NotNull(email);
			Assert.Equal(user.Email, email);
		}

		[Fact]
		public async Task ChangePhone() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var token = await UserManager.GenerateChangePhoneNumberTokenAsync(user, "+155587456");

			var result = await UserManager.ChangePhoneNumberAsync(user, "+155587456", token);

			Assert.True(result.Succeeded);
			Assert.True(await UserManager.IsPhoneNumberConfirmedAsync(user));

			var updated = await TestFixture.FindUser(user.Id);

			Assert.Equal("+155587456", updated.Phone);
			Assert.True(updated.PhoneConfirmed);
		}

		[Fact]
		public async Task GetPhone() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u.Phone = "+155587945");

			var phone = await UserManager.GetPhoneNumberAsync(user);

			Assert.NotNull(phone);
			Assert.Equal(user.Phone, phone);
		}

		[Fact]
		public async Task RegisterNewLogins() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var manager = Service<UserManager<MongoUser>>();

			var result = await manager.AddLoginAsync(user, new UserLoginInfo("facebook", "7488493993", "Facebook"));

			Assert.True(result.Succeeded);

			Assert.NotEmpty(user.Logins);
			Assert.Single(user.Logins);
			Assert.Contains(user.Logins, login => login.Provider == "facebook" && login.LoginKey == "7488493993");
		}

		[Fact]
		public async Task RemoveRegisteredLogin() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Logins.Add(new MongoUserLogin {
					Provider = "facebook",
					LoginKey = "7488493993",
					ProviderDisplayName = "Facebook"
				}));

			var manager = Service<UserManager<MongoUser>>();

			var result = await manager.RemoveLoginAsync(user, "facebook", "7488493993");

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.Empty(updated.Logins);
		}

		[Fact]
		public async Task FindUserByRegisteredLogin() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Logins.Add(new MongoUserLogin {
					Provider = "facebook",
					LoginKey = "7488493993",
					ProviderDisplayName = "Facebook"
				}));

			var result = await UserManager.FindByLoginAsync("facebook", "7488493993");

			Assert.NotNull(result);
			Assert.Equal(user.Id, result.Id);
			Assert.Equal(user.Name, result.Name);
			Assert.NotEmpty(result.Logins);
		}

		[Fact]
		public async Task GetUserLogins() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Logins.Add(new MongoUserLogin {
					Provider = "facebook",
					LoginKey = "7488493993",
					ProviderDisplayName = "Facebook"
				}));

			var result = await UserManager.GetLoginsAsync(user);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);
			Assert.Contains(result, login => login.LoginProvider == "facebook" && login.ProviderKey == "7488493993");
		}

		[Fact]
		public async Task GetUsersInRole() {
			var lookupNormalizer = Service<ILookupNormalizer>();

			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Roles.Add(lookupNormalizer.NormalizeName("user")));

			var manager = Service<UserManager<MongoUser>>();

			var result = await manager.GetUsersInRoleAsync("user");

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Single(result);
		}

		[Fact]
		public async Task RemoveUserFromRole() {
			var lookupNormalizer = Service<ILookupNormalizer>();

			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Roles.Add(lookupNormalizer.NormalizeName("user")));

			var result = await UserManager.RemoveFromRoleAsync(user, "user");

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.Empty(updated.Roles);
		}

		[Fact]
		public async Task RemoveUserFromRoleNotOwned() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.RemoveFromRoleAsync(user, "user");

			Assert.False(result.Succeeded);
		}

		[Fact]
		public async Task AddUserToRole() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.AddToRoleAsync(user, "user");

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			var lookupNormalizer = Service<ILookupNormalizer>();

			Assert.NotEmpty(updated.Roles);
			Assert.Single(updated.Roles);
			Assert.Equal(lookupNormalizer.NormalizeName("user"), updated.Roles[0]);
		}

		[Fact]
		public async Task AddUserToRoleAlreadyOwned() {
			var lookupNormalizer = Service<ILookupNormalizer>();

			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Roles.Add(lookupNormalizer.NormalizeName("user")));

			var result = await UserManager.AddToRoleAsync(user, "user");

			Assert.False(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotEmpty(updated.Roles);
			Assert.Single(updated.Roles);
			Assert.Equal(lookupNormalizer.NormalizeName("user"), updated.Roles[0]);
		}

		[Fact]
		public async Task AddNewClaim() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, "Tester"));

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotEmpty(updated.Claims);
			Assert.Contains(updated.Claims, claim => claim.Type == ClaimTypes.GivenName && claim.Value == "Tester");
		}

		[Fact]
		public async Task AddExistingClaim() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Claims.Add(new MongoClaim {
					Type = ClaimTypes.GivenName,
					Value = "Tester"
				}));

			var result = await UserManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, "Tester"));

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotEmpty(updated.Claims);
			Assert.Single(updated.Claims);
			Assert.Contains(updated.Claims, claim => claim.Type == ClaimTypes.GivenName && claim.Value == "Tester");
		}

		[Fact]
		public async Task ReplaceClaim() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Claims.Add(new MongoClaim {
					Type = ClaimTypes.GivenName,
					Value = "Tester"
				}));

			var oldClaim = new Claim(ClaimTypes.GivenName, "Tester");
			var newClaim = new Claim(ClaimTypes.GivenName, "Second Tester");
			var result = await UserManager.ReplaceClaimAsync(user, oldClaim, newClaim);

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotEmpty(updated.Claims);
			Assert.Contains(updated.Claims, claim => claim.Type == ClaimTypes.GivenName && claim.Value == "Second Tester");
		}

		[Fact]
		public async Task RemoveExistingClaim() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Claims.Add(new MongoClaim {
					Type = ClaimTypes.GivenName,
					Value = "Tester"
				}));

			var result = await UserManager.RemoveClaimAsync(user, new Claim(ClaimTypes.GivenName, "Tester"));

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.Empty(updated.Claims);
		}

		[Fact]
		public async Task RemoveNotExistingClaim() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Claims.Add(new MongoClaim {
					Type = ClaimTypes.GivenName,
					Value = "Tester"
				}));

			var result = await UserManager.RemoveClaimAsync(user, new Claim(ClaimTypes.PostalCode, "0194"));

			// Even if it did not exist Identity Framework does not return any errors
			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotEmpty(updated.Claims);
			Assert.Contains(updated.Claims, claim => claim.Type == ClaimTypes.GivenName && claim.Value == "Tester");
		}

		[Fact]
		public async Task GetUsersByClaim() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Claims.Add(new MongoClaim {
					Type = ClaimTypes.GivenName,
					Value = "Tester"
				}));

			var result = await UserManager.GetUsersForClaimAsync(new Claim(ClaimTypes.GivenName, "Tester"));

			Assert.NotEmpty(result);
			Assert.Single(result);
		}

		[Fact]
		public async Task GetUsersClaims() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Claims.Add(new MongoClaim {
					Type = ClaimTypes.GivenName,
					Value = "Tester"
				}));

			var result = await UserManager.GetClaimsAsync(user);

			Assert.NotEmpty(result);
			Assert.Single(result);
		}

		[Fact]
		public async Task FailedLogin() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.AccessFailedAsync(user);

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotNull(updated.AccessFailedCount);
			Assert.Equal(1, updated.AccessFailedCount.Value);
		}

		[Fact]
		public async Task LockoutAfterMaxFailedAccess() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			await UserManager.AccessFailedAsync(user);
			await UserManager.AccessFailedAsync(user);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.Null(updated.AccessFailedCount);
			Assert.NotNull(updated.LockoutEnd);
		}

		[Fact]
		public async Task SetNewAuthenticationToken() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.SetAuthenticationTokenAsync(user, "facebook", "access_token", "---token value---");

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotEmpty(updated.Tokens);
			Assert.Single(updated.Tokens);
			Assert.Contains(updated.Tokens, token => token.Provider == "facebook" && token.TokenName == "access_token");
		}

		[Fact]
		public async Task SetExistingAuthenticationToken() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Tokens.Add(new MongoUserToken {
					Provider = "facebook",
					TokenName = "access_token",
					Token = "---token value---"
				}));

			var result = await UserManager.SetAuthenticationTokenAsync(user, "facebook", "access_token", "---new token value---");

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotEmpty(updated.Tokens);
			Assert.Single(updated.Tokens);
			Assert.Contains(updated.Tokens, token => 
				token.Provider == "facebook" && 
				token.TokenName == "access_token" && 
				token.Token == "---new token value---");
		}

		[Fact]
		public async Task RemoveAuthenticationToken() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Tokens.Add(new MongoUserToken {
					Provider = "facebook",
					TokenName = "access_token",
					Token = "---token value---"
				}));

			var result = await UserManager.RemoveAuthenticationTokenAsync(user, "facebook", "access_token");

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.Empty(updated.Tokens);
		}

		[Fact]
		public async Task GetExistingAuthenticationToken() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u
				.Tokens.Add(new MongoUserToken {
					Provider = "facebook",
					TokenName = "access_token",
					Token = "---token value---"
				}));

			var result = await UserManager.GetAuthenticationTokenAsync(user, "facebook", "access_token");

			Assert.NotNull(result);
			Assert.Equal("---token value---", result);
		}

		[Fact]
		public async Task GetNotExistingAuthenticationToken() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.GetAuthenticationTokenAsync(user, "facebook", "access_token");

			Assert.Null(result);
		}


		[Fact]
		public async Task RemoveNotExistingAuthenticationToken() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.RemoveAuthenticationTokenAsync(user, "facebook", "access_token");

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.Empty(updated.Tokens);
		}


		[Fact]
		public async Task NewRecoveryCodes() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 5);

			Assert.NotNull(result);
			Assert.NotEmpty(result);
			Assert.Equal(5, result.Count());

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotNull(updated.RecoveryCodes);
			Assert.NotEmpty(updated.RecoveryCodes);
			Assert.Equal(5, updated.RecoveryCodes.Count);
		}

		[Fact]
		public async Task RedeemRecoveryCodes() {
			var code = Guid.NewGuid().ToString("N");

			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => {
				u.RecoveryCodes.Add(code);
				u.RecoveryCodes.Add(Guid.NewGuid().ToString("N"));
				u.RecoveryCodes.Add(Guid.NewGuid().ToString("N"));
			});

			var result = await UserManager.RedeemTwoFactorRecoveryCodeAsync(user, code);

			Assert.NotNull(result);
			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotEmpty(updated.RecoveryCodes);
			Assert.Equal(2, updated.RecoveryCodes.Count);
		}

		[Fact]
		public async Task SetNewAuthenticationKey() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com");

			var result = await UserManager.ResetAuthenticatorKeyAsync(user);

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindUser(user.Id);

			Assert.NotNull(updated.AuthenticationKey);
		}

		public void Dispose() {
			TestFixture.TestDisposed();
		}
	}
}
