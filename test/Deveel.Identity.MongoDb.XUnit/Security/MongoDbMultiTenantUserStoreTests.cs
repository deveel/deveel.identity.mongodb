using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using MongoDB.Bson;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Security {
	public class MongoDbMultiTenantUserStoreTests : IClassFixture<MongoDbIdentityTestFixture>, IDisposable {
		public MongoDbMultiTenantUserStoreTests(MongoDbIdentityTestFixture testFixture, ITestOutputHelper outputHelper) {
			TestFixture = testFixture;
			TestFixture.TestCreated(outputHelper);
		}

		public MongoDbIdentityTestFixture TestFixture { get; }

		public string TenantId { get; } = Guid.NewGuid().ToString("N");

		public IMongoDbUserManagerProvider<MongoUser> UserManagerProvider => TestFixture.Service<IMongoDbUserManagerProvider<MongoUser>>();

		public UserManager<MongoUser> TenantUserManager => UserManagerProvider.GetUserManager(TenantId);

		[Fact]
		public async Task CreateNewUserForTenant() {
			var user = new MongoUser {
				Name = "testUser",
				Email = "test@example.com"
			};

			var result = await TenantUserManager.CreateAsync(user);

			Assert.True(result.Succeeded);
			Assert.NotEqual(ObjectId.Empty, user.Id);
			Assert.NotNull(user.NormalizedName);
			Assert.NotNull(user.NormalizedEmail);
			Assert.Equal(TenantId, user.TenantId);
		}

		[Fact]
		public async Task FindExistingUserInTenant() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u.TenantId = TenantId);

			var result = await TenantUserManager.FindByIdAsync(user.Id.ToString());

			Assert.NotNull(result);
			Assert.Equal(user.Id, result.Id);
			Assert.Equal(TenantId, result.TenantId);
		}

		[Fact]
		public async Task FindNotExistingUserInTenant() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u.TenantId = TenantId);

			var userManager = UserManagerProvider.GetUserManager(Guid.NewGuid().ToString("N"));
			var result = await userManager.FindByIdAsync(user.Id.ToString());

			Assert.Null(result);
		}

		[Fact]
		public async Task DeleteExistingUserFromTenant() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u.TenantId = TenantId);

			var result = await TenantUserManager.DeleteAsync(user);

			Assert.True(result.Succeeded);

			var deleted = await TestFixture.FindUser(user.Id);

			Assert.Null(deleted);
		}

		[Fact]
		public async Task DeleteExistingUserFromOtherTenant() {
			var user = await TestFixture.CreateUser("testUser", "test@example.com", u => u.TenantId = TenantId);

			var userManager = UserManagerProvider.GetUserManager(Guid.NewGuid().ToString("N"));
			var result = await userManager.DeleteAsync(user);

			Assert.False(result.Succeeded);
			Assert.NotEmpty(result.Errors);
			Assert.Single(result.Errors);
			Assert.Equal(MongoDbStoreErrorCodes.UserNotFound, result.Errors.First().Code);

			var existing = await TestFixture.FindUser(user.Id);

			Assert.NotNull(existing);
		}

		public void Dispose() {
			TestFixture.TestDisposed();
		}
	}
}
