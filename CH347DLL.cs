using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    //internal class CH347DLL
    //{
    //}

    //由于vc版本的头文件中含有部分宏定义，在C#中无法实现（其中一部分转为常量），完整版请参考vc版CH347DLL.H
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 8)]
    public struct mUSB_SETUP_PKT
    {				// USB控制传输的建立阶段的数据请求包结构
        [FieldOffset(0)]
        public byte mUspReqType;                // 00H 请求类型
        [FieldOffset(1)]
        public byte mUspRequest;                // 01H 请求代码

        [FieldOffset(2)]
        public byte mUspValueLow;				// 02H 值参数低字节
        [FieldOffset(3)]
        public byte mUspValueHigh;              // 03H 值参数高字节
        [FieldOffset(2)]
        public UInt16 mUspValue;                    // 02H-03H 值参数

        [FieldOffset(4)]
        public byte mUspIndexLow;				// 04H 索引参数低字节
        [FieldOffset(5)]
        public byte mUspIndexHigh;              // 05H 索引参数高字节
        [FieldOffset(4)]
        public UInt16 mUspIndex;                    // 04H-05H 索引参数
        [FieldOffset(6)]
        public UInt16 mLength;					// 06H-07H 数据阶段的数据长度
    }
    /*
     mWIN32_COMMAND结构体C++定义如下
     
     typedef	struct	_WIN32_COMMAND {				// 定义WIN32命令接口结构
         union	{
             ULONG		mFunction;					// 输入时指定功能代码或者管道号
             NTSTATUS	mStatus;					// 输出时返回操作状态
         };
         ULONG			mLength;					// 存取长度,返回后续数据的长度
         union	{
             mUSB_SETUP_PKT	mSetupPkt;				// USB控制传输的建立阶段的数据请求
             UCHAR			mBuffer[ mCH347_PACKET_LENGTH ];	// 数据缓冲区,长度为0至255B
         };
     } mWIN32_COMMAND, *mPWIN32_COMMAND;

    C#中union功能由FieldOffset()代替

    由于上述结构体中最后一个union可能引起错误“对象字段由一个非对象字段不正确地对齐或重叠”
    因此将上述结构体拆分成2个结构体mWIN32_COMMAND_USB_SETUP_PKT与mWIN32_COMMAND_mBuffer，选其一即可。
    CH347DriverCommand函数将重载

     */

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    public struct mWIN32_COMMAND_USB_SETUP_PKT
    {               // 定义WIN32命令接口结构
        [FieldOffset(0)]
        public UInt32 mFunction;					// 输入时指定功能代码或者管道号
        [FieldOffset(0)]
        public Int32 mStatus;                   // 输出时返回操作状态
        [FieldOffset(4)]
        public UInt32 mLength;                  // 存取长度,返回后续数据的长度
        [FieldOffset(8)]
        public mUSB_SETUP_PKT mSetupPkt;                // USB控制传输的建立阶段的数据请求
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _DEV_INFOR
    {
        public byte iIndex;                 // 当前打开序号

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
        public byte[] DevicePath;           // 设备链接名,用于CreateFile
        public byte UsbClass;               // 0:CH347_USB_CH341, 2:CH347_USB_HID,3:CH347_USB_VCP
        public byte FuncType;               // 0:CH347_FUNC_UART,1:CH347_FUNC_SPI_I2C,2:CH347_FUNC_JTAG_I2C

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] DeviceID;               // USB\VID_xxxx&PID_xxxx
        public byte ChipMode;               // 芯片模式,0:Mode0(UART0/1); 1:Mode1(Uart1+SPI+I2C); 2:Mode2(HID Uart1+SPI+I2C) 3:Mode3(Uart1+Jtag+IIC)
        public IntPtr DevHandle;              // 设备句柄
        public UInt16 BulkOutEndpMaxSize;     // 上传端点大小
        public UInt16 BulkInEndpMaxSize;      // 下传端点大小
        public byte UsbSpeedType;           // USB速度类型，0:FS,1:HS,2:SS
        public byte CH347IfNum;             // 设备接口号: 0:UART,1:SPI/IIC/JTAG/GPIO
        public byte DataUpEndp;             // 端点地址
        public byte DataDnEndp;             // 端点地址

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] ProductString;      // USB产品字符串

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] ManufacturerString; // USB厂商字符串
        public UInt32 WriteTimeout;           // USB写超时
        public UInt32 ReadTimeout;            // USB读超时

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] FuncDescStr;        // 接口功能描述符
        public byte FirmwareVer;            // 固件版本
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Pack = 0)]
    public struct _SPI_CONFIG
    {
        [FieldOffset(0)]
        public byte iMode; // 0-3:SPI Mode0/1/2/3
        [FieldOffset(1)]
        public byte iClock; /* 0=60MHz, 1=30MHz, 2=15MHz, 3=7.5MHz,4=3.75MHz, 5=1.875MHz,6=937.5KHz,7=468.75KHz*/
        [FieldOffset(2)]
        public byte iByteOrder; // 0=低位在前(LSB), 1=高位在前(MSB)
        [FieldOffset(3)]
        public UInt16 iSpiWriteReadInterval; // SPI接口常规读取写入数据命令，单位为uS
        [FieldOffset(5)]
        public byte iSpiOutDefaultData; // SPI读数据时默认输出数据
        [FieldOffset(6)]
        public UInt32 iChipSelect; // 片选控制, 位7为0则忽略片选控制, 位7为1则参数有效: 位1位0为00/01分别选择CS1/CS2引脚作为低电平有效片选
        [FieldOffset(10)]
        public byte CS1Polarity; // 位0：片选CS1极性控制，0：低电平有效；1：高电平有效；
        [FieldOffset(11)]
        public byte CS2Polarity; // 位0：片选CS2极性控制，0：低电平有效；1：高电平有效；
        [FieldOffset(12)]
        public UInt16 iIsAutoDeativeCS; // 操作完成后是否自动撤消片选
        [FieldOffset(14)]
        public UInt16 iActiveDelay; // 设置片选后执行读写操作的延时时间,单位uS
        [FieldOffset(16)]
        public UInt32 iDelayDeactive; // 撤消片选后执行读写操作的延时时间,单位uS
    };

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    public struct mWIN32_COMMAND_mBuffer
    {				// 定义WIN32命令接口结构
        [FieldOffset(0)]
        public UInt32 mFunction;					// 输入时指定功能代码或者管道号
        [FieldOffset(0)]
        public Int32 mStatus;					// 输出时返回操作状态
        [FieldOffset(4)]
        public UInt32 mLength;					// 存取长度,返回后续数据的长度

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = CH347DLL.mCH347_PACKET_LENGTH), FieldOffset(8)]
        public byte[] mBuffer;	// 数据缓冲区,长度为0至255B

    }
    class CH347DLL
    {

        public const int mCH347_PACKET_LENGTH = 32;// CH347支持的数据包的长度
        public const int mCH347_PKT_LEN_SHORT = 8;//CH347支持的短数据包的长度

        // WIN32应用层接口命令
        //  public  const int		IOCTL_CH347_COMMAND	=	( FILE_DEVICE_UNKNOWN << 16 | FILE_ANY_ACCESS << 14 | 0x0f34 << 2 | METHOD_BUFFERED )	// 专用接口

        // public  const int		mWIN32_COMMAND_HEAD	=	mOFFSET( mWIN32_COMMAND, mBuffer );	// WIN32命令接口的头长度

        public const int mCH347_MAX_NUMBER = 16;			// 最多同时连接的CH347数

        public const int mMAX_BUFFER_LENGTH = 0x1000;       // 数据缓冲区最大长度4096

        //  public  const int		mMAX_COMMAND_LENGTH	=	( mWIN32_COMMAND_HEAD + mMAX_BUFFER_LENGTH );	// 最大数据长度加上命令结构头的长度

        public const int mDEFAULT_BUFFER_LEN = 0x0400;      // 数据缓冲区默认长度1024

        //   public  const int		mDEFAULT_COMMAND_LEN=	( mWIN32_COMMAND_HEAD + mDEFAULT_BUFFER_LEN );	// 默认数据长度加上命令结构头的长度

        // CH347端点地址
        public const int mCH347_ENDP_INTER_UP = 0x81;		// CH347的中断数据上传端点的地址
        public const int mCH347_ENDP_INTER_DOWN = 0x01;		// CH347的中断数据下传端点的地址
        public const int mCH347_ENDP_DATA_UP = 0x82;		// CH347的数据块上传端点的地址
        public const int mCH347_ENDP_DATA_DOWN = 0x02;		// CH347的数据块下传端点的地址


        // 设备层接口提供的管道操作命令
        public const int mPipeDeviceCtrl = 0x00000004;	// CH347的综合控制管道
        public const int mPipeInterUp = 0x00000005;	// CH347的中断数据上传管道
        public const int mPipeDataUp = 0x00000006;	// CH347的数据块上传管道
        public const int mPipeDataDown = 0x00000007;	// CH347的数据块下传管道

        // 应用层接口的功能代码
        public const int mFuncNoOperation = 0x00000000;	// 无操作
        public const int mFuncGetVersion = 0x00000001;	// 获取驱动程序版本号
        public const int mFuncGetConfig = 0x00000002;	// 获取USB设备配置描述符
        public const int mFuncSetTimeout = 0x00000009;	// 设置USB通讯超时
        public const int mFuncSetExclusive = 0x0000000b;	// 设置独占使用
        public const int mFuncResetDevice = 0x0000000c;	// 复位USB设备
        public const int mFuncResetPipe = 0x0000000d;	// 复位USB管道
        public const int mFuncAbortPipe = 0x0000000e;	// 取消USB管道的数据请求

        // CH347并口专用的功能代码
        public const int mFuncSetParaMode = 0x0000000f;	// 设置并口模式
        public const int mFuncReadData0 = 0x00000010;	// 从并口读取数据块0
        public const int mFuncReadData1 = 0x00000011;	// 从并口读取数据块1
        public const int mFuncWriteData0 = 0x00000012;	// 向并口写入数据块0
        public const int mFuncWriteData1 = 0x00000013;	// 向并口写入数据块1
        public const int mFuncWriteRead = 0x00000014;	// 先输出再输入
        public const int mFuncBufferMode = 0x00000020;	// 设定缓冲上传模式及查询缓冲区中的数据长度
        public const int FuncBufferModeDn = 0x00000021;	// 设定缓冲下传模式及查询缓冲区中的数据长度


        // USB设备标准请求代码
        public const int mUSB_CLR_FEATURE = 0x01;
        public const int mUSB_SET_FEATURE = 0x03;
        public const int mUSB_GET_STATUS = 0x00;
        public const int mUSB_SET_ADDRESS = 0x05;
        public const int mUSB_GET_DESCR = 0x06;
        public const int mUSB_SET_DESCR = 0x07;
        public const int mUSB_GET_CONFIG = 0x08;
        public const int mUSB_SET_CONFIG = 0x09;
        public const int mUSB_GET_INTERF = 0x0a;
        public const int mUSB_SET_INTERF = 0x0b;
        public const int mUSB_SYNC_FRAME = 0x0c;

        // CH347控制传输的厂商专用请求类型
        public const int mCH347_VENDOR_READ = 0xC0;		// 通过控制传输实现的CH347厂商专用读操作
        public const int mCH347_VENDOR_WRITE = 0x40;		// 通过控制传输实现的CH347厂商专用写操作

        // CH347控制传输的厂商专用请求代码
        public const int mCH347_PARA_INIT = 0xB1;	// 初始化并口
        public const int mCH347_I2C_STATUS = 0x52;		// 获取I2C接口的状态
        public const int mCH347_I2C_COMMAND = 0x53;		// 发出I2C接口的命令


        // CH347A并口操作命令代码
        public const int mCH347A_CMD_SET_OUTPUT = 0xA1;		// 设置并口输出
        public const int mCH347A_CMD_IO_ADDR = 0xA2;	// MEM带地址读写/输入输出,从次字节开始为命令流
        public const int mCH347A_CMD_PRINT_OUT = 0xA3;		// PRINT兼容打印方式输出,从次字节开始为数据流
        public const int mCH347A_CMD_PWM_OUT = 0xA4;		// PWM数据输出的命令包,从次字节开始为数据流
        public const int mCH347A_CMD_SHORT_PKT = 0xA5;		// 短包,次字节是该命令包的真正长度,再次字节及之后的字节是原命令包
        public const int mCH347A_CMD_SPI_STREAM = 0xA8;		// SPI接口的命令包,从次字节开始为数据流
        //public  const int		mCH347A_CMD_SIO_STREAM	0xA9		// SIO接口的命令包,从次字节开始为数据流
        public const int mCH347A_CMD_I2C_STREAM = 0xAA;		// I2C接口的命令包,从次字节开始为I2C命令流
        public const int mCH347A_CMD_UIO_STREAM = 0xAB;		// UIO接口的命令包,从次字节开始为命令流
        public const int mCH347A_CMD_PIO_STREAM = 0xAE;		// PIO接口的命令包,从次字节开始为数据流

        // CH347A控制传输的厂商专用请求代码
        public const int mCH347A_BUF_CLEAR = 0xB2;		// 清除未完成的数据
        public const int mCH347A_I2C_CMD_X = 0x54;		// 发出I2C接口的命令,立即执行
        public const int mCH347A_DELAY_MS = 0x5E;		// 以亳秒为单位延时指定时间
        public const int mCH347A_GET_VER = 0x5F;        // 获取芯片版本


        public const int mCH347A_CMD_IO_ADDR_W = 0x00;		// MEM带地址读写/输入输出的命令流:写数据,位6-位0为地址,下一个字节为待写数据
        public const int mCH347A_CMD_IO_ADDR_R = 0x80;		// MEM带地址读写/输入输出的命令流:读数据,位6-位0为地址,读出数据一起返回

        public const int mCH347A_CMD_I2C_STM_STA = 0x74;		// I2C接口的命令流:产生起始位
        public const int mCH347A_CMD_I2C_STM_STO = 0x75;		// I2C接口的命令流:产生停止位
        public const int mCH347A_CMD_I2C_STM_OUT = 0x80;		// I2C接口的命令流:输出数据,位5-位0为长度,后续字节为数据,0长度则只发送一个字节并返回应答
        public const int mCH347A_CMD_I2C_STM_IN = 0xC0;		// I2C接口的命令流:输入数据,位5-位0为长度,0长度则只接收一个字节并发送无应答
        public const int mCH347A_CMD_I2C_STM_MAX = ((0x3F < mCH347_PACKET_LENGTH) ? 0x3F : mCH347_PACKET_LENGTH);	// I2C接口的命令流单个命令输入输出数据的最大长度
        public const int mCH347A_CMD_I2C_STM_SET = 0x60;		// I2C接口的命令流:设置参数,位2=SPI的I/O数(0=单入单出,1=双入双出),位1位0=I2C速度(00=低速,01=标准,10=快速,11=高速)
        public const int mCH347A_CMD_I2C_STM_US = 0x40;		// I2C接口的命令流:以微秒为单位延时,位3-位0为延时值
        public const int mCH347A_CMD_I2C_STM_MS = 0x50;		// I2C接口的命令流:以亳秒为单位延时,位3-位0为延时值
        public const int mCH347A_CMD_I2C_STM_DLY = 0x0F;		// I2C接口的命令流单个命令延时的最大值
        public const int mCH347A_CMD_I2C_STM_END = 0x00;		// I2C接口的命令流:命令包提前结束

        public const int mCH347A_CMD_UIO_STM_IN = 0x00;		// UIO接口的命令流:输入数据D7-D0
        public const int mCH347A_CMD_UIO_STM_DIR = 0x40;		// UIO接口的命令流:设定I/O方向D5-D0,位5-位0为方向数据
        public const int mCH347A_CMD_UIO_STM_OUT = 0x80;	// UIO接口的命令流:输出数据D5-D0,位5-位0为数据
        public const int mCH347A_CMD_UIO_STM_US = 0xC0;		// UIO接口的命令流:以微秒为单位延时,位5-位0为延时值
        public const int mCH347A_CMD_UIO_STM_END = 0x20;		// UIO接口的命令流:命令包提前结束


        public const int MAX_DEVICE_PATH_SIZE = 128;			// 设备名称的最大字符数
        public const int MAX_DEVICE_ID_SIZE = 64;			// 设备ID的最大字符数
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void mPCH347_INT_ROUTINE(// 中断服务程序
                                                UInt32 iStatus);// 中断状态数据,参考下面的位说明
        // 位7-位0对应CH347的D7-D0引脚
        // 位8对应CH347的ERR#引脚, 位9对应CH347的PEMP引脚, 位10对应CH347的INT#引脚, 位11对应CH347的SLCT引脚

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347OpenDevice")]
        public static extern IntPtr CH347OpenDevice(  // 打开CH347设备,返回句柄,出错则无效
                                     UInt32 iIndex);  // 指定CH347设备序号,0对应第一个设备

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347CloseDevice")]
        public static extern void CH347CloseDevice(  // 关闭CH347设备
                                      UInt32 iIndex);  // 指定CH347设备序号

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347GetVersion")]
        public static extern bool CH347GetVersion();  // 获得DLL版本号,返回版本号


        [DllImport("CH347DLL.DLL", EntryPoint = "CH347GetDeviceInfor")]
        public static extern bool CH347GetDeviceInfor(UInt32 iIndex, ref _DEV_INFOR mDevInfor);  // Get Device Information

        //函数重载
        [DllImport("CH347DLL.DLL", EntryPoint = "CH347DriverCommand")]
        public static extern UInt32 CH347DriverCommand(  // 直接传递命令给驱动程序,出错则返回0,否则返回数据长度
                                UInt32 iIndex,  // 指定CH347设备序号,V1.6以上DLL也可以是设备打开后的句柄
                                 ref mWIN32_COMMAND_mBuffer ioCommand);  // 命令结构的指针

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347DriverCommand")]
        public static extern UInt32 CH347DriverCommand(  // 直接传递命令给驱动程序,出错则返回0,否则返回数据长度
                                UInt32 iIndex,  // 指定CH347设备序号,V1.6以上DLL也可以是设备打开后的句柄
                                 ref mWIN32_COMMAND_USB_SETUP_PKT ioCommand);  // 命令结构的指针
        // 该程序在调用后返回数据长度,并且仍然返回命令结构,如果是读操作,则数据返回在命令结构中,
        // 返回的数据长度在操作失败时为0,操作成功时为整个命令结构的长度,例如读一个字节,则返回mWIN32_COMMAND_HEAD+1,
        // 命令结构在调用前,分别提供:管道号或者命令功能代码,存取数据的长度(可选),数据(可选)
        // 命令结构在调用后,分别返回:操作状态代码,后续数据的长度(可选),
        //   操作状态代码是由WINDOWS定义的代码,可以参考NTSTATUS.H,
        //   后续数据的长度是指读操作返回的数据长度,数据存放在随后的缓冲区中,对于写操作一般为0

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347GetDrvVersion")]
        public static extern UInt32 CH347GetDrvVersion();  // 获得驱动程序版本号,返回版本号,出错则返回0

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347ResetDevice")]
        public static extern bool CH347ResetDevice(  // 复位USB设备
                             UInt32 iIndex);  // 指定CH347设备序号

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347GetDeviceDescr")]
        public static extern bool CH347GetDeviceDescr(  // 读取设备描述符
            UInt32 iIndex,  // 指定CH347设备序号
            byte[] oBuffer,  // 指向一个足够大的缓冲区,用于保存描述符
            ref UInt32 ioLength);  // 指向长度单元,输入时为准备读取的长度,返回后为实际读取的长度

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347GetConfigDescr")]
        public static extern bool CH347GetConfigDescr(  // 读取配置描述符
            UInt32 iIndex,  // 指定CH347设备序号
            byte[] oBuffer,  // 指向一个足够大的缓冲区,用于保存描述符
            ref UInt32 ioLength);  // 指向长度单元,输入时为准备读取的长度,返回后为实际读取的长度

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetIntRoutine")]
        public static extern bool CH347SetIntRoutine(  // 设定中断服务程序
            UInt32 iIndex,  // 指定CH347设备序号
            mPCH347_INT_ROUTINE iIntRoutine);  // 指定中断服务程序,为NULL则取消中断服务,否则在中断时调用该程序

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347ReadInter")]
        public static extern bool CH347ReadInter(  // 读取中断数据
            UInt32 iIndex,  // 指定CH347设备序号
            ref UInt32 iStatus);  // 指向一个双字单元,用于保存读取的中断状态数据,见下行
        // 位7-位0对应CH347的D7-D0引脚
        // 位8对应CH347的ERR#引脚, 位9对应CH347的PEMP引脚, 位10对应CH347的INT#引脚, 位11对应CH347的SLCT引脚


        [DllImport("CH347DLL.DLL", EntryPoint = "CH347AbortInter")]
        public static extern bool CH347AbortInter(  // 放弃中断数据读操作
            UInt32 iIndex);  // 指定CH347设备序号

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetParaMode")]
        public static extern bool CH347SetParaMode(  // 设置并口模式
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iMode);  // 指定并口模式: 0为EPP模式/EPP模式V1.7, 1为EPP模式V1.9, 2为MEM模式

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347InitParallel")]
        public static extern bool CH347InitParallel(  // 复位并初始化并口,RST#输出低电平脉冲
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iMode);  // 指定并口模式: 0为EPP模式/EPP模式V1.7, 1为EPP模式V1.9, 2为MEM模式, >= 0x00000100 保持当前模式

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347ReadData0")]
        public static extern bool CH347ReadData0(  // 从0#端口读取数据块
            UInt32 iIndex,  // 指定CH347设备序号
            byte[] oBuffer,  // 指向一个足够大的缓冲区,用于保存读取的数据
            ref UInt32 ioLength);  // 指向长度单元,输入时为准备读取的长度,返回后为实际读取的长度

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347ReadData1")]
        public static extern bool CH347ReadData1(  // 从1#端口读取数据块
            UInt32 iIndex,  // 指定CH347设备序号
            byte[] oBuffer,  // 指向一个足够大的缓冲区,用于保存读取的数据
            ref UInt32 ioLength);  // 指向长度单元,输入时为准备读取的长度,返回后为实际读取的长度

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347AbortRead")]
        public static extern bool CH347AbortRead(  // 放弃数据块读操作
            UInt32 iIndex);  // 指定CH347设备序号

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347WriteData0")]
        public static extern bool CH347WriteData0(  // 向0#端口写出数据块
            UInt32 iIndex,  // 指定CH347设备序号
            byte[] iBuffer,  // 指向一个缓冲区,放置准备写出的数据
            ref UInt32 ioLength);  // 指向长度单元,输入时为准备写出的长度,返回后为实际写出的长度

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347WriteData1")]
        public static extern bool CH347WriteData1(  // 向1#端口写出数据块
            UInt32 iIndex,  // 指定CH347设备序号
            byte[] iBuffer,  // 指向一个缓冲区,放置准备写出的数据
            ref UInt32 ioLength);  // 指向长度单元,输入时为准备写出的长度,返回后为实际写出的长度

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347AbortWrite")]
        public static extern bool CH347AbortWrite(  // 放弃数据块写操作
            UInt32 iIndex);  // 指定CH347设备序号

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347GetStatus")]
        public static extern bool CH347GetStatus(  // 通过CH347直接输入数据和状态
            UInt32 iIndex,  // 指定CH347设备序号
            ref UInt32 iStatus);  // 指向一个双字单元,用于保存状态数据,参考下面的位说明
        // 位7-位0对应CH347的D7-D0引脚
        // 位8对应CH347的ERR#引脚, 位9对应CH347的PEMP引脚, 位10对应CH347的INT#引脚, 位11对应CH347的SLCT引脚, 位23对应CH347的SDA引脚
        // 位13对应CH347的BUSY/WAIT#引脚, 位14对应CH347的AUTOFD#/DATAS#引脚,位15对应CH347的SLCTIN#/ADDRS#引脚

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347I2C_Set")]
        public static extern bool CH347I2C_Set(  // Set I2C parameters
            UInt32 iIndex,  // device serial number
            UInt32 iMode);  // I2C mode
                            //bit1-0
                            //00= 20KHz 
                            //01= 100KHz default
                            //10= 400KHz
                            //11= 750KHz
                            //bit 7-2 reserved 0

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347GPIO_Get")]
        public static extern bool CH347GPIO_Get(  // Get GPIO pins status
            UInt32 iIndex,  // device serial number
            ref byte iDir,      // GPIO Direction, 0 input, 1 output
            ref byte iData);    // GPIO high low status, 0 for low, 1 for high

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347GPIO_Set")]
        public static extern bool CH347GPIO_Set(  // Set GPIO pins status
            UInt32 iIndex,  // device serial number
            byte iEnable,   // GPIO Enable, 0 disable, 1 enable
            byte iDirOut,   // GPIO Direction, 0 input, 1 output
            byte iDataOut); // GPIO high low status, 0 for low, 1 for high


        [DllImport("CH347DLL.DLL", EntryPoint = "CH347Uart_Open")]
        public static extern IntPtr CH347Uart_Open(  // CH347 UART Open
                                     UInt32 iIndex);  // Device number


        [DllImport("CH347DLL.DLL", EntryPoint = "CH347Uart_Close")]
        public static extern bool CH347Uart_Close(  // CH347 UART Close
                             UInt32 iIndex);  // Device number

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347Uart_SetDeviceNotify")]
        public static extern bool CH347Uart_SetDeviceNotify(  // CH347 UART Notify
            UInt32 iIndex,  // CH347Device number
            string iDeviceID,  // pointer to a string of device ID and ends with \0
            mPCH347_NOTIFY_ROUTINE iNotifyRoutine);  // Routine,Cancel notify if NULL,run routine if routine is valid

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347Uart_Init")]
        public static extern bool CH347Uart_Init(  // CH347 UART Init
                             UInt32 iIndex,     // Device number
                             Int32 BaudRate,    // Uart BaudRate
                             byte ByteSize,     // Byte size 5 、 6 、 7 、 8 、 16
                             byte Parity,       // 0 None; 1 Odd; 2 Even; 3 Mark; 4 Space
                             byte StopBits,     // Option 0, 1, 2
                             byte ByteTimeout);  // setting byte timeout, unit 100us

        //1200、1800、2400、3600、4800、9600、14400、19200、28800、33600、38400、56000、57600、76800、115200、128000、153600、230400、460800、921600
        //1M、1.5M、2M、3M、4M、5M、6M、7M、7.5M.
        //The speed of 8M、9M for Dual UART only 

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347Uart_SetTimeout")]
        public static extern bool CH347Uart_SetTimeout(  // CH347 UART set timeout
                             UInt32 iIndex,     // Device number
                             UInt32 iWriteTimeout,    // Timeout setting
                             UInt32 iReadTimeout);    // Timeout readback


        [DllImport("CH347DLL.DLL", EntryPoint = "CH347Uart_Read")]
        public static extern bool CH347Uart_Read(  // CH347 UART Read
                             UInt32 iIndex,     // Device number
                             byte[] oBuffer,    // buffer for storage of input
                             UInt32 ioLength);    // the length to desired reading. it changes to actual received length

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347Uart_Write")]
        public static extern bool CH347Uart_Write(  // CH347 UART Write
                             UInt32 iIndex,     // Device number
                             byte[] iBuffer,    // buffer for output
                             UInt32 ioLength);    // the length to desired write. it changes to actual write length

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347Uart_QueryBufUpload")]
        public static extern bool CH347Uart_QueryBufUpload(  // CH347 UART Buffer size which is not captured yet
                             UInt32 iIndex,     // Device number
                             UInt64 ioLength);    // remaining byte quantity of the buffer 


        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SPI_Init")]
        public static extern bool CH347SPI_Init(  // CH347 SPI Init
                     UInt32 iIndex,     // Device number
                     ref _SPI_CONFIG mSpiCfgS);  // see SPI CONFIG (typedef struct)


        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SPI_GetCfg")]
        public static extern bool CH347SPI_GetCfg(  // CH347 SPI Get Config
             UInt32 iIndex,     // Device number
             _SPI_CONFIG mSpiCfgS);  // see SPI CONFIG (typedef struct)

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SPI_ChangeCS")]
        public static extern bool CH347SPI_ChangeCS(  // CH347 SPI change chip select
             UInt32 iIndex,     // Device number
             byte iStatus);  // 0 for disable chip select feature, 1 to enable



        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SPI_SetChipSelect")]
        public static extern bool CH347SPI_SetChipSelect(  // CH347 SPI set chip select
             UInt32 iIndex,     // Device number
             UInt16 iEnableSelect, // LSB 8bit for CS1, MSB 8bit for CS2. 0 to set, 1 to disable CS
             UInt16 iChipSelect, //LSB 8bit for CS1, MSB 8bit for CS2. 0 to disable, 1 to enable
             UInt32 iIsAutoDeativeCS, // LSB 16bit for CS1, MSB 16bit for CS2. feature of canceling CS after command complete
             UInt32 iActiveDelay, //LSB 16bit for CS1, MSB 16bit for CS2. delay between write and read, unit us
             UInt32 iDelayDeactrive); // LSB 16bit for CS1, MSB 16bit for CS2. canceling delay between write and read, unit us

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SPI_Write")]
        public static extern bool CH347SPI_Write(  // CH347 SPI write
             UInt32 iIndex,     // Device number
             UInt32 iChipSelect,     // bit7 1 to enable chip select. 0 to disable
             UInt32 iLength,     // byte quantity to write out
             UInt32 iWriteStep,     // page length of writing
             byte[] ioBuffer);     // Buffer to writing for MOSI

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SPI_Read")]
        public static extern bool CH347SPI_Read(  // CH347 SPI read
             UInt32 iIndex,     // Device number
             UInt32 iChipSelect,     // bit7 1 to enable chip select. 0 to disable
             UInt32 oLength,     // byte quantity to write out
             UInt32 iLength,     // byte quantity to read in
             byte[] ioBuffer);     // Buffer for MOSI output and storage of MISO

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SPI_WriteRead")]
        public static extern bool CH347SPI_WriteRead(  // CH347 SPI writeread
             UInt32 iIndex,     // Device number
             UInt32 iChipSelect,     // bit7 1 to enable chip select. 0 to disable
             UInt32 iLength,     // byte quantity to transfer
             byte[] ioBuffer);     // Buffer for MOSI output and storage of MISO


        [DllImport("CH347DLL.DLL", EntryPoint = "CH347ReadI2C")]
        public static extern bool CH347ReadI2C(  // 从I2C接口读取一个字节数据
            UInt32 iIndex,  // 指定CH347设备序号
            byte iDevice,  // 低7位指定I2C设备地址
            byte iAddr,  // 指定数据单元的地址
            ref byte oByte);  // 指向一个字节单元,用于保存读取的字节数据

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347WriteI2C")]
        public static extern bool CH347WriteI2C(  // 向I2C接口写入一个字节数据
            UInt32 iIndex,  // 指定CH347设备序号
            byte iDevice,  // 低7位指定I2C设备地址
            byte iAddr,  // 指定数据单元的地址
            byte iByte);  // 待写入的字节数据

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetExclusive")]
        public static extern bool CH347SetExclusive(  // 设置独占使用当前CH347设备
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iExclusive);  // 为0则设备可以共享使用,非0则独占使用

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetTimeout")]
        public static extern bool CH347SetTimeout(  // 设置USB数据读写的超时
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iWriteTimeout,  // 指定USB写出数据块的超时时间,以毫秒mS为单位,0xFFFFFFFF指定不超时(默认值)
            UInt32 iReadTimeout);  // 指定USB读取数据块的超时时间,以毫秒mS为单位,0xFFFFFFFF指定不超时(默认值)

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347ReadData")]
        public static extern bool CH347ReadData(  // 读取数据块
            UInt32 iIndex,  // 指定CH347设备序号
            byte[] oBuffer,  // 指向一个足够大的缓冲区,用于保存读取的数据
            ref UInt32 ioLength);  // 指向长度单元,输入时为准备读取的长度,返回后为实际读取的长度

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347WriteData")]
        public static extern bool CH347WriteData(  // 写出数据块
            UInt32 iIndex,  // 指定CH347设备序号
            byte[] iBuffer,  // 指向一个缓冲区,放置准备写出的数据
            ref UInt32 ioLength);  // 指向长度单元,输入时为准备写出的长度,返回后为实际写出的长度

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347GetDeviceName")]
        public static extern IntPtr CH347GetDeviceName(  // 返回指向CH347设备名称的缓冲区,出错则返回NULL
            UInt32 iIndex);  // 指定CH347设备序号,0对应第一个设备


        [DllImport("CH347DLL.DLL", EntryPoint = "CH347FlushBuffer")]
        public static extern bool CH347FlushBuffer(  // 清空CH347的缓冲区
            UInt32 iIndex);  // 指定CH347设备序号

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347WriteRead")]
        public static extern bool CH347WriteRead(  // 执行数据流命令,先输出再输入
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iWriteLength,  // 写长度,准备写出的长度
            byte[] iWriteBuffer,  // 指向一个缓冲区,放置准备写出的数据
            UInt32 iReadStep,  // 准备读取的单个块的长度, 准备读取的总长度为(iReadStep*iReadTimes)
            UInt32 iReadTimes,  // 准备读取的次数
            ref UInt32 oReadLength,  // 指向长度单元,返回后为实际读取的长度
            byte[] oReadBuffer);  // 指向一个足够大的缓冲区,用于保存读取的数据

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetDelaymS")]
        public static extern bool CH347SetDelaymS(  // 设置硬件异步延时,调用后很快返回,而在下一个流操作之前延时指定毫秒数
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iDelay);  // 指定延时的毫秒数

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347StreamI2C")]
        public static extern bool CH347StreamI2C(  // 处理I2C数据流,2线接口,时钟线为SCL引脚,数据线为SDA引脚(准双向I/O),速度约56K字节
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iWriteLength,  // 准备写出的数据字节数
            byte[] iWriteBuffer,  // 指向一个缓冲区,放置准备写出的数据,首字节通常是I2C设备地址及读写方向位
            UInt32 iReadLength,  // 准备读取的数据字节数
            byte[] oReadBuffer);  // 指向一个缓冲区,返回后是读入的数据

        public enum EEPROM_TYPE
        {
            ID_24C01,
            ID_24C02,
            ID_24C04,
            ID_24C08,
            ID_24C16,
            ID_24C32,
            ID_24C64,
            ID_24C128,
            ID_24C256,
            ID_24C512,
            ID_24C1024,
            ID_24C2048,
            ID_24C4096

        };

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347ReadEEPROM")]
        public static extern bool CH347ReadEEPROM(  // 从EEPROM中读取数据块,速度约56K字节
            UInt32 iIndex,  // 指定CH347设备序号
            EEPROM_TYPE iEepromID,  // 指定EEPROM型号
            UInt32 iAddr,  // 指定数据单元的地址
            UInt32 iLength,  // 准备读取的数据字节数
            byte[] oBuffer);  // 指向一个缓冲区,返回后是读入的数据

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347WriteEEPROM")]
        public static extern bool CH347WriteEEPROM(  // 向EEPROM中写入数据块
            UInt32 iIndex,  // 指定CH347设备序号
            EEPROM_TYPE iEepromID,  // 指定EEPROM型号
            UInt32 iAddr,  // 指定数据单元的地址
            UInt32 iLength,  // 准备写出的数据字节数
            byte[] iBuffer);  // 指向一个缓冲区,放置准备写出的数据

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347GetInput")]
        public static extern bool CH347GetInput(  // 通过CH347直接输入数据和状态,效率比CH347GetStatus更高
            UInt32 iIndex,  // 指定CH347设备序号
            ref UInt32 iStatus);  // 指向一个双字单元,用于保存状态数据,参考下面的位说明
        // 位7-位0对应CH347的D7-D0引脚
        // 位8对应CH347的ERR#引脚, 位9对应CH347的PEMP引脚, 位10对应CH347的INT#引脚, 位11对应CH347的SLCT引脚, 位23对应CH347的SDA引脚
        // 位13对应CH347的BUSY/WAIT#引脚, 位14对应CH347的AUTOFD#/DATAS#引脚,位15对应CH347的SLCTIN#/ADDRS#引脚

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetOutput")]
        public static extern bool CH347SetOutput(  // 设置CH347的I/O方向,并通过CH347直接输出数据
            /* ***** 谨慎使用该API, 防止修改I/O方向使输入引脚变为输出引脚导致与其它输出引脚之间短路而损坏芯片 ***** */
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iEnable,  // 数据有效标志,参考下面的位说明
                             // 位0为1说明iSetDataOut的位15-位8有效,否则忽略
                             // 位1为1说明iSetDirOut的位15-位8有效,否则忽略
                             // 位2为1说明iSetDataOut的7-位0有效,否则忽略
                             // 位3为1说明iSetDirOut的位7-位0有效,否则忽略
                             // 位4为1说明iSetDataOut的位23-位16有效,否则忽略
            UInt32 iSetDirOut,  // 设置I/O方向,某位清0则对应引脚为输入,某位置1则对应引脚为输出,并口方式下默认值为0x000FC000,参考下面的位说明
            UInt32 iSetDataOut);  // 输出数据,如果I/O方向为输出,那么某位清0时对应引脚输出低电平,某位置1时对应引脚输出高电平,参考下面的位说明
        // 位7-位0对应CH347的D7-D0引脚
        // 位8对应CH347的ERR#引脚, 位9对应CH347的PEMP引脚, 位10对应CH347的INT#引脚, 位11对应CH347的SLCT引脚
        // 位13对应CH347的WAIT#引脚, 位14对应CH347的DATAS#/READ#引脚,位15对应CH347的ADDRS#/ADDR/ALE引脚
        // 以下引脚只能输出,不考虑I/O方向: 位16对应CH347的RESET#引脚, 位17对应CH347的WRITE#引脚, 位18对应CH347的SCL引脚, 位29对应CH347的SDA引脚

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347Set_D5_D0")]
        public static extern bool CH347Set_D5_D0(  // 设置CH347的D5-D0引脚的I/O方向,并通过CH347的D5-D0引脚直接输出数据,效率比CH347SetOutput更高
            /* ***** 谨慎使用该API, 防止修改I/O方向使输入引脚变为输出引脚导致与其它输出引脚之间短路而损坏芯片 ***** */
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iSetDirOut,  // 设置D5-D0各引脚的I/O方向,某位清0则对应引脚为输入,某位置1则对应引脚为输出,并口方式下默认值为0x00全部输入
            UInt32 iSetDataOut);  // 设置D5-D0各引脚的输出数据,如果I/O方向为输出,那么某位清0时对应引脚输出低电平,某位置1时对应引脚输出高电平
        // 以上数据的位5-位0分别对应CH347的D5-D0引脚

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347StreamSPI3")]
        public static extern bool CH347StreamSPI3(  // 该API已失效,请勿使用
            UInt32 iIndex,
            UInt32 iChipSelect,
            UInt32 iLength,
            byte[] ioBuffer);

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347StreamSPI4")]
        public static extern bool CH347StreamSPI4(  // 处理SPI数据流,4线接口,时钟线为DCK/D3引脚,输出数据线为DOUT/D5引脚,输入数据线为DIN/D7引脚,片选线为D0/D1/D2,速度约68K字节
            /* SPI时序: DCK/D3引脚为时钟输出, 默认为低电平, DOUT/D5引脚在时钟上升沿之前的低电平期间输出, DIN/D7引脚在时钟下降沿之前的高电平期间输入 */
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iChipSelect,  // 片选控制, 位7为0则忽略片选控制, 位7为1则参数有效: 位1位0为00/01/10分别选择D0/D1/D2引脚作为低电平有效片选
            UInt32 iLength,  // 准备传输的数据字节数
            byte[] ioBuffer);  // 指向一个缓冲区,放置准备从DOUT写出的数据,返回后是从DIN读入的数据

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347StreamSPI5")]
        public static extern bool CH347StreamSPI5(  // 处理SPI数据流,5线接口,时钟线为DCK/D3引脚,输出数据线为DOUT/D5和DOUT2/D4引脚,输入数据线为DIN/D7和DIN2/D6引脚,片选线为D0/D1/D2,速度约30K字节*2
            /* SPI时序: DCK/D3引脚为时钟输出, 默认为低电平, DOUT/D5和DOUT2/D4引脚在时钟上升沿之前的低电平期间输出, DIN/D7和DIN2/D6引脚在时钟下降沿之前的高电平期间输入 */
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iChipSelect,  // 片选控制, 位7为0则忽略片选控制, 位7为1则参数有效: 位1位0为00/01/10分别选择D0/D1/D2引脚作为低电平有效片选
            UInt32 iLength,  // 准备传输的数据字节数
            byte[] ioBuffer,  // 指向一个缓冲区,放置准备从DOUT写出的数据,返回后是从DIN读入的数据
            byte[] ioBuffer2);  // 指向第二个缓冲区,放置准备从DOUT2写出的数据,返回后是从DIN2读入的数据

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347BitStreamSPI")]
        public static extern bool CH347BitStreamSPI(  // 处理SPI位数据流,4线/5线接口,时钟线为DCK/D3引脚,输出数据线为DOUT/DOUT2引脚,输入数据线为DIN/DIN2引脚,片选线为D0/D1/D2,速度约8K位*2
            UInt32 iIndex,  // 指定CH347设备序号
            UInt32 iLength,  // 准备传输的数据位数,一次最多896,建议不超过256
            byte[] ioBuffer);  // 指向一个缓冲区,放置准备从DOUT/DOUT2/D2-D0写出的数据,返回后是从DIN/DIN2读入的数据
        /* SPI时序: DCK/D3引脚为时钟输出, 默认为低电平, DOUT/D5和DOUT2/D4引脚在时钟上升沿之前的低电平期间输出, DIN/D7和DIN2/D6引脚在时钟下降沿之前的高电平期间输入 */
        /* ioBuffer中的一个字节共8位分别对应D7-D0引脚, 位5输出到DOUT, 位4输出到DOUT2, 位2-位0输出到D2-D0, 位7从DIN输入, 位6从DIN2输入, 位3数据忽略 */
        /* 在调用该API之前,应该先调用CH347Set_D5_D0设置CH347的D5-D0引脚的I/O方向,并设置引脚的默认电平 */

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetBufUpload")]
        public static extern bool CH347SetBufUpload(  // 设定内部缓冲上传模式
            UInt32 iIndex,  // 指定CH347设备序号,0对应第一个设备
            UInt32 iEnableOrClear);  // 为0则禁止内部缓冲上传模式,使用直接上传,非0则启用内部缓冲上传模式并清除缓冲区中的已有数据
        // 如果启用内部缓冲上传模式,那么CH347驱动程序创建线程自动接收USB上传数据到内部缓冲区,同时清除缓冲区中的已有数据,当应用程序调用CH347ReadData后将立即返回缓冲区中的已有数据

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347QueryBufUpload")]
        public static extern Int32 CH347QueryBufUpload(  // 查询内部上传缓冲区中的已有数据包个数,成功返回数据包个数,出错返回-1
            UInt32 iIndex);  // 指定CH347设备序号,0对应第一个设备

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetBufDownload")]
        public static extern bool CH347SetBufDownload(  // 设定内部缓冲下传模式
            UInt32 iIndex,  // 指定CH347设备序号,0对应第一个设备
            UInt32 iEnableOrClear);  // 为0则禁止内部缓冲下传模式,使用直接下传,非0则启用内部缓冲下传模式并清除缓冲区中的已有数据
        // 如果启用内部缓冲下传模式,那么当应用程序调用CH347WriteData后将仅仅是将USB下传数据放到内部缓冲区并立即返回,而由CH347驱动程序创建的线程自动发送直到完毕

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347QueryBufDownload")]
        public static extern Int32 CH347QueryBufDownload(  // 查询内部下传缓冲区中的剩余数据包个数(尚未发送),成功返回数据包个数,出错返回-1
            UInt32 iIndex);  // 指定CH347设备序号,0对应第一个设备

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347ResetInter")]
        public static extern bool CH347ResetInter(  // 复位中断数据读操作
            UInt32 iIndex);  // 指定CH347设备序号

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347ResetRead")]
        public static extern bool CH347ResetRead(  // 复位数据块读操作
            UInt32 iIndex);  // 指定CH347设备序号

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347ResetWrite")]
        public static extern bool CH347ResetWrite(  // 复位数据块写操作
            UInt32 iIndex);  // 指定CH347设备序号

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]

        public delegate void mPCH347_NOTIFY_ROUTINE(  // 设备事件通知回调程序
                    UInt32 iEventStatus);  // 设备事件和当前状态(在下行定义): 0=设备拔出事件, 3=设备插入事件


        public const int CH347_DEVICE_ARRIVA = 3;	// 设备插入事件,已经插入
        public const int CH347_DEVICE_REMOVE_PEND = 1;		// 设备将要拔出
        public const int CH347_DEVICE_REMOVE = 0;		// 设备拔出事件,已经拔出

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetDeviceNotify")]
        public static extern bool CH347SetDeviceNotify(  // 设定设备事件通知程序
            UInt32 iIndex,  // 指定CH347设备序号,0对应第一个设备
            string iDeviceID,  // 可选参数,指向字符串,指定被监控的设备的ID,字符串以\0终止
            mPCH347_NOTIFY_ROUTINE iNotifyRoutine);  // 指定设备事件回调程序,为NULL则取消事件通知,否则在检测到事件时调用该程序

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetupSerial")]
        public static extern bool CH347SetupSerial(  // 设定CH347的串口特性,该API只能用于工作于串口方式的CH347芯片
            UInt32 iIndex,  // 指定CH347设备序号,0对应第一个设备
            UInt32 iParityMode,  // 指定CH347串口的数据校验模式: NOPARITY/ODDPARITY/EVENPARITY/MARKPARITY/SPACEPARITY
            UInt32 iBaudRate);  // 指定CH347串口的通讯波特率值,可以是50至3000000之间的任意值

        /*  以下API可以用于工作于串口方式的CH347芯片,除此之外的API一般只能用于并口方式的CH347芯片
	        CH347OpenDevice
	        CH347CloseDevice
	        CH347SetupSerial
	        CH347ReadData
	        CH347WriteData
	        CH347SetBufUpload
	        CH347QueryBufUpload
	        CH347SetBufDownload
	        CH347QueryBufDownload
	        CH347SetDeviceNotify
	        CH347GetStatus
        //  以上是主要API,以下是次要API
	        CH347GetVersion
	        CH347DriverCommand
	        CH347GetDrvVersion
	        CH347ResetDevice
	        CH347GetDeviceDescr
	        CH347GetConfigDescr
	        CH347SetIntRoutine
	        CH347ReadInter
	        CH347AbortInter
	        CH347AbortRead
	        CH347AbortWrite
	        CH347ReadI2C
	        CH347WriteI2C
	        CH347SetExclusive
	        CH347SetTimeout
	        CH347GetDeviceName
	        CH347GetVerIC
	        CH347FlushBuffer
	        CH347WriteRead
	        CH347ResetInter
	        CH347ResetRead
	        CH347ResetWrite
        */
        [DllImport("CH347DLL.DLL", EntryPoint = "CH347OpenDeviceEx")]
        public static extern IntPtr CH347OpenDeviceEx(   // 打开CH347设备,返回句柄,出错则无效
            UInt32 iIndex);        // 指定CH347设备序号,0对应插入的第一个设备,1对应插入的第二个设备,为节约设备设备序号资源,用完后要关闭设备

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347CloseDeviceEx")]
        public static extern void CH347CloseDeviceEx(  // 关闭CH347设备
            UInt32 iIndex);        // 指定CH347设备序号

        [DllImport("CH347DLL.DLL", EntryPoint = "CCH347GetDeviceNameEx")]
        public static extern IntPtr CH347GetDeviceNameEx(   // 返回指向CH347设备名称的缓冲区,出错则返回NULL
            UInt32 iIndex);           // 指定CH347设备序号,0对应第一个设备

        [DllImport("CH347DLL.DLL", EntryPoint = "CH347SetDeviceNotifyEx")]
        public static extern bool CH347SetDeviceNotifyEx(       // 设定设备事件通知程序
            UInt32 iIndex,           // 指定CH347设备序号,0对应第一个设备
            string iDeviceID,        // 可选参数,指向字符串,指定被监控的设备的ID,字符串以\0终止
            mPCH347_NOTIFY_ROUTINE iNotifyRoutine); // 指定设备事件回调程序,为NULL则取消事件通知,否则在检测到事件时调用该程序


    }
}
