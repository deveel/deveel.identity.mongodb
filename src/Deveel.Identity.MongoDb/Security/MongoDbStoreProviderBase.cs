using System;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Deveel.Security {
	public abstract class MongoDbStoreProviderBase {
		private readonly ILoggerFactory loggerFactory;

		protected MongoDbStoreProviderBase(IOptions<MongoDbStoreProviderOptions> options, ILoggerFactory loggerFactory)
			: this(options.Value, loggerFactory) {
		}

		protected MongoDbStoreProviderBase(IOptions<MongoDbStoreProviderOptions> options)
			: this(options, NullLoggerFactory.Instance) {
		}

		protected MongoDbStoreProviderBase(MongoDbStoreProviderOptions options, ILoggerFactory loggerFactory) {
			Options = options;
			this.loggerFactory = loggerFactory;
		}

		protected MongoDbStoreProviderBase(MongoDbStoreProviderOptions options)
			: this(options, NullLoggerFactory.Instance) {
		}

		protected MongoDbStoreProviderOptions Options { get; }

		protected ILogger<TStore> CreateLogger<TStore>() => loggerFactory.CreateLogger<TStore>();

		protected MongoDbStoreOptions GetStoreOptions(string tenantId) {
			if (Options.MultiTenancy == null)
				throw new NotSupportedException("The multi-tenancy options were not set");

			var options = new MongoDbStoreOptions(Options);

			switch (Options.MultiTenancy.Handling) {
				case MultiTenancyHandling.TenantField:
					options.TenantId = tenantId;
					break;
				case MultiTenancyHandling.TenantDatabase:
					options.DatabaseName = FormatDatabaseName(tenantId);
					break;
				case MultiTenancyHandling.TenantCollection:
					options.UsersCollection = FormatCollection(tenantId, options.UsersCollection);
					options.RolesCollection = FormatCollection(tenantId, options.RolesCollection);
					break;
				case MultiTenancyHandling.None:
				default:
					break;
			}

			return options;
		}

		private string FormatDatabaseName(string tenantId) {
			var sb = new StringBuilder(Options.MultiTenancy.DatabaseFormat);
			sb.Replace("{database}", Options.DatabaseName);
			sb.Replace("{tenant}", tenantId);

			return sb.ToString();
		}

		private string FormatCollection(string tenantId, string collectionName) {
			var sb = new StringBuilder(Options.MultiTenancy.CollectionFormat);
			sb.Replace("{collection}", collectionName);
			sb.Replace("{tenant}", tenantId);

			return sb.ToString();
		}
	}
}
