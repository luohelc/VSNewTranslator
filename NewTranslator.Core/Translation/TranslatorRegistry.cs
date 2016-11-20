using System;
using System.Collections.Generic;
using System.Linq;
using NewTranslator.Core.Translation.Bing;
using NewTranslator.Core.Translation.Baidu;
using NewTranslator.Core.Translation.Youdao;

namespace NewTranslator.Core.Translation
{
	public static class TranslatorRegistry
	{
        public static List<BaseTranslator> Translators = new List<BaseTranslator> {
			new GoogleTranslator(),
            new BingTranslator(new BingConnector()),
            new BaiduTranslator(new BaiduConnector()),
            new YoudaoTranslator(new YoudaoConnector())
		};
        
	    public static string AllTranslationServicesToken
	    {
	        get { return Translators.GetTranslationServicesToken(); }
	    }

        public static BaseTranslator GetTranslator(string name)
		{
			return Translators.Find(t => t.Name == name);
		}

        public static IEnumerable<BaseTranslator> GetTranslators(string names)
        {
            var keys = names.Split(new []{ '|' }, StringSplitOptions.RemoveEmptyEntries);
            return Translators.Where(t => keys.Contains(t.Name));
        }
    }
    
    internal static class TranslatorRegistryExtensions
    {
        internal static string GetTranslationServicesToken(this IEnumerable<BaseTranslator> translators)
        {
            return string.Join("|", translators.Select(p => p.Name));
        }
    }
}
