using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.IO;

namespace EMSecure
{
    public partial class EMSecure : Form
    {
        public EMSecure()
        {
            InitializeComponent();
        }


        #region 设置界面属性注意事项
        //*************************************************************************
        // 窗体Form1属性注意事项                                      
        // 1.StartPostion窗体第一次出现位置设置为CenterScreen屏幕中心
        // 2.WindowState窗体初始可视状态设置为Normal(min\max)
        // 3.ShowIcon指示是否在窗体标题栏中显示图标设置为True(最小化状态栏显示图标)
        // 4.ShowInTaskbar确定窗体是否出现在Windows任务栏中设置为True
        // 5.BackgroundImageLayout背景图像布局设置为Stretch(拉伸)、Tile(默认)、
        //   None(左显)、None+RightToLeft=Yes(右显)、Center(居中)
        // 6.FormBorderStyle窗体边框和标题栏外观设置为None(无标题窗口)
        //*************************************************************************
        
        //*************************************************************************
        // button透明属性设置注意事项
        // 1.设置背景颜色为Transparent
        // 2.设置FlatStyle用户将鼠标移动到控件单击该控件外观设置为Flat
        // http://blog.csdn.net/yakson/article/details/8560524 详细介绍设置透明按钮
        //*************************************************************************

        //*************************************************************************
        // C#加载图片文件夹注意事项
        // 右击项目->属性->选择资源->添加资源 
        // http://www.cnblogs.com/MicroTeam/archive/2011/03/06/1972575.html
        //*************************************************************************

        //*************************************************************************
        // 移动无标题窗体的文章
        // http://hi.baidu.com/vkwendapgeahije/item/fb031af6f3bb760b85d27857
        //*************************************************************************

        #endregion

        #region 窗体特效
        public const Int32 AW_HOR_POSITIVE = 0x00000001;        //从左到右显示
        public const Int32 AW_HOR_NEGATIVE = 0x00000002;        //从右到左显示
        public const Int32 AW_VER_POSITIVE = 0x00000004;        //从上到下显示
        public const Int32 AW_VER_NEGATIVE = 0x00000008;        //从下到上显示
        public const Int32 AW_CENTER = 0x00000010;              //从中间向四周

        //隐藏窗口(关闭窗体使用),若使用AW_HIDE则使窗口向内重叠收缩窗口,否则使窗口向外扩展展开窗口
        public const Int32 AW_HIDE = 0x00010000;
        //激活窗口,在使用了AW_HIDE标志后不能使用这个标志
        public const Int32 AW_ACTIVATE = 0x00020000;
        //使用滑动类型,缺省则为滚动动画类型.当使用AW_CENTER标志时,这个标志就被忽略
        public const Int32 AW_SLIDE = 0x00040000;
        //透明渐变显示
        public const Int32 AW_BLEND = 0x00080000;

        //重写API函数,用来执行窗体动画显示操作 命名空间System.Runtime.InteropServices
        [DllImportAttribute("user32.dll")]
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        #endregion 窗体特效

        #region API鼠标函数mouse_event模拟鼠标操作
        //该函数功能:鼠标击键和鼠标动作
        //参考文章: http://hi.baidu.com/kind064100611/item/5062041ed1c5f2f686ad4ec1
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern void mouse_event(
            int dwFlags,      //标志位集,指定点击按钮和鼠标动作多种情况
            int dx,           //horizontal position or change
            int dy,           //vertical position or change
            int cButtons,     //wheel movement 指定鼠标轮移动的数量
            int dwExtraInfo   //application-defined information
        );

        const int MOUSEEVENTF_MOVE = 0x0001;        //移动鼠标 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;    //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTUP = 0x0004;      //模拟鼠标左键抬起 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;   //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTUP = 0x0010;     //模拟鼠标右键抬起 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;  //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;    //模拟鼠标中键抬起 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;    //标示是否采用绝对坐标

        //1.鼠标左键按下和松开两个事件的组合即一次单击
        //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0 );
        //2.模拟鼠标右键单击事件
        //mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0 );
        //3.两次连续的鼠标左键单击事件 构成一次鼠标双击事件
        //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0 );
        //mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0 );

        #endregion

        #region 鼠标移动操作\鼠标样式变换
        //该函数从当前线程中窗口释放鼠标捕获
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        
        //发送消息移动窗体
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public const int WM_SYSCOMMAND = 0x0112;    //向窗口发送消息
        public const int SC_MOVE = 0xF010;          //向窗口发送移动的消息
        public const int HTCAPTION = 0x0002;

        //鼠标位于窗体(底部位置)按下移动操作
        private void EMSecure_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        //鼠标位于paenl1(顶部位置)按下移动操作
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        //鼠标进入但顶部是panel控件,故需要鼠标移动至最下方Form后才能变换,响应panel事件
        private void EMSecure_MouseEnter(object sender, EventArgs e)
        {
            
        }

        //鼠标进入item手势变换
        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            //Hand手型鼠标 默认为Default鼠标箭头
            panel_mol1.Cursor = Cursors.Hand;
            panel_mol2.Cursor = Cursors.Hand;
            panel_mol3.Cursor = Cursors.Hand;
            panel_mol4.Cursor = Cursors.Hand;
            panel_mol5.Cursor = Cursors.Hand;
            panel_mol6.Cursor = Cursors.Hand;
            minButton.Cursor = Cursors.Hand;
            closeButton.Cursor = Cursors.Hand;
        }
        #endregion

        #region 设置透明罩\切换Pandel功能图标
        //*********************************************************************
        // 设置透明罩需要引用自定义控件MyOpaqueLayer
        // 1.由于采用"添加组件\控件"不能生成namespace MyOpaqueLayer
        //   所以采用拖拽至项目方法实现添加控件透明罩
        // 2.添加自定义类OpaqueCommand,其中含有显示透明罩和隐藏透明罩两种方法
        //   不会直接向Form[设计]中直接添加透明罩控件,故采用类中方法实现
        // 3.添加透明罩时,应该采用table控件1-6顺序添加
        //   由于不会使用ToolStrip控件和拖拽透明罩控件,只能添加6个Panel实现
        // 参考资料:
        // http://blog.csdn.net/yysyangyangyangshan/article/details/7078471
        // http://www.csharpwin.com/csharpspace/13238r8078.shtml
        // http://wenku.baidu.com/view/89a47f6e58fafab069dc02bf.html
        // http://www.cnblogs.com/JuneZhang/archive/2012/07/06/2579215.html
        //
        // 提醒:该开始采用MouseEnter进入设置透明罩,但由于ShowOpaqueLayer函数
        //      后可能阻塞了MouseDown,所以不能点击.但他俩函数能共存的.
        //      最后采用MouseDown,同时初始化是要图片Enable令为false,才能点击
        //      panel响应点击函数.
        //*********************************************************************

        //自定义类OpaqueCommand
        OpaqueCommand cmd1 = new OpaqueCommand();
        OpaqueCommand cmd2 = new OpaqueCommand();
        OpaqueCommand cmd3 = new OpaqueCommand();
        OpaqueCommand cmd4 = new OpaqueCommand();
        OpaqueCommand cmd5 = new OpaqueCommand();
        OpaqueCommand cmd6 = new OpaqueCommand();
        
        //定义点击panel时透明罩情况
        bool isClick1 = false;
        bool isClick2 = false;
        bool isClick3 = false;
        bool isClick4 = false;
        bool isClick5 = false;
        bool isClick6 = false;
        
        //**************************************************************
        //鼠标进入"清除IE"
        //private void panel_mol1_MouseEnter(object sender, EventArgs e){ }

        //鼠标点击"清除IE"
        private void panel_mol1_MouseDown(object sender, MouseEventArgs e)
        {
            //透明罩设置 没点击才取消透明罩
            cmd1.ShowOpaqueLayer(panel_mol1, 125, true);
            if (isClick2 == false) cmd2.HideOpaqueLayer();
            if (isClick3 == false) cmd3.HideOpaqueLayer();
            if (isClick4 == false) cmd4.HideOpaqueLayer();
            if (isClick5 == false) cmd5.HideOpaqueLayer();
            if (isClick6 == false) cmd6.HideOpaqueLayer();

            //自定义设置Panel切换函数
            //PanelIsDisplay(1);

            //自定义函数加载Form
            CleanIE ie = new CleanIE();
            Control_Add(ie);

            //点击按钮 isClick赋值 且保证点击按钮只能透明罩显示一个图标
            /*
            isClick1 = true;
            isClick2 = false;
            isClick3 = false;
            isClick4 = false;
            isClick5 = false;
            isClick6 = false;       
            */ 
        }

        //***************************************************************
        //鼠标进入"清除word"
        //private void panel_mol2_MouseEnter(object sender, EventArgs e){ }
        
        //鼠标点击"清除word"
        private void panel_mol2_MouseDown(object sender, MouseEventArgs e)
        {
            //透明罩设置 没点击才取消透明罩
            cmd2.ShowOpaqueLayer(panel_mol2, 125, true);
            if (isClick1 == false) cmd1.HideOpaqueLayer();
            if (isClick3 == false) cmd3.HideOpaqueLayer();
            if (isClick4 == false) cmd4.HideOpaqueLayer();
            if (isClick5 == false) cmd5.HideOpaqueLayer();
            if (isClick6 == false) cmd6.HideOpaqueLayer();

            //自定义设置Panel切换函数
            //PanelIsDisplay(1);

            //自定义函数加载Form
            CleanWord word = new CleanWord();
            Control_Add(word);
            
        }

        //***************************************************************
        //鼠标进入"清空回收站"
        //private void panel_mol3_MouseEnter(object sender, EventArgs e){ }
        
        //鼠标点击"清空回收站"
        private void panel_mol3_MouseDown(object sender, MouseEventArgs e)
        {
            //透明罩设置
            cmd3.ShowOpaqueLayer(panel_mol3, 125, true);
            if (isClick1 == false) cmd1.HideOpaqueLayer();
            if (isClick2 == false) cmd2.HideOpaqueLayer();
            if (isClick4 == false) cmd4.HideOpaqueLayer();
            if (isClick5 == false) cmd5.HideOpaqueLayer();
            if (isClick6 == false) cmd6.HideOpaqueLayer();

            //自定义函数加载Form
            CleanRecycle recycle = new CleanRecycle();
            Control_Add(recycle);

            
        }

        //***************************************************************
        //鼠标进入"u盘记录"
        //private void panel_mol4_MouseEnter(object sender, EventArgs e){ }

        //鼠标点击"u盘记录"
        private void panel_mol4_MouseDown(object sender, MouseEventArgs e)
        {
            //透明罩设置
            cmd4.ShowOpaqueLayer(panel_mol4, 125, true);
            if (isClick1 == false) cmd1.HideOpaqueLayer();
            if (isClick2 == false) cmd2.HideOpaqueLayer();
            if (isClick3 == false) cmd3.HideOpaqueLayer();
            if (isClick5 == false) cmd5.HideOpaqueLayer();
            if (isClick6 == false) cmd6.HideOpaqueLayer();

            //自定义函数加载Form
            CleanU u = new CleanU();
            Control_Add(u);
        }

        //***************************************************************
        //鼠标进入"电脑清理"
        //private void panel_mol5_MouseEnter(object sender, EventArgs e){ }

        //鼠标点击"电脑清理"
        private void panel_mol5_MouseDown(object sender, MouseEventArgs e)
        {
            //透明罩设置
            cmd5.ShowOpaqueLayer(panel_mol5, 125, true);
            if (isClick1 == false) cmd1.HideOpaqueLayer();
            if (isClick2 == false) cmd2.HideOpaqueLayer();
            if (isClick3 == false) cmd3.HideOpaqueLayer();
            if (isClick4 == false) cmd4.HideOpaqueLayer();
            if (isClick6 == false) cmd6.HideOpaqueLayer();

            //自定义函数加载Form
            CleanRecent recent = new CleanRecent();
            Control_Add(recent);
        }

        //***************************************************************
        //鼠标进入"文件粉碎"
        //private void panel_mol6_MouseEnter(object sender, EventArgs e){ }
        
        //鼠标点击"文件粉碎"
        private void panel_mol6_MouseDown(object sender, MouseEventArgs e)
        {
            //透明罩设置
            cmd6.ShowOpaqueLayer(panel_mol6, 125, true);
            if (isClick1 == false) cmd1.HideOpaqueLayer();
            if (isClick2 == false) cmd2.HideOpaqueLayer();
            if (isClick3 == false) cmd3.HideOpaqueLayer();
            if (isClick4 == false) cmd4.HideOpaqueLayer();
            if (isClick5 == false) cmd5.HideOpaqueLayer();

            //自定义函数加载Form
            CleanFile file = new CleanFile();
            Control_Add(file);
        }
        #endregion

        #region 窗体加载\最小化\关闭操作
        //加载窗体
        private void EMSecure_Load(object sender, EventArgs e)
        {
            AnimateWindow(this.Handle, 800, AW_SLIDE | AW_VER_POSITIVE);
            //初始化窗体
            this.label1.Text = "Eastmount 安全软件 1.0";

            //需要使6个图片控件不可用,才能获取6个panel控件
            this.pictureBox1.Enabled = false;
            this.pictureBox2.Enabled = false;
            this.pictureBox3.Enabled = false;
            this.pictureBox4.Enabled = false;
            this.pictureBox5.Enabled = false;
            this.pictureBox6.Enabled = false;

            //加载时IE显示透明罩功能
            cmd1.ShowOpaqueLayer(panel_mol1, 125, true);

            //载入时加载Form"清除IE"
            CleanIE ie = new CleanIE();
            Control_Add(ie);

        }

        //点击"最小化"按钮实现最小化
        private void minButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;   //最小化
        }

        //点击"退出"按钮实现退出程序
        private void closeButton_Click(object sender, EventArgs e)
        {
            AnimateWindow(this.Handle, 200, AW_CENTER + AW_HIDE);  //结束窗体动画
            Application.Exit();                                    //关闭程序窗口
        }
        #endregion

        #region 设计panel切换注意事项
        //*************************************************************************
        // panel控件注意事项
        // 为什么不能响应Panel控件的Click事件实现panel.Visible设置显示属性?
        // 而通过添加按钮button点击它能响应其Click实现设置Visible?
        // MouseEnter(鼠标进入)事件时就会相应透明罩显示,而不能同时相应MouseDown和
        // MouseClick(IsClick赋值就没有响应).开始猜测鼠标进入就不能点击,总是先响应
        // MouseEnter事件,但后来发现它是能同时响应的.
        // 可是调用ShowOpaque函数和HideOpaque后就不能响应MouseDown事件
        //
        // panel控件如何显示不同层的控件
        // 为什么使用Panel控件显示时只能显示最顶的panel,使用Visible隐藏式就无panel?
        // http://bbs.csdn.net/topics/390094454
        // 你的一个panel肯定parent被设置为另一个panel了,在窗体上拖放控件
        // 要注意,很容易就跑到别的控件里了.你打开视图-其他窗口-文档大纲看看层次关系 
        // 但是怎样设置文档大纲中控件的顺序呢?
        // 可以拖动至,但需要拖动至顶层.但是向panel添加控件非常麻烦,所以采用Form加载
        //*************************************************************************
        #endregion

        #region 自定义函数设置Panel切换
        //*************************************************************************
        // 开始准备通过Panel响应MouseClick事件设置其切换
        // 后无法实现改成自定义函数PanelIsDisplay()在MouseEnter中响应.
        // 自定义函数Panel添加控件比较麻烦所以采用panel动态添加Form功能实现
        // 但函数void PanelIsDisplay(int p)的方法非常不错.
        //*************************************************************************

        /// <summary>
        /// 点击标题菜单,对panel的显示
        /// </summary>
        /// <param name="p"></param>
        private void PanelIsDisplay(int p)
        {
            /*
            //设置panel显示界面 (IE\Word\回收站\U盘\电脑\文件粉碎)
            panelIE.Visible = false;
            panelWord.Visible = false;
            panelRecycle.Visible = false;
            panelU.Visible = false;
            panelRecent.Visible = false;
            panelFile.Visible = false;        

            switch (p)
            {
                case 1:  //显示"清除IE"
                    {
                        panelIE.Visible = true;
                    }
                    break;
                case 2:  //显示"清除Word"
                    {
                        panelWord.Visible = true;
                    }
                    break;
                case 3:  //显示"清空回收站"
                    {
                        panelRecycle.Visible = true;
                        this.panelRecycle.BackColor = Color.Red;
                    }
                    break;
                case 4:  //显示"清除U盘"
                    {
                        panelU.Visible = true;
                    }
                    break;
                case 5:  //显示"电脑清理"
                    {
                        panelRecent.Visible = true;
                    }
                    break;
                case 6:  //显示"电脑清理"
                    {
                        panelFile.Visible = true;
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
            */
        }


        /// <summary>
        /// 向panel2中加载窗体Form,MouseEnter事件调用
        /// </summary>
        /// <param name="p"></param>
        private void Control_Add(Form form)
        {
            //CleanIE CleanWord
            panel2.Controls.Clear();    //移除所有控件
            form.TopLevel = false;      //设置为非顶级窗体
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None; //设置窗体为非边框样式
            form.Dock = System.Windows.Forms.DockStyle.Fill;                  //设置样式是否填充整个panel
            panel2.Controls.Add(form);        //添加窗体
            form.Show();                      //窗体运行
        } 

        #endregion

    }
}