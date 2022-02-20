using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Deveel.Security {
	public class MongoDbRoleStore<TRole> : MongoDbStoreBase<TRole>,
		IRoleStore<TRole>,
		IRoleClaimStore<TRole>,
		IQueryableRoleStore<TRole>
		where TRole : MongoRole {
		public MongoDbRoleStore(IOptions<MongoDbStoreOptions> options) : base(options) {
		}

		public MongoDbRoleStore(IOptions<MongoDbStoreOptions> options, ILogger<MongoDbRoleStore<TRole>> logger) 
			: base(options, logger) {
		}

		public MongoDbRoleStore(MongoDbStoreOptions options, ILogger<MongoDbRoleStore<TRole>> logger) 
			: base(options, logger) {
		}

		public MongoDbRoleStore(MongoDbStoreOptions options) : base(options) {
		}

		protected override string CollectionName => Options.RolesCollection;

		public IQueryable<TRole> Roles => Collection.AsQueryable<TRole>();

		private Task<TRole> FindAsync(Expression<Func<TRole, bool>> exp, CancellationToken cancellationToken)
			=> FindAsync(new ExpressionFilterDefinition<TRole>(exp), cancellationToken);

		private async Task<TRole> FindAsync(FilterDefinition<TRole> filter, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();
			
			try {
				Trace("Trying to find a role in");
				
				var options = new FindOptions<TRole, TRole> { Limit = 1 };
				var result = await Collection.FindAsync(NormalizeFilter(filter), options, cancellationToken);

				var role = await result.FirstOrDefaultAsync(cancellationToken);

				if (role != null) {
					Trace("A role was found in");

				} else {
					Trace("None role was found in");
				}

				return role;
			} catch (Exception ex) {
				Error(ex, "The role could not be looked up in");

				throw new MongoException("Error while looking up for the role", ex);
			}
		}

		public async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			try {
				Trace("Creating a new role in");

				role.CreatedAt = DateTimeOffset.UtcNow;

				if (Options.HasTenantSet)
					role.TenantId = Options.TenantId;

				var options = new InsertOneOptions { BypassDocumentValidation = true };
				await Collection.InsertOneAsync(role, options, cancellationToken);

				Trace("New role with ID '{UserId}' created in", role.Id);

				return IdentityResult.Success;
			} catch (Exception ex) {
				Error(ex, "Could not create a new role in");

				return IdentityResult.Failed(new IdentityError { 
					Code = MongoDbStoreErrorCodes.UnknownError, 
					Description = "The storage system failed persisting the role" 
				});
			}
		}

		public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			try {
				Trace("Updating the role with ID '{RoleId}' in", role.Id);

				var options = new ReplaceOptions { IsUpsert = false };
				var result = await Collection.ReplaceOneAsync(IdFilter(role.Id), role, options, cancellationToken);

				if (result.MatchedCount == 0) {
					Warning("The role with ID '{RoleId}' was not found in", role.Id);

					return IdentityResult.Failed(new IdentityError {
						Code = MongoDbStoreErrorCodes.RoleNotFound,
						Description = $"The role with ID {role.Id} was not found and could not be updated"
					});
				}

				if (result.ModifiedCount == 0) {
					Warning("The role with ID '{RoleId}' was not modified in", role.Id);

					return IdentityResult.Failed(new IdentityError {
						Code = MongoDbStoreErrorCodes.RoleNotModified,
						Description = $"The role with ID {role.Id} was not updated"
					});
				}

				Trace("The role with ID '{RoleId}' was successfully updated in", role.Id);

				return IdentityResult.Success;
			} catch (Exception ex) {
				Error(ex, "The role with ID '{UserId}' was not updated in",role.Id);

				return IdentityResult.Failed(new IdentityError {
					Code = MongoDbStoreErrorCodes.UnknownError,
					Description = "Could not update the role in the storage system"
				});
			}
		}

		public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken) {
			if (role is null)
				throw new ArgumentNullException(nameof(role));

			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			try {
				Trace("Deleting the role with ID '{RoleId}' from", role.Id);

				var result = await Collection.DeleteOneAsync(IdFilter(role.Id), cancellationToken);
				if (result.DeletedCount == 0) {
					Warning("Inconsistent delete: role with ID '{UserId}' was not removed from", role.Id);

					return IdentityResult.Failed(new IdentityError {
						Code = MongoDbStoreErrorCodes.RoleNotFound,
						Description = "The role was not deleted from the storage"
					});
				} else if (result.DeletedCount == 1) {
					Trace("Role with ID '{RoleId}' was successfully deleted from", role.Id);
				} else if (result.DeletedCount > 1) {
					Trace("Incosistent delete: more than one role deleted while trying to remove the role with ID '{RoleId}' from", role.Id);
				}

				return IdentityResult.Success;
			} catch (Exception ex) {
				Error(ex, "The role '{RoleId}' was not deleted from", role.Id);

				return IdentityResult.Failed(new IdentityError {
					Code = MongoDbStoreErrorCodes.UnknownError,
					Description = "Could not delete the role from the storage system"
				});
			}

		}

		public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken) 
			=> GetAsync(() => role.Id == ObjectId.Empty ? null : role.Id.ToString(), cancellationToken);

		public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken) 
			=> GetAsync(() => role.Name, cancellationToken);

		public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
			=> SetAsync(() => role.Name = roleName, cancellationToken);

		public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
			=> GetAsync(() => role.NormalizedName, cancellationToken);

		public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken) 
			=> SetAsync(() => role.NormalizedName = normalizedName, cancellationToken);

		public async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken) {
			if (!ObjectId.TryParse(roleId, out var id))
				throw new ArgumentException($"The provided user ID {roleId} is not in a valid 24-digits format");

			Trace("Trying to find a role with ID '{RoleId}' in", roleId);

			return await FindAsync(x => x.Id == id, cancellationToken);
		}

		public async Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken) {
			Trace("Trying to find a role named '{RoleName}' in", normalizedRoleName);

			return await FindAsync(role => role.NormalizedName == normalizedRoleName, cancellationToken);
		}

		public Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken)
			=> GetAsync(() => (IList<Claim>) role.Claims?.Select(x => x.ToClaim()).ToList(), cancellationToken);

		public Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
			=> SetAsync(() => {
				if (role.Claims == null)
					role.Claims = new List<MongoClaim>();

				var oldClaim = role.Claims.FirstOrDefault(x => x.Type == claim.Type);
				if (oldClaim == null)
					role.Claims.Add(MongoClaim.Create(claim));
			}, cancellationToken);

		public Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
			=> SetAsync(() => {
				var oldClaim = role.Claims?.FirstOrDefault(x => x.Type == claim.Type);
				if (oldClaim != null)
					role.Claims.Remove(oldClaim);
			}, cancellationToken);
	}
}
