using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Models;
using Newtonsoft.Json.Linq;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
//            var tokenResponse = GetToken("client", "secret", GrantType.ClientCredentials).Result;
//            CallApi(tokenResponse).Wait();
            var tokenResponse = GetToken("ro.client", "secret", GrantType.ResourceOwnerPassword, "alice", "password")
                .Result;
//            CallApi(tokenResponse).Wait();
            Console.ReadKey();
        }

        private static async Task<TokenResponse> GetToken(string clientId, string clientSecret, string grantType,
            string username = null, string password = null, string api = "api1")
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return null;
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, clientId, clientSecret);
            TokenResponse tokenResponse;
            switch (grantType)
            {
                case GrantType.ClientCredentials:
                    tokenResponse = await tokenClient.RequestClientCredentialsAsync(api);
                    break;
                case GrantType.ResourceOwnerPassword:
                    tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(username, password, api);
                    break;
                default: throw new ApplicationException("unknown grant type.");
            }

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return null;
            }

            Console.WriteLine(tokenResponse.Json);
            return tokenResponse;
        }

        private static async Task CallApi(TokenResponse tokenResponse)
        {
            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}