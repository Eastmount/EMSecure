using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMSecure
{
    class OpaqueCommand
    {
        //透明罩
        private MyOpaqueLayer.MyOpaqueLayer m_OpaqueLayer = null;

        /// <summary>
        /// 显示透明层
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="alpha">透明度</param>
        /// <param name="isShowLoadingImage">是否显示图标</param>
        public void ShowOpaqueLayer(Control control, int alpha, bool isShowLoadingImage)
        {
            try
            {
                if (this.m_OpaqueLayer == null)
                {
                    this.m_OpaqueLayer = new MyOpaqueLayer.MyOpaqueLayer(alpha, isShowLoadingImage);
                    control.Controls.Add(this.m_OpaqueLayer);
                    this.m_OpaqueLayer.Dock = DockStyle.Fill;
                    this.m_OpaqueLayer.BringToFront();
                }
                this.m_OpaqueLayer.Enabled = true;
                this.m_OpaqueLayer.Visible = true;
            }
            catch (Exception msg)              //异常处理
            {
                MessageBox.Show(msg.Message);
            }
        }

        /// <summary>
        /// 隐藏透明层
        /// </summary>
        public void HideOpaqueLayer()
        {
            try
            {
                if (this.m_OpaqueLayer != null)
                {
                    this.m_OpaqueLayer.Visible = false;
                    this.m_OpaqueLayer.Enabled = false;
                }
            }
            catch (Exception msg)             //异常处理
            {
                MessageBox.Show(msg.Message);
            }
        }
    }
}
