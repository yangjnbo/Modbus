/*
   这是一个开源的Modbus协议库，用于在.NET平台上实现Modbus通信
   目前仅支持Modbus TCP Client 和 Server 的实现
   该库的代码采用MIT协议开源，您可以在遵守协议的前提下自由使用、修改和分发该库。
*/

using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using Modbus.Core;
using Modbus.Protocol;

namespace Modbus
{
    /// <summary>
    /// Modbus TCP 服务器类，用于实现 Modbus TCP 服务器功能
    /// </summary>
    public class ModbusTcpServer
    {
        /// <summary>
        /// 用于保护 Databases 列表的锁对象
        /// </summary>
        private readonly Lock _databasesLock = new();

        /// <summary>
        /// 数据库列表，存储不同 UnitId 的数据库
        /// </summary>
        private Database Database { get; init; }

        /// <summary>
        /// 服务器监听的端口号，默认为 502
        /// </summary>
        public int Port { get; init; } = 502;

        /// <summary>
        /// 写入寄存器事件，当有寄存器写入操作时触发
        /// </summary>
        public event EventHandler<WriteRegisterEventArgs>? WriteRegisterEven;

        /// <summary>
        /// 网络传输监听器，用于监听客户端连接
        /// </summary>
        private readonly TcpListener _tcpListener;

        /// <summary>
        /// 信号量，用于控制并发处理的客户端数量
        /// </summary>
        private readonly SemaphoreSlim _SemaphoreSlim;
        public int InitialCount = 100;
        public int maxCount = 1000;
        public bool IsRunning = false;

        /// <summary>
        /// 初始化 Modbus TCP 服务器实例
        /// </summary>
        /// <param name="port">服务器监听的端口号，默认为 502</param>
        /// <param name="unitid">默认数据库的 UnitId，默认为 1</param>
        public ModbusTcpServer(int port = 502, byte unitid = 1)
        {
            try
            {
                // 设置服务器监听端口
                Port = port;
                // 初始化数据库
                Database = new Database(unitid);
                // 初始化网络传输监听器
                _tcpListener = new TcpListener(IPAddress.Any, port);
                // 初始化信号量，初始计数为 100，最大计数为 1000
                _SemaphoreSlim = new SemaphoreSlim(InitialCount, maxCount);
            }
            catch (Exception ex)
            {
                // 记录服务器初始化时发生的异常
                ModbusLogger.LogError(ex, "TcpServer 初始化时发生异常");
                throw;
            }
        }

        /// <summary>
        /// 启动 Modbus TCP 服务器
        /// </summary>
        public void Start()
        {
            // 启动网络传输监听器
            _tcpListener.Start();
            IsRunning = true;
            // 启动服务器异步任务
            _ = RunAsync(CancellationToken.None);
            // 记录服务器启动日志
            ModbusLogger.LogInformation("Modbus Tcp Server 开始运行", DateTime.Now);
        }

        /// <summary>
        /// 停止 Modbus TCP 服务器
        /// </summary>
        public void Stop()
        {
            // 停止网络传输监听器
            if (IsRunning == true)
            {
                _tcpListener.Stop();
                IsRunning = false;
            }
            // 记录服务器停止日志
            ModbusLogger.LogInformation("Modbus Tcp Server 停止运行", DateTime.Now);
        }

        /// <summary>
        /// 异步运行服务器，处理客户端请求
        /// </summary>
        /// <param name="cts">取消令牌，用于取消任务</param>
        public async Task RunAsync(CancellationToken cts)
        {
            // 当网络传输监听器正在运行时持续循环
            while (IsRunning)
            {
                try
                {
                    // 接受新的客户端连接
                    TcpClient client = await _tcpListener.AcceptTcpClientAsync(cts);

                    // 等待信号量，确保并发数量不超过限制
                    await _SemaphoreSlim.WaitAsync(cts);

                    // 为每个客户端连接创建独立的长期运行异步任务
                    await HandleClientRequestAsync(client, cts);

                }
                catch (IOException ex)
                {
                    if (ex.Message.Contains("远程主机强迫关闭了一个现有的连接"))
                    {
                        ModbusLogger.LogInformation("客户端主动断开连接");
                    }
                    else
                    {
                        throw;
                    }
                }      
                catch
                {
                    ModbusLogger.LogError("接受客户端连接时发生异常");
                }
            }
        }

        /// <summary>
        /// 根据客户端请求获取响应报文
        /// </summary>
        /// <param name="Request_Adu">客户端请求报文</param>
        /// <returns>响应报文</returns>
        public Memory<byte> GetResponse(Memory<byte> Request_Pdu)
        {
            try
            {
                // 获取请求报文中的 UnitId
                byte UnitId = Request_Pdu.Span[0];
                ModbusLogger.LogInformation("处理请求 - UnitId: {UnitId}", UnitId);

                // 初始化响应报文的 PDU 部分
                Memory<byte> Response_Pdu = Memory<byte>.Empty;
                // 构建响应报文
                lock (_databasesLock)
                {
                    // 根据 UnitId 获取对应的数据库
                    if (UnitId != Database.UnitId)
                    { }

                    // 获取请求报文中的功能码
                    FuncationCode functionCode = (FuncationCode)Request_Pdu.Span[1];
                    ModbusLogger.LogInformation("处理功能码: {functionCode}", functionCode);
                    ushort startAddress = 0;
                    ushort count = 0;
                    // 根据不同的功能码处理请求
                    switch (functionCode)
                    {
                        case FuncationCode.ReadCoils:
                            // 从请求报文中读取起始地址和数量
                            startAddress = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(2, 2));
                            count = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(4, 2));
                            // 读取线圈状态
                            Response_Pdu = Database.ReadCoils(startAddress, count);
                            break;

                        case FuncationCode.ReadDiscreteInputs:
                            // 从请求报文中读取起始地址和数量
                            startAddress = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(2, 2));
                            count = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(4, 2));
                            // 读取离散输入状态
                            Response_Pdu = Database.ReadDiscreteInputs(startAddress, count);
                            break;

                        case FuncationCode.ReadHoldingRegisters:
                            // 从请求报文中读取起始地址和数量
                            startAddress = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(2, 2));
                            count = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(4, 2));
                            // 读取保持寄存器值
                            Response_Pdu = Database.ReadHoldingRegisters(startAddress, count);
                            break;

                        case FuncationCode.ReadInputRegisters:
                            // 从请求报文中读取起始地址和数量
                            startAddress = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(2, 2));
                            count = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(4, 2));
                            // 读取输入寄存器值
                            Response_Pdu = Database.ReadInputRegisters(startAddress, count);
                            break;

                        case FuncationCode.WriteSingleCoil:
                            // 从请求报文中读取起始地址和数量
                            startAddress = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(2, 2));
                            count = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(4, 2));
                            // 获取请求报文中的数据部分
                            Memory<byte> memdata = Request_Pdu.Slice(4, 2);
                            // 写入单个线圈
                            Response_Pdu = Database.WriteSingalCoil(startAddress, memdata);
                            // 触发写入寄存器事件
                            OnWriteRegisterEvent(new WriteRegisterEventArgs { Type = RegisterType.coils, index = startAddress, count = 1 });
                            break;

                        case FuncationCode.WriteSingleRegisters:
                            // 从请求报文中读取起始地址和数量
                            startAddress = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(2, 2));
                            count = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(4, 2));
                            // 获取请求报文中的数据部分
                            memdata = Request_Pdu.Slice(4, 2);
                            // 写入单个保持寄存器
                            Response_Pdu = Database.WriteSingalRegisters(startAddress, memdata);
                            // 触发写入寄存器事件
                            OnWriteRegisterEvent(new WriteRegisterEventArgs { Type = RegisterType.holdingRegisters, index = startAddress, count = 1 });
                            break;

                        case FuncationCode.WriteMultipleCoils:
                            // 从请求报文中读取起始地址和数量
                            startAddress = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(2, 2));
                            count = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(4, 2));
                            // 获取请求报文中数据的字节长度
                            int DataByteLength = Request_Pdu.Span[6];
                            // 获取请求报文中的数据部分
                            memdata = Request_Pdu.Slice(7, DataByteLength);
                            // 写入多个线圈
                            Response_Pdu = Database.WriteMultipleCoils(startAddress, count, memdata);
                            // 触发写入寄存器事件
                            OnWriteRegisterEvent(new WriteRegisterEventArgs { Type = RegisterType.coils, index = startAddress, count = count });
                            break;

                        case FuncationCode.WriteMultipleRegisters:
                            // 从请求报文中读取起始地址和数量
                            startAddress = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(2, 2));
                            count = BinaryPrimitives.ReadUInt16BigEndian(Request_Pdu.Span.Slice(4, 2));
                            // 获取请求报文中数据的字节长度
                            DataByteLength = Request_Pdu.Span[6];
                            // 获取请求报文中的数据部分
                            memdata = Request_Pdu.Slice(7, DataByteLength);
                            // 写入多个保持寄存器
                            Response_Pdu = Database.WriteMultipleRegisters(startAddress, count, memdata);
                            // 触发写入寄存器事件
                            OnWriteRegisterEvent(new WriteRegisterEventArgs { Type = RegisterType.holdingRegisters, index = startAddress, count = count });
                            break;

                        default:
                            // 未知功能码，构建非法功能异常响应报文
                            ModbusLogger.LogWarning("不支持的功能码: {functionCode}", functionCode);
                            Response_Pdu = new Memory<byte>([UnitId, (byte)(0x80 | (byte)functionCode), (byte)ExceptionCode.IllegalFunction]);
                            break;
                    }

                }
                return Response_Pdu;
            }
            catch (Exception ex)
            {
                // 记录解析请求报文时发生的异常
                ModbusLogger.LogError(ex, "解析请求报文时发生异常. UnitId: {UnitId}, FunctionCode: {functionCode}, Error: {ErrorMessage}",
                    Request_Pdu.Span[0],
                    (FuncationCode)Request_Pdu.Span[1],
                    ex.Message);
                return Memory<byte>.Empty;
            }
        }
 
        /// <summary>
        /// 异步处理客户端请求
        /// </summary>
        /// <param name="client">客户端连接</param>
        /// <param name="cancellationToken">取消令牌，用于取消任务</param>
        public async Task HandleClientRequestAsync(TcpClient client, CancellationToken cancellationToken = default)
        {
            try
            {
                // 获取客户端网络流
                using var stream = client.GetStream();

                // 当任务未取消且客户端连接有效时持续循环
                while (!cancellationToken.IsCancellationRequested && client.Connected)
                {
                    // 初始化读取缓冲区
                    Memory<byte> read_buffer = new byte[1024];
                    // 读取客户端发送的报文
                    var bytesRead = await stream.ReadAsync(read_buffer, cancellationToken);
                    if (bytesRead == 0)
                    {
                        ModbusLogger.LogInformation("客户端连接已关闭");
                        return;
                    }

                    // 获取请求报文
                    Memory<byte> Request_Adu = read_buffer[..bytesRead];
                    // 记录收到的请求报文
                    ModbusLogger.LogInformation("收到请求: {Request_Adu}", BitConverter.ToString(Request_Adu.ToArray()));

                    // 解析请求报文的 MBAP 头
                    var Request_Mbap = MbapHelper.GetMbap(Request_Adu);
                    // 验证并获取请求报文的 PDU 部分
                    Memory<byte> Request_Pdu = Request_Adu.ValidateAdu(Request_Mbap);
                    // 记录请求报文的 PDU 部分
                    ModbusLogger.LogInformation("请求 PDU: {Request_Pdu}", BitConverter.ToString(Request_Pdu.ToArray()));

                    // 构建响应报文
                    Memory<byte> Response_Pdu = GetResponse(Request_Pdu);

                    if (!Response_Pdu.IsEmpty)
                    {
                        // 构建响应报文
                        Memory<byte> Response_Adu = Response_Pdu.PrependMbap(Request_Mbap);
                        // 记录响应报文
                        ModbusLogger.LogInformation("发送响应: {Response_Adu}", BitConverter.ToString(Response_Adu.ToArray()));

                        // 发送响应报文
                        await stream.WriteAsync(Response_Adu, cancellationToken);
                    }
                    else
                    {
                        ModbusLogger.LogWarning("空响应报文，跳过发送");
                    }
                }
            }
            catch (Exception)
            {
                // 释放客户端
                client.Close();
                // 释放客户端资源
                client.Dispose();
                // 释放信号量
                _SemaphoreSlim.Release();
                throw;
            }
        }

        protected virtual void OnWriteRegisterEvent(WriteRegisterEventArgs e)
        {
            WriteRegisterEven?.Invoke(this, e);
        }

        public class WriteRegisterEventArgs : EventArgs
        {
            public RegisterType Type;
            public ushort index;
            public ushort count = 1;
        }
    }

}

