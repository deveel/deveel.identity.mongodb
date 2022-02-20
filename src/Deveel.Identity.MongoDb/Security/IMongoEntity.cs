using System;

using MongoDB.Bson;

namespace Deveel.Security {
	public interface IMongoEntity {
		ObjectId Id { get; }

		string TenantId { get; }

		DateTimeOffset CreatedAt { get; }
	}
}
