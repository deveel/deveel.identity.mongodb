using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using MongoDB.Bson;

using Xunit;
using Xunit.Abstractions;

namespace Deveel.Security {
	public class MongoDbMultiTenantRoleStoreTests : IClassFixture<MongoDbIdentityTestFixture>, IDisposable {
		public MongoDbMultiTenantRoleStoreTests(MongoDbIdentityTestFixture testFixture, ITestOutputHelper outputHelper) {
			TestFixture = testFixture;
			TestFixture.TestCreated(outputHelper);
		}

		public MongoDbIdentityTestFixture TestFixture { get; }

		public string TenantId { get; } = Guid.NewGuid().ToString("N");

		public IMongoDbRoleManagerProvider<MongoRole> RoleManagerProvider => TestFixture.Service<IMongoDbRoleManagerProvider<MongoRole>>();

		public RoleManager<MongoRole> TenantRoleManager => RoleManagerProvider.GetRoleManager(TenantId);

		[Fact]
		public async Task CreateNewRoleForTenant() {
			var role = new MongoRole {
				Name = "user",
			};

			var result = await TenantRoleManager.CreateAsync(role);

			Assert.True(result.Succeeded);
			Assert.NotEqual(ObjectId.Empty, role.Id);
			Assert.NotNull(role.NormalizedName);
			Assert.Equal(TenantId, role.TenantId);
		}

		[Fact]
		public async Task FindExistingRoleInTenant() {
			var role = await TestFixture.CreateRole("user", r => r.TenantId = TenantId);

			var result = await TenantRoleManager.FindByIdAsync(role.Id.ToString());

			Assert.NotNull(result);
			Assert.Equal(role.Id, result.Id);
			Assert.Equal(TenantId, result.TenantId);
		}


		public void Dispose() {
			TestFixture.TestDisposed();
		}
	}
}
