dotnet publish -c Release -o docs

rm ./docs/web.config

mv ./docs/wwwroot/* ./docs