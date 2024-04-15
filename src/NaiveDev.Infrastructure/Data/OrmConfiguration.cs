namespace NaiveDev.Infrastructure.Data
{
    /// <summary>
    /// ORM配置
    /// </summary>
    public class OrmConfiguration
    {
        /// <summary>
        /// ORM配置
        /// </summary>
        public OrmConfiguration()
        {
            Identity = string.Empty;
            Dbtype = -1;
            Enabled = false;
            Server = string.Empty;
            Port = string.Empty;
            Database = string.Empty;
            User = string.Empty;
            Password = string.Empty;
        }

        /// <summary>
        /// 连接标识
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// 数据库类型 0=MySql; 1=SqlServer; 2=Sqlite; 3=Oracle; 4=PostgreSQL;
        /// </summary>
        public int Dbtype { get; set; }

        /// <summary>
        /// 启用此连接
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 主机名
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 完整的数据库连接
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">不符合数据库的类型</exception>
        public string GetConnectionString()
        {
            return Dbtype switch
            {
                0 => $"Server={Server};Port={Port};Database={Database};Uid={User};Pwd={Password};",
                1 => $"Server={Server},{Port};Database={Database};User ID={User};Password={Password};encrypt=false;",
                2 => $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Database.Replace("//", "\\")}.db;")};",
                3 => $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={Server})(PORT={Port}))(CONNECT_DATA=(SERVICE_NAME={Database})));User Id={User};Password={Password};",
                4 => $"Host={Server};Port={Port};Database={Database};Username={User};Password={Password};",
                _ => throw new Exception("Invalid dbtype value in config file."),
            };
        }
    }
}
