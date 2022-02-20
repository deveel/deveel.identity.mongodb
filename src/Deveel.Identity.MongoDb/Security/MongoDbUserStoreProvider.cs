using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deveel.Security {
	public sealed class MongoDbUserStoreProvider : MongoDbUserStoreProvider<MongoUser> {
		public MongoDbUserStoreProvider(IOptions<MongoDbStoreProviderOptions> options) 
			: base(options) {
		}

		public MongoDbUserStoreProvider(IOptions<MongoDbStoreProviderOptions> options, ILoggerFactory loggerFactory) 
			: base(options, loggerFactory) {
		}

		public MongoDbUserStoreProvider(MongoDbStoreProviderOptions options) 
			: base(options) {
		}

		public MongoDbUserStoreProvider(MongoDbStoreProviderOptions options, ILoggerFactory loggerFactory) 
			: base(options, loggerFactory) {
		}

		public new MongoDbUserStore GetStore(string tenantId) {
			var options = GetStoreOptions(tenantId);

			var logger = CreateLogger<MongoDbUserStore>();
			return new MongoDbUserStore(options, logger);
		}
	}
}
