/*
   这是一个开源的Modbus协议库，用于在.NET平台上实现Modbus通信
   目前仅支持Modbus TCP Client 和 Server 的实现
   该库的代码采用MIT协议开源，您可以在遵守协议的前提下自由使用、修改和分发该库。
*/

using System.Buffers.Binary;
using Modbus.Core;

namespace Modbus.Protocol;

/// <summary>
/// 这是一个Modbus请求Pdu构建类
/// 提供了构建Modbus请求Pdu的方法
/// 所有的方法都是静态的，不需要实例化即可使用
/// 为了避免参数校验带来的性能损失，该类的方法都没有进行参数校验
/// 请在调用该类的方法之前，确保参数的合法性
/// 如果参数不合法，可能会抛出ArgumentException异常
/// 或者会导致Modbus Server 返回异常响应
/// </summary>
public static class Req_Builder
{
    /// <summary>
    /// 构建读取线圈状态请求Pdu
    /// </summary>
    /// <param name="unitid">从站地址</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="count">寄存器数量</param>
    /// <returns>Modbus请求Pdu</returns>
    /// <exception cref="ArgumentException">构建请求Pdu失败</exception>
    public static Memory<byte> Build_Req_ReadCoils(byte unitid, ushort startAddress, ushort count)
    {
        try
        {
            byte[] data = new byte[6];
            data[0] = unitid;
            data[1] = (byte)FuncationCode.ReadCoils;
            data[2] = (byte)(startAddress >> 8);
            data[3] = (byte)(startAddress & 0xFF);
            data[4] = (byte)(count >> 8);
            data[5] = (byte)(count & 0xFF);

            return data.AsMemory();
        }
        catch
        {
            throw new ArgumentException("构建请求Pdu失败");
        }
    }

    /// <summary>
    /// 构建读取离散输入状态请求Pdu
    /// </summary>
    /// <param name="unitid">从站地址</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="count">寄存器数量</param>
    /// <returns>Modbus请求Pdu</returns>
    /// <exception cref="ArgumentException">构建请求Pdu失败</exception>
    public static Memory<byte> Build_Req_ReadDiscreteInputs(byte unitid, ushort startAddress, ushort count)
    {
        try
        {
            byte[] data = new byte[6];
            data[0] = unitid;
            data[1] = (byte)FuncationCode.ReadDiscreteInputs;
            data[2] = (byte)(startAddress >> 8);
            data[3] = (byte)(startAddress & 0xFF);
            data[4] = (byte)(count >> 8);
            data[5] = (byte)(count & 0xFF);

            return data.AsMemory();
        }
        catch
        {
            throw new ArgumentException("构建请求Pdu失败");
        }
    }

    /// <summary>
    /// 构建读取保持寄存器状态请求Pdu
    /// </summary>
    /// <param name="unitid">从站地址</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="count">寄存器数量</param>
    /// <returns>Modbus请求Pdu</returns>
    /// <exception cref="ArgumentException">构建请求Pdu失败</exception>
    public static Memory<byte> Build_Req_ReadHoldingRegisters(byte unitid, ushort startAddress, ushort count)
    {
        try
        {
            byte[] data = new byte[6];
            data[0] = unitid;
            data[1] = (byte)FuncationCode.ReadHoldingRegisters;
            data[2] = (byte)(startAddress >> 8);
            data[3] = (byte)(startAddress & 0xFF);
            data[4] = (byte)(count >> 8);
            data[5] = (byte)(count & 0xFF);

            return data.AsMemory();
        }
        catch
        {
            throw new ArgumentException("构建请求Pdu失败");
        }
    }

    /// <summary>
    /// 构建读取输入寄存器状态请求Pdu
    /// </summary>
    /// <param name="unitid">从站地址</param>
    /// <param name="startAddress">起始地址</param>
    /// <param name="count">寄存器数量</param>
    /// <returns>Modbus请求Pdu</returns>
    /// <exception cref="ArgumentException">构建请求Pdu失败</exception>
    public static Memory<byte> Build_Req_ReadInputRegisters(byte unitid, ushort startAddress, ushort count)
    {
        try
        {
            byte[] data = new byte[6];
            data[0] = unitid;
            data[1] = (byte)FuncationCode.ReadInputRegisters;
            data[2] = (byte)(startAddress >> 8);
            data[3] = (byte)(startAddress & 0xFF);
            data[4] = (byte)(count >> 8);
            data[5] = (byte)(count & 0xFF);

            return data.AsMemory();
        }
        catch
        {
            throw new ArgumentException("构建请求Pdu失败");
        }
    }

    /// <summary>
    /// 构建写入单个线圈状态请求Pdu
    /// </summary>
    /// <param name="unitid">从站地址</param>
    /// <param name="address">线圈地址</param>
    /// <param name="value">线圈值，只能是开关量</param>
    /// <returns>Modbus请求Pdu</returns>
    /// <exception cref="ArgumentException">构建请求Pdu失败</exception>
    public static Memory<byte> Build_Req_WriteSingleCoil(byte unitid, ushort address, bool value)
    {
        try
        {
            byte[] data = new byte[6];
            data[0] = unitid;
            data[1] = (byte)FuncationCode.WriteSingleCoil;
            data[2] = (byte)(address >> 8);
            data[3] = (byte)(address & 0xFF);
            data[4] = (byte)(value ? 0xFF : 0x00);
            data[5] = 0x00;

            return data.AsMemory();
        }
        catch
        {
            throw new ArgumentException("构建请求Pdu失败");
        }
    }

    /// <summary>
    /// 构建写入单个保持寄存器状态请求Pdu
    /// </summary>
    /// <param name="unitid">从站地址</param>
    /// <param name="address">寄存器地址</param>
    /// <param name="value">寄存器值，必须2个字节</param>
    /// <returns>Modbus请求Pdu</returns>
    /// <exception cref="ArgumentException">构建请求Pdu失败</exception>
    public static Memory<byte> Build_Req_WriteSingleRegisters(byte unitid, ushort address, byte[] value)
    {
        try
        {
            byte[] data = new byte[6];
            data[0] = unitid;
            data[1] = (byte)FuncationCode.WriteSingleRegisters;
            data[2] = (byte)(address >> 8);
            data[3] = (byte)(address & 0xFF);
            data[4] = value[0];
            data[5] = value[1];

            return data.AsMemory();
        }
        catch
        {
            throw new ArgumentException("构建请求Pdu失败");
        }
    }

    /// <summary>
    /// 构建写入多个线圈状态请求Pdu
    /// </summary>
    /// <param name="unitid">从站地址</param>
    /// <param name="startAddress">线圈地址</param>
    /// <param name="values">线圈值，发送报文的线圈数量根据数组的长度确定</param>
    /// <returns>Modbus请求Pdu</returns>
    /// <exception cref="ArgumentException">构建请求Pdu失败</exception>
    public static Memory<byte> Build_Req_WriteMultipleCoils(byte unitid, ushort startAddress, bool[] values)
    {
        try
        {
            byte[] data = new byte[7 + (values.Length + 7) / 8];
            data[0] = unitid;
            data[1] = (byte)FuncationCode.WriteMultipleCoils;
            data[2] = (byte)(startAddress >> 8);
            data[3] = (byte)(startAddress & 0xFF);
            data[4] = (byte)(values.Length >> 8);
            data[5] = (byte)(values.Length & 0xFF);
            data[6] = (byte)((values.Length + 7) / 8);
            Memory<byte> bytes = values.ToByteArray();
            bytes.CopyTo(data.AsMemory(7));
            return data.AsMemory();
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("构建请求Pdu失败", ex);
        }
    }

    /// <summary>
    /// 构建写入多个保持寄存器状态请求Pdu
    /// </summary>
    /// <param name="unitid">从站地址</param>
    /// <param name="startAddress">寄存器地址</param>
    /// <param name="values">寄存器值，发送报文的寄存器数量根据数组的长度确定，每个寄存器2个字节，数组的长度必须是2的倍数</param>
    /// <returns>Modbus请求Pdu</returns>
    /// <exception cref="ArgumentException">构建请求Pdu失败</exception>
    public static Memory<byte> Build_Req_WriteMultipleRegisters(byte unitid, ushort startAddress, byte[] values)
    {
        try
        {
            byte[] data = new byte[7 + values.Length];
            data[0] = unitid;
            data[1] = (byte)FuncationCode.WriteMultipleRegisters;
            data[2] = (byte)(startAddress >> 8);
            data[3] = (byte)(startAddress & 0xFF);
            data[4] = (byte)((values.Length / 2) >> 8);
            data[5] = (byte)((values.Length / 2) & 0xFF);
            data[6] = (byte)(values.Length);
            Array.Copy(values, 0, data, 7, values.Length);


            return data.AsMemory();
        }
        catch
        {
            throw new ArgumentException("构建请求Pdu失败");
        }
    }

    /*
        以下提供了一些常用的转换方法，可以将常用的数据类型转换为Modbus协议要求的字节数组
        以下方法采用大端字节序，即高字节在低地址，低字节在高地址
        支持常见的 16位、32位、单精度浮点数转换为2字节、4字节数组
        如需其他数据类型的转换，请自行实现
    */

    /// <summary>
    /// 把ushort转换为2字节数组
    /// </summary>
    /// <param name="Value"></param>
    /// <returns>2字节数组</returns>
    public static byte[] ToBytes(this ushort Value)
    {
        byte[] Bytes = new byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(Bytes, Value);
        return Bytes;
    }

    /// <summary>
    /// 把short转换为2字节数组
    /// </summary>
    /// <param name="Value"></param>
    /// <returns>2字节数组</returns>
    public static byte[] ToBytes(this short Value)
    {
        byte[] Bytes = new byte[2];
        BinaryPrimitives.WriteInt16BigEndian(Bytes, Value);
        return Bytes;
    }

    /// <summary>
    /// 把int转换为4字节数组
    /// </summary>
    /// <param name="Value"></param>
    /// <returns>4字节数组</returns>
    /// 注意：将Int32写入到Modbusserver时，必须使用WriteMultipleRegisters方法
    public static byte[] ToBytes(this int Value)
    {
        byte[] Bytes = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(Bytes, Value);
        return Bytes;
    }

    /// <summary>
    /// 把uint转换为4字节数组
    /// </summary>
    /// <param name="Value"></param>
    /// <returns>4字节数组</returns>
    /// 注意：将UInt32写入到Modbusserver时，必须使用WriteMultipleRegisters方法
    public static byte[] ToBytes(this uint Value)
    {
        byte[] Bytes = new byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(Bytes, Value);
        return Bytes;
    }
    
    /// <summary>
    /// 把float转换为4字节数组
    /// </summary>
    /// <param name="Value"></param>
    /// <returns>4字节数组</returns>
    /// 注意：将float写入到Modbusserver时，必须使用WriteMultipleRegisters方法
    public static byte[] ToBytes(this float Value)
    {
        byte[] Bytes = new byte[4];
        BinaryPrimitives.WriteSingleBigEndian(Bytes, Value);
        return Bytes;
    }
}
