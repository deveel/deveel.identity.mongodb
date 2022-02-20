// Copyright 2022 Deveel
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Deveel.Security {
	public abstract class MongoDbStoreProviderBase {
		private readonly ILoggerFactory loggerFactory;

		protected MongoDbStoreProviderBase(IOptions<MongoDbStoreOptions> options, IOptions<MongoDbMultiTenancyOptions> multiTenancy, ILoggerFactory loggerFactory)
			: this(options.Value, multiTenancy.Value, loggerFactory) {
		}

		protected MongoDbStoreProviderBase(IOptions<MongoDbStoreOptions> options, IOptions<MongoDbMultiTenancyOptions> multiTenancy)
			: this(options, multiTenancy, NullLoggerFactory.Instance) {
		}

		protected MongoDbStoreProviderBase(MongoDbStoreOptions options, MongoDbMultiTenancyOptions multiTenancy, ILoggerFactory loggerFactory) {
			StoreOptions = options;
			MultiTenancy = multiTenancy;
			this.loggerFactory = loggerFactory;
		}

		protected MongoDbStoreProviderBase(MongoDbStoreOptions options, MongoDbMultiTenancyOptions multiTenancy)
			: this(options, multiTenancy, NullLoggerFactory.Instance) {
		}

		protected MongoDbStoreOptions StoreOptions { get; }

		protected MongoDbMultiTenancyOptions MultiTenancy { get; }

		protected ILogger<TStore> CreateLogger<TStore>() => loggerFactory.CreateLogger<TStore>();

		protected MongoDbStoreOptions GetStoreOptions(string tenantId) {
			if (MultiTenancy == null)
				throw new NotSupportedException("The multi-tenancy options were not set");

			var options = new MongoDbStoreOptions(StoreOptions);

			switch (MultiTenancy.Handling) {
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
			var sb = new StringBuilder(MultiTenancy.DatabaseFormat);
			sb.Replace("{database}", StoreOptions.DatabaseName);
			sb.Replace("{tenant}", tenantId);

			return sb.ToString();
		}

		private string FormatCollection(string tenantId, string collectionName) {
			var sb = new StringBuilder(MultiTenancy.CollectionFormat);
			sb.Replace("{collection}", collectionName);
			sb.Replace("{tenant}", tenantId);

			return sb.ToString();
		}
	}
}
