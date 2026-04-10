using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Charger le certificat (fichier .pfx)
var cert = new X509Certificate2("cert.pfx", "password");

// Configurer Kestrel en HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(8443, listenOptions =>
    {
        listenOptions.UseHttps(cert);
    });
});

var app = builder.Build();

// Endpoint simple
app.MapGet("/", () => "Hello HTTPS from Kestrel!");

// Lancer le serveur
app.Run();