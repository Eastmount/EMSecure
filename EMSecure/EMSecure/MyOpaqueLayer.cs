using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace MyOpaqueLayer
{
    /* 
     * [ToolboxBitmap(typeof(MyOpaqueLayer))]
     * 用于指定当把你做好的自定义控件添加到工具栏时,工具栏显示的图标。
     * 正确写法应该是
     * [ToolboxBitmap(typeof(XXXXControl),"xxx.bmp")]
     * 其中XXXXControl是你的自定义控件，"xxx.bmp"是你要用的图标名称。
    */
    [ToolboxBitmap(typeof(MyOpaqueLayer))]

    /// <summary>
    /// 自定义控件:透明罩控件(继承Control)
    /// </summary>
    public class MyOpaqueLayer : System.Windows.Forms.Control
    {
        private bool _transparentBG = true;       //是否使用透明
        private int _alpha = 125;                 //设置透明度
        
        private System.ComponentModel.Container components = new System.ComponentModel.Container();
        public MyOpaqueLayer()
            : this(125, true)
        {
        }

        public MyOpaqueLayer(int Alpha, bool IsShowLoadingImage)
        {
            SetStyle(System.Windows.Forms.ControlStyles.Opaque, true);  //设置控件样式 true应用于控件
            base.CreateControl();                                       //创建控件
            this._alpha = Alpha;
            //放置加载进度的图片代码此处被省略
        }

        //释放组件占用内存
        protected override void Dispose(bool disposing)                 
        {
            if (disposing)
            {
                if (!((components == null)))
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 自定义绘制窗体
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            float vlblControlWidth;
            float vlblControlHeight;

            Pen labelBorderPen;                     //定义Pen
            SolidBrush labelBackColorBrush;         //定义单色画笔

            if (_transparentBG)                     //使用透明
            {
                Color drawColor = Color.FromArgb(this._alpha, this.BackColor);
                labelBorderPen = new Pen(drawColor, 0);
                labelBackColorBrush = new SolidBrush(drawColor);
            }
            else
            {
                labelBorderPen = new Pen(this.BackColor, 0);
                labelBackColorBrush = new SolidBrush(this.BackColor);
            }
            base.OnPaint(e);
            vlblControlWidth = this.Size.Width;
            vlblControlHeight = this.Size.Height;
            e.Graphics.DrawRectangle(labelBorderPen, 0, 0, vlblControlWidth, vlblControlHeight);
            e.Graphics.FillRectangle(labelBackColorBrush, 0, 0, vlblControlWidth, vlblControlHeight);
        }

        //获取创建控件句柄时所需要的创建参数
        protected override CreateParams CreateParams //v1.10 
        {
            get
            {
                CreateParams cp = base.CreateParams;  //扩展派生类CreateParams属性
                cp.ExStyle |= 0x00000020;             //开启WS_EX_TRANSPARENT,使控件支持透明
                return cp;
            }
        }

        /*
         * [Category("myOpaqueLayer"), Description("是否使用透明,默认为True")]
         * 一般用于说明你自定义控件的属性（Property）
         * Category用于说明该属性属于哪个分类,Description自然就是该属性的含义解释。
         */
        [Category("MyOpaqueLayer"), Description("是否使用透明,默认为True")]
        public bool TransparentBG
        {
            get
            {
                return _transparentBG;
            }
            set
            {
                _transparentBG = value;
                this.Invalidate();
            }
        }
        //设置透明度
        [Category("MyOpaqueLayer"), Description("设置透明度")]
        public int Alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                _alpha = value;
                this.Invalidate();
            }
        }

        //初始化窗体
        private void InitializeComponent()
        {
            this.SuspendLayout();      //临时挂起控件的布局逻辑,它与ResumeLayout()配合使用
            this.ResumeLayout(false);  //恢复正常逻辑

        }
    }
}
