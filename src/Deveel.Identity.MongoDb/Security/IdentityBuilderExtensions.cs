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

namespace Deveel.Security {
	public static class IdentityBuilderExtensions {
		public static IdentityBuilder ConfigureMongo(this IdentityBuilder builder, IConfiguration configuration, string sectionName) {
			builder.Services.ConfigureMongoIdentity(configuration, sectionName);

			return builder;
		}

		public static IdentityBuilder ConfigureMongo(this IdentityBuilder builder, string sectionName) {
			builder.Services.ConfigureMongoIdentity(sectionName);

			return builder;
		}

		public static IdentityBuilder ConfigureMongo(this IdentityBuilder builder, Action<MongoDbStoreOptions> configure) {
			builder.Services.ConfigureMongoIdentity(configure);

			return builder;
		}

		public static IdentityBuilder ConfigureMongoMultiTenancy(this IdentityBuilder builder, IConfiguration configuration, string sectionName) {
			builder.Services.ConfigureMongoMultiTenancy(configuration, sectionName);

			return builder;
		}

		public static IdentityBuilder ConfigureMongoMultiTenancy(this IdentityBuilder builder, string sectionName) {
			builder.Services.ConfigureMongoMultiTenancy(sectionName);

			return builder;
		}

		public static IdentityBuilder ConfigureMongoMultiTenancy(this IdentityBuilder builder, Action<MongoDbMultiTenancyOptions> configure) {
			builder.Services.ConfigureMongoMultiTenancy(configure);

			return builder;
		}

		public static IdentityBuilder AddMongoUserStore<TUser>(this IdentityBuilder builder)
			where TUser : MongoUser
			=> builder.AddUserStore<MongoDbUserStore<TUser>>();

		public static IdentityBuilder AddMongoUserStore<TUser>(this IdentityBuilder builder, IConfiguration configuration, string sectionName)
			where TUser : MongoUser
			=> builder.ConfigureMongo(configuration, sectionName).AddMongoUserStore<TUser>();

		public static IdentityBuilder AddMongoUserStore<TUser>(this IdentityBuilder builder, string sectionName)
			where TUser : MongoUser
			=> builder.ConfigureMongo(sectionName).AddMongoUserStore<TUser>();

		public static IdentityBuilder AddMongoUserStore<TUser>(this IdentityBuilder builder, Action<MongoDbStoreOptions> configure)
			where TUser : MongoUser
			=> builder.ConfigureMongo(configure).AddMongoUserStore<TUser>();

		public static IdentityBuilder AddMongoUserStore(this IdentityBuilder builder)
			=> builder.AddUserStore<MongoDbUserStore>();

		public static IdentityBuilder AddMongoUserStore(this IdentityBuilder builder, IConfiguration configuration, string sectionName)
			=> builder.ConfigureMongo(configuration, sectionName).AddMongoUserStore();

		public static IdentityBuilder AddMongoUserStore(this IdentityBuilder builder, string sectionName)
			=> builder.ConfigureMongo(sectionName).AddMongoUserStore();

		public static IdentityBuilder AddMongoUserStore(this IdentityBuilder builder, Action<MongoDbStoreOptions> configure)
			=> builder.ConfigureMongo(configure).AddMongoUserStore();


		public static IdentityBuilder AddMongoRoleStore<TRole>(this IdentityBuilder builder)
			where TRole : MongoRole
			=> builder.AddRoles<TRole>().AddRoleStore<MongoDbRoleStore<TRole>>();

		public static IdentityBuilder AddMongoRoleStore<TRole>(this IdentityBuilder builder, IConfiguration configuration, string sectionName)
			where TRole : MongoRole
			=> builder.ConfigureMongo(configuration, sectionName).AddMongoRoleStore<TRole>();

		public static IdentityBuilder AddMongoRoleStore<TRole>(this IdentityBuilder builder, string sectionName)
			where TRole : MongoRole
			=> builder.ConfigureMongo(sectionName).AddMongoRoleStore<TRole>();

		public static IdentityBuilder AddMongoRoleStore<TRole>(this IdentityBuilder builder, Action<MongoDbStoreOptions> configure)
			where TRole : MongoRole
			=> builder.ConfigureMongo(configure).AddMongoRoleStore<TRole>();


		public static IdentityBuilder AddMongoRoleStore(this IdentityBuilder builder)
			=> builder.AddRoles<MongoRole>().AddRoleStore<MongoDbRoleStore>();

		public static IdentityBuilder AddMongoRoleStore(this IdentityBuilder builder, IConfiguration configuration, string sectionName)
			=> builder.ConfigureMongo(configuration, sectionName).AddMongoRoleStore();

		public static IdentityBuilder AddMongoRoleStore(this IdentityBuilder builder, string sectionName)
			=> builder.ConfigureMongo(sectionName).AddMongoRoleStore();

		public static IdentityBuilder AddMongoRoleStore(this IdentityBuilder builder, Action<MongoDbStoreOptions> configure)
			=> builder.ConfigureMongo(configure).AddMongoRoleStore();


		public static IdentityBuilder AddMongoStores<TUser, TRole>(this IdentityBuilder builder)
			where TUser : MongoUser
			where TRole : MongoRole
			=> builder.AddMongoUserStore<TUser>().AddMongoRoleStore<TRole>();

		public static IdentityBuilder AddMongoStores<TUser, TRole>(this IdentityBuilder builder, IConfiguration configuration, string sectionName)
			where TUser : MongoUser
			where TRole : MongoRole
			=> builder.ConfigureMongo(configuration, sectionName).AddMongoStores<TUser, TRole>();

		public static IdentityBuilder AddMongoStores<TUser, TRole>(this IdentityBuilder builder, string sectionName)
			where TUser : MongoUser
			where TRole : MongoRole
			=> builder.ConfigureMongo(sectionName).AddMongoStores<TUser, TRole>();

		public static IdentityBuilder AddMongoStores<TUser, TRole>(this IdentityBuilder builder, Action<MongoDbStoreOptions> configure)
			where TUser : MongoUser
			where TRole : MongoRole
			=> builder.ConfigureMongo(configure).AddMongoStores<TUser, TRole>();

		public static IdentityBuilder AddMongoStores(this IdentityBuilder builder)
			=> builder.AddMongoUserStore().AddMongoRoleStore();

		public static IdentityBuilder AddMongoStores(this IdentityBuilder builder, IConfiguration configuration, string sectionName)
			=> builder.ConfigureMongo(configuration, sectionName).AddMongoStores();

		public static IdentityBuilder AddMongoStores(this IdentityBuilder builder, string sectionName)
			=> builder.ConfigureMongo(sectionName).AddMongoStores();

		public static IdentityBuilder AddMongoStores(this IdentityBuilder builder, Action<MongoDbStoreOptions> configure)
			=> builder.ConfigureMongo(configure).AddMongoStores();

		public static IdentityBuilder AddMongoMultiTenancy(this IdentityBuilder builder, IConfiguration configuration, string sectionName) {
			builder.Services.AddMongoIdentityMultiTenancy();

			return builder.ConfigureMongoMultiTenancy(configuration, sectionName);
		}

		public static IdentityBuilder AddMongoMultiTenancy(this IdentityBuilder builder, string sectionName) {
			builder.Services.AddMongoIdentityMultiTenancy();

			return builder.ConfigureMongoMultiTenancy(sectionName);
		}

		public static IdentityBuilder AddMongoMultiTenancy(this IdentityBuilder builder, Action<MongoDbMultiTenancyOptions> configure) {
			builder.Services.AddMongoIdentityMultiTenancy();

			return builder.ConfigureMongoMultiTenancy(configure);
		}
	}
}
