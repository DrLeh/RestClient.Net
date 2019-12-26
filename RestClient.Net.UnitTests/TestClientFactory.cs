﻿using RestClientDotNet.Abstractions;
using System;
using System.Net.Http;

namespace RestClientDotNet.UnitTests
{
    public class TestClientFactory : IHttpClientFactory
    {
        HttpClient _testClient;

        public TestClientFactory(HttpClient testClient)
        {
            _testClient = testClient;
        }

        public TimeSpan Timeout { get => _testClient.Timeout; set => _testClient.Timeout = value; }

        public IRestHeaders DefaultRequestHeaders => new RestHttpRequestHeaders(_testClient.DefaultRequestHeaders);

        public HttpClient CreateClient(string name)
        {
            return _testClient;
        }

        public void Dispose()
        {
            _testClient.Dispose();
        }
    }
}
