using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deveel.Security {
	public class MongoDbUserStore : MongoDbUserStore<MongoUser> {
		public MongoDbUserStore(IOptions<MongoDbStoreOptions> options) 
			: base(options) {
		}

		public MongoDbUserStore(IOptions<MongoDbStoreOptions> options, ILogger<MongoDbUserStore> logger) 
			: base(options, logger) {
		}

		public MongoDbUserStore(MongoDbStoreOptions options, ILogger<MongoDbUserStore> logger) 
			: base(options, logger) {
		}

		public MongoDbUserStore(MongoDbStoreOptions options)
			: base(options) {
		}
	}
}
