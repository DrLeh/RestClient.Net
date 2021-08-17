#if !NET45

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net.Http;
using Urls;

namespace RestClient.Net.UnitTests
{
    [TestClass]
    public class MicrosoftDependencyInjectionTests2
    {
        private const string DefaultRestClientName = "RestClient";

        [TestMethod]
        [DataRow(DefaultRestClientName, true)]
        [DataRow("123", false)]
        public void TestDIMapping(string httpClientName, bool isEqual)
        {
            const int secondsTimeout = 123;

            var serviceCollection = new ServiceCollection()
                .AddSingleton<IGetString, GetString1>()
                .AddRestClient();

            _ = serviceCollection.AddHttpClient(httpClientName, new Action<HttpClient>((c) =>
                c.Timeout = new TimeSpan(0, 0, secondsTimeout)
            ));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var someService = serviceProvider.GetRequiredService<IGetString>();

            if (someService.Client is not Client client)
            {
                throw new InvalidOperationException("Nah");
            }

            Assert.AreEqual(DefaultRestClientName, client.Name);

            if (isEqual)
            {
                //Make sure we got the HttpClient that the Microsft DI put in the container
                Assert.AreEqual(secondsTimeout, client.lazyHttpClient.Value.Timeout.TotalSeconds);
            }
            else
            {
                Assert.AreNotEqual(secondsTimeout, client.lazyHttpClient.Value.Timeout.TotalSeconds);
            }
        }

        [TestMethod]
        public void TestDIMapping2()
        {
            const int secondsTimeout = 123;

            var serviceCollection = new ServiceCollection()
                .AddSingleton<IGetString, GetString2>()
                .AddRestClient();

            _ = serviceCollection.AddHttpClient("Jim", new Action<HttpClient>((c) =>
                c.Timeout = new TimeSpan(0, 0, secondsTimeout)
            ));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var someService = serviceProvider.GetRequiredService<IGetString>();

            if (someService.Client is not Client client)
            {
                throw new InvalidOperationException("Nah");
            }

            Assert.AreEqual("test", client.Name);
            Assert.AreEqual("Hi", client.BaseUrl.Host);
        }


        [TestMethod]
        public void DIConfigureWithInjectedService()
        {
            const int secondsTimeout = 123;
            var testUrl = "http://example.org";

            var urlProvider = new Mock<IUrlProvider>();
            urlProvider.Setup(x => x.GetUrl())
                .Returns(testUrl)
                .Verifiable();

            var serviceCollection = new ServiceCollection()
                .AddRestClient((o, sp) =>
                {
                    o.BaseUrl = new AbsoluteUrl(sp.GetRequiredService<IUrlProvider>().GetUrl());
                })
                .AddSingleton(urlProvider.Object);

            _ = serviceCollection.AddHttpClient("Jim", new Action<HttpClient>((c) =>
                c.Timeout = new TimeSpan(0, 0, secondsTimeout)
            ));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var c = serviceProvider.GetRequiredService<IClient>();

            if (c is not Client client)
            {
                throw new InvalidOperationException("Nah");
            }

            Assert.AreEqual("RestClient", client.Name);
            Assert.AreEqual(new AbsoluteUrl(testUrl), client.BaseUrl);
            urlProvider.VerifyAll();
        }
    }
}
#endif