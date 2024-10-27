using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKIT.FlurlHttpClient.Baidu.Translate;
using SKIT.FlurlHttpClient.Baidu.Translate.Models;

namespace LanguageChange
{
    public class BaiduTranslater
    {
        private BaiduTranslateClient Client { get; set; }

        public static BaiduTranslater Instance { get; } = new BaiduTranslater();

        private BaiduTranslater()
        {
            // https://api.fanyi.baidu.com/manage/developer 从这里取以下两个值 没有的可以注册一个
            var options = new BaiduTranslateClientOptions()
            {
                AppId = "201xxxxxx4979",
                AppSecret = "mCxxxxxxxxxnwnd"
            };

            this.Client = BaiduTranslateClientBuilder.Create(options).Build();
        }

        public async Task<string> GetEnglishText(string source)
        {
            /* 以通用文本翻译接口为例 */
            var request = new TranslateVipTranslateRequest()
            {
                QueryString = source,
                From = "zh",
                To = "en"
            };
            var response = await this.Client.ExecuteTranslateVipTranslateAsync(request);
            if (response.IsSuccessful())
            {
                return string.Join("\r\n", response.ResultList.Select(p => p.Destination));
            }
            else
            {
                return source;
            }

        }
    }
}
