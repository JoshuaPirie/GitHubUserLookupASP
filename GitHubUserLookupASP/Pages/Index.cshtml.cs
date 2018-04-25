using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace GitHubUserLookupASP.Pages {
    public class IndexModel : PageModel {
        private const string githubApiUrl = "https://api.github.com";
        private const string githubUsernameRegex = @"^[a-z\d](?:[a-z\d]|-(?=[a-z\d])){0,38}$";

        public Boolean IsError = false;
        public string ErrorMessage;
        public Boolean IsData = false;
        public User Data;

        public void OnGet() {
            string username = Request.Query["username"];
            if(username != null) {
                if(Regex.IsMatch(username, githubUsernameRegex, RegexOptions.IgnoreCase)) {
                    HttpClient client = new HttpClient();
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{githubApiUrl}/users/{username}");
                    requestMessage.Headers.Add("User-Agent", ".NET Client Application");
                    HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                    if(response.IsSuccessStatusCode) {
                        IsData = true;
                        string json = response.Content.ReadAsStringAsync().Result;
                        Data = JsonConvert.DeserializeObject<User>(json);
                    }
                    else {
                        IsError = true;
                        if((int)response.StatusCode == 404)
                            ErrorMessage = $"The user {username} does not exist.";
                        else
                            ErrorMessage = $"{((int)response.StatusCode).ToString()} {response.ReasonPhrase}";
                    }
                }
                else {
                    IsError = true;
                    ErrorMessage = $"{username} is not a valid GitHub username.";
                }
            }
        }
    }

    public class User {
        public string name;
        public string login;
        public string avatar_url;
        public string html_url;
        public string bio;
        public string company;
        public string location;
        public string email;
        public string blog;
        public int public_repos;
        public int public_gists;
        public int followers;
        public int following;
        public string created_at;
    }
}
