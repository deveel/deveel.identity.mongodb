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
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Deveel.Security {
	public abstract class MongoDbStoreBase<TEntity> : IDisposable
		where TEntity : class, IMongoEntity {
		private bool _disposed;

		protected MongoDbStoreBase(IOptions<MongoDbStoreOptions> options, ILogger logger)
			: this(options.Value, logger) {
		}

		protected MongoDbStoreBase(IOptions<MongoDbStoreOptions> options)
			: this(options, NullLogger.Instance) {
		}

		protected MongoDbStoreBase(MongoDbStoreOptions options, ILogger logger) {
			Options = options;
			Logger = logger;
		}

		protected MongoDbStoreBase(MongoDbStoreOptions options)
			: this(options, NullLogger.Instance) {
		}

		protected MongoDbStoreOptions Options { get; }

		protected ILogger Logger { get; }

		protected abstract string CollectionName { get; }

		protected string DatabaseName => Options.DatabaseName;

		protected IMongoClient Client {
			get {
				ThrowIfDisposed();
				if (String.IsNullOrWhiteSpace(Options.ConnectionString))
					throw new MongoConfigurationException("The connection string was not set");

				var settings = MongoClientSettings.FromConnectionString(Options.ConnectionString);

				if (Options.UseServerApiV1)
					settings.ServerApi = new ServerApi(ServerApiVersion.V1);

				return new MongoClient(settings);
			}
		}

		protected IMongoDatabase Database {
			get {
				ThrowIfDisposed();

				if (String.IsNullOrWhiteSpace(Options.DatabaseName))
					throw new MongoConfigurationException("The database name was not configured");

				return Client.GetDatabase(Options.DatabaseName);
			}
		}

		protected IMongoCollection<TEntity> Collection {
			get {
				ThrowIfDisposed();
				return Database.GetCollection<TEntity>(CollectionName);
			}
		}

		protected void ThrowIfDisposed() {
			if (_disposed)
				throw new ObjectDisposedException(GetType().FullName, "The store was disposed and cannot be accessed");
		}

		internal void Log(LogLevel level, Exception error, string prefix, params object[] args) {
			if (Logger.IsEnabled(level)) {
				var argc = args?.Length ?? 0;
				var addc = Options.HasTenantSet ? 3 : 2;
				var newArgs = new object[argc + addc];
				Array.Copy(args, newArgs, argc);
				newArgs[argc] = CollectionName;
				newArgs[argc + 1] = DatabaseName;
				if (Options.HasTenantSet)
					newArgs[argc + 2] = Options.TenantId;

				var sb = new StringBuilder(prefix);
				sb.Append(" the collection '{CollectionName}' of database '{DatabaseName}'");
				if (Options.HasTenantSet)
					sb.Append(" for tenant '{TenantId}'");

				Logger.Log(level, error, sb.ToString(), newArgs);
			}
		}

		internal void Trace(string prefix, params object[] args)
			=> Log(LogLevel.Trace, null, prefix, args);

		internal void Error(Exception error, string prefix, params object[] args)
			=> Log(LogLevel.Error, error, prefix, args);

		internal void Warning(string prefix, params object[] args)
			=> Log(LogLevel.Warning, null, prefix, args);

		// Implements the pattern that sets a property in a MongoUser
		internal Task SetAsync(Action action, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			action();

			return Task.CompletedTask;
		}

		internal Task<TValue> GetAsync<TValue>(Func<TValue> func, CancellationToken cancellationToken) {
			ThrowIfDisposed();
			cancellationToken.ThrowIfCancellationRequested();

			var value = func();

			return Task.FromResult(value);
		}


		public virtual void Dispose() {
			if (!_disposed) {
				_disposed = true;
			}
		}

		protected FilterDefinition<TEntity> NormalizeFilter(FilterDefinition<TEntity> filter) {
			if (Options.HasTenantSet) {
				var tenantFilter = Builders<TEntity>.Filter.Eq(x => x.TenantId, Options.TenantId);

				filter = Builders<TEntity>.Filter.And(tenantFilter, filter);
			}

			return filter;
		}

		protected FilterDefinition<TEntity> IdFilter(ObjectId id)
			=> NormalizeFilter(NormalizeFilter(Builders<TEntity>.Filter.Eq(x => x.Id, id)));
	}
}
