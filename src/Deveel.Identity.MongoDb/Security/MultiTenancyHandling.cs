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

namespace Deveel.Security {
	/// <summary>
	/// Enumerates the possible handling methodologies
	/// for multi-tenant contexts.
	/// </summary>
	public enum MultiTenancyHandling {
		/// <summary>
		/// Multi-tenancy is disabled
		/// </summary>
		None = 0,

		/// <summary>
		/// Tenant entities are identified by the 'TenantId'
		/// property value
		/// </summary>
		TenantField = 1,

		/// <summary>
		/// Every tenant has its own segregated database within
		/// the hosting cluster
		/// </summary>
		TenantDatabase = 2,

		/// <summary>
		/// The users and roles entities are stored in collections
		/// specific to the tenant
		/// </summary>
		TenantCollection = 3
	}
}
