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
using System.Security.Claims;

namespace Deveel.Security {
	/// <summary>
	/// The representation of a claim that can be
	/// stored in a MongoDB collection
	/// </summary>
	/// <remarks>
	/// Claims are useful to extend the information of
	/// users and roles, without altering the provided
	/// entities (<see cref="MongoUser"/> and <see cref="MongoRole" />)
	/// </remarks>
	public class MongoClaim {
		/// <summary>
		/// Gets or sets the type of the claim
		/// </summary>
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets the value of the claim
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the type of value handled by the claim
		/// </summary>
		/// <remarks>
		/// When this value is not set, the system implies the
		/// type of value is a string.
		/// </remarks>
		public string ValueType { get; set; }

		/// <summary>
		/// A simple factory method to create a claim
		/// </summary>
		/// <param name="claim">Te source claim used to construct another one</param>
		/// <returns>
		/// Returns an instance of <see cref="MongoClaim"/> that holds the
		/// type and value o the source 
		/// </returns>
		public static MongoClaim Create(Claim claim)
			=> new MongoClaim {
				Type = claim.Type,
				Value = claim.Value,
				ValueType = claim.ValueType
			};

		public Claim ToClaim()
			=> new Claim(Type, Value, ValueType);
	}
}