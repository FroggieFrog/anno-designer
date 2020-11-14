using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace AnnoDesigner.Core.Helper
{
    public static class ConnectivityHelper
    {
        private static HttpClient _httpClient;
        private const string URL = @"https://www.github.com";
        private const string SECOND_URL = @"https://www.google.com";

        public static HttpClient LocalHttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    var handler = new HttpClientHandler();
                    if (handler.SupportsAutomaticDecompression)
                    {
                        handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    }

                    _httpClient = new HttpClient(handler, true);
                    _httpClient.Timeout = TimeSpan.FromSeconds(30);
                    _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue($"anno-designer-81", "1.0"));

                    //detect DNS changes (default is infinite)
                    //ServicePointManager.FindServicePoint(new Uri(BASE_URI)).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    //defaut is 2 minutes
                    ServicePointManager.DnsRefreshTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    //increases the concurrent outbound connections
                    if (ServicePointManager.DefaultConnectionLimit < 1024)
                    {
                        ServicePointManager.DefaultConnectionLimit = 1024;
                    }
                    //only allow secure protocols
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                }

                return _httpClient;
            }
        }

        public static async Task<bool> IsConnected()
        {
            var result = false;

            var isInternetAvailable = false;

            try
            {
                var requestResult = await LocalHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, URL)).ConfigureAwait(false);
                isInternetAvailable = requestResult.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                isInternetAvailable = false;
            }

            //service outage? try second url
            if (!isInternetAvailable)
            {
                try
                {
                    var requestResult = await LocalHttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, SECOND_URL)).ConfigureAwait(false);
                    isInternetAvailable = requestResult.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    isInternetAvailable = false;
                }
            }

            if (isInternetAvailable)
            {
                result = IsNetworkAvailable;
            }

            return result;
        }

        public static bool IsNetworkAvailable
        {
            get { return NetworkInterface.GetIsNetworkAvailable(); }
        }
    }
}
