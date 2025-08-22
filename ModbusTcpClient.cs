/*
   这是一个开源的Modbus协议库，用于在.NET平台上实现Modbus通信
   目前仅支持Modbus TCP Client 和 Server 的实现
   该库的代码采用MIT协议开源，您可以在遵守协议的前提下自由使用、修改和分发该库。
*/

using System.Net;
using System.Net.Sockets;
using Modbus.Core;
using Modbus.Protocol;

namespace Modbus;
public partial class ModbusTcpClient
{
    // Tcp Client
    private TcpClient _client = new();
    // 发送或接收超时信号源
    private CancellationTokenSource _cts = new();
    // 私有变量 _Cancelltimeout
    private int _Cancelltimeout;
    // IP地址
    public string Ip = "127.0.0.1";
    // Port
    public int Port = 502;
    // 发送或接收的超时时间 实际总超时时间 = 发送超时时间 + 服务器响应时间 + 接收超时时间
    public int Timeout
    {
        get => _Cancelltimeout;
        set
        {
            _Cancelltimeout = value;
            if (value > 0)
                _cts = new CancellationTokenSource(_Cancelltimeout);
        }
    }
    // 服务器响应时间
    public int ServerHandelTimeout = 20;
    // 发送超时时间
    public int SendTimeout = 20;
    // 接收超时时间
    public int ReceiveTimeout = 20;
    public async Task<bool[]> ReadCoilsAsync(byte unitid, ushort startaddress, ushort count)
    {
        Memory<byte> RequestPdu = Req_Builder.Build_Req_ReadCoils(unitid, startaddress, count);
        Mbap RequestMbap = new();
        Memory<byte> RequestAdu = RequestPdu.PrependMbap(RequestMbap);
        Memory<byte> ResponseAdu = await SendRequestAndGetResponseAsync(RequestAdu);
        Memory<byte> ResponsePdu = ResponseAdu.ValidateAdu(RequestMbap);
        return ResponsePdu[3..].ToBoolArray(count);

    }
    public async Task<bool[]> ReadDiscreteInputsAsync(byte unitid, ushort startaddress, ushort count)
    {
        Memory<byte> RequestPdu = Req_Builder.Build_Req_ReadDiscreteInputs(unitid, startaddress, count);
        Mbap RequestMbap = new();
        Memory<byte> RequestAdu = RequestPdu.PrependMbap(RequestMbap);
        Memory<byte> ResponseAdu = await SendRequestAndGetResponseAsync(RequestAdu);
        Memory<byte> ResponsePdu = ResponseAdu.ValidateAdu(RequestMbap);
        return ResponsePdu[3..].ToBoolArray(count);

    }
    public async Task<Memory<byte>> ReadHoldingRegistersAsync(byte unitid, ushort startaddress, ushort count)
    {
        Memory<byte> RequestPdu = Req_Builder.Build_Req_ReadHoldingRegisters(unitid, startaddress, count);
        Mbap RequestMbap = new();
        Memory<byte> RequestAdu = RequestPdu.PrependMbap(RequestMbap);
        Memory<byte> ResponseAdu = await SendRequestAndGetResponseAsync(RequestAdu);
        Memory<byte> ResponsePdu = ResponseAdu.ValidateAdu(RequestMbap);
        return ResponsePdu[3..];
    }
    public async Task<Memory<byte>> ReadInputRegistersAsync(byte unitid, ushort startaddress, ushort count)
    {
        Memory<byte> RequestPdu = Req_Builder.Build_Req_ReadInputRegisters(unitid, startaddress, count);
        Mbap RequestMbap = new();
        Memory<byte> RequestAdu = RequestPdu.PrependMbap(RequestMbap);
        Memory<byte> ResponseAdu = await SendRequestAndGetResponseAsync(RequestAdu);
        Memory<byte> ResponsePdu = ResponseAdu.ValidateAdu(RequestMbap);
        return ResponsePdu[3..];
    }
    public async Task<bool> WriteSingleCoilAsync(byte unitid, ushort startaddress, bool data)
    {
        Memory<byte> RequestPdu = Req_Builder.Build_Req_WriteSingleCoil(unitid, startaddress, data);
        Mbap RequestMbap = new();
        Memory<byte> RequestAdu = RequestPdu.PrependMbap(RequestMbap);
        Memory<byte> ResponseAdu = await SendRequestAndGetResponseAsync(RequestAdu);
        Memory<byte> ResponsePdu = ResponseAdu.ValidateAdu(RequestMbap);
        if (ResponsePdu.Length > 3)
        {
            ModbusLogger.LogInformation("写入单个线圈成功");

            return true;
        }
        else
        {
            ModbusLogger.LogError($"写入单个线圈失败,{(ExceptionCode)(ResponsePdu.Span[2])}");
            return false;
        }
    }
    public async Task<bool> WriteSingleRegisterAsync(byte unitid, ushort startaddress, byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "输入数据不能为null");
        }
        if (data.Length != 2)
        {
            throw new ArgumentException("写入单个寄存器时数据长度必须为2字节", nameof(data));
        }

        Memory<byte> RequestPdu = Req_Builder.Build_Req_WriteSingleRegisters(unitid, startaddress, data);
        Mbap RequestMbap = new();
        Memory<byte> RequestAdu = RequestPdu.PrependMbap(RequestMbap);
        Memory<byte> ResponseAdu = await SendRequestAndGetResponseAsync(RequestAdu);
        Memory<byte> ResponsePdu = ResponseAdu.ValidateAdu(RequestMbap);
        if (ResponsePdu.Length > 3)
        {
            ModbusLogger.LogInformation("写入单个线圈成功");

            return true;
        }
        else
        {
            ModbusLogger.LogError($"写入单个线圈失败,{(ExceptionCode)(ResponsePdu.Span[2])}");
            return false;
        }
    }
    public async Task<bool> WriteMultipleCoilsAsync(byte unitid, ushort startaddress, bool[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "输入数据不能为null");
        }
        if (data.Length == 0)
        {
            throw new ArgumentException("写入多个线圈时数据不能为空", nameof(data));
        }

        Memory<byte> RequestPdu = Req_Builder.Build_Req_WriteMultipleCoils(unitid, startaddress, data);
        Mbap RequestMbap = new();
        Memory<byte> RequestAdu = RequestPdu.PrependMbap(RequestMbap);
        Memory<byte> ResponseAdu = await SendRequestAndGetResponseAsync(RequestAdu);
        Memory<byte> ResponsePdu = ResponseAdu.ValidateAdu(RequestMbap);
        if (ResponsePdu.Length > 3)
        {
            ModbusLogger.LogInformation("写入单个线圈成功");

            return true;
        }
        else
        {
            ModbusLogger.LogError($"写入单个线圈失败,{(ExceptionCode)(ResponsePdu.Span[2])}");
            return false;
        }
    }
    public async Task<bool> WriteMultipleRegistersAsync(byte unitid, ushort startaddress, byte[] data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data), "输入数据不能为null");
        }
        if (data.Length % 2 != 0)
        {
            throw new ArgumentException("写入多个寄存器时数据长度必须是2的倍数", nameof(data));
        }
        if (data.Length == 0)
        {
            throw new ArgumentException("写入多个寄存器时数据不能为空", nameof(data));
        }

        Memory<byte> RequestPdu = Req_Builder.Build_Req_WriteMultipleRegisters(unitid, startaddress, data);
        Mbap RequestMbap = new();
        Memory<byte> RequestAdu = RequestPdu.PrependMbap(RequestMbap);
        Memory<byte> ResponseAdu = await SendRequestAndGetResponseAsync(RequestAdu);
        Memory<byte> ResponsePdu = ResponseAdu.ValidateAdu(RequestMbap);
        if (ResponsePdu.Length > 3)
        {
            ModbusLogger.LogInformation("写入单个线圈成功");

            return true;
        }
        else
        {
            ModbusLogger.LogError($"写入单个线圈失败,{(ExceptionCode)(ResponsePdu.Span[2])}");
            return false;
        }
    }
    private async Task<Memory<byte>> SendRequestAndGetResponseAsync(Memory<byte> request_adu)
    {
        const int maxRetryCount = 3;
        int retryCount = 0;

        while (retryCount < maxRetryCount)
        {
            try
            {
                // 建立连接
                if (!_client.Connected)
                {
                    _client = new TcpClient();
                    await _client.ConnectAsync(IPAddress.Parse(Ip), Port, _cts.Token);
                    ModbusLogger.LogInformation("成功连接到服务器 {Ip}:{Port}", Ip, Port);
                }

                // 发送请求报文
                var stream = _client.GetStream();
                await stream.WriteAsync(request_adu, _cts.Token);

                // 等待服务器处理
                await Task.Delay(ServerHandelTimeout, _cts.Token);

                // 初始化接收缓冲区
                Memory<byte> buffer = new byte[1024];
                var readnumber = await stream.ReadAsync(buffer, _cts.Token);

                if (readnumber == 0)
                {
                    ModbusLogger.LogWarning("接收到空响应，尝试重新连接");
                    _client.Dispose();
                    _client = new TcpClient();
                    retryCount++;
                    continue;
                }

                return buffer[..readnumber];
            }
            catch (OperationCanceledException)
            {
                ModbusLogger.LogError("操作超时");
                throw new TimeoutException("操作超时");
            }
            catch (SocketException ex)
            {
                ModbusLogger.LogError($"Socket错误: {ex.SocketErrorCode}");
                throw;
            }
            catch (IOException ex)
            {
                ModbusLogger.LogError($"IO错误: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                ModbusLogger.LogError($"未知错误: {ex.Message}");
                throw;
            }
        }

        throw new IOException("达到最大重试次数仍未能成功获取响应");
    }

}
