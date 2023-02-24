### Default appsettings.json

```
{
  "Serilog": {
    "MinimumLevel": "Verbose",
    "WriteTo": [
      {
        "Name": "Console",
        "outputTemplate": "{Timestamp:G}[{Level:u3}] ** {Message} ** ({SourceContext}) {NewLine:1}{Exception:1}"
      }]
  },
  
  "ConnectionStrings": {
    "Default": "Server=...;User=...;Password=...;Database=...;Allow User Variables=true;",
    "RabbitMq": "amqp://admin:...@mq.example.com:5672"
  },
  
  "Spaces": {
    "AccessKey": "...",
    "SecretKey": "...",
    "BucketName": "...",
    "CdnUrl": "https://cdn.example.com",
    "ServiceUrl": "https://ams3.digitaloceanspaces.com"
  },
  
  "Delays": {
    "SecondsToBlockAccountAfterFetched": 10800,
    "SecondsToWaitAfterAccountFetchFailed": 30,
    "SecondsToWaitAfterAccountLoginFailed": 60
  },
  
  "BrowserSettings": {
    "Driver": "firefox",
    "LoadImages": true,
    "RunHeadless": false,
    "Authentication": true
  }
}
```