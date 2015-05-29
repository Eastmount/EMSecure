using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//添加命名控件
using System.IO;           //文件
using Microsoft.Win32;     //注册表

namespace EMSecure
{
    public partial class CleanRecent : Form
    {
        public CleanRecent()
        {
            InitializeComponent();
        }

        #region 初始化
        //定义Num记录listBox1中获取的文件数 是否删除成功
        public int Num = 0;            //最近浏览文件个数
        public int NumAdd = 0;         //地址栏文件个数
        public bool IsDelete = false;  //是否删除

        //窗体载入初始化窗体
        private void CleanRecent_Load(object sender, EventArgs e)
        {
            //label1赋值
            this.label1.Text = "查看和删除电脑中最近浏览文件信息及路径,包括打开的办公文件、视频文件、\n";
            this.label1.Text +="音乐文件、文件夹等各种文件.通过Ctrl+R打开运行输入Recent亦可查看.";

            //设置textbox居中并初始化
            this.textBox1.TextAlign = HorizontalAlignment.Center;
            this.textBox1.Text = "0个文件";
            
        }
        #endregion

        #region 通过fileSystemWatcher控件获取最近浏览文件记录并显示
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //清空listBox1
                listBox1.Items.Clear();
                Num = 0;
                //通过自定义类
                foreach (var file in RecentlyFileHelper.GetRecentlyFiles())
                {
                    listBox1.Items.Add(file);
                    Num++;
                }

                //获取recent路径 GetFolderPath获取系统特殊路径
                var recentFolder = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

                //fileSystemWatcher控件 监视文件系统更改通知
                //在文件或目录发生更改时引发事件
                fileSystemWatcher1.Path = recentFolder;
                fileSystemWatcher1.Created += new System.IO.FileSystemEventHandler(fileSystemWatcher1_Created);

                //显示获取文件总数
                this.textBox1.Text = Num + "个文件";
            }
            catch (Exception msg) //异常处理
            {
                MessageBox.Show(msg.Message);
            }
        }

        //当在指定Path(即recent路径)中创建文件和目录时增加ShortCut
        private void fileSystemWatcher1_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            listBox1.Items.Add(RecentlyFileHelper.GetShortcutTargetFile(e.FullPath));
        }
        #endregion

        #region 删除最近浏览文件记录
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {               
                //获取recent路径 GetFolderPath获取系统特殊路径
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
                //设置文件类型
                string type = "*.lnk";
                //获取该目录下所有该类型的文件
                string[] files = Directory.GetFiles(path,type);
                //定义变量
                FileInfo fi = null;
                int count = 0;

                if (MessageBox.Show("确认要删除该最近浏览文件信息?", "提示",
                            System.Windows.Forms.MessageBoxButtons.YesNo,
                            System.Windows.Forms.MessageBoxIcon.Question) ==
                            System.Windows.Forms.DialogResult.Yes)
                {
                    //循环调用Delete方法删除文件
                    foreach (string file in files)
                    {
                        fi = new FileInfo(file);
                        fi.Delete();
                        count++;
                    }
                    //删除文件0个 即该文件夹已经清空时提示
                    if (count == 0)
                    {
                        MessageBox.Show("最近浏览文件信息已经清空!", "信息提示",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    else
                    {
                        //显示删除操作后的信息
                        MessageBox.Show("最近浏览文件信息删除成功!", "信息提示",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    this.textBox1.Text = count + "个文件";
                    listBox1.Items.Clear();
                }
                
            }
            catch (Exception msg) //异常处理
            {
                MessageBox.Show(msg.Message);
            }
        }
        #endregion

        #region 获取我的电脑地址栏使用记录
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //清除内容
                listBox1.Items.Clear();
                NumAdd = 0;
                //定义注册表顶级结点 命名空间Microsoft.Win32
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey
                    ("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\TypedPaths", true);
                //判断键是否存在
                if (key != null)
                {
                    //检索包含此项关联的所有值名称 即url1 url2 url3
                    string[] names = key.GetValueNames();
                    foreach (string str in names)
                    {
                        //获取url中相关联的值
                        listBox1.Items.Add(key.GetValue(str).ToString());
                        NumAdd++;
                    }
                    //显示获取文件总数
                    this.textBox1.Text = NumAdd + "个文件";
                }

                /* 注册表的使用操作
                //创建键
                //在HKEY_CURRENT_USER下创建Eastmount键
                RegistryKey test1 = Registry.CurrentUser.CreateSubKey("Eastmount");
                //创建键结构 HKEY_CURRENT_USER\Software\Eastmount\test2
                RegistryKey test2 = Registry.CurrentUser.CreateSubKey(@"Software\Eastmount\test2");

                //删除HKEY_CURRENT_USER下创建Eastmount键
                Registry.CurrentUser.DeleteSubKey("Eastmount");
                //删除创建的子键test2
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Eastmount");

                //获取Environment中路径
                string strPath;
                strPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Environment",
                    "path", "Return this default if path does not exist");
                MessageBox.Show(strPath);

                //设置键值Version=1.25
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\YourSoftware", "Version", "1.25");     
                //设置键值
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths", "url4", @"E:\dota");
                
                //隐藏桌面我的电脑
                RegistryKey rgK = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\HideDesktopIcons\NewStartPanel");
                rgK.SetValue("{20D04FE0-3AEA-1069-A2D8-08002B30309D}",0);
                */
            }
            catch (Exception msg) //异常处理
            {
                MessageBox.Show(msg.Message);
            }
        }
        #endregion


    }
}
