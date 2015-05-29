using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;            //DirectoryInfo

namespace EMSecure
{
    public partial class CleanWord : Form
    {
        public CleanWord()
        {
            InitializeComponent();
        }

        #region 初始化
        //定义Num记录listBox1中获取的文件数 是否删除成功
        public int Num = 0;
        public bool IsDelete = false;

        private void CleanWord_Load(object sender, EventArgs e)
        {
            //label1赋值
            this.label1.Text = "查看和删除电脑中中电脑最近使用办公文当信息,其中主要是Office办公软件,\n";
            this.label1.Text += "包括word、ppt和excel等.同时可以获取文档的文件名、使用时间、文档路径等信息";

            //设置textbox居中并初始化
            this.textBox1.TextAlign = HorizontalAlignment.Center;
            this.textBox1.Text = "0个文件";
        }
        #endregion

        #region 获取最近使用office记录
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //清空listBox
                listBox1.Items.Clear();
                Num = 0;

                //获取recent路径 GetFolderPath获取系统特殊路径
                //string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos);
                //异常处理 文件夹不存在会报错
                string dirPath = @"C:\Users\dell\AppData\Roaming\Microsoft\Office\Recent";
                listBox1.Items.Add("文件路径:" + dirPath);
                listBox1.Items.Add("文件信息:");

                //遍历所有的文件夹 显示所有文件
                DirectoryInfo dir = new DirectoryInfo(dirPath);
                foreach (FileInfo file in dir.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        string FileNo = Num.ToString().PadRight(4, ' ');  //左对齐编号
                        //获取信息无后缀名".lnk" 快捷图标
                        string FileName = System.IO.Path.GetFileNameWithoutExtension(file.ToString());

                        //index为系统文件,并非Office文档
                        if (FileName != "index")
                        {
                            Num++;               //Num记录文件总数和序列号
                            //文件序列+文件名
                            listBox1.Items.Add("No." + FileNo + " 【" + FileName + "】");
                        }

                    }
                    catch (Exception msg)     //异常处理
                    {
                        MessageBox.Show(msg.Message);
                    }
                }

                if (Num == 0)
                {
                    MessageBox.Show("最近浏览Office办公文档信息已空!", "信息提示",
                        System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Information);
                }

                //显示获取文件总数
                this.textBox1.Text = Num + "个文件";
            }
            catch (Exception ex) //异常处理
            {
                MessageBox.Show(ex.Message);
            }
        }  
        #endregion

        #region 删除最近浏览文档 lnk快捷图标
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //获取recent路径 GetFolderPath获取系统特殊路径
                //string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos);
                string dirPath = @"C:\Users\dell\AppData\Roaming\Microsoft\Office\Recent";
                //设置文件类型
                string type = "*.lnk";
                //获取该目录下所有该类型的文件
                string[] files = Directory.GetFiles(dirPath, type);
                //定义变量
                FileInfo fi = null;
                int count = 0;

                if (MessageBox.Show("确认要删除该最近浏览Office办公文档信息?", "提示",
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
                        MessageBox.Show("最近浏览Office办公文档信息已经清空!", "信息提示",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Information);
                    }
                    else
                    {
                        //显示删除操作后的信息
                        MessageBox.Show("最近浏览Office办公文档信息删除成功!", "信息提示",
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

    }
}
