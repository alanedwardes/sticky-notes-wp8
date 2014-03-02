using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using sticky_notes_wp8.Data;

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
            public struct Boards
            {
                public const string List = "boards/list";
            }
            public struct Notes
            {
                public const string List = "notes/list";
            }
        }

        public class Session
        {
            public string id;
            public string created;
        }

        public class User
        {
            public int id;
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

        public class NotesListResponse
        {
            public List<Note> notes;
        }

        public class RepositoryResponse<T>
        {
            public int code;
            public T data;
        }

        public class BoardsListResponse
        {
            public List<Board> boards;
        }

        const string ENDPOINT = "http://stickyapi.alanedwardes.com/";

        public static string DictionaryToQueryString(Dictionary<string, string> dictionary)
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

        public static async Task<RepositoryResponse<T>> HttpPostAsync<T>(string apiMethod, Dictionary<string, string> parameters)
        {
            var data = DictionaryToQueryString(parameters);
            var httpClient = new HttpClient();
            var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

            // Timeout is 500 milliseconds
            httpClient.Timeout = TimeSpan.FromMilliseconds(1000);

            HttpResponseMessage response;
            try
            {
                response = await httpClient.PostAsync(ENDPOINT + apiMethod, stringContent);
            }
            catch (TaskCanceledException)
            {
                return new RepositoryResponse<T> { code = (int)HttpStatusCode.RequestTimeout };
            }

            var repositoryResponse = new RepositoryResponse<T> { code = (int)response.StatusCode };

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    ContractResolver = new CamelCaseToUnderscorePropertyNamesContractResolver()
                };

                repositoryResponse.data = JsonConvert.DeserializeObject<T>(content);
            }

            return repositoryResponse;
        }

        public async Task<RepositoryResponse<LoginResponse>> UserLogin(string username, string password)
        {
            var data = new Dictionary<string, string>();
            data.Add("username", username);
            data.Add("password", password);
            return await HttpPostAsync<LoginResponse>(APIMethods.User.Login, data);
        }

        public async Task<RepositoryResponse<BoardsListResponse>> BoardsList(string token)
        {
            var data = new Dictionary<string, string>();
            data.Add("token", token);
            return await HttpPostAsync<BoardsListResponse>(APIMethods.Boards.List, data);
        }

        public async Task<RepositoryResponse<NotesListResponse>> NotesList(string token, int boardId)
        {
            var data = new Dictionary<string, string>();
            data.Add("token", token);
            data.Add("boardID", boardId.ToString());
            return await HttpPostAsync<NotesListResponse>(APIMethods.Notes.List, data);
        }
    }
}
