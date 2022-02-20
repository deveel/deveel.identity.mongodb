using System;
using System.Security.Claims;

namespace Deveel.Security {
	public class MongoClaim {
		public string Type { get; set; }

		public string Value { get; set; }

		public string ValueType { get; set; }

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