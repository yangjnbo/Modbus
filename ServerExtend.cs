/*
   这是一个开源的Modbus协议库，用于在.NET平台上实现Modbus通信
   目前仅支持Modbus TCP Client 和 Server 的实现
   该库的代码采用MIT协议开源，您可以在遵守协议的前提下自由使用、修改和分发该库。
*/

using Modbus.Core;

namespace Modbus;
public static class ServerExtend
{
    public static void AddDatabase(this List<Database> Databases, Database db)
    {
        try
        {
            if (!Databases.Contains(db))
            {
                Databases.Add(db);
            }
            else
            {
                int index = Databases.FindIndex(d => d.UnitId == db.UnitId);
                if (index != -1)
                {
                    Databases[index] = db;
                }
            }
        }
        catch (Exception ex)
        {
            ModbusLogger.LogError(ex, "添加或更新数据库时发生错误");
        }
    }

    public static void AddDatabase(this List<Database> Databases, byte unitid)
    {
        try
        {
            var db = new Database(unitid);

            if (!Databases.Contains(db))
            {
                Databases.Add(db);
            }
            else
            {
                int index = Databases.FindIndex(d => d.UnitId == db.UnitId);
                if (index != -1)
                {
                    Databases[index] = db;
                }
            }
        }
        catch (Exception ex)
        {
            ModbusLogger.LogError(ex, "添加或更新数据库时发生错误");
        }
    }
    public static void AddDatabase(this List<Database> Databases, byte unitid, int size)
    {
        try
        {
            var db = new Database(unitid, size);

            if (!Databases.Contains(db))
            {
                Databases.Add(db);
            }
            else
            {
                int index = Databases.FindIndex(d => d.UnitId == db.UnitId);
                if (index != -1)
                {
                    Databases[index] = db;
                }
            }
        }
        catch (Exception ex)
        {
            ModbusLogger.LogError(ex, "添加或更新数据库时发生错误");
        }
    }

    public static bool RemoveDatabase(this List<Database> Databases, Database database)
    {
        try
        {
            return Databases.Remove(database);
        }
        catch (Exception ex)
        {
            ModbusLogger.LogError(ex, "移除数据库时发生错误");
            return false;
        }
    }
    public static bool RemoveDatabase(this List<Database> Databases, byte unitid)
    {
        try
        {
            foreach (Database db in Databases)
            {
                if (db.UnitId == unitid)
                {
                    Databases.Remove(db);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            ModbusLogger.LogError(ex, "移除数据库时发生错误");
            return false;
        }
    }

    public static Database? GetByUnitid(this List<Database> Databases, byte unitid)
    {
        try
        {
            foreach (Database db in Databases)
            {
                if (db.UnitId == unitid)
                {
                    return db;
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            ModbusLogger.LogError(ex, "根据UnitId获取数据库时发生错误");
            return null;
        }
    }
}