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

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Deveel.Security {
	public static class ServiceCollectionExtenions {
		//private static IServiceCollection AddMongoIdentityStores(this IServiceCollection services) {
		//	services.AddSingleton<IUserStore<MongoUser>, MongoDbUserStore>();
		//	services.AddSingleton<MongoDbUserStore<MongoUser>>();
		//	services.AddSingleton<MongoDbUserStore>();

		//	services.AddSingleton<IRoleStore<MongoRole>, MongoDbRoleStore>();
		//	services.AddSingleton<MongoDbRoleStore<MongoRole>>();
		//	services.AddSingleton<MongoDbRoleStore>();

		//	return services;
		//}

		internal static IServiceCollection AddMongoIdentityMultiTenancy(this IServiceCollection services) {
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

		public static IServiceCollection ConfigureMongoIdentity(this IServiceCollection services, string sectionName) {
			services.AddOptions<MongoDbStoreOptions>()
				.Configure<IConfiguration>((options, config) => Configure(config, sectionName, options));

			return services;
		}

		public static IServiceCollection ConfigureMongoIdentity(this IServiceCollection services, IConfiguration configuration, string sectionName) {
			services.AddOptions<MongoDbStoreOptions>()
				.Configure(options => Configure(configuration, sectionName, options));

			return services;
		}

		public static IServiceCollection ConfigureMongoIdentity(this IServiceCollection services, Action<MongoDbStoreOptions> configure) {
			services.AddOptions<MongoDbStoreOptions>()
				.Configure(configure);

			return services;
		}

		public static IServiceCollection ConfigureMongoMultiTenancy(this IServiceCollection services, string sectionName) {
			services.AddOptions<MongoDbMultiTenancyOptions>()
				.Configure<IConfiguration>((options, config) => Configure(config, sectionName, options));

			return services;
		}

		public static IServiceCollection ConfigureMongoMultiTenancy(this IServiceCollection services, IConfiguration configuration, string sectionName) {
			services.AddOptions<MongoDbMultiTenancyOptions>()
				.Configure(options => Configure(configuration, sectionName, options));

			return services;
		}

		public static IServiceCollection ConfigureMongoMultiTenancy(this IServiceCollection services, Action<MongoDbMultiTenancyOptions> configure) {
			services.AddOptions<MongoDbMultiTenancyOptions>()
				.Configure(configure);

			return services;
		}

		//public static IServiceCollection AddMongoIdentityStores(this IServiceCollection services, string sectionName)
		//	=> services.ConfigureMongoIdentity(sectionName).AddMongoIdentityStores();

		//public static IServiceCollection AddMongoIdentityStores(this IServiceCollection services, IConfiguration configuration, string sectionName)
		//	=> services.ConfigureMongoIdentity(configuration, sectionName).AddMongoIdentityStores();

		//public static IServiceCollection AddMongoIdentityStores(this IServiceCollection services, Action<MongoDbStoreOptions> configure)
		//	=> services.ConfigureMongoIdentity(configure).AddMongoIdentityStores();
	}
}
