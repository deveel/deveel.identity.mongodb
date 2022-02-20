using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deveel.Security {
	public class MongoDbRoleStoreProvider<TRole> : MongoDbStoreProviderBase, IMongoDbRoleStoreProvider<TRole>
		where TRole : MongoRole {
		public MongoDbRoleStoreProvider(IOptions<MongoDbStoreProviderOptions> options) : base(options) {
		}

		public MongoDbRoleStoreProvider(IOptions<MongoDbStoreProviderOptions> options, ILoggerFactory loggerFactory) : base(options, loggerFactory) {
		}

		public MongoDbRoleStoreProvider(MongoDbStoreProviderOptions options) : base(options) {
		}

		public MongoDbRoleStoreProvider(MongoDbStoreProviderOptions options, ILoggerFactory loggerFactory) : base(options, loggerFactory) {
		}

		protected ILogger<MongoDbRoleStore<TRole>> CreateLogger() => CreateLogger<MongoDbRoleStore<TRole>>();

		public MongoDbRoleStore<TRole> GetStore(string tenantId) {
			var options = GetStoreOptions(tenantId);

			var logger = CreateLogger();
			return new MongoDbRoleStore<TRole>(options, logger);
		}
	}
}
