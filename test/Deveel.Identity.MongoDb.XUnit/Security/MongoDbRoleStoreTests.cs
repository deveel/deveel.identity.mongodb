using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using MongoDB.Bson;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Security {
	public class MongoDbRoleStoreTests : IClassFixture<MongoDbIdentityTestFixture>, IDisposable {
		public MongoDbRoleStoreTests(MongoDbIdentityTestFixture testFixture, ITestOutputHelper outputHelper) {
			TestFixture = testFixture;
			TestFixture.TestCreated(outputHelper);
		}

		public MongoDbIdentityTestFixture TestFixture { get; }

		public RoleManager<MongoRole> RoleManager => TestFixture.Service<RoleManager<MongoRole>>();

		[Fact]
		public async Task CreateNewRole() {
			var role = new MongoRole {
				Name = "user"
			};

			var result = await RoleManager.CreateAsync(role);

			Assert.True(result.Succeeded);

			var created = await TestFixture.FindRole(role.Id);

			Assert.NotNull(created);
			Assert.Equal(role.Name, created.Name);
		}

		[Fact]
		public async Task CreateDuplicatedRole() {
			await TestFixture.CreateRole("user");

			var role = new MongoRole {
				Name = "user"
			};

			var result = await RoleManager.CreateAsync(role);

			Assert.False(result.Succeeded);
		}

		[Fact]
		public async Task CheckExistingRole() {
			await TestFixture.CreateRole("user");

			var result = await RoleManager.RoleExistsAsync("user");

			Assert.True(result);
		}


		[Fact]
		public async Task DeleteExistingRole() {
			var role = await TestFixture.CreateRole("user");

			var result = await RoleManager.DeleteAsync(role);

			Assert.True(result.Succeeded);

			var deleted = await TestFixture.FindRole(role.Id);
			Assert.Null(deleted);
		}

		[Fact]
		public async Task DeleteNotExistingRole() {
			var role = new MongoRole {
				Id = ObjectId.GenerateNewId(),
				Name = "user"
			};

			var result = await RoleManager.DeleteAsync(role);

			Assert.False(result.Succeeded);
			Assert.NotEmpty(result.Errors);
			Assert.Single(result.Errors);
			Assert.Equal(MongoDbStoreErrorCodes.RoleNotFound, result.Errors.First().Code);
		}

		[Fact]
		public async Task UpdateExistingRole() {
			var role = await TestFixture.CreateRole("user");

			var tenantId = Guid.NewGuid().ToString("N");
			role.TenantId = tenantId;

			var result = await RoleManager.UpdateAsync(role);

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindRole(role.Id);

			Assert.Equal(tenantId, updated.TenantId);
		}

		[Fact]
		public async Task UpdateNotExistingRole() {
			var role = new MongoRole {
				Id = ObjectId.GenerateNewId(),
				Name = "testUser"
			};

			var tenantId = Guid.NewGuid().ToString("N");
			role.TenantId = tenantId;

			var result = await RoleManager.UpdateAsync(role);

			Assert.False(result.Succeeded);
			Assert.NotEmpty(result.Errors);
			Assert.Single(result.Errors);
			Assert.Equal(MongoDbStoreErrorCodes.RoleNotFound, result.Errors.First().Code);
		}

		[Fact]
		public async Task FindByNameExistingRole() {
			var role = await TestFixture.CreateRole("user");

			var result = await RoleManager.FindByNameAsync("user");

			Assert.NotNull(result);
			Assert.Equal(role.Id, result.Id);
			Assert.Equal(role.Name, result.Name);
		}

		[Fact]
		public async Task FindByNameNotExistingRole() {
			var result = await RoleManager.FindByNameAsync("user");

			Assert.Null(result);
		}


		[Fact]
		public async Task FindRoleById() {
			var role = await TestFixture.CreateRole("user");

			var result = await RoleManager.FindByIdAsync(role.Id.ToString());

			Assert.NotNull(result);
			Assert.Equal(role.Id, result.Id);
		}

		[Fact]
		public async Task SetRoleName() {
			var role = await TestFixture.CreateRole("user");

			var result = await RoleManager.SetRoleNameAsync(role, "newUser");

			Assert.True(result.Succeeded);

			Assert.Equal("newUser", role.Name);
		}

		[Fact]
		public async Task AddNewClaim() {
			var role = await TestFixture.CreateRole("admin");

			var result = await RoleManager.AddClaimAsync(role, new Claim("scope", "create:user"));

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindRole(role.Id);

			Assert.NotEmpty(updated.Claims);
			Assert.Contains(updated.Claims, claim => claim.Type == "scope" && claim.Value == "create:user");
		}

		[Fact]
		public async Task AddExistingClaim() {
			var role = await TestFixture.CreateRole("admin", r => r
				.Claims.Add(new MongoClaim {
					Type = "scope",
					Value = "create:user"
				}));

			var result = await RoleManager.AddClaimAsync(role, new Claim("scope", "create:user"));

			Assert.False(result.Succeeded);

			var updated = await TestFixture.FindRole(role.Id);

			Assert.NotEmpty(updated.Claims);
			Assert.Single(updated.Claims);
			Assert.Contains(updated.Claims, claim => claim.Type == "scope" && claim.Value == "create:user");
		}

		[Fact]
		public async Task RemoveExistingClaim() {
			var role = await TestFixture.CreateRole("admin", r => r
				.Claims.Add(new MongoClaim {
					Type = "scope",
					Value = "create:user"
				}));

			var result = await RoleManager.RemoveClaimAsync(role, new Claim("scope", "create:user"));

			Assert.True(result.Succeeded);

			var updated = await TestFixture.FindRole(role.Id);

			Assert.Empty(updated.Claims);
		}

		[Fact]
		public async Task RemoveNotExistingClaim() {
			var user = await TestFixture.CreateRole("admin", r => r
				.Claims.Add(new MongoClaim {
					Type = "scope",
					Value = "create:user"
				}));

			var result = await RoleManager.RemoveClaimAsync(user, new Claim(ClaimTypes.PostalCode, "0194"));

			Assert.False(result.Succeeded);

			var updated = await TestFixture.FindRole(user.Id);

			Assert.NotEmpty(updated.Claims);
			Assert.Contains(updated.Claims, claim => claim.Type == "scope" && claim.Value == "create:user");
		}


		public void Dispose() {
			TestFixture.TestDisposed();
		}
	}
}
