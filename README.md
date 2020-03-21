# Hike Tracker Server dotnet

Creeated via the following
* `dotnet new web -o app`
*  `dotnet add package Microsoft.Azure.ServiceBus` (example to add package)
* Created ACR repo and AKS cluster via Azure portal (names hiketracker and hike-tracker-kube respectively)
* Added secrets to Azure vault via Azure portal

To hook up proper AKS permissions, including to vault
```bash
az aks install-cli
az aks get-credentials --resource-group Default-Networking --name hike-tracker-kube
kubectl get nodes # to confirm connection
az aks update -n hike-tracker-kube -g Default-Networking --attach-acr hiketracker
kubectl apply -f https://raw.githubusercontent.com/Azure/aad-pod-identity/master/deploy/infra/deployment-rbac.yaml

# put clientId and id into azureidentity.yaml, save full-identity-id for below
az identity create -g Default-Networking -n hike-tracker-ident -o json

clientid=$(az aks show -g Default-Networking -n hike-tracker-kube --query servicePrincipalProfile.clientId -o tsv)
az role assignment create --role "Managed Identity Operator" --assignee $clientid --scope <full-identity-id>
```

## Build and Run

Much of this assumes that you're logged into my personal azure subscription from your local machine.

### Locally

In dev mode

```bash
cd app
dotnet run
```

In prod mode

```bash
cd app
dotnet publish -c Release
dotnet run bin/release/netcoreapp3.1/app.dll
```

### In a container locally

```bash
docker build hike-tracker-server-dotnet .
docker run -p 8080:80 hike-tracker-server-dotnet
# test with curl http://localhost:8080/api/hike
```

Currently, this will fail to start because the container can't authenticate to azure vault. You could overcome rthis by starting
the container in interactive mode, installing the azure cli, logging in via `az login`, and then starting the app

```bash
docker -it --entrypoint bash run -p 8080:80 hike-tracker-server-dotnet
curl -sL https://aka.ms/InstallAzureCLIDeb | bash
az login # follow instructions on screen
dotnet app.dll
```

### Deploy to Azure Container Registry (ACR)

First deploy

```bash
az acr login --name hiketracker
docker build -t hiketracker.azurecr.io/hike-tracker-server-dotnet .
docker push hiketracker.azurecr.io/hike-tracker-server-dotnet
kubectl apply -f hike-tracker.yaml
kubectl get service hike-tracker-server-dotnet --watch # wait until EXTERNAL-IP has a value if first deploy, then kill via ctrl-c
```

Subsequent

```bash
az acr login --name hiketracker
docker build -t hiketracker.azurecr.io/hike-tracker-server-dotnet .
docker push hiketracker.azurecr.io/hike-tracker-server-dotnet
kubectl delete pods -l app=hike-tracker-server-dotnet #forces redeploy
```
