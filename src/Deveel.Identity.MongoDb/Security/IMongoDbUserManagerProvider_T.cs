using System;

using Microsoft.AspNetCore.Identity;

namespace Deveel.Security {
	public interface IMongoDbUserManagerProvider<TUser> where TUser : MongoUser {
		UserManager<TUser> GetUserManager(string tenantId);
	}
}
