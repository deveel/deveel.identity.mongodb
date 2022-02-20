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
	public sealed class MongoDbUserStoreProvider : MongoDbUserStoreProvider<MongoUser> {
		public MongoDbUserStoreProvider(IOptions<MongoDbStoreOptions> options, IOptions<MongoDbMultiTenancyOptions> multiTenancy)
			: base(options, multiTenancy) {
		}

		public MongoDbUserStoreProvider(IOptions<MongoDbStoreOptions> options, IOptions<MongoDbMultiTenancyOptions> multiTenancy, ILoggerFactory loggerFactory)
			: base(options, multiTenancy, loggerFactory) {
		}

		public MongoDbUserStoreProvider(MongoDbStoreOptions options, MongoDbMultiTenancyOptions multiTenancy)
			: base(options, multiTenancy) {
		}

		public MongoDbUserStoreProvider(MongoDbStoreOptions options, MongoDbMultiTenancyOptions multiTenancy, ILoggerFactory loggerFactory)
			: base(options, multiTenancy, loggerFactory) {
		}

		public new MongoDbUserStore GetStore(string tenantId) {
			var options = GetStoreOptions(tenantId);

			var logger = CreateLogger<MongoDbUserStore>();
			return new MongoDbUserStore(options, logger);
		}
	}
}
