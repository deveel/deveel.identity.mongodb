using System;

namespace Deveel.Security {
	public class MongoDbOptions {
		public string ConnectionString { get; set; }

		public string DatabaseName { get; set; }

		public string RolesCollection { get; set; } = "roles";

		public string UsersCollection { get; set; } = "users";
	}
}
