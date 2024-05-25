using System.Reflection;
using NaiveDev.Infrastructure.Extensions;
using NaiveDev.WebHost;
using NLog.Web;

// 创建Web应用构建器
var builder = WebApplication.CreateBuilder(args);

// 配置Autofac作为依赖注入容器
builder.Host.AddAutofac(Assembly.GetExecutingAssembly());

// 添加缓存支持
builder.Host.AddCache();

// 配置日志系统
builder.Logging.AddNLog("nlog.config");
builder.Host.UseNLog();

// 注册服务
var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);

// 构建Web应用
var app = builder.Build();
// 配置中间件
startup.Configure(app, startup.Env);

// 运行Web应用
app.Run();