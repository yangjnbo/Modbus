/*
   这是一个开源的Modbus协议库，用于在.NET平台上实现Modbus通信
   目前仅支持Modbus TCP Client 和 Server 的实现
   该库的代码采用MIT协议开源，您可以在遵守协议的前提下自由使用、修改和分发该库。
*/

namespace Modbus.Core;

/// <summary>
/// Modbus 功能码 
/// </summary>
public enum FuncationCode
{
    ReadCoils = 0x01,
    ReadDiscreteInputs = 0x02,
    ReadHoldingRegisters = 0x03,
    ReadInputRegisters = 0x04,
    WriteSingleCoil = 0x05,
    WriteSingleRegisters = 0x06,
    WriteMultipleCoils = 0x0F,
    WriteMultipleRegisters = 0x10,
}


/// <summary>
/// Modbus 常见异常码
/// </summary>
public enum ExceptionCode
{
    IllegalFunction = 0x01,
    IllegalDataAddress = 0x02,
    IllegalDataValue = 0x03,
    SlaveDeviceFailure = 0x04,
    MemoryParityError = 0x05,
    GatewayPathUnavailable = 0x06,
    GatewayTargetDeviceFailedToRespond = 0x07,

    Other = 0x80,  // 自定义其它异常
}
