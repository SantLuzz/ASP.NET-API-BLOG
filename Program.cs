using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//configuranado o token
var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
//adicionando o esquema de autenticação
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => //mostrando com descriptografar o token
{
    //passando os parametro para validação do token e descriptografar
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});



builder.Services
    .AddControllers()
    //suprime a validação automática dos controllers
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddDbContext<BlogDataContext>();
////sempre criar um nova instancia
//builder.Services.AddTransient();
////Dura por requisição (sempre ao iniciar um request)
//builder.Services.AddScoped();
////um por app, sempre vai estar na memória da aplicação, uma vez chamado vai ser sempre a mesma instancia
//builder.Services.AddSingleton();

//token da aplicação
builder.Services.AddTransient<TokenService>();

var app = builder.Build();

app.MapControllers();

//definindo que usa autorização e autenticação
app.UseAuthentication(); //quem é o usuário
app.UseAuthorization(); //o que ele pode fazer

app.Run();
