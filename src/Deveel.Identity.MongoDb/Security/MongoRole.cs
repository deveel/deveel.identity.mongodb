using System;
using System.Collections.Generic;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Deveel.Security {
	public class MongoRole : IMongoEntity {
		[BsonId]
		public ObjectId Id { get; set; }

		public string Name { get; set; }

		public string NormalizedName { get; set; }

		public string TenantId { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public List<MongoClaim> Claims { get; set; } = new List<MongoClaim>();
	}
}
