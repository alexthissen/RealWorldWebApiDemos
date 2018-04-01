# TODO: Parametrize URL and name 
iwr https://localhost:44305/swagger/v1/swagger.json -o RealWorldWebAPIs.json
autorest --input-file=RealWorldWebAPIs.json --csharp --output-folder=GameServerWebAPI.ClientSdk --namespace=GameServerWebAPI.ClientSdk

dotnet new sln -n GameServerSDK --force
dotnet new classlib -o GameServerWebAPI.ClientSdk
dotnet sln add GameServerWebAPI.ClientSdk
cd GameServerWebAPI.ClientSdk

dotnet add package Microsoft.AspNetCore --version 2.1-preview1-final
dotnet add package Microsoft.Rest.ClientRuntime
dotnet add package Newtonsoft.Json --version 10.0.1

dotnet restore
dotnet build
dotnet pack --include-symbols