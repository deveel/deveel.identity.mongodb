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
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Deveel.Security {
	public class MongoDbRoleManagerProvider : MongoDbRoleManagerProvider<MongoRole> {
		public MongoDbRoleManagerProvider(IMongoDbRoleStoreProvider<MongoRole> storeProvider,
			IEnumerable<IRoleValidator<MongoRole>> roleValidators,
			ILookupNormalizer keyNormalizer,
			IdentityErrorDescriber errors,
			ILoggerFactory loggerFactory) : base(storeProvider, roleValidators, keyNormalizer, errors, loggerFactory) {
		}
	}
}
