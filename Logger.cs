using Microsoft.Extensions.Logging;

namespace Modbus
{
    /// <summary>
    /// 全局统一的日志记录器
    /// </summary>
    public static class ModbusLogger
    {
        private static ILogger _logger = CreateLogger();
        private static readonly Lock _lock = new();
        private static LogLevel _minimumLogLevel = LogLevel.Information;

        /// <summary>
        /// 创建日志记录器实例
        /// </summary>
        /// <returns>日志记录器实例</returns>
        private static ILogger CreateLogger()
        {
            try
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    builder.AddConsole();
                    builder.AddDebug();
                    builder.SetMinimumLevel(_minimumLogLevel);
                });
                return loggerFactory.CreateLogger("Modbus");
            }
            catch
            {
                // 创建一个简单的控制台日志记录器作为备选
                return new LoggerFactory().CreateLogger("Modbus");
            }
        }

        /// <summary>
        /// 设置最小日志级别
        /// </summary>
        /// <param name="level">日志级别</param>
        public static void SetMinimumLogLevel(LogLevel level)
        {
            lock (_lock)
            {
                _minimumLogLevel = level;
                try
                {
                    // 重建logger以应用新的日志级别
                    _logger = CreateLogger();
                }
                catch
                {
                    // 若重建日志记录器时发生异常，可根据实际需求扩展异常处理逻辑，此处不做处理
                }
            }
        }

        /// <summary>
        /// 记录信息级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogInformation(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, args);
            }
        }

        /// <summary>
        /// 记录信息级别的日志（带异常）
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogInformation(Exception ex, string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(ex, message, args);
            }
        }

        /// <summary>
        /// 记录警告级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogWarning(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(message, args);
            }
        }

        /// <summary>
        /// 记录警告级别的日志（带异常）
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogWarning(Exception ex, string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(ex, message, args);
            }
        }

        /// <summary>
        /// 记录错误级别的日志
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogError(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message, args);
            }
        }

        /// <summary>
        /// 记录错误级别的日志（带异常）
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="message">日志消息</param>
        /// <param name="args">格式化参数</param>
        public static void LogError(Exception ex, string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ex, message, args);
            }
        }
    }
}