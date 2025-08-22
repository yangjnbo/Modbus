# Modbus TCP 通信库

## 功能特性
- 支持标准Modbus TCP协议(RFC 标准)
- 完整的服务端(ModbusTcpServer)和客户端(ModbusTcpClient)实现
- 核心功能模块：
  - Core/：寄存器数据库管理(Database.cs)
  - Protocol/：MBAP协议处理(Mbap.cs)
- 支持多平台、多架构运行(.NET 9编译通过)
- 通讯指令采用异步设计，支持高并发
- 集成Microsoft.Extensions.Logging日志系统

## 快速开始
- 服务端示例 参见ModbusServer/ExampleServer.cs
- 客户端示例 参见ModbusClient/ExsampleClient.cs

## 项目结构
Modbus/
├── Core/          # 核心数据模型
│   ├── Database.cs    # 寄存器数据库
│   ├── Funcation.cs   # 功能码处理
│   └── Register.cs    # 寄存器模型
├── Protocol/      # 协议处理
│   ├── Mbap.cs       # MBAP协议头
│   └── ReqBuilder.cs # 请求构建器
├── ModbusTcpServer.cs    # 服务端实现
├── ModbusTcpClient.cs    # 客户端实现
└── Logger.cs      # 日志系统集成
