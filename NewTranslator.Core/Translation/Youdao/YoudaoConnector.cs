
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NewTranslator.Core.Translation.Youdao
{
    public class YoudaoConnector
    {
        private DateTime _tokenExpires;
        private string _currentToken;
        private ClientCredential _lastClientCredential;

        private ClientCredential ClientCredential
        {
            get
            {
                var clientCredential = ClientCredentialRepository.Current.GetCredential(Constants.TranslatorNames.Bing);
                if (clientCredential != _lastClientCredential)
                {
                    _tokenExpires = DateTime.Now;
                }
                _lastClientCredential = clientCredential;
                return _lastClientCredential;
            }
        }

        public string GetCurrentToken()
        {
            if ((_tokenExpires - DateTime.Now).TotalMinutes < 1)
            {
                RenewToken();
            }
            return _currentToken;
        }

        private void RenewToken()
        {
            if (ClientCredential == null || !ClientCredential.IsValid)
                return;

            var data = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", ClientCredential.ClientId },
                { "client_secret", ClientCredential.ClientSecret },
                { "scope", Constants.YoudaoTranslator.ClientScope }
            };

            string response = Utils.GetHttpResponse(Constants.YoudaoTranslator.TokenUrl, Utils.CreateQuerystring(data));
            JObject jToken = JObject.Parse(response);
            _currentToken = jToken["access_token"].Value<string>();
            _tokenExpires = DateTime.Now.AddSeconds(int.Parse(jToken["expires_in"].Value<string>()));
        }

        private async Task<string> GetDataAsync(string method, Dictionary<string, string> data = null)
        {
            if (ClientCredential == null || !ClientCredential.IsValid)
            {
                throw new InvalidOperationException("To use the Youdao translator you must enter the credentials in 'Tools > Options... > Translator'");
            }

            var task = await Utils.GetHttpResponseAsync(Constants.YoudaoTranslator.BaseUrl, Utils.CreateQuerystring(data));

            return task;
        }

        public virtual Task<string> GetLanguageNamesAsync(string locale, IEnumerable<string> codes)
        {
            return GetDataAsync("GetLanguageNames", new Dictionary<string, string>{
                { "locale", "en" },
                { "languageCodes", JsonConvert.SerializeObject(codes) }
            });
        }

        public virtual Task<string> GetLanguagesForTranslateAsync()
        {
            return GetDataAsync("GetLanguagesForTranslate");
        }

        public virtual Task<string> GetTranslationsAsync(string text, string sourceLang, string destLang)
        {
            var salt = DateTime.Now.Ticks;
            var sign = string.Empty;
            var dataBytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}{1}{2}{3}", Constants.YoudaoTranslator.TokenClientID, text, salt, Constants.YoudaoTranslator.TokenClientSecret)));
            foreach (var item in dataBytes)
            {
                sign += item.ToString("x2");
            }
            var cleanText = text.Trim();
            return GetDataAsync("GetTranslations", new Dictionary<string, string> {
                { "keyfrom", Constants.YoudaoTranslator.TokenClientSecret},
                { "key",Constants.YoudaoTranslator.TokenClientID},
                {"type", "data" },
                { "doctype","json" },
                { "version","1.1" },
                {"q" , text}
            });
        }

        private string GetData(string method, Dictionary<string, string> data = null)
        {
            if (ClientCredential == null || !ClientCredential.IsValid)
                return null;

            data = data ?? new Dictionary<string, string>();
            WebClient client = new WebClient { Headers = { ["Authorization"] = "Bearer " + GetCurrentToken() } };
            return client.DownloadString(Constants.YoudaoTranslator.BaseUrl + method + "?" + Utils.CreateQuerystring(data));
        }

        public virtual string GetLanguageNames(string locale, IEnumerable<string> codes)
        {
            return GetData("GetLanguageNames", new Dictionary<string, string>{
                { "locale", "en" },
                { "languageCodes", JsonConvert.SerializeObject(codes) }
            });
        }

        public virtual string GetLanguagesForTranslate()
        {
            return GetData("GetLanguagesForTranslate");
        }
    }
}
