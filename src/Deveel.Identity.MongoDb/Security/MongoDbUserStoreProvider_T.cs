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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deveel.Security {
	public class MongoDbUserStoreProvider<TUser> : MongoDbStoreProviderBase, IMongoDbUserStoreProvider<TUser> where TUser : MongoUser {
		public MongoDbUserStoreProvider(IOptions<MongoDbStoreOptions> options, IOptions<MongoDbMultiTenancyOptions> multiTenancy, ILoggerFactory loggerFactory)
			: base(options, multiTenancy, loggerFactory) {
		}

		public MongoDbUserStoreProvider(IOptions<MongoDbStoreOptions> options, IOptions<MongoDbMultiTenancyOptions> multiTenancy)
			: base(options, multiTenancy) {
		}

		public MongoDbUserStoreProvider(MongoDbStoreOptions options, MongoDbMultiTenancyOptions multiTenancy, ILoggerFactory loggerFactory)
			: base(options, multiTenancy, loggerFactory) {
		}

		public MongoDbUserStoreProvider(MongoDbStoreOptions options, MongoDbMultiTenancyOptions multiTenancy)
			: base(options, multiTenancy) {
		}

		protected ILogger<MongoDbUserStore<TUser>> CreateLogger() => CreateLogger<MongoDbUserStore<TUser>>();

		public virtual MongoDbUserStore<TUser> GetStore(string tenantId) {
			var options = GetStoreOptions(tenantId);

			var logger = CreateLogger();
			return new MongoDbUserStore<TUser>(options, logger);
		}
	}
}
