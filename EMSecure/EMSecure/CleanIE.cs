using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;  //DllImport
using System.IO;                       //Directory 目录  
using Microsoft.Win32;                 //RegistryKey

namespace EMSecure
{
    //不能把COM接口放置前面 否则查看[设计]时会提醒无法加载CleanIE类
    public partial class CleanIE : Form
    {
        public CleanIE()
        {
            InitializeComponent();
        }

        #region 初始化
        //定义Num记录listBox1中获取的文件数 是否删除成功
        public int Num = 0;
        public bool IsDelete = false;

        //窗体载入初始化窗体
        private void CleanIE_Load(object sender, EventArgs e)
        {
            this.label1.ForeColor = Color.Black;
            this.label1.Text = "清除上网记录信息,包括IE缓存文件、Cookies文件、最近浏览记录、访问过的网址\n"
                + "地址栏网址、IE浏览器历史记录、IE表格和密码记录及临时文件!!!";
            listBox1.Items.Clear();       //清空
        }
        #endregion

        #region 获取IE地址栏输入网址
        /// <summary>
        /// 获取IE地址栏输入网址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            /***************************************************************************
            //由于在使用Environment.GetFolderPath获取Cookies时删除后还有很多Cookies文件
            //我的猜测是其他浏览器的Cookies信息,同时显示Cookies不如显示地址栏网址直接
            //清空listBox
            listBox1.Items.Clear();
            //获取Cookies路径
            string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
            listBox1.Items.Add("文件路径:" + dirPath);

            //遍历所有的文件夹 显示所有文件
            DirectoryInfo dir = new DirectoryInfo(dirPath);     
            foreach (FileInfo file in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                try
                {
                    //文件完成路径+文件名
                    listBox1.Items.Add("Cookies文件  " + file);
                }
                catch (Exception msg)     //异常处理
                {
                    MessageBox.Show(msg.Message);
                }
            }
            ****************************************************************************/

            //清空listBox
            listBox1.Items.Clear();
            //定义注册表顶级节点 其命名空间是using Microsoft.Win32;
            RegistryKey historykey;
            //检索当前用户CurrentUser子项Software\\Microsoft\\Internet Explorer\\typedURLs
            historykey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Internet Explorer\\typedURLs", true);
            if (historykey != null)
            {
                //获取检索的所有值
                String[] names = historykey.GetValueNames();
                foreach (String str in names)
                {
                    listBox1.Items.Add(historykey.GetValue(str).ToString());
                }
            }
            else
            {
                MessageBox.Show(this, "IE地址栏没有要获取的网址", "提示对话框", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region 获取IE最近访问记录
        /// <summary>
        /// 获取IE最近访问记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //**********************************************************************
            // 第二种方法参照:http://blog.sina.com.cn/s/blog_589d32f5010007xf.html
            // 参考资料:
            // http://social.msdn.microsoft.com/Forums/windows/zh-CN/89cfff57-8f84-4b9d-a7a8-bc3aad6e0196/browsing-history
            // http://bbs.csdn.net/topics/290070046
            // http://hi.baidu.com/wayright/item/8af0f88a59aca157e63d19f2
            // http://stackoverflow.com/questions/4305757/iurlhistorystg2clearhistory-does-not-work-from-a-service
            //************************************************************************
            
            try
            {
                //清空listBox
                listBox1.Items.Clear();

                //定义变量
                IUrlHistoryStg2 vUrlHistoryStg2 = (IUrlHistoryStg2)new UrlHistory();
                IEnumSTATURL vEnumSTATURL = vUrlHistoryStg2.EnumUrls();
                STATURL vSTATURL;
                uint vFectched;
                Int64 number = 1;            //记录URL标号

                //循环获取IE浏览器记录
                while (vEnumSTATURL.Next(1, out vSTATURL, out vFectched) == 0)
                {
                    //添加网址的标题
                    if (vSTATURL.pwcsTitle == null)
                    {
                        listBox1.Items.Add(string.Format("第{0}条 无标题", number));
                    }
                    else
                    {
                        listBox1.Items.Add(string.Format("第{0}条 {1}", number, vSTATURL.pwcsTitle));
                    }
                    //添加URL
                    listBox1.Items.Add(string.Format("{0}", vSTATURL.pwcsUrl));
                    //换行
                    listBox1.Items.Add(" ");
                    number++;
                }
                if(number==1)
                {
                    MessageBox.Show(this, "IE历史记录已经删空", "提示对话框", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception msg)  //异常处理
            {
                MessageBox.Show(msg.Message);
            }
        }
        #endregion

        #region 删除上网痕迹
        //可参考我的博客:http://blog.csdn.net/eastmount/article/details/18821221
        //ShellExecute函数ShowCmd参数可选值
        public enum ShowCommands : int
        {
            SW_HIDE = 0,            //隐藏窗口并活动状态另一个窗口(激活)
            SW_SHOWNORMAL = 1,      //用原来的大小和位置显示窗口,激活
            SW_NORMAL = 1,          //同SW_SHOWNORMAL
            SW_SHOWMINIMIZED = 2,   //最小化窗口,激活
            SW_SHOWMAXIMIZED = 3,   //最大化窗口,激活
            SW_MAXIMIZE = 3,        //同SW_SHOWMAXIMIZED
            SW_SHOWNOACTIVATE = 4,  //用最近的大小和位置显示,不改变活动窗口(不激活)
            SW_SHOW = 5,            //同SW_SHOWNORMAL
            SW_MINIMIZE = 6,        //最小化窗口,不激活
            SW_SHOWMINNOACTIVE = 7, //同SW_MINIMIZE
            SW_SHOWNA = 8,          //同SW_SHOWNOACTIVATE
            SW_RESTORE = 9,         //同SW_SHOWNORMAL
            SW_SHOWDEFAULT = 10,    //同SW_SHOWNORMAL
            SW_FORCEMINIMIZE = 11,  //最小化窗口
            SW_MAX = 11             //同SW_SHOWNORMAL
        }

        //调用Win32 API
        //ShellExecute函数运行一个外部程序并对程序进行控制(执行成功返回应用程序句柄)
        [DllImport("shell32.dll")]
        static extern IntPtr ShellExecute(
            IntPtr hwnd,           //用于指定父窗口句柄
            string lpOperation,    //用于指定要进行的操作.其中open打开FileName指定文件或文件夹 print打印 explore浏览(runas edit find)
            string lpFileName,     //用于指定要操作的文件名或要执行的程序文件名
            string lpParameters,   //给要打开程序指定参数.若FileName是可执行程序,则此参数指定命令行参数.否则打开的是文件此处参数为nil
            string lpDirectory,    //缺省目录,用于指定默认目录
            ShowCommands nShowCmd  //打开选项.若FileName参数是可执行程序,则此参数指定程序窗口的初始显示方式,否则此参数为0
        );

        /// <summary>
        /// 删除上网痕迹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.CheckState == CheckState.Unchecked && checkBox2.CheckState == CheckState.Unchecked &&
                    checkBox3.CheckState == CheckState.Unchecked && checkBox4.CheckState == CheckState.Unchecked &&
                    checkBox5.CheckState == CheckState.Unchecked && checkBox6.CheckState == CheckState.Unchecked)
                {
                    MessageBox.Show(this, "请选择要删除的内容,才能实现删除功能.", "提示对话框", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                //借助RunDll32.exe运行Internet选项对应dll文件,实现清除IE缓存
                //根据选中状态执行代码
                if (checkBox6.CheckState == CheckState.Checked)  //清除所有记录
                {
                    //6.Delete All (全部删除)
                    //RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 255
                    //7.Delete All - "Also delete files and settings stored by add-ons"
                    //RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 4351
                    ShellExecute(IntPtr.Zero, "open", "rundll32.exe",
                        "InetCpl.cpl,ClearMyTracksByProcess 255", "", ShowCommands.SW_SHOWMAXIMIZED);
                }
                else
                {
                    if (checkBox1.CheckState == CheckState.Checked)  //清除历史记录
                    {
                        //1.History (历史记录) RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 1
                        ShellExecute(IntPtr.Zero, "open", "rundll32.exe",
                            "InetCpl.cpl,ClearMyTracksByProcess 1", "", ShowCommands.SW_SHOWMAXIMIZED);
                    }
                    if (checkBox2.CheckState == CheckState.Checked)  //清除Cookies
                    {
                        //2.Cookies RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 2
                        ShellExecute(IntPtr.Zero, "open", "rundll32.exe",
                            "InetCpl.cpl,ClearMyTracksByProcess 2", "", ShowCommands.SW_SHOWMAXIMIZED);
                    }
                    if (checkBox3.CheckState == CheckState.Checked)  //清除临时文件
                    {
                        //3.Temporary Internet Files (Internet临时文件) 
                        //RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 8
                        ShellExecute(IntPtr.Zero, "open", "rundll32.exe",
                            "InetCpl.cpl,ClearMyTracksByProcess 8", "", ShowCommands.SW_SHOWMAXIMIZED);
                    }
                    if (checkBox4.CheckState == CheckState.Checked)  //清除表单数据
                    {
                        //4.Form. Data (表单数据) RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 16
                        ShellExecute(IntPtr.Zero, "open", "rundll32.exe",
                            "InetCpl.cpl,ClearMyTracksByProcess 16", "", ShowCommands.SW_SHOWMAXIMIZED);
                    }
                    if (checkBox5.CheckState == CheckState.Checked)   //清除
                    {
                        //5.Passwords (密码) RunDll32.exe InetCpl.cpl,ClearMyTracksByProcess 32
                        ShellExecute(IntPtr.Zero, "open", "rundll32.exe",
                            "InetCpl.cpl,ClearMyTracksByProcess 32", "", ShowCommands.SW_SHOWMAXIMIZED);
                    }

                }
            }
            catch (Exception msg)  //异常处理
            {
                MessageBox.Show(msg.Message);
            }       
        }
        #endregion

    }

    #region COM接口实现获取IE历史记录
    //自定义结构 IUrlHistory
    public struct STATURL
    {
        public static uint SIZEOF_STATURL =
            (uint)Marshal.SizeOf(typeof(STATURL));
        public uint cbSize;                    //网页大小
        [MarshalAs(UnmanagedType.LPWStr)]      //网页Url
        public string pwcsUrl;
        [MarshalAs(UnmanagedType.LPWStr)]      //网页标题
        public string pwcsTitle;
        public System.Runtime.InteropServices.ComTypes.FILETIME
            ftLastVisited,                     //网页最近访问时间
            ftLastUpdated,                     //网页最近更新时间
            ftExpires;
        public uint dwFlags;
    }

    //ComImport属性通过guid调用com 组件
    [ComImport, Guid("3C374A42-BAE4-11CF-BF7D-00AA006946EE"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IEnumSTATURL
    {
        [PreserveSig]
        //搜索IE历史记录匹配的搜索模式并复制到指定缓冲区
        uint Next(uint celt, out STATURL rgelt, out uint pceltFetched);
        void Skip(uint celt);
        void Reset();
        void Clone(out IEnumSTATURL ppenum);
        void SetFilter(
            [MarshalAs(UnmanagedType.LPWStr)] string poszFilter,
            uint dwFlags);
    }

    [ComImport, Guid("AFA0DC11-C313-11d0-831A-00C04FD5AE38"),
        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IUrlHistoryStg2
    {
        #region IUrlHistoryStg methods
        void AddUrl(
            [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
            [MarshalAs(UnmanagedType.LPWStr)] string pocsTitle,
            uint dwFlags);

        void DeleteUrl(
            [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
            uint dwFlags);

        void QueryUrl(
            [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
            uint dwFlags,
            ref STATURL lpSTATURL);

        void BindToObject(
            [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
            ref Guid riid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvOut);

        IEnumSTATURL EnumUrls();
        #endregion

        void AddUrlAndNotify(
            [MarshalAs(UnmanagedType.LPWStr)] string pocsUrl,
            [MarshalAs(UnmanagedType.LPWStr)] string pocsTitle,
            uint dwFlags,
            [MarshalAs(UnmanagedType.Bool)] bool fWriteHistory,
            [MarshalAs(UnmanagedType.IUnknown)] object    /*IOleCommandTarget*/
            poctNotify,
            [MarshalAs(UnmanagedType.IUnknown)] object punkISFolder);

        void ClearHistory();       //清除历史记录
    }

    [ComImport, Guid("3C374A40-BAE4-11CF-BF7D-00AA006946EE")]
    class UrlHistory /* : IUrlHistoryStg[2] */ { }
    #endregion
}
