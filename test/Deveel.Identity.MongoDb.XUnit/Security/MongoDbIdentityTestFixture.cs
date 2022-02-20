using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Mongo2Go;

using MongoDB.Bson;
using MongoDB.Driver;

using Xunit.Abstractions;

namespace Deveel.Security {
	public class MongoDbIdentityTestFixture : IDisposable {
		private MongoDbRunner mongo;
		private ITestOutputHelper outputHelper;
		private IServiceProvider serviceProvider;

		private const string UsersCollectionName = "users";
		private const string RolesCollectionName = "roles";
		private const string DatabaseName = "identity";

		public void TestCreated(ITestOutputHelper outputHelper) {
			this.outputHelper = outputHelper;
			mongo = MongoDbRunner.Start(logger: NullLogger.Instance);
		}

		public void TestDisposed() {
			serviceProvider = null;
			mongo?.Dispose();
		}


		private IServiceProvider BuildServiceProvider() {
			var services = new ServiceCollection()
				.AddMongoIdentityStores(options => {
					options.ConnectionString = mongo.ConnectionString;
					options.DatabaseName = DatabaseName;
					options.UsersCollection = UsersCollectionName;
					options.RolesCollection = RolesCollectionName;
				})
				.AddMongoIdentityMultiTenancy(options => {
					options.ConnectionString = mongo.ConnectionString;
					options.DatabaseName = DatabaseName;
					options.UsersCollection = UsersCollectionName;
					options.RolesCollection = RolesCollectionName;
					options.MultiTenancy.Handling = MultiTenancyHandling.TenantField;
				})
				.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Trace).AddXUnit(outputHelper));

			services.AddIdentityCore<MongoUser>(options => {
				options.User.RequireUniqueEmail = true;
				options.Lockout.MaxFailedAccessAttempts = 2;
			})
			.AddMongoUserStore()
			.AddMongoRoleStore()
			.AddTokenProvider<TestEmailTwoFactorAuthentication<MongoUser>>("Default")
			.AddTokenProvider<TestPhoneTwoFactorAuthentication<MongoUser>>("Phone");

			return services.BuildServiceProvider();
		}

		private IMongoCollection<MongoUser> UsersCollection {
			get {
				var client = new MongoClient(mongo.ConnectionString);
				var database = client.GetDatabase(DatabaseName);
				return database.GetCollection<MongoUser>(UsersCollectionName);
			}
		}

		private IMongoCollection<MongoRole> RolesCollection {
			get {
				var client = new MongoClient(mongo.ConnectionString);
				var database = client.GetDatabase(DatabaseName);
				return database.GetCollection<MongoRole>(RolesCollectionName);
			}
		}

		public Task CreateUser(MongoUser user)
			=> UsersCollection.InsertOneAsync(user);

		public async Task<MongoUser> CreateUser(string name, string email, Action<MongoUser> configure = null) {
			var lookupNormalizer = Service<ILookupNormalizer>();

			var user = new MongoUser {
				Name = name,
				NormalizedName = lookupNormalizer.NormalizeName(name),
				Email = email,
				NormalizedEmail = lookupNormalizer.NormalizeEmail(email),
				SecurityStamp = Guid.NewGuid().ToString(),
			};

			configure?.Invoke(user);

			await CreateUser(user);

			return user;
		}

		public async Task<MongoUser> FindUser(ObjectId id) {
			var result = await UsersCollection.FindAsync(user => user.Id == id);
			return await result.FirstOrDefaultAsync();
		}

		public Task CreateRole(MongoRole role)
			=> RolesCollection.InsertOneAsync(role);

		public async Task<MongoRole> CreateRole(string name, Action<MongoRole> configure = null) {
			var lookupNormalizer = Service<ILookupNormalizer>();

			var role = new MongoRole {
				Name = name,
				NormalizedName = lookupNormalizer.NormalizeName(name)
			};

			configure?.Invoke(role);

			await CreateRole(role);

			return role;
		}

		public async Task<MongoRole> FindRole(ObjectId id) {
			var result = await RolesCollection.FindAsync(role => role.Id == id);
			return await result.FirstOrDefaultAsync();
		}

		public TService Service<TService>() {
			if (serviceProvider == null)
				serviceProvider = BuildServiceProvider();

			return serviceProvider.GetService<TService>();
		}

		public void Dispose() {
			mongo?.Dispose();
		}
	}
}
