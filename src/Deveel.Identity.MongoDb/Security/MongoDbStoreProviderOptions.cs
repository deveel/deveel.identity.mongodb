using System;

namespace Deveel.Security {
	public class MongoDbStoreProviderOptions : MongoDbOptions {
		public MongoDbMultiTenancy MultiTenancy { get; set; } = new MongoDbMultiTenancy();
	}
}
