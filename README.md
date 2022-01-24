# weather.io
ASP.NET CORE api - weather mock for playing with HTTPS on local IIS, local docker, and remote k8s 

## build & run

From powershell run
 ```
 dotnet run
 ```

## test

 the test using either http://localhost:5200/weatherforecast
 or 
 ```
 curl -X GET "https://localhost:5200/WeatherForecast" -H  "accept: text/plain"
 ```

 for https test use\
 ```
 curl -X GET "https://localhost:5201/WeatherForecast" -H  "accept: text/plain"
 ```

# Dev env
## Handle System.InvalidOperationExceprtion : Unable to configure HTTPS endpoint for dev env

Just create some developer certificates by running the following command
```
dotnet dev-certs https
```
You will get a security warning for a self-signed certificate. just accept and click "yes".
(Note -is you are using VS2019 or above, it will create it for you).
You can see the developer certificate for use with "localhost".
Tou xcan see it in `certmgr`, under `trusted root certificate authorities`->`certificate`, undel localhost.

# create a custom domain on local machine

## edit hosts file (for local name)
Cheat the DNS!, browse to `c:\Windows\System32\drivers\etc\hosts.` file, and add your local ip and map it to weather.io name

```
10.30.50.29 weather.io
```
## Create a new selfsigned certificate

```
$cert = New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dns weather.io
$pwd = ConvertTo-SecureString -String "Ngsoft123" -Force -AsPlainText
$certpath = "Cert:\localmachine\my\$(cert.Thumbprint)"
Export-PfxCertificate -Cert $certpath -FilePath c:\temp\weather.pfx -Password $pwd
```

the run `certmgr`, and under `trusted root certificate authorities`->`certificate` rightclick, import the certificate at `c:\temp\weather.pfx` , using the password `Ngsoft123`

! Even in development environemt it would be wise to use developer secret ! Put a simple JSON based on your windows profile!
in csproj add under propertygroup a secret
```
    <UserSecretsId>e54ee127-f902-4a7b-96c1-18e23446aacd</UserSecretsId>
```
Then run 
```
dotnet user-secrets set "CertPassword" "Ngsoft123"
```
!note it's in `c:\Users\<your name here>\AppData\Roaming\Microsoft\UserSecrets\e54ee127-f902-4a7b-96c1-18e23446aacd\secrets.json`

Add path of certificate to your `appsetting.json` -
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "CertPath": "c:\\temp\\weather.pfx"
}
```

Then add "CertPassword" reference in you `program.cs` code 
```
 public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    HostConfig.CertPath = context.Configuration["CertPath"];//from JSON Files
                    HostConfig.CertPasswoord = context.Configuration["CertPassword"];//from Secret Manager storage, in user profile app data 
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        //serverOptions.UseHttps(HostConfig.CertPath, HostConfig.CertPasswoord);
                        serverOptions.ListenAnyIP(5200);
                        serverOptions.ListenAnyIP(5201, listenOptions =>
                        {
                            listenOptions.UseHttps(HostConfig.CertPath, HostConfig.CertPasswoord);
                        });
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class HostConfig
    {
        public static string CertPath { get; set; }
        public static string CertPasswoord { get; set; }

    }
```

### If you get System.Security.Cryptography.CryptographicException: Access denied

Run your vscode / posh / terminal as windows admin. Its a problem to readd access the appdata folder (roaming profile)

### curl fails
```
curl failed to verify the legitimacy of the server and therefore could not
establish a secure connection to it. To learn more about this situation and
how to fix it, please visit the web page mentioned above.
```

just run
```
curl -k -X GET "https://localhost:5201/WeatherForecast" -H  "accept: text/plain"
```
You need to pass the `-k` or `--insecure` option to the curl command. This option explicitly allows curl to perform “insecure” SSL connections and transfers. All SSL connections are attempted to be made secure by using the CA certificate bundle installed by default. 

# TODO

read about [order of precedence of config params](https://devblogs.microsoft.com/premier-developer/order-of-precedence-when-configuring-asp-net-core/)

```
JSON Files
XML Files
INI Files
Command-line arguments
Environment variables
In-memory .NET objects
Secret Manager storage
Encrypted in Azure Key Vault
```