/*
   这是一个开源的Modbus协议库，用于在.NET平台上实现Modbus通信
   目前仅支持Modbus TCP Client 和 Server 的实现
   该库的代码采用MIT协议开源，您可以在遵守协议的前提下自由使用、修改和分发该库。
*/

namespace Modbus.Core;

public abstract class Register
{
    public RegisterType Type { get; init; }
    public Register(RegisterType type)
    {
        Type = type;
    }
}
public class Coil : Register
{
    private bool _value;
    public bool Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
            }
        }
    }
    public Coil() : base(RegisterType.coils) { }
}
public class discreteInput : Register
{
    private bool _value;
    public bool Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
            }
        }
    }
    public discreteInput() : base(RegisterType.discreteInputs) { }
}
public class holdingRegister : Register
{
    public byte HighByte { get; set; }
    public byte LowByte { get; set; }
    public byte[] Bytes
    {
        get
        {
            return new byte[] { HighByte, LowByte };
        }
        set
        {
            if (value.Length >= 2)
            {
                if (HighByte != value[0] || LowByte != value[1])
                {
                    HighByte = value[0];
                    LowByte = value[1];
                }
            }
        }
    }
    public holdingRegister() : base(RegisterType.holdingRegisters) { }
}
public class inputRegister : Register
{
    public byte HighByte { get; set; }
    public byte LowByte { get; set; }
    public byte[] Bytes
    {
        get
        {
            return new byte[] { HighByte, LowByte };
        }
        set
        {
            if (value.Length >= 2)
            {
                if (HighByte != value[0] || LowByte != value[1])
                {
                    HighByte = value[0];
                    LowByte = value[1];

                }
            }
        }
    }
    public inputRegister() : base(RegisterType.inputRegisters) { }
}
public enum RegisterType
{
    coils,
    discreteInputs,
    holdingRegisters,
    inputRegisters
}



