using System.Collections.Generic;
using System.Linq;
using NewTranslator.Core.Translation;

namespace NewTranslator.Core
{
    public class Constants
    {
        internal static IEnumerable<TranslationLanguage> CommunAvailableLanguages =
            GoogleTranslator.AvailableLanguages.Intersect(BingTranslator.AvailableLanguages);

        public class TranslatorNames
        {
            public const string Bing = "Bing";
            public const string Google = "Google";
            public const string Baidu = "Baidu";
            public const string Youdao = "Youdao";
        }

        internal static class TranslationCache
        {
            public const string UserEditedItemHeader = "Edited";
        }

        internal class GoogleTranslator
        {
            internal const string BaseUrl = "http://translate.google.com/translate_a/t";

            public static TranslationLanguageCollection AvailableLanguages = new TranslationLanguageCollection
            {
                {"af", "Afrikaans"},
                {"sq", "Albanian"},
                {"ar", "Arabic"},
                {"be", "Belarusian"},
                {"bg", "Bulgarian"},
                {"ca", "Catalan"},
                {"zh-CN", "Chinese (Simplified)"},
                {"zh-TW", "Chinese (Traditional)"},
                {"hr", "Croatian"},
                {"cs", "Czech"},
                {"da", "Danish"},
                {"nl", "Dutch"},
                {"en", "English"},
                {"et", "Estonian"},
                {"tl", "Filipino"},
                {"fi", "Finnish"},
                {"fr", "French"},
                {"gl", "Galician"},
                {"de", "German"},
                {"el", "Greek"},
                {"iw", "Hebrew"},
                {"hi", "Hindi"},
                {"hu", "Hungarian"},
                {"is", "Icelandic"},
                {"id", "Indonesian"},
                {"ga", "Irish"},
                {"it", "Italian"},
                {"ja", "Japanese"},
                {"ko", "Korean"},
                {"lv", "Latvian"},
                {"lt", "Lithuanian"},
                {"mk", "Macedonian"},
                {"ms", "Malay"},
                {"mt", "Maltese"},
                {"fa", "Persian"},
                {"pl", "Polish"},
                {"pt", "Portugese"},
                {"ro", "Romanian"},
                {"ru", "Russian"},
                {"sr", "Serbian"},
                {"sk", "Slovak"},
                {"sl", "Slovenian"},
                {"es", "Spanish"},
                {"sw", "Swahili"},
                {"sv", "Swedish"},
                {"th", "Thai"},
                {"tr", "Turkish"},
                {"uk", "Ukranian"},
                {"vi", "Vietnamese"},
                {"cy", "Welsh"},
                {"yi", "Yiddish"}
            };
        }

        internal class BingTranslator
        {
            internal const string BaseUrl = "http://api.microsofttranslator.com/v2/ajax.svc/";
            //access token

            internal const string TokenUrl = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";

            /*
            internal const string TokenClientID = "VsTranslator";
            internal const string TokenClientSecret = "SVJTxigXb3ezDDm6ZG5hn/FC20YUbV37clW3zw8hLLE=";
            */

            internal const string TokenClientID = "VsTranslatorImproved";
            internal const string TokenClientSecret = "4/iivVWhh+3Ojj7mJvQCUNKdkGRc81rBjURx4VyYbJQ=";
            internal const string ClientScope = "http://api.microsofttranslator.com";

            internal static TranslationLanguageCollection AvailableLanguages = new TranslationLanguageCollection
            {
                {"ar", "Arabic"},
                {"bs-Latn", "Bosnian (Latin)"},
                {"bg", "Bulgarian"},
                {"ca", "Catalan"},
                {"zh-CHS", "Chinese Simplified"},
                {"zh-CHT", "Chinese Traditional"},
                {"hr", "Croatian"},
                {"cs", "Czech"},
                {"da", "Danish"},
                {"nl", "Dutch"},
                {"en", "English"},
                {"et", "Estonian"},
                {"fi", "Finnish"},
                {"fr", "French"},
                {"de", "German"},
                {"el", "Greek"},
                {"ht", "Haitian Creole"},
                {"he", "Hebrew"},
                {"hi", "Hindi"},
                {"mww", "Hmong Daw"},
                {"hu", "Hungarian"},
                {"id", "Indonesian"},
                {"it", "Italian"},
                {"ja", "Japanese"},
                {"tlh", "Klingon"},
                {"tlh-Qaak", "Klingon (pIqaD)"},
                {"ko", "Korean"},
                {"lv", "Latvian"},
                {"lt", "Lithuanian"},
                {"ms", "Malay"},
                {"mt", "Maltese"},
                {"no", "Norwegian"},
                {"fa", "Persian"},
                {"pl", "Polish"},
                {"pt", "Portuguese"},
                {"otq", "Querétaro Otomi"},
                {"ro", "Romanian"},
                {"ru", "Russian"},
                {"sr-Cyrl", "Serbian (Cyrillic)"},
                {"sr-Latn", "Serbian (Latin)"},
                {"sk", "Slovak"},
                {"sl", "Slovenian"},
                {"es", "Spanish"},
                {"sv", "Swedish"},
                {"th", "Thai"},
                {"tr", "Turkish"},
                {"uk", "Ukrainian"},
                {"ur", "Urdu"},
                {"vi", "Vietnamese"},
                {"cy", "Welsh"},
                {"yua", "Yucatec Maya"}
            };
        }

        internal class BaiduTranslator
        {
            internal const string BaseUrl = "http://api.fanyi.baidu.com/api/trans/vip/translate";
            //access token

            internal const string TokenUrl = "";

            /*
            internal const string TokenClientID = "VsTranslator";
            internal const string TokenClientSecret = "SVJTxigXb3ezDDm6ZG5hn/FC20YUbV37clW3zw8hLLE=";
            */

            internal const string TokenClientID = "20160222000012981";
            internal const string TokenClientSecret = "qPTxvESUVtl05DfRfXEg";
            internal const string ClientScope = "";

            internal static TranslationLanguageCollection AvailableLanguages = new TranslationLanguageCollection
            {                
                {"zh", "中文"},
                { "en","英语"},
                { "yue","粤语"},
                { "wyw", "文言文" },
                { "jp" , "日语" },
                {"kor","韩语" },
                { "fra","法语" },
                { "spa", "西班牙语" },
                { "th", "泰语" },
                { "ara" ,"阿拉伯语" },
                { "ru", "俄语" },
                { "pt",  "葡萄牙语" },
                { "de",  "德语" },
                { "it",  "意大利语" },
                { "el",  "希腊语" },
                { "nl" , "荷兰语" },
                { "pl" , "波兰语" },
                { "bul", "保加利亚语" },
                { "est", "爱沙尼亚语" },
                { "dan", "丹麦语" },
                { "fin", "芬兰语" },
                { "cs",  "捷克语" },
                { "rom", "罗马尼亚语" },
                { "slo","斯洛文尼亚语" },
                { "swe", "瑞典语" },
                { "hu",  "匈牙利语" },
                {"cht", "繁体中文"},
            };

        }

        internal class YoudaoTranslator
        {
            internal const string BaseUrl = "http://fanyi.youdao.com/openapi.do";
            //access token

            internal const string TokenUrl = "";

            /*
            internal const string TokenClientID = "VsTranslator";
            internal const string TokenClientSecret = "SVJTxigXb3ezDDm6ZG5hn/FC20YUbV37clW3zw8hLLE=";
            */

            internal const string TokenClientID = "130874911";
            internal const string TokenClientSecret = "gTranslator";
            internal const string ClientScope = "";

            internal static TranslationLanguageCollection AvailableLanguages = new TranslationLanguageCollection{};
        }
    }
}