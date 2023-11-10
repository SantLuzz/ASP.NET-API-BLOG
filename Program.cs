using System.IO.Compression;
using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);


//Carregando as dependencias do Swager
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

    //definindo o encaminhamento autom�tico para https
    app.UseHttpsRedirection();

    //definindo que usa autoriza��o e autentica��o
    app.UseAuthentication(); //quem � o usu�rio
    app.UseAuthorization(); //o que ele pode fazer

    //mapeando os controles
    app.MapControllers();

    //definindo que a resposta deve ser zipada
    app.UseResponseCompression();

    //falando que o servidor pode renderizar html, css, imagens
    app.UseStaticFiles();


    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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
    builder.Services.AddMemoryCache(); //adiciona suporte para cache
    builder.Services.AddResponseCompression(options =>
    {
        //options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>(); //define a forma de compress�o
        //options.Providers.Add<CustomCompressionProvider>();
    });
    //configurando a compress�o
    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Optimal;
    });
    builder.Services
        .AddControllers()
        //suprime a valida��o autom�tica dos controllers
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        })
        //mudando a forma padr�o de serializar e desserializar para json
        .AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.ReferenceHandler =  ReferenceHandler.IgnoreCycles; //ignorando referencia circular
            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault; //quando tiver objeto null n�o renderiza
        });
}

void ConfigureServices(WebApplicationBuilder builder)
{
    //pegando a minha connection string no app settings
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    //configurando a conex�o com o banco de dados e passando a string de conex�o
    builder.Services.AddDbContext<BlogDataContext>(options =>
        options.UseSqlServer(connectionString));

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