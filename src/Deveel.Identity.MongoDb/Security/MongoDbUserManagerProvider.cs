using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Deveel.Security {
	public class MongoDbUserManagerProvider : MongoDbUserManagerProvider<MongoUser> {
		public MongoDbUserManagerProvider(IMongoDbUserStoreProvider<MongoUser> storeProvider, 
			IOptions<IdentityOptions> identityOptions, 
			IPasswordHasher<MongoUser> passwordHasher, 
			IEnumerable<IUserValidator<MongoUser>> userValidators, 
			IEnumerable<IPasswordValidator<MongoUser>> passwordValidators, 
			ILookupNormalizer keyNormalizer, 
			IdentityErrorDescriber errors, 
			IServiceProvider services, 
			ILoggerFactory loggerFactory) : base(storeProvider, identityOptions, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, loggerFactory) {
		}
	}
}
