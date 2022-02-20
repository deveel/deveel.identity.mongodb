using System;

using Microsoft.AspNetCore.Identity;

namespace Deveel.Security {
	public interface IMongoDbRoleManagerProvider<TRole> where TRole : MongoRole {
		RoleManager<TRole> GetRoleManager(string tenantId);
	}
}
