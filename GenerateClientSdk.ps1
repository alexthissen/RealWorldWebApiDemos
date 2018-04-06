# TODO: Parametrize URL and name 
iwr https://localhost:44305/swagger/v2/swagger.json -o RealWorldWebAPIs.json
autorest --input-file=RealWorldWebAPIs.json --csharp --output-folder=GameServerWebAPI.ClientSdk --namespace=GameServerWebAPI.ClientSdk
# Fix basePath address

dotnet new sln -n GameServerSDK --force
dotnet new classlib -o GameServerWebAPI.ClientSdk --force
dotnet sln GameServerSDK.sln add GameServerWebAPI.ClientSdk
cd GameServerWebAPI.ClientSdk

dotnet add package Microsoft.AspNetCore --version 2.1-preview1-final
dotnet add package Microsoft.Rest.ClientRuntime --version 2.3.11
dotnet add package Newtonsoft.Json --version 10.0.1

dotnet restore
dotnet build
dotnet pack --include-symbols