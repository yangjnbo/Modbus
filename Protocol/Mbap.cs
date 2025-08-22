/*
   这是一个开源的Modbus协议库，用于在.NET平台上实现Modbus通信
   目前仅支持Modbus TCP Client 和 Server 的实现
   该库的代码采用MIT协议开源，您可以在遵守协议的前提下自由使用、修改和分发该库。
*/

using System.Buffers.Binary;

namespace Modbus.Protocol;
/// <summary>
/// 表示 Modbus 应用协议头（MBAP）结构。
/// MBAP 头用于在 Modbus TCP 通信中标识事务、协议和数据长度。
/// </summary>
public struct Mbap
{
    /// <summary>
    /// 事务标识符，用于匹配请求和响应。
    /// </summary>
    public ushort TransactionId;

    /// <summary>
    /// 协议标识符，Modbus 协议固定为 0x0000。
    /// </summary>
    public ushort ProtocolId;

    /// <summary>
    /// 后续数据的长度（不包含 MBAP 头本身）。
    /// </summary>
    public ushort Length;

    /// <summary>
    /// 将 MBAP 结构转换为字节数组。
    /// </summary>
    /// <returns>转换后的字节数组。</returns>
    public readonly byte[] GetBytes()
    {
        try
        {
            // 创建一个长度为 6 的字节数组，因为每个 ushort 占 2 字节，共 3 个 ushort
            byte[] bytes = new byte[6];
            // 将事务标识符以大端字节序写入字节数组
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(0, 2), TransactionId);
            // 将协议标识符以大端字节序写入字节数组
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(2, 2), ProtocolId);
            // 将长度以大端字节序写入字节数组
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan(4, 2), Length);
            return bytes;
        }
        catch(Exception ex)
        {
            // 记录 MBAP 编码时出现的异常
            ModbusLogger.LogError(ex, "Mbap 编码时出现异常");
            // 出现异常时返回全 0 的字节数组，原代码返回长度 6 数组，此处保持一致
            return [0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
        }
    }
}



