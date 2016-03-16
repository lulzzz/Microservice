﻿#region using

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This persistence handler uses Azure Blob storage as its underlying storage mechanism.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceMessageHandlerAzureBlobStorageBase<K, E> : PersistenceManagerHandlerJsonBase<K, E, PersistenceStatistics, PersistenceCommandPolicy>
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This is the azure storage wrapper.
        /// </summary>
        protected StorageServiceBase mStorage;

        protected Func<K, string> mIdMaker;

        protected string mDirectory;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="credentials">The azure storage credentials.</param>
        /// <param name="entityName">The options entity name. If this is not presented then the entity name will be used.</param>
        /// <param name="versionPolicy">The versioning policy.</param>
        /// <param name="defaultTimeout">The default timeout for async requests.</param>
        /// <param name="accessType">The azure access type. BlobContainerPublicAccessType.Off is the default.</param>
        /// <param name="options">The optional blob request options.</param>
        /// <param name="context">The optional operation context.</param>
        /// <param name="retryPolicy">Persistence retry policy</param>
        public PersistenceMessageHandlerAzureBlobStorageBase(StorageCredentials credentials
            , Func<E, K> keyMaker
            , Func<K, string> idMaker
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off
            , BlobRequestOptions options = null
            , OperationContext context = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<RepositoryHolder<K, E>, JsonHolder<K>> jsonMaker = null
            )
            : base(entityName:entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy:persistenceRetryPolicy
                  , resourceProfile:resourceProfile
                  , cacheManager: cacheManager
                  , keyMaker: keyMaker
                  , referenceMaker:referenceMaker
                  , jsonMaker: jsonMaker)
        {
            mDirectory = entityName ?? typeof(E).Name;
            mStorage = new StorageServiceBase(credentials, "persistence", accessType, options, context, defaultTimeout: defaultTimeout);
            mIdMaker = idMaker;
        }
        #endregion

        protected override string KeyStringMaker(K key)
        {
            return mIdMaker(key);
        }

        #region StartInternal()
        /// <summary>
        /// This override starts the storage agent.
        /// </summary>
        protected override void StartInternal()
        {
            base.StartInternal();
            mStorage.Start();
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This override stops the storage agent.
        /// </summary>
        protected override void StopInternal()
        {
            mStorage.Stop();
            base.StopInternal();
        }
        #endregion

        private PersistenceResponseHolder<E> PersistenceResponseFormat(StorageResponseHolder result)
        {
            if (result.IsSuccess)
                return new PersistenceResponseHolder<E>() { StatusCode = result.StatusCode, Content = result.Content, IsSuccess = true, Entity = mTransform.EntityDeserializer(result.Content) };
            else
                return new PersistenceResponseHolder<E>() { StatusCode = result.IsTimeout ? 504 : result.StatusCode, IsSuccess = false, IsTimeout = result.IsTimeout };
        }

        #region InternalCreate
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task<IResponseHolder<E>> InternalCreate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var jsonHolder = JsonMaker(rq);
            var blob = Encoding.UTF8.GetBytes(jsonHolder.Json);

            var result = await mStorage.Create(jsonHolder.Id, blob
                , contentType: "application/json; charset=utf-8"
                , version: jsonHolder.Version, directory: mDirectory);

            return PersistenceResponseFormat(result);
        }
        #endregion
        #region InternalRead
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task<IResponseHolder<E>> InternalRead(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var result = await mStorage.Read(mIdMaker(rq.Key), directory: mDirectory);

            return PersistenceResponseFormat(result);
        }
        #endregion
        #region InternalUpdate
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task<IResponseHolder<E>> InternalUpdate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var jsonHolder = JsonMaker(rq);
            var blob = Encoding.UTF8.GetBytes(jsonHolder.Json);

            var result = await mStorage.Update(jsonHolder.Id, blob
                , contentType: "application/json; charset=utf-8"
                , version: jsonHolder.Version, directory: mDirectory);

            return PersistenceResponseFormat(result);
        }
        #endregion
        #region InternalDelete
        /// <summary>
        /// Delete request.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task<IResponseHolder> InternalDelete(K key, PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var result = await mStorage.Delete(mIdMaker(rq.Key), directory: mDirectory);

            return PersistenceResponseFormat(result);
        }
        #endregion
        #region InternalVersion
        /// <summary>
        /// Version.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The response.</param>
        /// <param name="prq">The incoming payload.</param>
        /// <param name="prs">The outgoing payload.</param>
        protected override async Task<IResponseHolder> InternalVersion(K key, PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            var result = await mStorage.Version(mIdMaker(rq.Key), directory: mDirectory);

            return PersistenceResponseFormat(result);
        }

        #endregion
    }
}
