using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Deveel.Security {
	public class MongoDbUserStoreProvider<TUser> : MongoDbStoreProviderBase, IMongoDbUserStoreProvider<TUser> where TUser : MongoUser {
		public MongoDbUserStoreProvider(IOptions<MongoDbStoreProviderOptions> options, ILoggerFactory loggerFactory) 
			: base(options, loggerFactory) {
		}

		public MongoDbUserStoreProvider(IOptions<MongoDbStoreProviderOptions> options) 
			: base(options) {
		}

		public MongoDbUserStoreProvider(MongoDbStoreProviderOptions options, ILoggerFactory loggerFactory) 
			: base(options, loggerFactory) {
		}

		public MongoDbUserStoreProvider(MongoDbStoreProviderOptions options) : base(options) {
		}

		protected ILogger<MongoDbUserStore<TUser>> CreateLogger() => CreateLogger<MongoDbUserStore<TUser>>();

		public virtual MongoDbUserStore<TUser> GetStore(string tenantId) {
			var options = GetStoreOptions(tenantId);

			var logger = CreateLogger();
			return new MongoDbUserStore<TUser>(options, logger);
		}
	}
}
