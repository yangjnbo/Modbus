/*
   这是一个开源的Modbus协议库，用于在.NET平台上实现Modbus通信
   目前仅支持Modbus TCP Client 和 Server 的实现
   该库的代码采用MIT协议开源，您可以在遵守协议的前提下自由使用、修改和分发该库。
*/

using System.Buffers.Binary;

namespace Modbus;

public static class ModbusTcpClientExtensions
{
    public static bool[] ToBoolArray(this Memory<byte> data, ushort count)
    {
        bool[] boolArray = new bool[count];
        for (int i = 0; i < count; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = i % 8;
            boolArray[i] = (data.Span[byteIndex] & (0x01 << bitIndex)) != 0;
        }
        return boolArray;
    }
    public static Memory<byte> ToByteArray(this bool[] data)
    {
        int dataLength = data.Length;
        int bytelength = (dataLength + 7) / 8;
        byte[] _bytes = new byte[bytelength];
        for (int i = 0; i < data.Length; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = i % 8;
            if (data[i])
            {
                _bytes[byteIndex] |= (byte)(1 << bitIndex);
            }
        }
        return _bytes;
    }

    public static ushort[] ToUInt16Array(this Memory<byte> data, ushort count = 1)
    {
        if (data.Length < count * 2)
        {
            throw new ArgumentException("数据长度不足");
        }

        ushort[] ushortArray = new ushort[count];
        for (int i = 0; i < count; i++)
        {
            int byteIndex = i * 2;
            ushortArray[i] = BinaryPrimitives.ReadUInt16BigEndian(data.Span.Slice(byteIndex, i * 2));

        }
        return ushortArray;
    }

    public static short[] ToInt16Array(this Memory<byte> data, ushort count = 1)
    {
        if (data.Length < count * 2)
        {
            throw new ArgumentException("数据长度不足");
        }

        short[] shortArray = new short[count];
        for (int i = 0; i < count; i++)
        {
            int byteIndex = i * 2;
            shortArray[i] = BinaryPrimitives.ReadInt16BigEndian(data.Span.Slice(byteIndex, i * 2));
        }
        return shortArray;
    }
    public static uint[] ToUInt32Array(this Memory<byte> data, ushort count = 1)
    {
        if (data.Length < count * 4)
        {
            throw new ArgumentException("数据长度不足");
        }
        uint[] uintArray = new uint[count];
        for (int i = 0; i < count; i++)
        {
            int byteIndex = i * 4;
            uintArray[i] = BinaryPrimitives.ReadUInt32BigEndian(data.Span.Slice(byteIndex, i * 4));

        }
        return uintArray;
    }
    public static int[] ToInt32Array(this Memory<byte> data, ushort count = 1)

    {
        int[] intArray = new int[count];
        for (int i = 0; i < count; i++)
        {
            int byteIndex = i * 4;
            intArray[i] = BinaryPrimitives.ReadInt32BigEndian(data.Span.Slice(byteIndex, i * 4));
        }
        return intArray;
    }
    public static float[] ToFloatArray(this Memory<byte> data, ushort count = 1)
    {
        if (data.Length < count * 4)
        {
            throw new ArgumentException("数据长度不足");
        }

        float[] floatArray = new float[count];
        for (int i = 0; i < count; i++)
        {
            int byteIndex = i * 4;
            floatArray[i] = BinaryPrimitives.ReadSingleBigEndian(data.Span.Slice(byteIndex, i * 4));

        }
        return floatArray;
    }
    public static double[] ToDoubleArray(this Memory<byte> data, ushort count = 1)
    {
        if (data.Length < count * 8)
        {
            throw new ArgumentException("数据长度不足");
        }

        double[] doubleArray = new double[count];
        for (int i = 0; i < count; i++)
        {
            int byteIndex = i * 8;
            doubleArray[i] = BinaryPrimitives.ReadDoubleBigEndian(data.Span.Slice(byteIndex, i * 8));

        }
        return doubleArray;
    }
}
