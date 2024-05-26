using System.Reflection;
using NaiveDev.Infrastructure.Data;
using NaiveDev.Infrastructure.Extensions;
using NaiveDev.Infrastructure.Jwt;
using NaiveDev.Infrastructure.Middleware;

namespace NaiveDev.WebHost
{
    /// <summary>
    /// 启动项
    /// </summary>
    /// <param name="configuration">应用程序配置属性</param>
    /// <param name="env">提供有关应用程序运行所在的 Web 托管环境的信息</param>
    public class Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        /// <summary>
        /// 应用程序配置属性
        /// </summary>
        public IConfiguration Configuration { get; } = configuration;

        /// <summary>
        /// 提供有关应用程序运行所在的 Web 托管环境的信息
        /// </summary>
        public IWebHostEnvironment Env { get; } = env;

        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="services">服务集</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // 从配置文件读取ORM配置并配置到服务中
            services.Configure<List<PersistenceConfiguration>>(Configuration.GetSection("Persistence"));

            // 从配置文件读取缓存配置并配置到服务中
            services.Configure<CacheConfiguration>(Configuration.GetSection("Cache"));

            // 添加仓储服务
            services.AddRepository();

            // 添加ORM服务
            services.AddOrm();

            // 添加内存缓存服务
            services.AddMemoryCache();

            // 添加MVC控制器支持
            services.AddController();

            // 添加Swagger API文档支持
            services.AddSwagger();

            // 配置路由选项，将URL转换为小写
            services.AddRouting(options => options.LowercaseUrls = true);

            // 添加跨域支持，并定义一个允许所有源的策略
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // 添加JWT认证服务
            services.AddJwt();

            // 添加授权服务
            services.AddAuthorization();

            // 添加HttpClient支持
            services.AddHttpClient();

            // 注册MediatR及其依赖注入支持
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// 中间件配置
        /// </summary>
        /// <param name="app">用于配置 http 管道和路由的 Web 应用程序</param>
        /// <param name="env">提供有关应用程序运行所在的 Web 托管环境的信息</param>
        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            // 使用前面定义的跨域策略
            app.UseCors("AllowAll");

            // 启用静态文件服务
            app.UseStaticFiles();

            // 使用状态码页面中间件
            app.UseStatusCodePages();

            // 启用路由匹配
            app.UseRouting();

            // 如果是开发环境，启用Swagger和开发者异常页面
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggers();
                app.UseDeveloperExceptionPage();
            }

            // 使用IP黑名单中间件
            app.UseMiddleware<IPBlackMiddleware>();

            // 使用IP白名单中间件
            app.UseMiddleware<IPWhiteMiddleware>();

            // 使用限流中间件
            app.UseMiddleware<RateLimitMiddleware>();

            // 启用事务中间件
            app.UseTransaction();

            // 启用认证中间件
            app.UseAuthentication();

            // 启用授权中间件
            app.UseAuthorization();

            // 使用自定义的Accessor中间件
            app.UseMiddleware<AccessorMiddleware>();

            // 使用验证中间件
            app.UseMiddleware<ValidationMiddleware>();

            // 将控制器映射到路由
            app.MapControllers();

            // 创建服务作用域，用于执行数据库初始化操作
            using var scope = app.Services.CreateScope();
            // 获取SqlSugarSeed服务，并初始化数据库和表
            scope.ServiceProvider.GetRequiredService<SqlSugarSeed>().InitDatabase().InitTable();
        }
    }
}