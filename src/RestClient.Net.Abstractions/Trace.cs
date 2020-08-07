﻿using System;

namespace RestClient.Net.Abstractions
{
    public class Trace
    {
        public HttpRequestMethod HttpRequestMethod { get; }
        public Uri RequestUri { get; }
#pragma warning disable CA1819 
        public byte[] BodyData { get; }
#pragma warning restore CA1819 
        public TraceEvent RestEvent { get; }
        public int? HttpStatusCode { get; }
        public IHeadersCollection HeadersCollection { get; }
        public string Message { get; }

        public Trace(
            HttpRequestMethod httpRequestMethod,
            TraceEvent traceType,
            Uri requestUri = null,
            byte[] bodyData = null,
            int? httpStatusCode = null,
            IHeadersCollection headersCollection = null,
            string message = null)
        {
            HttpRequestMethod = httpRequestMethod;
            RequestUri = requestUri;
            BodyData = bodyData;
            RestEvent = traceType;
            HttpStatusCode = httpStatusCode;
            HeadersCollection = headersCollection;
            Message = message;
        }
    }
}
