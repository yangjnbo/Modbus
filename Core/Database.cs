/*
   这是一个开源的Modbus协议库，用于在.NET平台上实现Modbus通信
   目前仅支持Modbus TCP Client 和 Server 的实现
   该库的代码采用MIT协议开源，您可以在遵守协议的前提下自由使用、修改和分发该库。
*/

namespace Modbus.Core;

public class Database
{
    public byte UnitId { get; init; }
    public Coil[] Coils { get; init; }
    public discreteInput[] DiscreteInputs { get; init; }
    public holdingRegister[] HoldingRegisters { get; init; }
    public inputRegister[] InputRegisters { get; init; }
    public Database(byte unitId = 0x01, int size = 9999)
    {
        UnitId = unitId;
        Coils = new Coil[size];
        DiscreteInputs = new discreteInput[size];
        HoldingRegisters = new holdingRegister[size];
        InputRegisters = new inputRegister[size];
        // 初始化所有元素
        for (int i = 0; i < size; i++)
        {
            Coils[i] = new Coil(){ Value = false };
            DiscreteInputs[i] = new discreteInput(){ Value = false };
            HoldingRegisters[i] = new holdingRegister(){ HighByte = 0, LowByte = 0 };
            InputRegisters[i] = new inputRegister(){ HighByte = 0, LowByte = 0 };
        }
    }

    public Memory<byte> ReadCoils(int index, int count)
    {
        if (index < 0 || index >= Coils.Length)
        {
            var exceptionCode = ExceptionCode.IllegalDataAddress;
            return new byte[] { UnitId, 0x81, (byte)exceptionCode };
        }
        if (count == 0 || count > (Coils.Length - index))
        {
            var exceptionCode = ExceptionCode.IllegalDataValue;
            return new byte[] { UnitId, 0x81, (byte)exceptionCode };
        }

        Memory<byte> data = new byte[(count + 7) / 8];

        // 遍历需要读取的线圈数量
        for (int i = 0; i < count; i++)
        {
            // 计算当前线圈对应的字节索引，每8个线圈存储在一个字节中
            int byteIndex = i / 8;
            // 计算当前线圈在字节中的位索引
            int bitIndex = i % 8;
            // 如果当前线圈的值为 true，则将对应位设置为 1，否则为 0
            // 并将结果与原字节进行按位或操作，更新该字节的值
            data.Span[byteIndex] = (byte)(data.Span[byteIndex] | (Coils[index + i].Value ? 1 << bitIndex : 0));
        }

        // 构建响应
        /*
        根据 Modbus 规范，读取线圈（功能码 0x01）的响应报文格式如下：
        1. 第 1 字节：从站地址（UnitId）
        2. 第 2 字节：功能码（0x01）
        3. 第 3 字节：字节计数（字节数 = (线圈数量 + 7) / 8）
        4. 第 4-... 字节：线圈状态（每个线圈占用 1 位，按字节存储）
        */
        Memory<byte> response = new byte[3 + data.Length];
        response.Span[0] = UnitId;
        response.Span[1] = 0x01;
        response.Span[2] = (byte)(data.Length);
        data.Span.CopyTo(response.Span[3..]);
        return response;

    }

    public Memory<byte> ReadDiscreteInputs(int index, int count)
    {
        if (index < 0 || index >= DiscreteInputs.Length)
        {
            var exceptionCode = ExceptionCode.IllegalDataAddress;
            return new byte[] { UnitId, 0x82, (byte)exceptionCode };
        }
        if (count == 0 || count > (DiscreteInputs.Length - index))
        {
            var exceptionCode = ExceptionCode.IllegalDataValue;
            return new byte[] { UnitId, 0x82, (byte)exceptionCode };
        }

        int byteCount = (count + 7) / 8;
        Memory<byte> data = new byte[byteCount];
        // 遍历需要读取的线圈数量
        for (int i = 0; i < count; i++)
        {
            // 计算当前线圈对应的字节索引，每8个线圈存储在一个字节中
            int byteIndex = i / 8;
            // 计算当前线圈在字节中的位索引
            int bitIndex = i % 8;
            // 如果当前线圈的值为 true，则将对应位设置为 1，否则为 0
            // 并将结果与原字节进行按位或操作，更新该字节的值
            data.Span[byteIndex] = (byte)(data.Span[byteIndex] | (DiscreteInputs[index + i].Value ? 1 << bitIndex : 0));
        }

        // 构建响应
        /*
        根据 Modbus 规范，读取离散输入（功能码 0x02）的响应报文格式如下：
        1. 第 1 字节：从站地址（UnitId）
        2. 第 2 字节：功能码（0x02）
        3. 第 3 字节：字节计数（字节数 = (线圈数量 + 7) / 8）
        4. 第 4-... 字节：线圈状态（每个线圈占用 1 位，按字节存储）
        */
        Memory<byte> response = new byte[3 + data.Length];
        response.Span[0] = UnitId;
        response.Span[1] = 0x02;
        response.Span[2] = (byte)(data.Length);
        data.Span.CopyTo(response.Span[3..]);
        return response;

    }

    public Memory<byte> ReadHoldingRegisters(int index, int count)
    {
        if (index < 0 || index >= HoldingRegisters.Length)
        {
            var exceptionCode = ExceptionCode.IllegalDataAddress;
            return new byte[] { UnitId, 0x83, (byte)exceptionCode };
        }
        if (count == 0 || count > (HoldingRegisters.Length - index))
        {
            var exceptionCode = ExceptionCode.IllegalDataValue;
            return new byte[] { UnitId, 0x83, (byte)exceptionCode };
        }

        Memory<byte> data = new byte[count * 2];
        for (int i = 0; i < count; i++)
        {
            data.Span[i * 2] = HoldingRegisters[index + i].HighByte;
            data.Span[i * 2 + 1] = HoldingRegisters[index + i].LowByte;
        }

        // 构建响应
        /*
        根据 Modbus 规范，读取保持寄存器（功能码 0x03）的响应报文格式如下：
        1. 第 1 字节：从站地址（UnitId）
        2. 第 2 字节：功能码（0x03）
        3. 第 3 字节：字节计数（字节数 = 寄存器数量 * 2）
        4. 第 4-... 字节：寄存器数据（每个寄存器占用 2 字节，按字节存储）
        */
        Memory<byte> response = new byte[3 + data.Length];
        response.Span[0] = UnitId;
        response.Span[1] = 0x03;
        response.Span[2] = (byte)(data.Length);
        data.Span.CopyTo(response.Span[3..]);
        return response;
    }

    public Memory<byte> ReadInputRegisters(int index, int count)
    {

        if (index < 0 || index >= InputRegisters.Length)
        {
            var exceptionCode = ExceptionCode.IllegalDataAddress;
            return new byte[] { UnitId, 0x84, (byte)exceptionCode };
        }
        if (count == 0 || count > (InputRegisters.Length - index))
        {
            var exceptionCode = ExceptionCode.IllegalDataValue;
            return new byte[] { UnitId, 0x84, (byte)exceptionCode };
        }

        Memory<byte> data = new byte[count * 2];
        for (int i = 0; i < count; i++)
        {
            data.Span[i * 2] = InputRegisters[index + i].HighByte;
            data.Span[i * 2 + 1] = InputRegisters[index + i].LowByte;
        }

        // 构建响应
        /*
        根据 Modbus 规范，读取输入寄存器（功能码 0x04）的响应报文格式如下：
        1. 第 1 字节：从站地址（UnitId）
        2. 第 2 字节：功能码（0x04）
        3. 第 3 字节：字节计数（字节数 = 寄存器数量 * 2）
        4. 第 4-... 字节：寄存器数据（每个寄存器占用 2 字节，按字节存储）
        */
        Memory<byte> response = new byte[3 + data.Length];
        response.Span[0] = UnitId;
        response.Span[1] = 0x04;
        response.Span[2] = (byte)(data.Length);
        data.Span.CopyTo(response.Span[3..]);
        return response;

    }

    public Memory<byte> WriteSingalCoil(ushort index, Memory<byte> data)
    {
        if (index >= Coils.Length || index < 0)
        {
            var exceptionCode = ExceptionCode.IllegalDataAddress;
            return new byte[] { UnitId, 0x85, (byte)exceptionCode };
        }
        if (data.Length != 2 || data.Span[1] != 0x00 || (data.Span[0] != 0x00 && data.Span[0] != 0xFF))
        {
            var exceptionCode = ExceptionCode.IllegalDataValue;
            return new byte[] { UnitId, 0x85, (byte)exceptionCode };
        }

        Coils[index].Value = data.Span[0] == 0xFF;

        // 构建响应
        byte[] response =
        [
            UnitId,
            0x05,
            (byte)(index >> 8),
            (byte)(index & 0xFF),
            data.Span[0],
            data.Span[1],
        ];
        return response;
    }

    public Memory<byte> WriteSingalRegisters(ushort index, Memory<byte> data)
    {
        if (index >= HoldingRegisters.Length || index < 0)
        {
            var exceptionCode = ExceptionCode.IllegalDataAddress;
            return new byte[] { UnitId, 0x86, (byte)exceptionCode };
        }
        if (data.Length != 2 || data.Span[0] < 0 || data.Span[1] < 0)
        {
            var exceptionCode = ExceptionCode.IllegalDataValue;
            return new byte[] { UnitId, 0x86, (byte)exceptionCode };
        }

        HoldingRegisters[index].HighByte = data.Span[0];
        HoldingRegisters[index].LowByte = data.Span[1];

        // 构建响应
        byte[] response =
        [
            UnitId,
            0x06,
            (byte)(index >> 8),
            (byte)(index & 0xFF),
            data.Span[0],
            data.Span[1],
        ];
        return response;
    }

    public Memory<byte> WriteMultipleCoils(ushort index, ushort count, Memory<byte> data)
    {
        if (index >= Coils.Length)
        {
            var exceptionCode = ExceptionCode.IllegalDataAddress;
            return new byte[] { UnitId, 0x8F, (byte)exceptionCode };
        }
        if (count == 0 || count > (Coils.Length - index))
        {
            var exceptionCode = ExceptionCode.IllegalDataValue;
            return new byte[] { UnitId, 0x8F, (byte)exceptionCode };
        }

        for (int i = 0; i < count; i++)
        {
            int byteIndex = i / 8;
            int bitIndex = i % 8;
            Coils[index + i].Value = (data.Span[byteIndex] & (1 << bitIndex)) != 0;
        }

        // 构建响应
        /*
        根据 Modbus 规范，写入多个线圈（功能码 0x0F）的响应报文格式如下：
        1. 第 1 字节：从站地址（UnitId）
        2. 第 2 字节：功能码（0x0F）
        3. 第 3-4 字节：写入的线圈地址
        4. 第 5-6 字节：写入的线圈数量
        */
        byte[] response =
        [
            UnitId,
            0x0F,
            (byte)(index >> 8),
            (byte)(index & 0xFF),
            (byte)(count >> 8),
            (byte)(count & 0xFF),
        ];
        return response;
    }

    public Memory<byte> WriteMultipleRegisters(int index, ushort count, Memory<byte> data)
    {
        if (index < 0 || index >= HoldingRegisters.Length || (index + count) > HoldingRegisters.Length)
        {
            var exceptionCode = ExceptionCode.IllegalDataAddress;
            return new byte[] { UnitId, 0x90, (byte)exceptionCode };
        }
        if (data.Length == 0 || count == 0 || count > (HoldingRegisters.Length - index) || data.Length != count * 2 ||
            data.Span.ToArray().Any(b => b < 0))
        {
            var exceptionCode = ExceptionCode.IllegalDataValue;
            return new byte[] { UnitId, 0x90, (byte)exceptionCode };
        }

        for (int i = 0; i < count; i++)
        {
            HoldingRegisters[index + i].HighByte = data.Span[i * 2];
            HoldingRegisters[index + i].LowByte = data.Span[i * 2 + 1];
        }

        // 构建响应
        byte[] response =
        [
            UnitId,
            0x10,
            (byte)(index >> 8),
            (byte)(index & 0xFF),
            (byte)(count >> 8),
            (byte)(count & 0xFF),
        ];
        return response;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Database database)
            return database.UnitId == UnitId;

        return false;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString()
    {
        return $"Modbus Server Database UintId = {UnitId}";
    }

}

