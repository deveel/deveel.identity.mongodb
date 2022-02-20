using System;

namespace Deveel.Security {
	public static class MongoDbStoreErrorCodes {
		public const string UnknownError = "MONGO-0100";
		public const string UserNotFound = "MONGO-0301";
		public const string UserNotModified = "MONGO-0302";
		public const string RoleNotFound = "MONGO-0401";
		public const string RoleNotModified = "MONGO-0402";
	}
}
