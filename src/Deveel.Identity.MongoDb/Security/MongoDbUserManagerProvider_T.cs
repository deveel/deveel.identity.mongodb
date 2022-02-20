using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
