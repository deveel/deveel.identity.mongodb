using System;

using Microsoft.AspNetCore.Identity;

namespace Deveel.Security {
	public interface IMongoDbUserStoreProvider<TUser> where TUser : MongoUser {
		MongoDbUserStore<TUser> GetStore(string tenantId);
	}
}
