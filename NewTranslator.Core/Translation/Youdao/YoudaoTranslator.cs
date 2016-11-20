using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NewTranslator.Core.Translation.Baidu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewTranslator.Core.Translation.Youdao
{
    public class YoudaoTranslator : BaseTranslator
    {

        internal bool InitLanguagesFromApi = false;

        private readonly YoudaoConnector _connector;
        private readonly object _syncRoot = new object();

        public YoudaoTranslator(YoudaoConnector connector)
        {
            EnableCache = true;
            _connector = connector;
        }

        public override string Name
        {
            get { return Constants.TranslatorNames.Youdao; }
        }

        public override string AccessibleName
        {
            get { return "Youdao Translator"; }
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
            /* 
                {
                    "errorCode":0
                    "query":"good",
                    "translation":["好"], // 有道翻译
                    "basic":{ // 有道词典-基本词典
                        "phonetic":"gʊd"
                        "uk-phonetic":"gʊd" //英式发音
                        "us-phonetic":"ɡʊd" //美式发音
                        "explains":[
                            "好处",
                            "好的"
                            "好"
                        ]
                    },
                    "web":[ // 有道词典-网络释义
                        {
                            "key":"good",
                            "value":["良好","善","美好"]
                        },
                        {...}
                    ]
                }
             * */

            TranslationResult res = new TranslationResult();
            JToken jTranslations = json["translation"];
            IEnumerable<string> translations = jTranslations
                .Select(t => new BaiduTranslation
                {
                    Text = t.Value<string>()
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

            res.SourceLanguage = "en";
            res.DestinationLanguage = "zh";
            return res;
        }
    }
}
