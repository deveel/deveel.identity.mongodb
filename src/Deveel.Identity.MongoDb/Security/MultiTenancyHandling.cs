using System;

namespace Deveel.Security {
	public enum MultiTenancyHandling {
		None = 0,
		TenantField = 1,
		TenantDatabase = 2,
		TenantCollection = 3
	}
}
