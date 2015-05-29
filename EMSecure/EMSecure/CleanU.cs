using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;       //Registry
using System.IO;             //文件

namespace EMSecure
{
    public partial class CleanU : Form
    {
        public CleanU()
        {
            InitializeComponent();
        }

        //存储删除的第一条USB痕迹信息 每次删除仅仅删除第一条信息
        public string deleteFirstUSB = "";

        #region 初始化
        //定义Num记录listBox1中获取的文件数 是否删除成功
        public int Num = 0;
        public bool IsDelete = false;

        //窗体载入初始化窗体
        private void CleanU_Load(object sender, EventArgs e)
        {
            this.label1.Text = "查看和删除电脑中中USB存储介质的信息,信息包括:\nU盘\\移动硬盘\\移动设备的生产厂商、时间、序列号等";
            this.label2.Text = "";
            this.label3.Text = "温馨提示";
            richTextBox1.Clear();    //清空内容
        }
        #endregion

        #region 检测USB存储介质使用信息
        //获取USB使用信息
        private void button1_Click(object sender, EventArgs e)
        {
            //清空内容
            richTextBox1.Clear();    
            //定义注册表顶级节点 其命名空间是using Microsoft.Win32;
            RegistryKey USBKey;
            //检索子项    
            USBKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\USBSTOR", false);
            //记录获取数量赋初值
            Num = 0;

            //检索所有子项USBSTOR下的字符串数组
            foreach (string sub1 in USBKey.GetSubKeyNames())
            {
                RegistryKey sub1key = USBKey.OpenSubKey(sub1, false);
                foreach (string sub2 in sub1key.GetSubKeyNames())
                {
                    try
                    {
                        /* OpenSubKey 检索子项 函数原型 
                        public RegistryKey OpenSubKey(
                            string name,     //要打开的子项名称或路径
                            bool writable    //如果需要项的写访问权限=true
                        )
                        */
        
                        //打开sub1key的子项
                        RegistryKey sub2key = sub1key.OpenSubKey(sub2, false);
                        //检索Service=disk(磁盘)值的子项 cdrom(光盘)
                        if (sub2key.GetValue("Service", "").Equals("disk"))
                        {
                            String Path = "USBSTOR" + "\\" + sub1 + "\\" + sub2;
                            String Name = (string)sub2key.GetValue("FriendlyName", "");
                                                                                 
                            string fileName = "usbDeleteLog.txt";     //文件名称
                            bool isDelete = false;                    //是否删除
                            string content = "";                      //文件内容
                            //判断是否文件存在
                            if (!File.Exists(fileName))
                            {
                                //创建写入文件
                                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                                StreamWriter sw = new StreamWriter(fs);
                                sw.WriteLine();
                                sw.Close();
                                fs.Close();
                            }
                            else
                            {
                                StreamReader sReader = new StreamReader(fileName);
                                content = sReader.ReadToEnd();
                                isDelete = content.Contains(Path);
                                sReader.Close();
                            }
                            
                            //不存在
                            if(isDelete==false)
                            {
                                //记录第一条信息删除
                                if (Num == 0) deleteFirstUSB = Path;

                                //添加信息
                                Num++;
                                string num = "信息标号 No." + Num + "\r\n";
                                string name = "USB名称  " + Name + "\r\n";
                                string uid = "UID标记  " + sub2 + "\r\n";
                                string path = "路径信息 " + Path + "\r\n";
                                //获取当前时间
                                string time = "log时间 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + "\r\n";

                                //利用文件知识存储后台USB管理信息
                                string usbFileLog = "usbFileLog.txt";
                                if (!File.Exists(usbFileLog))
                                {
                                    //创建写入文件
                                    FileStream fs = new FileStream(usbFileLog, FileMode.Create, FileAccess.Write);
                                    StreamWriter sw = new StreamWriter(fs);
                                    sw.WriteLine(num + name + uid + path + time);
                                    sw.Close();
                                    fs.Close();
                                }
                                else
                                {
                                    //写入文件
                                    using (StreamWriter sw = new StreamWriter(usbFileLog, true))
                                    {
                                        sw.WriteLine(num + name + uid + path + time);
                                        sw.Close();
                                    }
                                }
                                //添加信息
                                richTextBox1.AppendText(num);
                                richTextBox1.AppendText(name);
                                richTextBox1.AppendText(uid);
                                richTextBox1.AppendText(path + "\r\n");                                
                            }                                                       
                        }
                    }
                    catch (Exception msg) //异常处理
                    {
                        MessageBox.Show(msg.Message);
                    }
                }
            }

            //提示
            if (Num == 0)
            {
                MessageBox.Show(this, "USB使用记录已经删空,无信息!", "提示对话框", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                label2.Text = "☺☺☺☺☺☺☺\n秀璋提醒您:\nUSB信息已空\n☺☺☺☺☺☺☺";
            }
            else
            {
                label2.Text = "☺☺☺☺☺☺☺\n秀璋提醒您:\nUSB信息获取成功\n☺☺☺☺☺☺☺";
            }
        }      
        #endregion

        #region 获取系统的USB痕迹信息(未实现获取所有信息) 
        /*
        // 需要扫描的注册表键值
        private string[] _keyNames = {
            "Enum\\USB",
            "Enum\\USBSTOR",
            "Control\\DeviceClasses\\{53f56307-b6bf-11d0-94f2-00a0c91efb8b}",
            //"Control\\DeviceClasses\\{a5dcbf10-6530-11d2-901f-00c04fb951ed}",
        };

        private List<string> _usbKeyNames = null;
        private Dictionary<int, string> _vendorIds = null;

        
        private void OOPS(string errMsg)
        {
            MessageBox.Show(errMsg, "USB 存储设备使用痕迹检测", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 初始化设备 ID - 厂商名 列表。
        /// </summary>
        private void InitVendorId()
        {
            string[] spliter = { "\n" };
            _vendorIds = new Dictionary<int, string>();
            foreach (string line in Properties.Resources.VendorIDs.Split(spliter, StringSplitOptions.RemoveEmptyEntries))
            {
                _vendorIds.Add(int.Parse(line.Substring(0, 4), NumberStyles.HexNumber), line.Substring(5));
            }
        }

        /// <summary>
        /// 由 ID 值查找厂商名称。
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        private string FindVendorName(string vendorId)
        {
            int vendorIdInt = int.Parse(vendorId, NumberStyles.HexNumber);
            foreach (KeyValuePair<int, string> keyValue in _vendorIds)
            {
                if (keyValue.Key == vendorIdInt)
                    return keyValue.Value;
            }

            return string.Empty;
        }

        /// <summary>
        /// 枚举以下键值：
        /// HKEY_LOCAL_MACHINE\SYSTEM\ControlSetxxx\Enum\USB
        /// HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSetxxx\Enum\USB
        /// </summary>
        /// <param name="usbstorKey"></param>
        private void EnumerateUsb(RegistryKey usbstorKey)
        {
            Regex regx = new Regex("^\\w{8}&\\w{8}$");
            foreach (string subUsbstorKey in usbstorKey.GetSubKeyNames())
            {
                if (!regx.IsMatch(subUsbstorKey.ToUpper())) continue;

                RegistryKey tempUsbKey = usbstorKey.OpenSubKey(subUsbstorKey);
                if (null == tempUsbKey) continue;
                if (tempUsbKey.SubKeyCount == 0) continue;

                RegistryKey subTempUsbKey = tempUsbKey.OpenSubKey(tempUsbKey.GetSubKeyNames()[0]);
                if (null == subTempUsbKey) continue;
                object valueService = subTempUsbKey.GetValue("Service");
                if (null != valueService)
                {
                    if (!string.Equals(valueService.ToString().ToUpper(), "USBSTOR") &&
                        !string.Equals(valueService.ToString().ToUpper(), "WUDFRD") &&
                        !string.Equals(valueService.ToString().ToUpper(), "VBOXUSB"))
                        continue;

                    if (!_usbKeyNames.Contains(subUsbstorKey))
                    {
                        _usbKeyNames.Add(subUsbstorKey);
                        ListViewItem item = this.lvwList.Items.Add(string.Empty, 0);
                        item.SubItems.Add(subUsbstorKey);
                        item.Tag = subUsbstorKey;

                        object valueFriendlyName = subTempUsbKey.GetValue("FriendlyName");
                        if (null == valueFriendlyName)
                        {
                            string vendorId = subUsbstorKey.Substring(4, 4);
                            item.SubItems.Add(FindVendorName(vendorId));
                        }
                        else
                            item.SubItems.Add(valueFriendlyName.ToString());
                    }
                }
                subTempUsbKey.Close();
                tempUsbKey.Close();
            }
        }

        /// <summary>
        /// 枚举以下键值：
        /// HKEY_LOCAL_MACHINE\SYSTEM\ControlSetxxx\Enum\USBSTOR
        /// HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSetxxx\Enum\USBSTOR
        /// HKEY_LOCAL_MACHINE\SYSTEM\ControlSetxxx\Control\DeviceClasses\{53f56307-b6bf-11d0-94f2-00a0c91efb8b}
        /// HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSetxxx\Control\DeviceClasses\{53f56307-b6bf-11d0-94f2-00a0c91efb8b}
        /// </summary>
        /// <param name="usbstorKey"></param>
        private void EnumerateUsbstorDeviceClasses(RegistryKey usbstorKey)
        {
            foreach (string subUsbstorKey in usbstorKey.GetSubKeyNames())
            {
                if (!_usbKeyNames.Contains(subUsbstorKey))
                {
                    _usbKeyNames.Add(subUsbstorKey);
                    ListViewItem item = this.lvwList.Items.Add(string.Empty, 0);
                    item.SubItems.Add(subUsbstorKey);
                    item.Tag = subUsbstorKey;

                    RegistryKey usbFoundKey = usbstorKey.OpenSubKey(subUsbstorKey);
                    if (null == usbFoundKey)
                        continue;

                    if (usbFoundKey.SubKeyCount > 0)
                    {
                        RegistryKey subUsbFoundKey = usbFoundKey.OpenSubKey(usbFoundKey.GetSubKeyNames()[0]);
                        if (null == subUsbFoundKey)
                            continue;

                        object value = subUsbFoundKey.GetValue("FriendlyName");
                        if (null == value)
                            item.SubItems.Add(string.Empty);
                        else
                            item.SubItems.Add(value.ToString());
                        subUsbFoundKey.Close();
                    }
                    usbFoundKey.Close();
                }
            }
        }

        /// <summary>
        /// 检测 USB 存储设备使用痕迹。
        /// </summary>
        private void Detect()
        {
            _usbKeyNames = new List<string>();

            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey systemKey = hklm.OpenSubKey("SYSTEM");

            if (null == systemKey)
            {
                OOPS("检测失败。无法打开系统注册表项 HKEY_LOCAL_MACHINE\\SYSTEM 。");
                return;
            }

            foreach (string sysKey in systemKey.GetSubKeyNames())
            {
                if (!sysKey.ToUpper().StartsWith("CONTROLSET") &&
                    !sysKey.ToUpper().StartsWith("CURRENTCONTROLSET"))
                    continue;

                RegistryKey subSystemKey = systemKey.OpenSubKey(sysKey);
                if (null == subSystemKey)
                    continue;

                foreach (string key in _keyNames)
                {
                    RegistryKey usbstorKey = subSystemKey.OpenSubKey(key);
                    if (null == usbstorKey)
                        continue;

                    if (string.Equals(key.ToUpper(), "ENUM\\USB"))
                    {
                        EnumerateUsb(usbstorKey);
                    }
                    else
                    {
                        EnumerateUsbstorDeviceClasses(usbstorKey);
                    }
                    usbstorKey.Close();
                }
                subSystemKey.Close();
            }
            systemKey.Close();
        }

        /// <summary>
        /// 执行删除操作。安装服务，使用 SYSTEM 权限来操作注册表，以便删除相应的键值。
        /// </summary>
        private void Delete()
        {
            if (ServiceHelper.StartService())
                this.lvwList.Items.Clear();
        }
        */ 
        #endregion

        #region 删除USB信息
        //自定义变量
        private string[] _keyNames = {
            "Enum\\USB",
            "Enum\\USBSTOR",
            "Control\\DeviceClasses\\{53f56307-b6bf-11d0-94f2-00a0c91efb8b}",
        };

        //删除USB信息
        private void button2_Click(object sender, EventArgs e)
        {
            //定义注册表顶级节点 其命名空间是using Microsoft.Win32;
            RegistryKey USBKey;
            //检索子项    
            USBKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Enum\USBSTOR", false);            

            //检索所有子项USBSTOR下的字符串数组
            foreach (string sub1 in USBKey.GetSubKeyNames())
            {
                RegistryKey sub1key = USBKey.OpenSubKey(sub1, false);
                foreach (string sub2 in sub1key.GetSubKeyNames())
                {
                    try
                    {                     
                        //打开sub1key的子项
                        RegistryKey sub2key = sub1key.OpenSubKey(sub2, false);

                        //检索Service=disk(磁盘)值的子项 cdrom(光盘)
                        if (sub2key.GetValue("Service", "").Equals("disk"))
                        {
                            String Path = "USBSTOR" + "\\" + sub1 + "\\" + sub2;

                            try
                            {
                                //其中sub2存储的是USBSTOR/Disk&Ven_agio&Prod_&Rev_/00A1234567AF&0中
                                if (deleteFirstUSB == Path)
                                {
                                    //获取删除路径信息
                                    string usb = @"SYSTEM\CurrentControlSet\Enum\" + deleteFirstUSB;

                                    //usb = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\USBSTOR\Disk&Ven_aigo&Prod_&Rev_\00A1234567AF&0";
                                    //USBSTOR\Disk&Ven_aigo&Prod_&Rev_\00A1234567AF&0

                                    //确认删除
                                    if (MessageBox.Show("确认要删除第一条USB信息?路径:" + usb, "验证提示",
                                        System.Windows.Forms.MessageBoxButtons.YesNo,
                                        System.Windows.Forms.MessageBoxIcon.Question) ==
                                        System.Windows.Forms.DialogResult.Yes)
                                    {     
                                        //删除文件
                                        //Registry.LocalMachine.DeleteSubKeyTree(usb,true);
                                        //设置键值为空(该子项不存在,因此无法删除)
                                        //Registry.SetValue(usb, "Service", "null");

                                        //文件存储后台USB删除log信息
                                        string usbDeleteLog = "usbDeleteLog.txt";
                                        if (!File.Exists(usbDeleteLog))
                                        {
                                            //创建写入文件
                                            FileStream fs = new FileStream(usbDeleteLog, FileMode.Create, FileAccess.Write);
                                            StreamWriter sw = new StreamWriter(fs);
                                            sw.WriteLine(Path);
                                            sw.Close();
                                            fs.Close();
                                        }
                                        else
                                        {
                                            //写入文件
                                            using (StreamWriter sw = new StreamWriter(usbDeleteLog, true))
                                            {
                                                sw.WriteLine(Path);
                                                sw.Close();
                                            }
                                        }
                                        MessageBox.Show("删除USB痕迹成功!路径:" + usb, "信息提示",
                                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        label2.Text = "☺☺☺☺☺☺☺\n秀璋提醒您:\nUSB信息删除成功\n☺☺☺☺☺☺☺";
                                        //清空内容
                                        richTextBox1.Clear();    
                                    }
                                }
                            }
                            catch (Exception m) //异常处理
                            {
                                MessageBox.Show(m.Message);
                            }
                        }
                    }
                    catch (Exception msg) //异常处理
                    {
                        MessageBox.Show(msg.Message);
                    }
                }
            }           
        }    
        #endregion

    }
}
