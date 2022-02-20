using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Deveel.Security {
	public class MongoUser : IMongoEntity {
		public string TenantId { get; set; }

		public string Email { get; set; }

		public bool EmailConfirmed { get; set; }

		public string Name { get; set; }

		public string PasswordHash { get; set; }

		public string Phone { get; set; }

		public bool PhoneConfirmed { get; set; }

		[BsonId]
		public ObjectId Id { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public string NormalizedName { get; set; }

		public string NormalizedEmail { get; set; }

		public bool LockoutEnabled { get; set; }

		public DateTimeOffset? LockoutEnd { get; set; }

		public int? AccessFailedCount { get; set; }

		public List<MongoUserLogin> Logins { get; set; } = new List<MongoUserLogin>();

		public bool TwoFactorsEnabled { get; set; }
		
		public string SecurityStamp { get; set; }

		public List<string> RecoveryCodes { get; set; } = new List<string>();

		public List<string> Roles { get; set; } = new List<string>();

		public List<MongoClaim> Claims { get; set; } = new List<MongoClaim>();

		public string AuthenticationKey { get; set; }

		public List<MongoUserToken> Tokens { get; set; } = new List<MongoUserToken>();
	}
}
