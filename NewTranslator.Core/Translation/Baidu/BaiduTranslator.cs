using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTranslator.Core.Translation.Baidu
{
    public class BaiduTranslator : BaseTranslator
    {

        internal bool InitLanguagesFromApi = false;

        private readonly BaiduConnector _connector;
        private readonly object _syncRoot = new object();

        public BaiduTranslator(BaiduConnector connector)
        {
            EnableCache = true;
            _connector = connector;
        }

        public override string Name
        {
            get { return Constants.TranslatorNames.Baidu; }
        }

        public override string AccessibleName
        {
            get { return "Baidu Translator"; }
        }

        public override bool NeedCredentials { get { return true; } }

        private List<TranslationLanguage> _sourceLanguages;
        public override List<TranslationLanguage> SourceLanguages
        {
            get
            {
                if (_sourceLanguages == null)
                {
                    InitLanguages();
                }
                return _sourceLanguages;
            }
            protected set { _sourceLanguages = value; }
        }

        private List<TranslationLanguage> _targetLanguages;
        public override List<TranslationLanguage> TargetLanguages
        {
            get
            {
                if (_targetLanguages == null)
                {
                    InitLanguages();
                }
                return _targetLanguages;
            }
            protected set { _targetLanguages = value; }
        }

        private void InitLanguages()
        {
            if (!InitLanguagesFromApi)
            {
                TargetLanguages = Constants.BaiduTranslator.AvailableLanguages.Copy();
                SourceLanguages = new TranslationLanguageCollection { { "", "Auto-detect" } };
                SourceLanguages.AddRange(TargetLanguages);
                return;
            }

            lock (_syncRoot)
            {
                if (_sourceLanguages == null || _targetLanguages == null)
                {
                    string[] codes = null;
                    string[] names = null;
                    try
                    {
                        codes = JsonConvert.DeserializeObject<string[]>(_connector.GetLanguagesForTranslate());
                        names = JsonConvert.DeserializeObject<string[]>(_connector.GetLanguageNames("en", codes));
                    }
                    catch
                    {
                        codes = new string[0];
                        names = new string[0];
                    }
                    var languages = new List<TranslationLanguage>(codes.Length);
                    for (int i = 0; i < codes.Length; i++)
                    {
                        languages.Add(new TranslationLanguage(codes[i], names[i]));
                    }
                    languages.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
                    _targetLanguages = languages;
                    _sourceLanguages = new List<TranslationLanguage> { new TranslationLanguage("", "Auto-detect") };
                    _sourceLanguages.AddRange(_targetLanguages);
                }
            }
        }

        protected override async Task<TranslationResult> GetRemoteTranslationAsync(string text, string sourceLang, string destinationLang)
        {
            var translations = await _connector.GetTranslationsAsync(text, sourceLang, destinationLang);
            JObject json = JObject.Parse(translations);
            var result = ParseResponse(json);
            result.DestinationLanguage = destinationLang;
            result.OriginalText = text;
            result.TranslationSource = Name;
            return result;
        }

        private TranslationResult ParseResponse(JObject json)
        {
            /* {
             *      from:"en",
             *      to:"zh",
             *      trans_result:{
             *          src:"",
             *          dst:""
             *      }
             * }        
           * */

            TranslationResult res = new TranslationResult();
            JToken jTranslations = json["trans_result"];
            IEnumerable<string> translations = jTranslations
                .Select(t => new BaiduTranslation
                {
                    Text = t["dst"].Value<string>()
                })
                .Select(t => t.Text);

            //filtering duplicates
            //LINQ Distinct is not guaranteed to preserve order
            HashSet<string> distinctValues = new HashSet<string>();
            foreach (string t in translations)
            {
                if (!distinctValues.Contains(t))
                {
                    res.Sentences.Add(t);
                    distinctValues.Add(t);
                }
            }

            res.SourceLanguage = json.Value<string>("from");
            res.DestinationLanguage = json.Value<string>("to");
            return res;
        }
    }

}
