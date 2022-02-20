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
using Microsoft.Extensions.Options;

namespace Deveel.Security {
	public class MongoDbUserManagerProvider<TUser> : IMongoDbUserManagerProvider<TUser> where TUser : MongoUser {
		private readonly IMongoDbUserStoreProvider<TUser> storeProvider;
		private readonly IOptions<IdentityOptions> identityOptions;
		private readonly IPasswordHasher<TUser> passwordHasher;
		private readonly IEnumerable<IUserValidator<TUser>> userValidators;
		private readonly IEnumerable<IPasswordValidator<TUser>> passwordValidators;
		private readonly ILookupNormalizer keyNormalizer;
		private readonly IdentityErrorDescriber errors;
		private readonly IServiceProvider services;
		private readonly ILoggerFactory loggerFactory;

		public MongoDbUserManagerProvider(IMongoDbUserStoreProvider<TUser> storeProvider, IOptions<IdentityOptions> identityOptions, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILoggerFactory loggerFactory) {
			this.storeProvider = storeProvider;
			this.identityOptions = identityOptions;
			this.passwordHasher = passwordHasher;
			this.userValidators = userValidators;
			this.passwordValidators = passwordValidators;
			this.keyNormalizer = keyNormalizer;
			this.errors = errors;
			this.services = services;
			this.loggerFactory = loggerFactory;
		}

		public UserManager<TUser> GetUserManager(string tenantId) {
			var store = storeProvider.GetStore(tenantId);
			var logger = loggerFactory.CreateLogger<UserManager<TUser>>();

			return new UserManager<TUser>(store, identityOptions, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger);
		}
	}
}
