using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Puzzle66.Components;

var builder = WebApplication.CreateBuilder(args);

// Load local appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Load Azure Key Vault
var keyVaultUri = new Uri("https://[YOUR-AAP-NAME].vault.azure.net/");
var credential = new DefaultAzureCredential();
// Manually fetch secrets from Key Vault
var client = new SecretClient(keyVaultUri, credential);

// Retrieve secrets and add them to configuration
var keyVaultSecrets = new Dictionary<string, string?>
{
	{ "MyPersonalSettings:FavoriteColor", (await client.GetSecretAsync("MyPersonalSettings--FavoriteColor")).Value.Value },
	{ "MyPersonalSettings:FavoriteNumber", (await client.GetSecretAsync("MyPersonalSettings--FavoriteNumber")).Value.Value },
};

// Add secrets to configuration
builder.Configuration.AddInMemoryCollection(keyVaultSecrets);



// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
