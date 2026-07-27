using System;
using System.Linq;
using System.Windows;

namespace CountdownDay
{
    public partial class App : System.Windows.Application
    {
        // 动态切换语言的核心代码
        public static void SwitchLanguage(string cultureCode)
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri($"Languages/{cultureCode}.xaml", UriKind.Relative)
            };

            // 替换全局 MergedDictionaries
            var merged = Current.Resources.MergedDictionaries;
            var oldDict = merged.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.StartsWith("Languages/"));
            
            if (oldDict != null)
            {
                merged.Remove(oldDict);
            }
            merged.Add(dict);
        }

        // 从资源字典中获取指定 Key 的字符串
        public static string GetString(string key)
        {
            return Current.Resources[key]?.ToString() ?? key;
        }
    }
}