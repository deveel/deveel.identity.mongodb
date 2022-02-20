using System;

namespace Deveel.Security {
	public class MongoDbMultiTenancy {
		public MultiTenancyHandling Handling { get; set; } = MultiTenancyHandling.None;

		public string DatabaseFormat { get; set; } = "{tenant}_{database}";

		public string CollectionFormat { get; set; } = "{tenant}_{collection}";
	}
}
