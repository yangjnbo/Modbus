/*
   这是一个开源的Modbus协议库，用于在.NET平台上实现Modbus通信
   目前仅支持Modbus TCP Client 和 Server 的实现
   该库的代码采用MIT协议开源，您可以在遵守协议的前提下自由使用、修改和分发该库。
*/

using System.Buffers.Binary;

namespace Modbus.Protocol;
/// <summary>
/// 这是一个Modbus MBAP头解析类
/// 提供了解析Modbus MBAP头的方法
/// 所有的方法都是静态的，不需要实例化即可使用
/// </summary>
public static class MbapHelper
{
    /// <summary>
    /// 从ADU数据中解析出MBAP头
    /// </summary>
    /// <param name="adu">ADU数据</param>
    /// <returns>解析出的MBAP头</returns>
    public static Mbap GetMbap(this Memory<byte> adu)
    {
        // 检查ADU数据是否为空或长度是否小于6字节，MBAP头固定为6字节，若不满足条件则无法解析
        if (adu.IsEmpty || adu.Length < 6)
        {
            throw new ArgumentException("ADU数据为空或长度不足, 无法解析MBAP头");
        }
        // 创建一个新的Mbap对象用于存储解析结果
        Mbap mbap = new()
        {
            // 从ADU数据的前2个字节（索引0-1）以大端序读取事务标识符
            TransactionId = BinaryPrimitives.ReadUInt16BigEndian(adu.Span[0..2]),
            // 从ADU数据的第3、4个字节（索引2-3）以大端序读取协议标识符
            ProtocolId = BinaryPrimitives.ReadUInt16BigEndian(adu.Span[2..4]),
            // 从ADU数据的第5、6个字节（索引4-5）以大端序读取长度
            Length = BinaryPrimitives.ReadUInt16BigEndian(adu.Span[4..6])
        };
        // 返回解析得到的MBAP头
        return mbap;
    }

    /// <summary>
    ///  验证ADU数据是否有效
    /// </summary>
    /// <param name="adu">ADU数据</param>
    /// <returns>返回不包含MBAP头的PDU数据</returns>
    public static Memory<byte> ValidateAdu(this Memory<byte> adu, Mbap mbap = new Mbap())
    {
        // 从ADU数据中解析出MBAP头
        Mbap Adu_mbap = adu.GetMbap();
        
        // 验证事务标识符是否匹配，若不匹配则抛出异常
        if (Adu_mbap.TransactionId != mbap.TransactionId)
            throw new ArgumentException("TransactionId不匹配");
        // 验证协议标识符是否为0（Modbus协议Id），若不是则抛出异常
        if (Adu_mbap.ProtocolId != 0)
            throw new ArgumentException("ProtocolId不是正确的Modbus协议Id");
        // 验证MBAP头中的长度字段是否与ADU去除MBAP头后的长度一致，若不一致则抛出异常
        if (Adu_mbap.Length != adu.Length - 6)
            throw new ArgumentException("Length不匹配");
        // 返回不包含MBAP头的PDU数据
        Memory<byte> pdu = new(new byte[adu.Length - 6]);
        // 将ADU中去除MBAP头后的数据复制到新的内存空间中
        adu[6..].Span.CopyTo(pdu.Span);
        return pdu;
    }

    /// <summary>
    ///  将MBAP头添加到PDU中，当mbap参数为空时，使用默认值
    /// </summary>
    /// <param name="pdu">PDU数据</param>
    /// <param name="mbap">MBAP头</param>
    /// <returns>包含MBAP头的PDU数据</returns>
    public static Memory<byte> PrependMbap(this Memory<byte> pdu, Mbap mbap = new Mbap())
    {
        // 设置MBAP头中的长度字段为PDU数据的长度
        mbap.Length = (ushort)pdu.Length;
        // 创建一个新的内存空间，用于存储包含MBAP头的完整数据，长度为PDU长度加上MBAP头的6字节
        Memory<byte> bytes = new byte[pdu.Length + 6];
        // 将MBAP头转换为字节数组并复制到新内存空间的开头
        mbap.GetBytes().CopyTo(bytes.Span);
        // 将PDU数据复制到新内存空间中MBAP头之后的位置
        pdu.Span.CopyTo(bytes.Span[6..]);
        // 返回包含MBAP头的完整数据
        return bytes;
    }
}