using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace sticky_notes_wp8.Services
{
    class OnlineRepository
    {
        struct APIMethods
        {
            public struct User
            {
                public const string Login = "user/login";
            }
        }

        public class Session
        {
            public string id;
            public string created;
        }

        public class User
        {
            public string id;
            public string firstName;
            public string surname;
            public string email;
            public string password;
        }

        public class LoginResponse
        {
            public User user;
            public Session session;
        }

        public class RepositoryResponse<T>
        {
            public int code;
            public T data;
        }

        const string ENDPOINT = "http://stickyapi.alanedwardes.com/";

        public string DictionaryToQueryString(Dictionary<string, string> dictionary)
        {
            var stringBuilder = new StringBuilder();
            bool isFirst = true;
            foreach (var dict in dictionary)
            {
                if (!isFirst) stringBuilder.Append("&");
                isFirst = false;
                stringBuilder.AppendFormat("{0}={1}",
                    HttpUtility.UrlEncode(dict.Key),
                    HttpUtility.UrlEncode(dict.Value));
            }

            return stringBuilder.ToString();
        }

        public async Task<RepositoryResponse<T>> HttpPostAsync<T>(string apiMethod, Dictionary<string, string> parameters)
        {
            var data = DictionaryToQueryString(parameters);
            var httpClient = new HttpClient();
            var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

            // Timeout is 500 milliseconds
            httpClient.Timeout = TimeSpan.FromMilliseconds(500);

            var response = await httpClient.PostAsync(ENDPOINT + apiMethod, stringContent);

            var repositoryResponse = new RepositoryResponse<T> { code = (int)response.StatusCode };

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                repositoryResponse.data = JsonConvert.DeserializeObject<T>(content);
            }

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<LoginResponse>> userLogin(string username, string password)
        {
            var credentials = new Dictionary<string, string>();
            credentials.Add("username", username);
            credentials.Add("password", password);
            return await HttpPostAsync<LoginResponse>(APIMethods.User.Login, credentials);
        }
    }
}
