using TesteOAuth2Basico.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

new Bootstrapper(builder.Services);

builder.Services.AddControllers();
builder.Services.AddAuthorization(); 
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen();
// Adicione serviços ao contêiner
var app = builder.Build();
app.UseCors("AllowSpecificOrigin");
// Configure o pipeline de solicitação HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
