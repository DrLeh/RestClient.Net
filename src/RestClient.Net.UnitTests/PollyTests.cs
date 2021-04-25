﻿

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using RestClient.Net.Abstractions;
using RestClient.Net.DependencyInjection;
using RestClient.Net.UnitTests.Model;
using RestClientApiSamples;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Urls;

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

namespace RestClient.Net.UnitTests
{
    //It sucks that we have to create a class in this way. The old version was far less verbose. 
    //TODO: Look in to another way to achieve this

    public class PollySendHttpRequestMessage : ISendHttpRequestMessage
    {
        private readonly AsyncRetryPolicy<HttpResponseMessage> policy;

        public PollySendHttpRequestMessage(AsyncRetryPolicy<HttpResponseMessage> policy) => this.policy = policy;

        public int Tries { get; private set; }

        public void Dispose() { }

        public Task<HttpResponseMessage> SendHttpRequestMessage<TRequestBody>(
            IGetHttpRequestMessage httpRequestMessageFunc,
            IRequest<TRequestBody> request,
            ILogger logger,
            ISerializationAdapter serializationAdapter) =>
            policy.ExecuteAsync(() =>
            {
                if (httpRequestMessageFunc == null) throw new ArgumentNullException(nameof(httpRequestMessageFunc));
                if (request == null) throw new ArgumentNullException(nameof(request));

                var httpClient = new HttpClient();

                var httpRequestMessage = httpRequestMessageFunc.GetHttpRequestMessage(request, logger, serializationAdapter);

                //On the third try change the Url to a the correct one
                if (Tries == 2)
                {
                    httpRequestMessage.RequestUri =
                    new AbsoluteUrl(MainUnitTests.LocalBaseUriString)
                    .WithRelativeUrl(new RelativeUrl("Person"))
                    .ToUri();
                }

                Tries++;
                return httpClient.SendAsync(httpRequestMessage, request.CancellationToken);
            });
    }

    [TestClass]
    public class PollyTests
    {
        [TestMethod]
        public async Task TestPollyManualIncorrectUri()
        {
            var policy = HttpPolicyExtensions
              .HandleTransientHttpError()
              .OrResult(response => response.StatusCode == HttpStatusCode.NotFound)
              .RetryAsync(3);

            using var sendHttpRequestFunc = new PollySendHttpRequestMessage(policy);

            using var client = new Client(
                new ProtobufSerializationAdapter(),
                new(MainUnitTests.LocalBaseUriString),
                logger: null,
                sendHttpRequest: sendHttpRequestFunc,
                name: null);

            var person = new Person { FirstName = "Bob", Surname = "Smith" };

            //Note the Uri here is deliberately incorrect. It will cause a 404 Not found response. This is to make sure that polly is working
            person = await client.PostAsync<Person, Person>(person, new("person2"));
            Assert.AreEqual("Bob", person.FirstName);
            Assert.AreEqual(3, sendHttpRequestFunc.Tries);
        }


        [TestMethod]
        public async Task TestPollyWithDependencyInjection()
        {
            //Configure a Polly policy
            var policy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            //Create a Microsoft IoC Container
            var serviceCollection = new ServiceCollection();
            _ = serviceCollection.AddSingleton(typeof(ISerializationAdapter), typeof(NewtonsoftSerializationAdapter))
            .AddLogging()
            //Add the Polly policy to the named HttpClient instance
            .AddHttpClient("rc").
                SetHandlerLifetime(TimeSpan.FromMinutes(5)).
                AddPolicyHandler(policy);

            //Provides mapping for Microsoft's IHttpClientFactory (This is what makes the magic happen)
            _ = serviceCollection.AddRestClient();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var clientFactory = serviceProvider.GetRequiredService<CreateClient>();

            //Create a Rest Client that will get the HttpClient by the name of rc
            var client = clientFactory("rc", (o) => o.BaseUrl = new("https://restcountries.eu/rest/v2/"));

            //Make the call
            _ = await client.GetAsync<List<RestCountry>>();

            //TODO: Implement this completely to ensure that the policy is being applied
        }

    }
}
