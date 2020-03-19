# Hike Tracker Server dotnet

Creeated via
* `dotnet new web -o app`
*  `dotnet add package Microsoft.Azure.ServiceBus` (example)

## Build and Run

```bash
cd app
dotnet publish -c Release
cd ..
docker build hike-tracker-server-dotnet .
docker run -p 80:80 ike-tracker-server-dotnet
# test with curl http://localhost/api/hike
```
