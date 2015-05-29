using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;              //Directory 目录 
using System.Reflection;      //BindingFlags 枚举

namespace EMSecure
{
    class RecentlyFileHelper
    {
        /// <summary>
        /// 函数功能:获取ShortCut
        /// </summary>
        /// <param name="shortcutFilename"></param>
        /// <returns></returns>
        public static string GetShortcutTargetFile(string shortcutFilename)
        {
            var type = Type.GetTypeFromProgID("WScript.Shell");  //获取WScript.Shell类型
            object instance = Activator.CreateInstance(type);    //创建该类型实例
            var result = type.InvokeMember("CreateShortCut", BindingFlags.InvokeMethod, null, instance, new object[] { shortcutFilename });
            var targetFile = result.GetType().InvokeMember("TargetPath", BindingFlags.GetProperty, null, result, null) as string;
            return targetFile;
        }

        //获取文件信息
        public static IEnumerable<string> GetRecentlyFiles()
        {
            var recentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Recent);  //获取Recent路径
            return from file in Directory.EnumerateFiles(recentFolder)
                   where Path.GetExtension(file) == ".lnk"
                   select GetShortcutTargetFile(file);
        }
    }
}
