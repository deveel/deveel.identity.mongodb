using System;

namespace Deveel.Security {
	public interface IMongoDbRoleStoreProvider<TRole> where TRole : MongoRole {
		MongoDbRoleStore<TRole> GetStore(string tenantId);
	}
}
