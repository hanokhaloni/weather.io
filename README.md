# weather.io
ASP.NET CORE api - weather mock for playing with HTTPS on local IIS, local docker, and remote k8s 

## build
 ```
 dotnet run
 ```

 the test using either http://localhost:5200/weatherforecast
 or 
 ```
 curl -X GET "https://localhost:5200/WeatherForecast" -H  "accept: text/plain"
 ```

### Handle System.InvalidOperationExceprtion : Unable to configure HTTPS endpoint for dev env
Just create some developer certificates by running the following command
```
dotnet dev-certs https
```
You will get a security warning for a self-signed certificate. just accept and click "yes".
(Note -is you are using VS2019 or above, it will create it for you).
You can see the developer certificate for use with "localhost".
Tou xcan see it in certmgr, under 'trusted root certificate authorities'->'certificate', undel localhost.