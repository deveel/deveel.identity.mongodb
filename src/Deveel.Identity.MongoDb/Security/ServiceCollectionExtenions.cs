using System;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Security {
	public static class ServiceCollectionExtenions {
		private static IServiceCollection AddMongoIdentityStores(this IServiceCollection services) {
			services.AddSingleton<IUserStore<MongoUser>, MongoDbUserStore>();
			services.AddSingleton<MongoDbUserStore<MongoUser>>();
			services.AddSingleton<MongoDbUserStore>();

			services.AddSingleton<IRoleStore<MongoRole>, MongoDbRoleStore>();
			services.AddSingleton<MongoDbRoleStore<MongoRole>>();
			services.AddSingleton<MongoDbRoleStore>();

			return services;
		}

		private static IServiceCollection AddMongoIdentityMultiTenancy(this IServiceCollection services) {
			services.AddSingleton<IMongoDbUserStoreProvider<MongoUser>, MongoDbUserStoreProvider>();
			services.AddSingleton<MongoDbUserStoreProvider>();
			services.AddSingleton<MongoDbUserStoreProvider<MongoUser>>();

			services.AddSingleton<IMongoDbRoleStoreProvider<MongoRole>, MongoDbRoleStoreProvider>();
			services.AddSingleton<MongoDbRoleStoreProvider>();
			services.AddSingleton<MongoDbRoleStoreProvider<MongoRole>>();

			services.TryAddTransient<IMongoDbUserManagerProvider<MongoUser>, MongoDbUserManagerProvider>();
			services.TryAddTransient<IMongoDbRoleManagerProvider<MongoRole>, MongoDbRoleManagerProvider>();

			return services;
		}

		private static void Configure<TOptions>(IConfiguration config, string sectionName, TOptions options) {
			var section = config.GetSection(sectionName);
			if (section != null)
				section.Bind(options);
		}

		public static IServiceCollection AddMongoIdentityStores(this IServiceCollection services, string sectionName) {
			services.AddOptions<MongoDbStoreOptions>()
				.Configure<IConfiguration>((options, config) => Configure(config, sectionName, options));

			return services.AddMongoIdentityStores();
		}

		public static IServiceCollection AddMongoIdentityStores(this IServiceCollection services, IConfiguration configuration, string sectionName) {
			services.AddOptions<MongoDbStoreOptions>()
				.Configure(options => Configure(configuration, sectionName, options));

			return services.AddMongoIdentityStores();
		}

		public static IServiceCollection AddMongoIdentityStores(this IServiceCollection services, Action<MongoDbStoreOptions> configure) {
			services.AddOptions<MongoDbStoreOptions>()
				.Configure(configure);

			return services.AddMongoIdentityStores();
		}


		public static IServiceCollection AddMongoIdentityMultiTenancy(this IServiceCollection services, string sectionName) {
			services.AddOptions<MongoDbStoreProviderOptions>()
				.Configure<IConfiguration>((options, config) => Configure(config, sectionName, options));

			return services.AddMongoIdentityMultiTenancy();
		}

		public static IServiceCollection AddMongoIdentityMultiTenancy(this IServiceCollection services, IConfiguration configuration, string sectionName) {
			services.AddOptions<MongoDbStoreProviderOptions>()
				.Configure(options => Configure(configuration, sectionName, options));

			return services.AddMongoIdentityMultiTenancy();
		}

		public static IServiceCollection AddMongoIdentityMultiTenancy(this IServiceCollection services, Action<MongoDbStoreProviderOptions> configure) {
			services.AddOptions<MongoDbStoreProviderOptions>()
				.Configure(configure);

			return services.AddMongoIdentityMultiTenancy();
		}
	}
}
