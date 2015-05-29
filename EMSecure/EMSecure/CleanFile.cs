using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//添加命名空间
using System.IO;                      //DriveInfo
using System.Diagnostics;             //Process
using System.Runtime.InteropServices; //LayoutKind.Sequential
using System.Threading;               //线程Thread
using System.Security.Cryptography;   //RNGCryptoServiceProvider

namespace EMSecure
{
    public partial class CleanFile : Form
    {
        public CleanFile()
        {
            InitializeComponent();
        }

        //全局变量
        public string fileNamePublic = ""; //文件名
        public string filePathPublic = ""; //文件路径

        #region TreeView树形显示磁盘下文件夹
        /// <summary>
        /// IconIndexs类 对应ImageList中5张图片的序列
        /// </summary>
        private class IconIndexes
        {
            public const int MyComputer = 0;      //我的电脑
            public const int ClosedFolder = 1;    //文件夹关闭
            public const int OpenFolder = 2;      //文件夹打开
            public const int FixedDrive = 3;      //磁盘盘符
            public const int MyDocuments = 4;     //我的文档
        }
        #endregion

        #region 初始化程序及载入树状图文件结构
        //定义Num记录listBox1中获取的文件数 是否删除成功
        public int Num = 0;
        public bool IsDelete = false;

        /// <summary>
        /// 窗体加载Load事件 初始化
        /// </summary>
        private void CleanFile_Load(object sender, EventArgs e)
        {
            //赋值
            this.label1.Text = "在保密项目文件中,很多软件都可对其进行恢复.需要对文件进行粉碎,\n";
            this.label1.Text += "确保其无法恢复并删除文件分区上的信息.\n";

            //ComboBox事件
            //角色赋值
            int[] comNum = {1,2,3,4,5,6};
            for (int i = 0; i < 6; i++)
            {
                comboBox1.Items.Add(comNum[i]);
            }

            /*********************************************************/
            /* 下面是重点代码:实现载入对话框时就加载电脑的树状图路径 */
            /*********************************************************/

            //实例化TreeNode类 TreeNode(string text,int imageIndex,int selectImageIndex)            
            TreeNode rootNode = new TreeNode("我的电脑",
                IconIndexes.MyComputer, IconIndexes.MyComputer);  //载入显示 选择显示
            rootNode.Tag = "我的电脑";                            //树节点数据
            rootNode.Text = "我的电脑";                           //树节点标签内容
            this.directoryTree.Nodes.Add(rootNode);               //树中添加根目录

            //显示MyDocuments(我的文档)结点
            var myDocuments = Environment.GetFolderPath           //获取计算机我的文档文件夹
                (Environment.SpecialFolder.MyDocuments);
            TreeNode DocNode = new TreeNode(myDocuments);
            DocNode.Tag = "我的文档";                            //设置结点名称
            DocNode.Text = "我的文档";
            DocNode.ImageIndex = IconIndexes.MyDocuments;         //设置获取结点显示图片
            DocNode.SelectedImageIndex = IconIndexes.MyDocuments; //设置选择显示图片
            rootNode.Nodes.Add(DocNode);                          //rootNode目录下加载节点
            DocNode.Nodes.Add("");

            //循环遍历计算机所有逻辑驱动器名称(盘符)
            foreach (string drive in Environment.GetLogicalDrives())
            {
                //实例化DriveInfo对象 命名空间System.IO
                var dir = new DriveInfo(drive);
                switch (dir.DriveType)           //判断驱动器类型
                {
                    case DriveType.Fixed:        //仅取固定磁盘盘符 Removable-U盘 
                        {
                            //Split仅获取盘符字母
                            TreeNode tNode = new TreeNode(dir.Name.Split(':')[0]);
                            tNode.Name = dir.Name;
                            tNode.Tag = tNode.Name;
                            tNode.ImageIndex = IconIndexes.FixedDrive;         //设置获取结点显示图片
                            tNode.SelectedImageIndex = IconIndexes.FixedDrive; //设置选择显示图片
                            directoryTree.Nodes.Add(tNode);                    //加载驱动节点
                            tNode.Nodes.Add("");
                        }
                        break;
                    case DriveType.Removable:
                         {
                            //Split仅获取盘符字母
                            TreeNode tNode = new TreeNode(dir.Name.Split(':')[0]);
                            tNode.Name = dir.Name;
                            tNode.Tag = tNode.Name;
                            tNode.ImageIndex = IconIndexes.FixedDrive;         //设置获取结点显示图片
                            tNode.SelectedImageIndex = IconIndexes.FixedDrive; //设置选择显示图片
                            directoryTree.Nodes.Add(tNode);                    //加载驱动节点
                            tNode.Nodes.Add("");
                        }
                        break;
                }
            }
            rootNode.Expand();                  //展开树状视图

            //调用SetListView()函数初始化设置ListView
            SetListView();

        }

        /// <summary>
        /// 自定义函数设置ListView控件初始属性
        /// </summary>
        private void SetListView()
        {
            //行和列是否显示网格线
            this.filesList.GridLines = false;
            //显示方式(注意View是Details详细显示)
            this.filesList.View = View.Details;
            //是否可编辑
            this.filesList.LabelEdit = true;
            //没有足够的空间显示时,是否添加滚动条
            this.filesList.Scrollable = true;
            //对表头进行设置
            this.filesList.HeaderStyle = ColumnHeaderStyle.Clickable;
            //是否可以选择行
            this.filesList.FullRowSelect = true;

            //设置listView列标题头 宽度为2/15 9/15 2/15 2/15 
            //其中设置标题头自动适应宽度,-1根据内容设置宽度,-2根据标题设置宽度
            this.filesList.Columns.Add("序号", 2 * filesList.Width / 15);
            this.filesList.Columns.Add("名称", 9 * filesList.Width / 15);
            this.filesList.Columns.Add("大小", 2 * filesList.Width / 15);
            this.filesList.Columns.Add("类型", 2 * filesList.Width / 15);
        }
        #endregion

        #region Treelist结点展开事件
        /// <summary>
        /// 在结点展开后发生 展开子结点
        /// </summary>
        private void directoryTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            e.Node.Expand();
        }
 
        /// <summary>
        /// 在将要展开结点时发生 加载子结点
        /// </summary>
        private void directoryTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeViewItems.Add(e.Node);
        }
        #endregion

        #region 自定义类TreeViewItems Add加载子目录
        /// <summary>
        /// 自定义类TreeViewItems 调用其Add(TreeNode e)方法加载子目录
        /// </summary>
        public static class TreeViewItems
        {
            public static void Add(TreeNode e)
            {
                //try..catch异常处理
                try
                {
                    //判断"我的电脑"Tag 上面加载的该结点没指定其路径
                    if (e.Tag.ToString() != "我的电脑")
                    {
                        e.Nodes.Clear();                               //清除空节点再加载子节点
                        TreeNode tNode = e;                            //获取选中\展开\折叠结点
                        string path = tNode.Name;                      //路径  

                        //获取"我的文档"路径
                        if (e.Tag.ToString() == "我的文档")
                        {
                            path = Environment.GetFolderPath           //获取计算机我的文档文件夹
                                (Environment.SpecialFolder.MyDocuments);
                            MessageBox.Show("秀璋提示你,粉碎文件尽量粉碎常用磁盘目录.\n系统目录可能无法访问给你带来不便!",
                                "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        if (e.Tag.ToString() == "C:\\")
                        {
                            MessageBox.Show("秀璋提示你,粉碎文件尽量不要粉碎C盘.\n系统目录可能无法访问或删除无法恢复给你带来不便!",
                               "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        //获取指定目录中的子目录名称并加载结点
                        string[] dics = Directory.GetDirectories(path);
                        foreach (string dic in dics)
                        {
                            TreeNode subNode = new TreeNode(new DirectoryInfo(dic).Name); //实例化
                            subNode.Name = new DirectoryInfo(dic).FullName;               //完整目录
                            subNode.Tag = subNode.Name;
                            subNode.ImageIndex = IconIndexes.ClosedFolder;       //设置获取节点显示图片
                            subNode.SelectedImageIndex = IconIndexes.OpenFolder; //设置选择节点显示图片
                            tNode.Nodes.Add(subNode);
                            subNode.Nodes.Add("");                               //加载空节点 实现+号
                        }
                    }
                }
                catch (Exception msg)
                {
                    MessageBox.Show(msg.Message);                   //异常处理
                }
            }
        }
        #endregion   

        #region ListView显示选中文件夹中文件内容
        /// <summary>
        /// 获取节点的路径:递归调用产生节点对应文件夹的路径
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetPathFromNode(TreeNode node)
        {
            //注意:树形控件中我只赋值Tag\Name,使用Text时赋值即可使用
            if (node.Parent == null)
            {
                return node.Name;
            }
            //Path.Combine组合产生路径 如 Path.Combine("A","B")则生成"A\\B"
            return Path.Combine(GetPathFromNode(node.Parent), node.Name);
        }

        /// <summary>
        /// 更改选定内容后发生 后去当前节点名字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void directoryTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                //定义变量
                long length;                        //文件大小
                string path;                        //文件路径
                TreeNode clickedNode = e.Node;      //获取当前选中结点

                //移除ListView所有项 
                this.filesList.Items.Clear();

                //获取路径赋值path              
                if (clickedNode.Tag.ToString() == "我的文档")
                {
                    //获取计算机我的文档文件夹
                    path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                else
                {
                    //通过自定义函数GetPathFromNode获取结点路径
                    path = GetPathFromNode(clickedNode);
                }

                //由于"我的电脑"为空结点,无需处理,否则会出现路径获取错误或没有找到"我的电脑"路径
                if (clickedNode.Tag.ToString() != "我的电脑")
                {
                    //数据更新 UI暂时挂起直到EndUpdate绘制控件,可以有效避免闪烁并大大提高加载速度
                    this.filesList.BeginUpdate();
                    //实例目录与子目录
                    DirectoryInfo dir = new DirectoryInfo(path);
                    //为全局变量赋值
                    filePathPublic = path;     

                    //获取当前目录文件列表
                    FileInfo[] fileInfo = dir.GetFiles();
                    //循环输出获取文件信息
                    for (int i = 0; i < fileInfo.Length; i++)
                    {
                        ListViewItem listItem = new ListViewItem();
                        //listItem.SubItems[0].Text = fileInfo[i].Name;             //文件名(方法二)
                        listItem.Text = "[" + (i + 1) + "] ";                       //序列
                        listItem.ForeColor = Color.Blue;                            //设置行颜色
                        listItem.SubItems.Add(fileInfo[i].Name);           //显示文件名
        
                        //length/1024转换为KB字节数整数值 Ceiling返回最小整数值 Divide除法
                        length = fileInfo[i].Length;                                //获取当前文件大小字节
                        listItem.SubItems.Add(Math.Ceiling(decimal.Divide(length, 1024)) + " KB");

                        //获取文件最后访问时间
                        //listItem.SubItems.Add(fileInfo[i].LastWriteTime.ToString());

                        //获取文件扩展名时可用Substring除去点 否则显示".txt文件"
                        listItem.SubItems.Add(fileInfo[i].Extension + "文件");
                        //加载数据至filesList
                        this.filesList.Items.Add(listItem);
                    }
                    //结束数据处理,UI界面一次性绘制 否则可能出现闪动情况
                    this.filesList.EndUpdate();
                }
            }
            catch (Exception msg)  //异常处理
            {
                MessageBox.Show(msg.Message);
            }

        }
        #endregion   

        #region 鼠标右击事件 添加contextMenuStrip控件
        private void filesList_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                //禁止多选
                filesList.MultiSelect = false;
                //鼠标右键
                if (e.Button == MouseButtons.Right)
                {
                    //filesList.ContextMenuStrip = contextMenuStrip1;
                    //选中列表中数据才显示 空白处不显示
                    String fileName = filesList.SelectedItems[0].Text; //获取选中文件名
                    Point p = new Point(e.X, e.Y);
                    contextMenuStrip1.Show(filesList, p);
                }
            }
            catch (Exception msg) //异常处理
            {
                MessageBox.Show(msg.Message);
            }
        }
        #endregion

        #region SDelete参数及调用
        //******************************************************************//
        // SDelete程序删除一个或多个文件或目录,亦可清理逻辑磁盘上可用空间
        // sdelete [-p passes] [-s] [-q] <文件或目录>  //<file or directory>
        // sdelete [-p passes] [-z] [驱动器符号]       //[drive letter]
        // 参数含义
        // -a Remove Read-Only attribute
        // -c Clean free space
        // -p passes 指定覆盖操作的执行遍数(default is 1)
        // -s or -r 对子目录执行递归操作
        // -q 不显示错误(静默模式)
        // -z 清理可用空间(good for virtual disk optimization)
        //******************************************************************//

        //调用SDelete删除程序
        private void SDelete()
        {
            //文件没有选中
            if (this.filesList.SelectedItems.Count == 0)
                return;
            //获取选中文件
            var selectedItem = this.filesList.SelectedItems[0];
            //全局变量文件名 SubItems[1]表示文件名
            fileNamePublic = filePathPublic + "\\" + selectedItem.SubItems[1].Text;
            //删除遍数
            int number = 2;

            //粉碎文件
            try
            {
                //检查文件是否存在
                if (!File.Exists(fileNamePublic))
                {
                    MessageBox.Show("输入文件不存在!");
                }
                else
                {
                    //初始化进程 命名空间 using System.Diagnostics
                    Process p = new Process();
                    //设置要启动应用程序或文档名
                    p.StartInfo.FileName = @"C:\WINDOWS\system32\cmd.exe";
                    string command = "/c sdelete" + string.Format("-p {0} -q \"{1}\"", number, fileNamePublic);
                    p.StartInfo.Arguments = command;              //参数
                    p.StartInfo.UseShellExecute = false;          //是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true;     //接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true;    //由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true;     //重定向标准错误输出
                    p.StartInfo.CreateNoWindow = true;            //不显示程序窗口
                    p.Start();                                    //启动程序
                    string output = p.StandardOutput.ReadToEnd();
                    //文件不存在记录输出流信息
                    if (!File.Exists(fileNamePublic))
                    {
                        MessageBox.Show("文件粉碎成功");
                    }
                }
            }
            catch (Exception msg) //异常处理
            {
                MessageBox.Show(msg.Message);
            }

        }
        #endregion

        #region 打开文件操作
        //设置contextMenuStrip1控件中"打开文件"Name为openFile
        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //文件没有选中
            if (this.filesList.SelectedItems.Count == 0)
                return;
            //获取选中文件
            var selectedItem = this.filesList.SelectedItems[0];
            //全局变量文件名 SubItems[1]表示文件名
            fileNamePublic = filePathPublic + "\\" + selectedItem.SubItems[1].Text;
            //MessageBox.Show(fileNamePublic);
            try
            {
                //实例化一个新的Process类 命名空间using System.Diagnostics;
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = fileNamePublic; //指定要启动的文件路径
                    p.StartInfo.CreateNoWindow = false; //在当前窗口启动程序
                    //指定窗口的显示样式
                    p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
                    p.StartInfo.UseShellExecute = true; //使用操作系统的shell启动进程
                    p.Start(); //开始打开文件
                }
            }
            catch (Exception msg) //异常处理
            {
                MessageBox.Show(msg.Message);
            }   
        }
        #endregion

        #region 删除文件
        //删除文件
        //文件删除到回收站中
        private const int FO_DELETE = 3;               //删除
        private const int FOF_SILENT = 0x0004;         //不显示进度条提示框
        private const int FOF_NOCONFIRMATION = 0x0010; //不出现任何对话框
        private const int FOF_ALLOWUNDO = 0x0040;      //允许撤销
        private const int FOF_NOCONFIRMMKDIR = 0x0200; //创建文件夹的时候不用确认

        //添加命名空间using System.Runtime.InteropServices;
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

        //设置contextMenuStrip1控件中"删除文件"Name为deleteFile
        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //文件没有选中
            if (this.filesList.SelectedItems.Count == 0)
                return;
            //获取选中文件
            var selectedItem = this.filesList.SelectedItems[0];
            //全局变量文件名 SubItems[1]表示文件名
            fileNamePublic = filePathPublic + "\\" + selectedItem.SubItems[1].Text;
            //MessageBox.Show(fileNamePublic);
            try
            {
                if (MessageBox.Show("确认要删除该最近浏览文件信息?", "提示",
                            System.Windows.Forms.MessageBoxButtons.YesNo,
                            System.Windows.Forms.MessageBoxIcon.Question) ==
                            System.Windows.Forms.DialogResult.Yes)
                {
                    //删除文件
                    Delete(fileNamePublic, true);
                    //移除文件
                    foreach (ListViewItem item in this.filesList.SelectedItems)
                    {
                        this.filesList.Items.Remove(item);
                    }
                }
                MessageBox.Show(this, "成功删除了文件！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception msg) //异常处理
            {
                MessageBox.Show(msg.Message);
            }   
        }
        #endregion

        #region 粉碎文件
        //最初使用SDelete实现粉碎文件,但是cmd能实现,使用Process调用总是提示Sdelete命令不存在
        //使用自己的方法删除也挺好的,感觉可能没微软提供的完成,但原理更加清晰
        //杨友山 http://blog.csdn.net/yysyangyangyangshan/article/details/18423731
        //该方法是删除文件并填充文件内容使其不可恢复
        private void cleanFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //文件没有选中
            if (this.filesList.SelectedItems.Count == 0)
                return;
            //获取选中文件
            var selectedItem = this.filesList.SelectedItems[0];
            //全局变量文件名 SubItems[1]表示文件名
            fileNamePublic = filePathPublic + "\\" + selectedItem.SubItems[1].Text;
            //MessageBox.Show(fileNamePublic);

            try
            {
                if (string.IsNullOrEmpty(fileNamePublic))
                {
                    MessageBox.Show("请选择要粉碎的文件!","信息提示", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //粉碎文件
                if (MessageBox.Show("确认要粉碎该文件并使其不可恢复?", "提示",
                            System.Windows.Forms.MessageBoxButtons.YesNo,
                            System.Windows.Forms.MessageBoxIcon.Question) ==
                            System.Windows.Forms.DialogResult.Yes)
                {
                    //获取删除文件的遍数                    
                    int number = 1;
                    //未选择时为空
                    if (comboBox1.Text.ToString() != "")
                    {
                        number = Convert.ToInt32(comboBox1.Text.ToString());
                        //MessageBox.Show(comboBox1.Text.ToString());
                    }

                    for (int i = 0; i < number; i++)
                    {
                        //粉碎文件
                        DoAbsoluteDeleteFile(fileNamePublic);
                    }

                    //移除文件
                    foreach (ListViewItem item in this.filesList.SelectedItems)
                    {
                        this.filesList.Items.Remove(item);
                    }
                }
            }
            catch (Exception msg) //异常处理
            {
                MessageBox.Show(msg.Message);
            }
        }

        //粉碎文件事件
        public void DoAbsoluteDeleteFile(object filePath)
        {
            try
            {
                string filename = filePath.ToString();        //文件名
                if (string.IsNullOrEmpty(filename)) return;   //文件为空或不存在

                if (File.Exists(filename))
                {
                    //设置文件属性为标准文件
                    File.SetAttributes(filename, FileAttributes.Normal);
                    //设置文件大小
                    double sectors = Math.Ceiling(new FileInfo(filename).Length / 512.0);
                    byte[] dummyBuffer = new byte[512];
                    //使用加密服务提供程序CSP实现加密随机数生成器
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    //文件流
                    FileStream inputStream = new FileStream(filename, FileMode.Open);
                    inputStream.Position = 0;
                    for (int sectorsWritten = 0; sectorsWritten < sectors; sectorsWritten++)
                    {
                        //加密随机值填充数组
                        rng.GetBytes(dummyBuffer);
                        inputStream.Write(dummyBuffer, 0, dummyBuffer.Length);
                        sectorsWritten++;
                    }
                    inputStream.SetLength(0);
                    inputStream.Close();
                    //时间 年月日时分秒
                    DateTime dt = new DateTime(2049, 1, 1, 0, 0, 0);
                    //创造该文件日期和时间
                    File.SetCreationTime(filename, dt);
                    //设置上次访问文件日期和时间
                    File.SetLastAccessTime(filename, dt);
                    //设置上次写入文件的日期和时间
                    File.SetLastWriteTime(filename, dt);
                    File.Delete(filename);
                    MessageBox.Show("文件粉碎成功!", "信息提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception e) //异常处理
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion

        #region 配置选项
        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("请选择要粉碎文件的遍数,默认1遍!", "信息提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion
    }
}
