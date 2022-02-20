using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Xunit;

namespace Deveel.Security {
	public static class MongoDbConfigurationTests {
		[Fact]
		public static void ConfigureStoreFromSection() {
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string> {
					{ "MongoDb:ConnectionString", "mongodb://127.0.0.1:2749" },
					{ "MongoDb:DatabaseName", "testdb" },
					{ "MongoDb:UsersCollection", "users" },
					{ "MongoDb:RolesCollection", "roles" }
				})
				.Build();

			var services = new ServiceCollection()
				.ConfigureMongoIdentity(configuration, "MongoDb")
				.BuildServiceProvider();

			var options = services.GetService<IOptions<MongoDbStoreOptions>>();

			Assert.NotNull(options);
			Assert.NotNull(options.Value);

			var storeOptions = options.Value;
			Assert.NotNull(storeOptions.ConnectionString);
			Assert.NotEmpty(storeOptions.ConnectionString);
			Assert.Equal("mongodb://127.0.0.1:2749", storeOptions.ConnectionString);
			Assert.NotNull(storeOptions.DatabaseName);
			Assert.NotEmpty(storeOptions.DatabaseName);
			Assert.Equal("testdb", storeOptions.DatabaseName);
			Assert.NotNull(storeOptions.UsersCollection);
			Assert.NotEmpty(storeOptions.UsersCollection);
			Assert.Equal("users", storeOptions.UsersCollection);
			Assert.NotNull(storeOptions.RolesCollection);
			Assert.NotEmpty(storeOptions.RolesCollection);
			Assert.Equal("roles", storeOptions.RolesCollection);
			Assert.Null(storeOptions.TenantId);
			Assert.False(storeOptions.HasTenantSet);
		}

		[Fact]
		public static void ConfigureStoreFromSectionInServices() {
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string> {
					{ "MongoDb:ConnectionString", "mongodb://127.0.0.1:2749" },
					{ "MongoDb:DatabaseName", "testdb" },
					{ "MongoDb:UsersCollection", "users" },
					{ "MongoDb:RolesCollection", "roles" }
				})
				.Build();

			var services = new ServiceCollection()
				.AddSingleton<IConfiguration>(configuration)
				.ConfigureMongoIdentity("MongoDb")
				.BuildServiceProvider();

			var options = services.GetService<IOptions<MongoDbStoreOptions>>();

			Assert.NotNull(options);
			Assert.NotNull(options.Value);

			var storeOptions = options.Value;
			Assert.NotNull(storeOptions.ConnectionString);
			Assert.NotEmpty(storeOptions.ConnectionString);
			Assert.Equal("mongodb://127.0.0.1:2749", storeOptions.ConnectionString);
			Assert.NotNull(storeOptions.DatabaseName);
			Assert.NotEmpty(storeOptions.DatabaseName);
			Assert.Equal("testdb", storeOptions.DatabaseName);
			Assert.NotNull(storeOptions.UsersCollection);
			Assert.NotEmpty(storeOptions.UsersCollection);
			Assert.Equal("users", storeOptions.UsersCollection);
			Assert.NotNull(storeOptions.RolesCollection);
			Assert.NotEmpty(storeOptions.RolesCollection);
			Assert.Equal("roles", storeOptions.RolesCollection);
			Assert.Null(storeOptions.TenantId);
			Assert.False(storeOptions.HasTenantSet);
		}

		[Fact]
		public static void ConfigureStoreFromAction() {
			var services = new ServiceCollection()
				.ConfigureMongoIdentity(options => {
					options.ConnectionString = "mongodb://127.0.0.1:2749";
					options.DatabaseName = "testdb";
					options.UsersCollection = "users";
					options.RolesCollection = "roles";
				})
				.BuildServiceProvider();

			var options = services.GetService<IOptions<MongoDbStoreOptions>>();

			Assert.NotNull(options);
			Assert.NotNull(options.Value);

			var storeOptions = options.Value;
			Assert.NotNull(storeOptions.ConnectionString);
			Assert.NotEmpty(storeOptions.ConnectionString);
			Assert.Equal("mongodb://127.0.0.1:2749", storeOptions.ConnectionString);
			Assert.NotNull(storeOptions.DatabaseName);
			Assert.NotEmpty(storeOptions.DatabaseName);
			Assert.Equal("testdb", storeOptions.DatabaseName);
			Assert.NotNull(storeOptions.UsersCollection);
			Assert.NotEmpty(storeOptions.UsersCollection);
			Assert.Equal("users", storeOptions.UsersCollection);
			Assert.NotNull(storeOptions.RolesCollection);
			Assert.NotEmpty(storeOptions.RolesCollection);
			Assert.Equal("roles", storeOptions.RolesCollection);
			Assert.Null(storeOptions.TenantId);
			Assert.False(storeOptions.HasTenantSet);
		}

		[Fact]
		public static void AddStoreFromSection() {
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string> {
					{ "MongoDb:ConnectionString", "mongodb://127.0.0.1:2749" },
					{ "MongoDb:DatabaseName", "testdb" },
					{ "MongoDb:UsersCollection", "users" },
					{ "MongoDb:RolesCollection", "roles" }
				})
				.Build();

			var services = new ServiceCollection();

			services.AddIdentityCore<MongoUser>()
				.AddMongoStores(configuration, "MongoDb");

			var provider =services.BuildServiceProvider();

			var options = provider.GetService<IOptions<MongoDbStoreOptions>>();

			Assert.NotNull(options);
			Assert.NotNull(options.Value);

			var storeOptions = options.Value;
			Assert.NotNull(storeOptions.ConnectionString);
			Assert.NotEmpty(storeOptions.ConnectionString);
			Assert.Equal("mongodb://127.0.0.1:2749", storeOptions.ConnectionString);
			Assert.NotNull(storeOptions.DatabaseName);
			Assert.NotEmpty(storeOptions.DatabaseName);
			Assert.Equal("testdb", storeOptions.DatabaseName);
			Assert.NotNull(storeOptions.UsersCollection);
			Assert.NotEmpty(storeOptions.UsersCollection);
			Assert.Equal("users", storeOptions.UsersCollection);
			Assert.NotNull(storeOptions.RolesCollection);
			Assert.NotEmpty(storeOptions.RolesCollection);
			Assert.Equal("roles", storeOptions.RolesCollection);
			Assert.Null(storeOptions.TenantId);
			Assert.False(storeOptions.HasTenantSet);
		}

		[Fact]
		public static void AddStoreFromSectionInConfiguration() {
			var configuration = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string> {
					{ "MongoDb:ConnectionString", "mongodb://127.0.0.1:2749" },
					{ "MongoDb:DatabaseName", "testdb" },
					{ "MongoDb:UsersCollection", "users" },
					{ "MongoDb:RolesCollection", "roles" }
				})
				.Build();

			var services = new ServiceCollection()
				.AddSingleton<IConfiguration>(configuration);

			services.AddIdentityCore<MongoUser>()
				.AddMongoStores("MongoDb");

			var provider = services.BuildServiceProvider();

			var options = provider.GetService<IOptions<MongoDbStoreOptions>>();

			Assert.NotNull(options);
			Assert.NotNull(options.Value);

			var storeOptions = options.Value;
			Assert.NotNull(storeOptions.ConnectionString);
			Assert.NotEmpty(storeOptions.ConnectionString);
			Assert.Equal("mongodb://127.0.0.1:2749", storeOptions.ConnectionString);
			Assert.NotNull(storeOptions.DatabaseName);
			Assert.NotEmpty(storeOptions.DatabaseName);
			Assert.Equal("testdb", storeOptions.DatabaseName);
			Assert.NotNull(storeOptions.UsersCollection);
			Assert.NotEmpty(storeOptions.UsersCollection);
			Assert.Equal("users", storeOptions.UsersCollection);
			Assert.NotNull(storeOptions.RolesCollection);
			Assert.NotEmpty(storeOptions.RolesCollection);
			Assert.Equal("roles", storeOptions.RolesCollection);
			Assert.Null(storeOptions.TenantId);
			Assert.False(storeOptions.HasTenantSet);
		}
	}
}
