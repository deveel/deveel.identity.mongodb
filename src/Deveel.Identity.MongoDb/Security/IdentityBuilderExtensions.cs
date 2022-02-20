using System;

using Microsoft.AspNetCore.Identity;

namespace Deveel.Security {
	public static class IdentityBuilderExtensions {
		public static IdentityBuilder AddMongoUserStore<TUser>(this IdentityBuilder builder)
			where TUser : MongoUser
			=> builder.AddUserStore<MongoDbUserStore<TUser>>();

		public static IdentityBuilder AddMongoUserStore(this IdentityBuilder builder)
			=> builder.AddUserStore<MongoDbUserStore>();

		public static IdentityBuilder AddMongoRoleStore<TRole>(this IdentityBuilder builder)
			where TRole : MongoRole
			=> builder.AddRoles<TRole>().AddRoleStore<MongoDbRoleStore<TRole>>();

		public static IdentityBuilder AddMongoRoleStore(this IdentityBuilder builder)
			=> builder.AddRoles<MongoRole>().AddRoleStore<MongoDbRoleStore>();
	}
}
