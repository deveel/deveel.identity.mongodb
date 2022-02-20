using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deveel.Security {
	public class MongoDbRoleStoreProvider : MongoDbRoleStoreProvider<MongoRole> {
		public MongoDbRoleStoreProvider(IOptions<MongoDbStoreProviderOptions> options, ILoggerFactory loggerFactory) 
			: base(options, loggerFactory) {
		}

		public MongoDbRoleStoreProvider(IOptions<MongoDbStoreProviderOptions> options) 
			: base(options) {
		}

		public MongoDbRoleStoreProvider(MongoDbStoreProviderOptions options) 
			: base(options) {
		}

		public MongoDbRoleStoreProvider(MongoDbStoreProviderOptions options, ILoggerFactory loggerFactory) 
			: base(options, loggerFactory) {
		}

		public new MongoDbRoleStore GetStore(string tenantId) {
			var options = GetStoreOptions(tenantId);

			var logger = CreateLogger<MongoDbRoleStore>();
			return new MongoDbRoleStore(options, logger);
		}
	}
}
