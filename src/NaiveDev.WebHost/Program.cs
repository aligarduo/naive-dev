using System.Reflection;
using NaiveDev.Infrastructure.Extensions;
using NaiveDev.WebHost;
using NLog.Web;

// ����WebӦ�ù�����
var builder = WebApplication.CreateBuilder(args);

// ����Autofac��Ϊ����ע������
builder.Host.AddAutofac(Assembly.GetExecutingAssembly());

// ��ӻ���֧��
builder.Host.AddCache();

// ������־ϵͳ
builder.Logging.AddNLog("nlog.config");
builder.Host.UseNLog();

// ע�����
var startup = new Startup(builder.Configuration, builder.Environment);
startup.ConfigureServices(builder.Services);

// ����WebӦ��
var app = builder.Build();
// �����м��
startup.Configure(app, startup.Env);

// ����WebӦ��
app.Run();