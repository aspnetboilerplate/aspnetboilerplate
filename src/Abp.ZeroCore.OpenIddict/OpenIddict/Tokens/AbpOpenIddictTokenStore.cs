using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Domain.Uow;
using Abp.OpenIddict.Applications;
using Abp.OpenIddict.Authorizations;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;

namespace Abp.OpenIddict.Tokens
{
    public class AbpOpenIddictTokenStore : AbpOpenIddictStoreBase<IOpenIddictTokenRepository>,
        IOpenIddictTokenStore<OpenIddictTokenModel>
    {
        protected IOpenIddictApplicationRepository ApplicationRepository { get; }
        protected IOpenIddictAuthorizationRepository AuthorizationRepository { get; }

        public AbpOpenIddictTokenStore(
            IOpenIddictTokenRepository repository,
            IUnitOfWorkManager unitOfWorkManager,
            IGuidGenerator guidGenerator,
            IOpenIddictApplicationRepository applicationRepository,
            IOpenIddictAuthorizationRepository authorizationRepository,
            IOpenIddictDbConcurrencyExceptionHandler concurrencyExceptionHandler)
            : base(repository, unitOfWorkManager, guidGenerator, concurrencyExceptionHandler)
        {
            ApplicationRepository = applicationRepository;
            AuthorizationRepository = authorizationRepository;
        }

        public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        {
            return await Repository.CountAsync();
        }

        public virtual ValueTask<long> CountAsync<TResult>(
            Func<IQueryable<OpenIddictTokenModel>, IQueryable<TResult>> query, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual async ValueTask CreateAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await Repository.InsertAsync(token.ToEntity());
                token = (await Repository.FindByIdAsync(token.Id, cancellationToken)).ToModel();
            });
        }

        public virtual async ValueTask DeleteAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            try
            {
                await Repository.DeleteAsync(token.ToEntity());
            }
            catch (AbpDbConcurrencyException e)
            {
                Logger.LogError(e, e.Message);
                await ConcurrencyExceptionHandler.HandleAsync(e);
                throw new OpenIddictExceptions.ConcurrencyException(e.Message, e.InnerException);
            }
        }

        public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindAsync(string subject, string client,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(subject, nameof(subject));
            Check.NotNullOrEmpty(client, nameof(client));

            var tokens = await Repository.FindAsync(subject, Guid.Parse(client), cancellationToken);
            foreach (var token in tokens)
            {
                yield return token.ToModel();
            }
        }

        public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindAsync(string subject, string client,
            string status, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(subject, nameof(subject));
            Check.NotNullOrEmpty(client, nameof(client));
            Check.NotNullOrEmpty(status, nameof(status));

            var tokens = await Repository.FindAsync(subject, Guid.Parse(client), status,
                cancellationToken);
            foreach (var token in tokens)
            {
                yield return token.ToModel();
            }
        }

        public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindAsync(string subject, string client,
            string status, string type, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(subject, nameof(subject));
            Check.NotNullOrEmpty(client, nameof(client));
            Check.NotNullOrEmpty(status, nameof(status));
            Check.NotNullOrEmpty(type, nameof(type));

            var tokens = await Repository.FindAsync(subject, Guid.Parse(client), status, type,
                cancellationToken);
            foreach (var token in tokens)
            {
                yield return token.ToModel();
            }
        }

        public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindByApplicationIdAsync(string identifier,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(identifier, nameof(identifier));

            var tokens = await Repository.FindByApplicationIdAsync(Guid.Parse(identifier), cancellationToken);
            foreach (var token in tokens)
            {
                yield return token.ToModel();
            }
        }

        public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindByAuthorizationIdAsync(string identifier,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(identifier, nameof(identifier));

            var tokens =
                await Repository.FindByAuthorizationIdAsync(Guid.Parse(identifier), cancellationToken);
            foreach (var token in tokens)
            {
                yield return token.ToModel();
            }
        }

        public virtual async ValueTask<OpenIddictTokenModel> FindByIdAsync(string identifier,
            CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(identifier, nameof(identifier));

            return (await Repository.FindByIdAsync(Guid.Parse(identifier), cancellationToken))
                .ToModel();
        }

        public virtual async ValueTask<OpenIddictTokenModel> FindByReferenceIdAsync(string identifier,
            CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(identifier, nameof(identifier));

            return (await Repository.FindByReferenceIdAsync(identifier, cancellationToken)).ToModel();
        }

        public virtual async IAsyncEnumerable<OpenIddictTokenModel> FindBySubjectAsync(string subject,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(subject, nameof(subject));

            var tokens = await Repository.FindBySubjectAsync(subject, cancellationToken);
            foreach (var token in tokens)
            {
                yield return token.ToModel();
            }
        }

        public virtual ValueTask<string> GetApplicationIdAsync(OpenIddictTokenModel token,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string>(token.ApplicationId?.ToString());
        }

        public virtual ValueTask<TResult> GetAsync<TState, TResult>(
            Func<IQueryable<OpenIddictTokenModel>, TState, IQueryable<TResult>> query, TState state,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual ValueTask<string> GetAuthorizationIdAsync(OpenIddictTokenModel token,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string>(token.AuthorizationId?.ToString());
        }

        public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIddictTokenModel token,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            if (token.CreationDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.CreationDate.Value, DateTimeKind.Utc));
        }

        public virtual ValueTask<DateTimeOffset?> GetExpirationDateAsync(OpenIddictTokenModel token,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            if (token.ExpirationDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.ExpirationDate.Value, DateTimeKind.Utc));
        }

        public virtual ValueTask<string> GetIdAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string>(token.Id.ToString());
        }

        public virtual ValueTask<string> GetPayloadAsync(OpenIddictTokenModel token,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string>(token.Payload);
        }

        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(
            OpenIddictTokenModel token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            if (string.IsNullOrEmpty(token.Properties))
            {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary
                    .Create<string, JsonElement>());
            }

            using (var document = JsonDocument.Parse(token.Properties))
            {
                var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    builder[property.Name] = property.Value.Clone();
                }

                return new ValueTask<ImmutableDictionary<string, JsonElement>>(builder.ToImmutable());
            }
        }

        public virtual ValueTask<DateTimeOffset?> GetRedemptionDateAsync(OpenIddictTokenModel token,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            if (token.RedemptionDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.RedemptionDate.Value, DateTimeKind.Utc));
        }

        public virtual ValueTask<string> GetReferenceIdAsync(OpenIddictTokenModel token,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string>(token.ReferenceId);
        }

        public virtual ValueTask<string> GetStatusAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string>(token.Status);
        }

        public virtual ValueTask<string> GetSubjectAsync(OpenIddictTokenModel token,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string>(token.Subject);
        }

        public virtual ValueTask<string> GetTypeAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            return new ValueTask<string>(token.Type);
        }

        public virtual ValueTask<OpenIddictTokenModel> InstantiateAsync(CancellationToken cancellationToken)
        {
            return new ValueTask<OpenIddictTokenModel>(new OpenIddictTokenModel
            {
                Id = GuidGenerator.Create()
            });
        }

        public virtual async IAsyncEnumerable<OpenIddictTokenModel> ListAsync(int? count, int? offset,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var tokens = await Repository.ListAsync(count, offset, cancellationToken);
            foreach (var token in tokens)
            {
                yield return token.ToModel();
            }
        }

        public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
            Func<IQueryable<OpenIddictTokenModel>, TState, IQueryable<TResult>> query, TState state,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual async ValueTask<long> PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
        {
            using (var uow = UnitOfWorkManager.Begin(new UnitOfWorkOptions()
                   {
                       Scope = TransactionScopeOption.RequiresNew,
                       IsTransactional = true,
                       IsolationLevel = IsolationLevel.RepeatableRead
                   }))
            {
                var date = threshold.UtcDateTime;
                var count = await Repository.PruneAsync(date, cancellationToken: cancellationToken);
                await uow.CompleteAsync();
                return count;
            }
        }

        public async ValueTask<long> RevokeAsync(string subject, string client, string status, string type,
            CancellationToken cancellationToken)
        {
            using (var uow = UnitOfWorkManager.Begin(new UnitOfWorkOptions()
                   {
                       Scope = TransactionScopeOption.RequiresNew,
                       IsTransactional = true,
                       IsolationLevel = IsolationLevel.RepeatableRead
                   }))
            {
                var count = await Repository.RevokeBySubjectAsync(
                    subject,
                    cancellationToken: cancellationToken
                );

                await uow.CompleteAsync();
                return count;
            }
        }

        public async ValueTask<long> RevokeByApplicationIdAsync(string identifier, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkManager.Begin(new UnitOfWorkOptions()
                   {
                       Scope = TransactionScopeOption.RequiresNew,
                       IsTransactional = true,
                       IsolationLevel = IsolationLevel.RepeatableRead
                   }))
            {
                var count = await Repository.RevokeByApplicationIdAsync(ConvertIdentifierFromString(identifier), cancellationToken: cancellationToken);
                await uow.CompleteAsync();
                return count;
            }
        }

        public async ValueTask<long> RevokeByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            using (var uow = UnitOfWorkManager.Begin(new UnitOfWorkOptions()
                   {
                       Scope = TransactionScopeOption.RequiresNew,
                       IsTransactional = true,
                       IsolationLevel = IsolationLevel.RepeatableRead
                   }))
            {
                var count = await Repository.RevokeByAuthorizationIdAsync(ConvertIdentifierFromString(identifier), cancellationToken: cancellationToken);
                await uow.CompleteAsync();
                return count;
            }
        }

        public virtual async ValueTask<long> RevokeBySubjectAsync(string subject, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkManager.Begin(new UnitOfWorkOptions()
                   {
                       Scope = TransactionScopeOption.RequiresNew,
                       IsTransactional = true,
                       IsolationLevel = IsolationLevel.RepeatableRead
                   }))
            {
                var count = await Repository.RevokeBySubjectAsync(subject, cancellationToken: cancellationToken);
                await uow.CompleteAsync();
                return count;
            }
        }

        public virtual async ValueTask SetApplicationIdAsync(OpenIddictTokenModel token, string identifier,
            CancellationToken cancellationToken)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                Check.NotNull(token, nameof(token));

                if (!string.IsNullOrEmpty(identifier))
                {
                    var application = await ApplicationRepository.GetAsync(Guid.Parse(identifier));
                    token.ApplicationId = application.Id;
                }
                else
                {
                    token.ApplicationId = null;
                }
            });
        }

        public virtual async ValueTask SetAuthorizationIdAsync(OpenIddictTokenModel token, string identifier,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (!string.IsNullOrEmpty(identifier))
                {
                    var authorization = await AuthorizationRepository.GetAsync(Guid.Parse(identifier));
                    token.AuthorizationId = authorization.Id;
                }
                else
                {
                    token.AuthorizationId = null;
                }
            });
        }

        public virtual ValueTask SetCreationDateAsync(OpenIddictTokenModel token, DateTimeOffset? date,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.CreationDate = date?.UtcDateTime;

            return default;
        }

        public virtual ValueTask SetExpirationDateAsync(OpenIddictTokenModel token, DateTimeOffset? date,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.ExpirationDate = date?.UtcDateTime;

            return default;
        }

        public virtual ValueTask SetPayloadAsync(OpenIddictTokenModel token, string payload,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.Payload = payload;

            return default;
        }

        public virtual ValueTask SetPropertiesAsync(OpenIddictTokenModel token,
            ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            if (properties is null || properties.IsEmpty)
            {
                token.Properties = null;
                return default;
            }

            token.Properties = WriteStream(writer =>
            {
                writer.WriteStartObject();
                foreach (var property in properties)
                {
                    writer.WritePropertyName(property.Key);
                    property.Value.WriteTo(writer);
                }

                writer.WriteEndObject();
            });

            return default;
        }

        public virtual ValueTask SetRedemptionDateAsync(OpenIddictTokenModel token, DateTimeOffset? date,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.RedemptionDate = date?.UtcDateTime;

            return default;
        }

        public virtual ValueTask SetReferenceIdAsync(OpenIddictTokenModel token, string identifier,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.ReferenceId = identifier;

            return default;
        }

        public virtual ValueTask SetStatusAsync(OpenIddictTokenModel token, string status,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.Status = status;

            return default;
        }

        public virtual ValueTask SetSubjectAsync(OpenIddictTokenModel token, string subject,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.Subject = subject;

            return default;
        }

        public virtual ValueTask SetTypeAsync(OpenIddictTokenModel token, string type,
            CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            token.Type = type;

            return default;
        }

        public virtual async ValueTask UpdateAsync(OpenIddictTokenModel token, CancellationToken cancellationToken)
        {
            Check.NotNull(token, nameof(token));

            var entity = await UnitOfWorkManager.WithUnitOfWorkAsync(async () => await Repository.GetAsync(token.Id));

            try
            {
                await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
                {
                    await Repository.UpdateAsync(token.ToEntity(entity));
                });
            }
            catch (AbpDbConcurrencyException e)
            {
                Logger.LogError(e, e.Message);
                await ConcurrencyExceptionHandler.HandleAsync(e);
                throw new OpenIddictExceptions.ConcurrencyException(e.Message, e.InnerException);
            }

            token = await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
                (await Repository.FindByIdAsync(entity.Id, cancellationToken: cancellationToken)).ToModel()
            );
        }
    }
}