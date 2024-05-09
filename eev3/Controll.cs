using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace eev3
{
    public class Mp3info
    {
        public string name { get; set; }
        public string mp3url { get; set; }

    }
    public class Mp3bag
    {
        public string nextUrl { get; set; }
        public List<string> mp3Urls { get; set; }
    }

    public class PlayInfo
    {
        /// <summary>
        /// 1为正常获取
        /// </summary>
        public int msg { get; set; }
        /// <summary>
        /// 歌词id
        /// </summary>
        public int lkid { get; set; }
        /// <summary>
        /// 歌名
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string pic { get; set; }
        /// <summary>
        /// 下载地址
        /// </summary>
        public string url { get; set; }
    }
 
    public class Controll
    {
        private string SAVEPATH ;
        private string HOST = "http://www.eev3.com";
        private HttpClient CLIENT;
        public Controll(string SavePath)
        {
            if (!SavePath.EndsWith(@"\") && !SavePath.EndsWith("/"))
                SavePath += "/";
            SAVEPATH = SavePath;
            CLIENT = new HttpClient();  
        }
        ~Controll()
        {
            CLIENT?.Dispose();
        }
     

        public async Task BeginSearch(string title, int maxCount)
        {
            var url = $"{HOST}/so/{title.ToUrlEncode()}.html";
            var list = new List<string>();
            do
            {
                var html = await NetUrl.GetAsync(url);
                var bag = Format(html);
                url = bag.nextUrl;
                if (bag.mp3Urls.Count > 0)
                {
                    list.AddRange(bag.mp3Urls);
                    if (list.Count > maxCount)
                        break;
                }
                else
                    break;
            } while (!url.IsEmpty());
            list = list.Distinct().ToList();

            await ExecTaskAsync(GetAndSave, list, 5);
        }
        private async Task GetAndSave(string id)
        {
            var info = await GetPlayInfo(id);
            if (info != null && info.msg == 1)
            {
                await SaveFile(info);
            }
        }
        private Mp3bag Format(string html)
        {
            var bag = new Mp3bag();
            var list = new List<string>();
            var play_list = html.FindHtmlObject("div", "class", "play_list");
            var ul = play_list.FindHtmlObject("ul");
            var lis = ul.FindHtmlObjects("li");
            foreach (var li in lis)
            {
                var name = li.FindHtmlObject("div", "class", "name");
                var href = name.FindHtmlAttValue("a", "href");
                var file = href.RegexExtract("mp3/(.*?).html");
                if (!file.IsEmpty())
                {
                    list.Add(file);
                }
            }
            var page = play_list.FindHtmlObject("div","class","page");
            var pages = page.SplitString("<a");
            foreach (var item in pages)
            {
                var att = $"<a{item}";
                if (att.ContainsStrings("下一页"))
                {
                    var nextUlr = att.FindHtmlAttValue("a", "href");
                    if (!nextUlr.IsEmpty())
                    {
                        bag.nextUrl = HOST + nextUlr;
                        break;
                    }
                }
            }
            bag.mp3Urls = list;
            return bag;
        }
        private async Task<PlayInfo> GetPlayInfo(string file)
        {
            var url = $"{HOST}/js/play.php";
            var json = await NetUrl.PostAsync(url, $"id={file}&type=mp3", $"{HOST}/mp3/{file}.html");
            return json.DeserializeObject<PlayInfo>() ;
        }
        private async Task SaveFile(PlayInfo info)
        {
            info.title = info.title.Replace("&", "_")
                .Replace("\\","").Replace("/","")
                .Replace("|","").Replace("*","")
                .Replace("?","").Replace("<","")
                .Replace(">","").Replace("\"","")
                .Replace(":","");
            var filePath = $"{SAVEPATH}{info.title}";
            var mp3Path = $"{filePath}.mp3";
            var lrcPath = $"{filePath}.lrc";
            await DownloadMp3Async(info.url, mp3Path);
            DownloadLrc(info, lrcPath); 
        }
        private async Task DownloadMp3Async(string url, string localFilePath)
        {
            try
            {
                using (var response = await CLIENT.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var destinationStream = new FileStream(localFilePath, FileMode.Create))
                            {
                                await contentStream.CopyToAsync(destinationStream);
                                Console.WriteLine("文件下载成功");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"文件下载失败，状态码：{response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下载过程中出现错误： {ex.Message}");
            }
        }
        private async Task DownloadLrc(PlayInfo info, string localFilePath)
        {
            var url = $"https://api.44h4.com/lc.php?cid={info.lkid}";
            var json = await NetUrl.GetAsync(url);
            var lrc = json.FindJsonValue("lrc");
            if (!lrc.IsEmpty())
            {
                try
                {
                    File.WriteAllText(localFilePath, lrc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(  $"保存歌词异常 {ex .Message}");
                }
            }
        }
        private async Task ExecTaskAsync<T>(Func<T, Task> action, List<T> value, int maxCount)
        {
            await Task.Run(() =>
            {
                Semaphore semaphore = new Semaphore(maxCount, maxCount);
                List<Task> tasks = new List<Task>();
                foreach (var item in value)
                {
                    semaphore.WaitOne();
                    var task = Task.Run(async () =>
                    {
                        await action(item);
                        semaphore.Release();
                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());

            });
        }
    }
}
