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

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Deveel.Security {
	/// <summary>
	/// A role entity that can be stored in a MongoDB
	/// database system
	/// </summary>
	public class MongoRole : IMongoEntity {
		/// <summary>
		/// Gets or sets the unique identifier of the role entity
		/// </summary>
		[BsonId]
		public ObjectId Id { get; set; }

		/// <summary>
		/// Gets the unique name of the role
		/// </summary>
		/// <remarks>
		/// In a multi-tenant context, a role with the same
		/// name could be defined by multiple tenants, but
		/// not visible by each other's.
		/// </remarks>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets a normalized version of the role name,
		/// used by the framework for lookups.
		/// </summary>
		public string NormalizedName { get; set; }

		/// <summary>
		/// Gets or sets the identifier of tenant of the role
		/// </summary>
		public string TenantId { get; set; }

		public DateTimeOffset CreatedAt { get; set; }

		public List<MongoClaim> Claims { get; set; } = new List<MongoClaim>();
	}
}
