using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;              //File命名空间
using System.Runtime.InteropServices;

namespace EMSecure
{
    public partial class CleanRecycle : Form
    {
        public CleanRecycle()
        {
            InitializeComponent();
        }

        #region 初始化
        //定义Num记录listBox1中获取的文件数 是否删除成功
        public int Num = 0;
        public bool IsDelete = false;

        //窗体载入初始化窗体
        private void CleanRecycle_Load(object sender, EventArgs e)
        {
            this.label1.Text = "删除电脑中的文件、并清空回收站,让电脑变得更加迅捷\n你的电脑值得拥有!!!";
            this.label2.Text = "";
            this.label3.Text = "温馨提示";
            listBox1.Items.Clear();       //清空
        }
        #endregion

        #region 添加文件
        /// <summary>
        /// 点击"添加文件"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName != "")
            {
                listBox1.Items.Add(openFileDialog1.FileName.ToString());
                Num++;
                IsDelete = false;
            }
            else
            {
                MessageBox.Show(this, "对不起,打开文件失败!", "提示对话框", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 文件添加错误,back操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();       //清空
            Num = 0;
            MessageBox.Show(this, "列表栏添加要删除的文件已清空", "提示对话框", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        #endregion

        #region 删除文件
        
        //文件删除到回收站中
        private const int FO_DELETE = 3;               //删除
        private const int FOF_SILENT = 0x0004;         //不显示进度条提示框
        private const int FOF_NOCONFIRMATION = 0x0010; //不出现任何对话框
        private const int FOF_ALLOWUNDO = 0x0040;      //允许撤销
        private const int FOF_NOCONFIRMMKDIR = 0x0200; //创建文件夹的时候不用确认

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEOPSTRUCT
        {
            public int hwnd;                     //父窗口句柄,0为桌面
            public int wFunc;                    //功能标志 FO_COPY复制 FO_DELETE删除 FO_MOVE移动 FO_RENAME重命名
            public string pFrom;                 //source file源文件或者源文件夹
            public string pTo;                   //destination目的文件或文件夹
            public int fFlags;                   //控制文件的标志位 FOF_ALLOWUNDO 准许撤销 FOF_CONFIRMMOUSE 没有被使用
            public bool fAnyOperationsAborted;
            public int hNameMappings;
            public string lpszProgressTitle;
        }

        //SHFileOperation外壳函数 实现文件操作 参数SHFILEOPSTRUCT结构
        [DllImport("shell32.dll")]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        /// <summary>
        /// 删除文件 Delete("c:\\test.txt",true) 把"c:/test.text"删除到回收箱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static int Delete(string sPath, bool recycle)
        {
            SHFILEOPSTRUCT FileOp = new SHFILEOPSTRUCT();
            FileOp.hwnd = 0;
            FileOp.wFunc = FO_DELETE;      //实现操作是删除文件
            FileOp.fFlags = 0;
            FileOp.fFlags = FileOp.fFlags | FOF_SILENT;
            FileOp.fFlags = FileOp.fFlags | FOF_NOCONFIRMATION;
            FileOp.fFlags = FileOp.fFlags | FOF_NOCONFIRMMKDIR;
            if (recycle)
            {
                FileOp.fFlags = FileOp.fFlags | FOF_ALLOWUNDO;
            }
            FileOp.pFrom = sPath + "\0";
            return SHFileOperation(ref FileOp);
        }

        /// <summary>
        /// 点击"删除文件"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            try 
            {
                //是否删除成功
                IsDelete = false;

                if (Num == 0)
                {
                    MessageBox.Show(this, "请添加文件才能删除!", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //循环删除listBox控件中的文件
                for (int i = 0; i < Num; i++)
                {
                    if (listBox1.Items[i].ToString() == "")
                    {
                        MessageBox.Show(this, "文件路径及名称不能为空！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        label2.Text = "秀璋提醒您:\n文件删除失败!\n☺☺☺☺☺☺☺";
                    }
                    else if (!File.Exists(listBox1.Items[i].ToString()))
                    {
                        MessageBox.Show(this, "要删除的文件不存在！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        label2.Text = "秀璋提醒您:\n文件删除失败!\n☺☺☺☺☺☺☺";
                    }
                    else
                    {
                        //使用File.Delete删除是不添加至回收站的,系统文件不能访问
                        //File.Delete(listBox1.Items[i].ToString());

                        //自定义函数Delete删除至回收站
                        Delete(listBox1.Items[i].ToString(),true);
                        IsDelete = true;
                    }
                }
                if (IsDelete) //删除成功
                {
                    Num = 0;
                    listBox1.Items.Clear();
                    MessageBox.Show(this, "成功删除了文件！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    label2.Text = "秀璋提醒您:\n文件删除成功!\n☺☺☺☺☺☺☺";
                }                                  
            }
            catch(Exception msg)  //异常处理
            {
                MessageBox.Show(msg.Message);
            }

        }
        #endregion

        #region 清空回收站

        //变量
        const int SHERB_NOCONFIRMATION = 0x000001;   //不显示确认删除的对话框
        const int SHERB_NOPROGRESSUI = 0x000002;     //不显示删除过程的进度条
        const int SHERB_NOSOUND = 0x000004;          //当删除完成时,不播放声音

        [DllImportAttribute("shell32.dll")]           //声明API函数 System.Runtime.InteropServices
        private static extern int SHEmptyRecycleBin(IntPtr handle, string root, int falgs);

        /// <summary>
        /// 点击"清空回收站"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //不显示进度条声音和过程删除
            //SHEmptyRecycleBin(this.Handle, "", SHERB_NOCONFIRMATION + SHERB_NOPROGRESSUI + SHERB_NOSOUND);
            SHEmptyRecycleBin(this.Handle, "", 0);
            label2.Text = "秀璋提醒您:\n回收站已清空!\n☺☺☺☺☺☺☺";
        }
        #endregion

    }
}
