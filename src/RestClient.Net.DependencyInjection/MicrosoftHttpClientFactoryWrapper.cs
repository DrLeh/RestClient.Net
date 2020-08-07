﻿using snh = System.Net.Http;

namespace RestClient.Net.DependencyInjection
{
    public class MicrosoftHttpClientFactoryWrapper 
    {
        #region Public Properties
        private snh.IHttpClientFactory HttpClientFactory { get; }
        #endregion

        #region Constructor
        public MicrosoftHttpClientFactoryWrapper(snh.IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }
        #endregion

        #region Implementation
        public snh.HttpClient CreateClient(string name)
        {
            return HttpClientFactory.CreateClient(name);
        }
        #endregion
    }
}
