using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deveel.Security {
	public class MongoDbRoleStore : MongoDbRoleStore<MongoRole> {
		public MongoDbRoleStore(IOptions<MongoDbStoreOptions> options) : base(options) {
		}

		public MongoDbRoleStore(IOptions<MongoDbStoreOptions> options, ILogger<MongoDbRoleStore> logger) : base(options, logger) {
		}

		public MongoDbRoleStore(MongoDbStoreOptions options) : base(options) {
		}

		public MongoDbRoleStore(MongoDbStoreOptions options, ILogger<MongoDbRoleStore> logger) : base(options, logger) {
		}
	}
}
