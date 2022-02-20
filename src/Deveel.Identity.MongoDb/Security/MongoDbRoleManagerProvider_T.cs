using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Deveel.Security {
	public class MongoDbRoleManagerProvider<TRole> : IMongoDbRoleManagerProvider<TRole> where TRole : MongoRole {
		private readonly IMongoDbRoleStoreProvider<TRole> storeProvider;
		private readonly IEnumerable<IRoleValidator<TRole>> roleValidators;
		private readonly ILookupNormalizer keyNormalizer;
		private readonly IdentityErrorDescriber errors;
		private readonly ILoggerFactory loggerFactory;

		public MongoDbRoleManagerProvider(IMongoDbRoleStoreProvider<TRole> storeProvider, 
			IEnumerable<IRoleValidator<TRole>> roleValidators, 
			ILookupNormalizer keyNormalizer, 
			IdentityErrorDescriber errors, 
			ILoggerFactory loggerFactory) {
			this.storeProvider = storeProvider;
			this.roleValidators = roleValidators;
			this.keyNormalizer = keyNormalizer;
			this.errors = errors;
			this.loggerFactory = loggerFactory;
		}

		public RoleManager<TRole> GetRoleManager(string tenantId) {
			var store = storeProvider.GetStore(tenantId);
			var logger = loggerFactory.CreateLogger<RoleManager<TRole>>();

			return new RoleManager<TRole>(store, roleValidators, keyNormalizer, errors, logger);
		}
	}
}
