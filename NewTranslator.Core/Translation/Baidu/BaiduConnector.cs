using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NewTranslator.Core.Translation.Baidu
{
    public class BaiduConnector
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
                { "scope", Constants.BaiduTranslator.ClientScope }
            };

            string response = Utils.GetHttpResponse(Constants.BaiduTranslator.TokenUrl, Utils.CreateQuerystring(data));
            JObject jToken = JObject.Parse(response);
            _currentToken = jToken["access_token"].Value<string>();
            _tokenExpires = DateTime.Now.AddSeconds(int.Parse(jToken["expires_in"].Value<string>()));
        }

        private async Task<string> GetDataAsync(string method, Dictionary<string, string> data = null)
        {
            if (ClientCredential == null || !ClientCredential.IsValid)
            {
                throw new InvalidOperationException("To use the Baidu translator you must enter the credentials in 'Tools > Options... > Translator'");
            }

            var task=await Utils.GetHttpResponseAsync(Constants.BaiduTranslator.BaseUrl, Utils.CreateQuerystring(data));   

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
            var dataBytes = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}{1}{2}{3}", Constants.BaiduTranslator.TokenClientID, text, salt, Constants.BaiduTranslator.TokenClientSecret)));
            foreach (var item in dataBytes)
            {
                sign += item.ToString("x2");
            }
            var cleanText = text.Trim();
            return GetDataAsync("GetTranslations", new Dictionary<string, string> {
                {"from",string.IsNullOrWhiteSpace(sourceLang)? "auto":sourceLang },
                { "to",destLang},
                { "q",text},
                { "appid",Constants.BaiduTranslator.TokenClientID},
                {"salt" ,salt.ToString()},
                { "sign",sign}
            });
        }

        private string GetData(string method, Dictionary<string, string> data = null)
        {
            if (ClientCredential == null || !ClientCredential.IsValid)
                return null;

            data = data ?? new Dictionary<string, string>();
            WebClient client = new WebClient { Headers = { ["Authorization"] = "Bearer " + GetCurrentToken() } };
            return client.DownloadString(Constants.BingTranslator.BaseUrl + method + "?" + Utils.CreateQuerystring(data));
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

