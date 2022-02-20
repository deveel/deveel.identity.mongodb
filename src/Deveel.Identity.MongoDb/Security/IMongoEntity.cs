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

using MongoDB.Bson;

namespace Deveel.Security {
	/// <summary>
	/// The contract of an entity that can be stored
	/// in a MongoDV store
	/// </summary>
	public interface IMongoEntity {
		/// <summary>
		/// Gets the unique identifier of the entity
		/// </summary>
		ObjectId Id { get; }

		/// <summary>
		/// Optionally gets the identifier of the tenant owning
		/// the entity (if in a multi-tenant context).
		/// </summary>
		string TenantId { get; }

		/// <summary>
		/// Gets the time-stamp that indicates when the entity
		/// was created.
		/// </summary>
		DateTimeOffset CreatedAt { get; }
	}
}
