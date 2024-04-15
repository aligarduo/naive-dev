using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NaiveDev.Infrastructure.Data;
using NaiveDev.Infrastructure.Entities;
using SqlSugar;

namespace NaiveDev.Infrastructure.Extensions
{
    /// <summary>
    /// ORM相关的扩展方法类，提供ORM相关的服务注册方法
    /// </summary>
    public static class OrmExtensions
    {
        /// <summary>
        /// 将ORM相关的服务添加到IServiceCollection中
        /// </summary>
        /// <param name="services">IServiceCollection实例，用于添加服务</param>
        public static void AddOrm(this IServiceCollection services)
        {
            // 添加作用域内的ISqlSugarClient服务
            services.AddScoped<ISqlSugarClient>(provider =>
            {
                // 获取ORM配置信息
                List<OrmConfiguration> Configuration = provider.GetRequiredService<IOptions<List<OrmConfiguration>>>().Value;

                // 根据配置信息创建ConnectionConfig列表
                List<ConnectionConfig> connectionConfigs = Configuration.Select(db => new ConnectionConfig()
                {
                    // 初始化键类型设置为属性
                    InitKeyType = InitKeyType.Attribute,
                    // 配置ID
                    ConfigId = db.Identity,
                    // 连接字符串
                    ConnectionString = db.GetConnectionString(),
                    // 数据库类型
                    DbType = (DbType)db.Dbtype,
                    // 是否自动关闭连接
                    IsAutoCloseConnection = true,
                    // 更多设置
                    MoreSettings = new ConnMoreSettings()
                    {
                        // 是否自动清除数据缓存
                        IsAutoRemoveDataCache = true
                    },
                }).ToList();

                // 使用ConnectionConfig列表创建SqlSugarClient实例并返回
                return new SqlSugarClient(connectionConfigs);
            });

            // 添加作用域内的SqlSugarSeed服务。  
            services.AddScoped(provider =>
            {
                // 创建用于存放ISqlSugarClient的列表
                List<ISqlSugarClient> sqlSugarClients = [];

                // 获取ORM配置信息
                List<OrmConfiguration> Configuration = provider.GetRequiredService<IOptions<List<OrmConfiguration>>>().Value;

                // 遍历配置信息，为每个数据库配置创建CustomSqlSugarClient实例并添加到列表中
                foreach (var configItem in Configuration)
                {
                    sqlSugarClients.Add(new CustomSqlSugarClient(new ConnectionConfig
                    {
                        // 配置ID
                        ConfigId = configItem.Identity,
                        // 连接字符串
                        ConnectionString = configItem.GetConnectionString(),
                        // 数据库类型
                        DbType = (DbType)configItem.Dbtype,
                        // 是否自动关闭连接
                        IsAutoCloseConnection = true,
                        // 初始化键类型设置为属性
                        InitKeyType = InitKeyType.Attribute
                    }));
                }

                // 使用ISqlSugarClient列表创建SqlSugarSeed实例并返回。  
                return new SqlSugarSeed(sqlSugarClients);
            });
        }
    }

    /// <summary>
    /// 自定义的SqlSugarClient类，继承自SqlSugarClient，用于扩展SqlSugar的功能
    /// </summary>
    public class CustomSqlSugarClient(ConnectionConfig config) : SqlSugarClient(config)
    {
        /// <summary>
        /// 自定义属性，获取或设置当前实例的配置ID
        /// </summary>
        public object ConfigId { get; set; } = config.ConfigId;
    }

    /// <summary>
    /// SqlSugar种子
    /// </summary>
    public class SqlSugarSeed(List<ISqlSugarClient> SqlSugarClients)
    {
        private readonly List<ISqlSugarClient> _SqlSugarClients = SqlSugarClients;

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public SqlSugarSeed InitDatabase()
        {
            Console.WriteLine("Starting database initialization...");

            foreach (var Client in _SqlSugarClients)
            {
                Console.WriteLine($"=> {Client.Ado.Context.CurrentConnectionConfig.ConfigId}");
                Client.DbMaintenance.CreateDatabase();
            }

            Console.WriteLine("Database initialization completed.");

            return this;
        }

        /// <summary>  
        /// 初始化数据表  
        /// </summary>  
        public SqlSugarSeed InitTable()
        {
            // 获取当前应用程序域的基目录路径  
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
            // 获取基目录下的所有dll文件，并排除Microsoft的dll  
            var files = Directory.GetFiles(directoryPath, "*.dll")
                .Where(file => !file.Contains("Microsoft"))
                .Where(file => !file.Contains("System"))
                .Where(file => !file.Contains("Azure"))
                .Where(file => !file.Contains("Autofac"))
                .Where(file => !file.Contains("Caching"))
                .Where(file => !file.Contains("CSRedisCore"))
                .Where(file => !file.Contains("IGeekFan"))
                .Where(file => !file.Contains("MediatR"))
                .Where(file => !file.Contains("NLog"))
                .Where(file => !file.Contains("SqlSugarCore"))
                .Where(file => !file.Contains("Swashbuckle"))
                .ToList();

            // 遍历每个dll文件  
            foreach (var file in files)
            {
                try
                {
                    // 加载dll文件为程序集对象  
                    var assembly = System.Reflection.Assembly.LoadFrom(file);
                    // 获取程序集中所有继承自EntityBase的子类类型  
                    var types = assembly.GetTypes()
                        .Where(type => type.IsSubclassOf(typeof(EntityBase)))
                        .ToList(); // 将查询结果转换为列表，以便后续操作  

                    if (types.Count != 0)
                    {
                        Console.WriteLine("Starting data table initialization...");

                        foreach (var table in types)
                        {
                            // 获取第一个TenantAttribute（如果存在）  
                            if (table.GetTypeInfo().GetCustomAttributes(typeof(TenantAttribute), true).FirstOrDefault() is not TenantAttribute tenantAttribute) continue;

                            // 根据TenantAttribute的configId找到对应的SqlSugarClient  
                            var client = _SqlSugarClients.OfType<CustomSqlSugarClient>().FirstOrDefault(c => c.ConfigId.ToString() == tenantAttribute.configId.ToString());
                            if (client == null) continue;

                            Console.WriteLine($"=> {table.Name}");
                            // 初始化数据表  
                            client.CodeFirst.InitTables(table);
                        }

                        Console.WriteLine("Data table initialization completed.");
                    }
                }
                catch (Exception ex)
                {
                    // 记录异常信息以便调试  
                    Console.WriteLine($"Failed to load and process DLL {Path.GetFileName(file)}: {ex.Message}");
                }
            }

            return this;
        }
    }
}