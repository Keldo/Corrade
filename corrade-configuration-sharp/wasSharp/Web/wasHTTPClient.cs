///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using wasSharp.Collections.Utilities;

namespace wasSharp.Web
{
    ///////////////////////////////////////////////////////////////////////////
    //    Copyright (C) 2014 Wizardry and Steamworks - License: GNU GPLv3    //
    ///////////////////////////////////////////////////////////////////////////
    // <summary>A portable HTTP client.</summar>
    public class wasHTTPClient : IDisposable
    {
        private readonly HttpClient HTTPClient;
        private readonly string MediaType;

        public wasHTTPClient(ProductInfoHeaderValue userAgent, CookieContainer cookieContainer, string mediaType,
            uint timeout) : this(userAgent, cookieContainer, mediaType, null, null, timeout)
        {
        }

        public wasHTTPClient(ProductInfoHeaderValue userAgent, string mediaType, uint timeout)
            : this(userAgent, new CookieContainer(), mediaType, null, null, timeout)
        {
        }

        public wasHTTPClient(ProductInfoHeaderValue userAgent, CookieContainer cookieContainer, string mediaType,
            AuthenticationHeaderValue authentication, Dictionary<string, string> headers, uint timeout)
        {
            var HTTPClientHandler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };
            if (HTTPClientHandler.SupportsRedirectConfiguration)
            {
                HTTPClientHandler.AllowAutoRedirect = true;
            }
            if (HTTPClientHandler.SupportsProxy)
            {
                HTTPClientHandler.Proxy = WebRequest.DefaultWebProxy;
                HTTPClientHandler.UseProxy = true;
            }
            if (HTTPClientHandler.SupportsAutomaticDecompression)
            {
                HTTPClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            HTTPClientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;

            HTTPClient = new HttpClient(HTTPClientHandler, false);
            HTTPClient.DefaultRequestHeaders.UserAgent.Add(userAgent);
            if (authentication != null)
            {
                HTTPClient.DefaultRequestHeaders.Authorization = authentication;
            }
            // Add some standard headers:
            // Accept - for socially acceptable security of mod_sec
            switch (headers != null)
            {
                case false:
                    headers = new Dictionary<string, string> {{"Accept", @"*/*"}};
                    break;
                default:
                    if (!headers.ContainsKey("Accept"))
                    {
                        headers.Add("Accept", @"*/*");
                    }
                    break;
            }
            foreach (var header in headers)
            {
                HTTPClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            HTTPClient.Timeout = TimeSpan.FromMilliseconds(timeout);
            MediaType = mediaType;
        }

        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2014 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Sends a PUT request to an URL with binary data.
        /// </summary>
        /// <param name="URL">the url to send the message to</param>
        /// <param name="data">key-value pairs to send</param>
        /// <param name="headers">headers to send with the request</param>
        public async Task<byte[]> PUT(string URL, byte[] data, Dictionary<string, string> headers)
        {
            try
            {
                using (var content = new ByteArrayContent(data))
                {
                    using (
                        var request = new HttpRequestMessage
                        {
                            RequestUri = new Uri(URL),
                            Method = HttpMethod.Put,
                            Content = content
                        })
                    {
                        foreach (var header in headers)
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }
                        // Add some standard headers:
                        // Accept - for socially acceptable security of mod_sec
                        if (!headers.ContainsKey("Accept"))
                        {
                            headers.Add("Accept", @"*/*");
                        }
                        using (var response = await HTTPClient.SendAsync(request))
                        {
                            return response.IsSuccessStatusCode
                                ? await response.Content.ReadAsByteArrayAsync()
                                : null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2014 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Sends a PUT request to an URL with text.
        /// </summary>
        /// <param name="URL">the url to send the message to</param>
        /// <param name="data">key-value pairs to send</param>
        /// <param name="headers">headers to send with the request</param>
        public async Task<byte[]> PUT(string URL, string data, Dictionary<string, string> headers)
        {
            try
            {
                using (var content =
                    new StringContent(data, Encoding.UTF8, MediaType))
                {
                    using (
                        var request = new HttpRequestMessage
                        {
                            RequestUri = new Uri(URL),
                            Method = HttpMethod.Put,
                            Content = content
                        })
                    {
                        foreach (var header in headers)
                        {
                            request.Headers.Add(header.Key, header.Value);
                        }
                        // Add some standard headers:
                        // Accept - for socially acceptable security of mod_sec
                        if (!headers.ContainsKey("Accept"))
                        {
                            headers.Add("Accept", @"*/*");
                        }
                        using (var response = await HTTPClient.SendAsync(request))
                        {
                            return response.IsSuccessStatusCode
                                ? await response.Content.ReadAsByteArrayAsync()
                                : null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2014 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Sends a PUT request to an URL with binary data.
        /// </summary>
        /// <param name="URL">the url to send the message to</param>
        /// <param name="data">key-value pairs to send</param>
        public async Task<byte[]> PUT(string URL, byte[] data)
        {
            try
            {
                using (var content = new ByteArrayContent(data))
                {
                    using (var response = await HTTPClient.PutAsync(URL, content))
                    {
                        return response.IsSuccessStatusCode
                            ? await response.Content.ReadAsByteArrayAsync()
                            : null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2014 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Sends a PUT request to an URL with text.
        /// </summary>
        /// <param name="URL">the url to send the message to</param>
        /// <param name="data">key-value pairs to send</param>
        public async Task<byte[]> PUT(string URL, string data)
        {
            try
            {
                using (var content =
                    new StringContent(data, Encoding.UTF8, MediaType))
                {
                    using (var response = await HTTPClient.PutAsync(URL, content))
                    {
                        return response.IsSuccessStatusCode
                            ? await response.Content.ReadAsByteArrayAsync()
                            : null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2014 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Sends a POST request to an URL with set key-value pairs.
        /// </summary>
        /// <param name="URL">the url to send the message to</param>
        /// <param name="message">key-value pairs to send</param>
        public async Task<byte[]> POST(string URL, Dictionary<string, string> message)
        {
            try
            {
                using (var content =
                    new StringContent(KeyValue.Encode(message), Encoding.UTF8, MediaType))
                {
                    using (var response = await HTTPClient.PostAsync(URL, content))
                    {
                        return response.IsSuccessStatusCode
                            ? await response.Content.ReadAsByteArrayAsync()
                            : null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2014 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Sends a GET request to an URL with set key-value pairs.
        /// </summary>
        /// <param name="URL">the url to send the message to</param>
        /// <param name="message">key-value pairs to send</param>
        public async Task<byte[]> GET(string URL, Dictionary<string, string> message)
        {
            try
            {
                using (var response =
                    await HTTPClient.GetAsync(URL + "?" + KeyValue.Encode(message)))
                {
                    return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////
        //    Copyright (C) 2016 Wizardry and Steamworks - License: GNU GPLv3    //
        ///////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Sends a GET request to an URL with set key-value pairs.
        /// </summary>
        /// <param name="URL">the url to send the message to</param>
        public async Task<byte[]> GET(string URL)
        {
            try
            {
                using (var response = await HTTPClient.GetAsync(URL))
                {
                    return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Dispose()
        {
            HTTPClient?.Dispose();
        }
    }
}