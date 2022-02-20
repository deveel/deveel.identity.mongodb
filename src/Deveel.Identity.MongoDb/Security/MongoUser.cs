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
	/// A user entity that can be stored in a MongoDB
	/// database system
	/// </summary>
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
