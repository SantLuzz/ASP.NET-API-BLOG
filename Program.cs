using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

var app = builder.Build();
LoadConfiguration(app);



void LoadConfiguration(WebApplication app)
{
    //definindo as configura��es do appsettings
    Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");
    //carregando o n� de SMTP
    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection("Smtp").Bind(smtp);
    Configuration.Smtp = smtp;

    //definindo que usa autoriza��o e autentica��o
    app.UseAuthentication(); //quem � o usu�rio
    app.UseAuthorization(); //o que ele pode fazer
    app.UseStaticFiles(); //falando que o servidor pode renderizar html, css, imagens
    app.MapControllers();
    app.Run();
}

void ConfigureAuthentication(WebApplicationBuilder builder)
{
    //configuranado o token
    var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
    //adicionando o esquema de autentica��o
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x => //mostrando com descriptografar o token
    {
        //passando os parametro para valida��o do token e descriptografar
        x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

}

void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services
    .AddControllers()
    //suprime a valida��o autom�tica dos controllers
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
}

void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<BlogDataContext>();
    ////sempre criar um nova instancia
    //builder.Services.AddTransient();
    ////Dura por requisi��o (sempre ao iniciar um request)
    //builder.Services.AddScoped();
    ////um por app, sempre vai estar na mem�ria da aplica��o, uma vez chamado vai ser sempre a mesma instancia
    //builder.Services.AddSingleton();

    //token da aplica��o
    builder.Services.AddTransient<TokenService>();
    //Servi�o de email da aplica��o
    builder.Services.AddTransient<EmailService>();
}