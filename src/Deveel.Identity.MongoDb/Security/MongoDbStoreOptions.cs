using System;

namespace Deveel.Security {
	public class MongoDbStoreOptions : MongoDbOptions {
		public MongoDbStoreOptions() {
		}

		public MongoDbStoreOptions(MongoDbOptions options) {
			ConnectionString = options.ConnectionString;
			DatabaseName = options.DatabaseName;
			RolesCollection = options.RolesCollection;
			UsersCollection = options.UsersCollection;
		}

		public string TenantId { get; set; }

		public bool HasTenantSet => !String.IsNullOrWhiteSpace(TenantId);
	}
}
