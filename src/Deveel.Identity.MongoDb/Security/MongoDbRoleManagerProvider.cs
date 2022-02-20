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
