﻿
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RestClient.Net.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace RestClient.Net
{
    public class ClientFactory
    {
        #region Fields
        private readonly Func<string, Lazy<IClient>> createClientFunc;
        private readonly ConcurrentDictionary<string, Lazy<IClient>> clients;
        private readonly CreateHttpClient createHttpClient;
        private readonly ILoggerFactory loggerFactory;
        #endregion

        #region Public Properties
        public ISerializationAdapter SerializationAdapter { get; }
        #endregion

        #region Constructor
        public ClientFactory(
            CreateHttpClient createHttpClient,
#if !NET45
            ISerializationAdapter? serializationAdapter = null,
#else
            ISerializationAdapter serializationAdapter,
#endif
            ILoggerFactory? loggerFactory = null)
        {
#if !NET45
            SerializationAdapter = serializationAdapter ?? new JsonSerializationAdapter();
#else
            SerializationAdapter = serializationAdapter;
#endif
            this.createHttpClient = createHttpClient;
            this.loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

            clients = new ConcurrentDictionary<string, Lazy<IClient>>();

            createClientFunc = (name) => new Lazy<IClient>(() => MintClient(name), LazyThreadSafetyMode.ExecutionAndPublication);
        }
        #endregion

        #region Implementation
        public IClient CreateClient(string name)
            => name == null ? throw new ArgumentNullException(nameof(name)) : clients.GetOrAdd(name, createClientFunc(name)).Value;
        #endregion

        #region Private Methods
        private IClient MintClient(string name, Uri? baseUri = null) =>
            new Client(
                SerializationAdapter,
                baseUri,
                logger: loggerFactory?.CreateLogger<Client>(),
                createHttpClient: createHttpClient,
                name: name);
        #endregion
    }
}