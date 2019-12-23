﻿using RestClientDotNet.Abstractions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestClientDotNet
{
    public static class Extensions
    {
        #region Misc
        /// <summary>
        /// Sets the Authorization header for Basic Authentication with the specified credentials
        /// </summary>
        public static void UseBasicAuthentication(this IRestClient restClient, string userName, string password)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password));
            restClient.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
        }
        #endregion

        #region Get
        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient)
        {
            return GetAsync<T>(restClient, default(Uri));
        }

        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient, string resource)
        {
            try
            {
                return GetAsync<T>(restClient, new Uri(resource, UriKind.Relative));
            }
            catch (UriFormatException ufe)
            {
                if (ufe.Message == "A relative URI cannot be created because the 'uriString' parameter represents an absolute URI.")
                {
                    throw new UriFormatException(Messages.ErrorMessageAbsoluteUriAsString, ufe);
                }

                throw;
            }
        }

        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient, Uri resource)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return GetAsync<T>(restClient, resource, restClient.DefaultContentType);
        }

        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient, Uri resource, string contentType)
        {
            return GetAsync<T>(restClient, resource, contentType, default);
        }

        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return GetAsync<T>(restClient, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponse<T>> GetAsync<T>(this IRestClient restClient, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<T, object>(resource, HttpVerb.Get, contentType, null, cancellationToken);
        }
        #endregion

        #region Delete
        public static Task<RestResponse> DeleteAsync(this IRestClient restClient, string resource)
        {
            return DeleteAsync(restClient, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponse> DeleteAsync(this IRestClient restClient, Uri resource)
        {
            return DeleteAsync(restClient, resource, default);
        }

        public static async Task<RestResponse> DeleteAsync(this IRestClient restClient, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return await restClient.SendAsync<object, object>(resource, HttpVerb.Delete, restClient.DefaultContentType, null, cancellationToken);
        }
        #endregion

        #region Put
        public static Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, string resource)
        {
            return PutAsync<TReturn, TBody>(restClient, body, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource)
        {
            return PutAsync<TReturn, TBody>(restClient, body, resource, default);
        }

        public static Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PutAsync<TReturn, TBody>(restClient, body, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponse<TReturn>> PutAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TReturn, TBody>(resource, HttpVerb.Put, contentType, body, cancellationToken);
        }
        #endregion

        #region Post
        public static Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body)
        {
            return PostAsync<TReturn, TBody>(restClient, body, default(Uri));
        }

        public static Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, string resource)
        {
            return PostAsync<TReturn, TBody>(restClient, body, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource)
        {
            return PostAsync<TReturn, TBody>(restClient, body, resource, default);
        }

        public static Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PostAsync<TReturn, TBody>(restClient, body, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponse<TReturn>> PostAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TReturn, TBody>(resource, HttpVerb.Post, contentType, body, cancellationToken);
        }
        #endregion

        #region Patch
        public static Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, string resource)
        {
            return PatchAsync<TReturn, TBody>(restClient, body, new Uri(resource, UriKind.Relative));
        }

        public static Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PatchAsync<TReturn, TBody>(restClient, body, resource, restClient.DefaultContentType, default);
        }

        public static Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return PatchAsync<TReturn, TBody>(restClient, body, resource, restClient.DefaultContentType, cancellationToken);
        }

        public static Task<RestResponse<TReturn>> PatchAsync<TReturn, TBody>(this IRestClient restClient, TBody body, Uri resource, string contentType, CancellationToken cancellationToken)
        {
            if (restClient == null) throw new ArgumentNullException(nameof(restClient));
            return restClient.SendAsync<TReturn, object>(resource, HttpVerb.Patch, contentType, body, cancellationToken);
        }
        #endregion
    }
}