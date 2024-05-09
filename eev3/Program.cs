using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eev3
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("回车键开始采集");
            Console.ReadLine();
            var name = "王力宏";
            var path = $"E:\\Media\\{name}";
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var controll = new Controll(path);
            await controll.BeginSearch(name, 2000);
            Console.WriteLine("采集任务已完成！任意键退出！");
            Console.ReadKey();
        }
    }
}
