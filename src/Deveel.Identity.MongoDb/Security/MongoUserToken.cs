using System;

namespace Deveel.Security {
	public class MongoUserToken {
		public string Provider { get; set; }

		public string TokenName { get; set; }

		public string Token { get; set; }
	}
}
