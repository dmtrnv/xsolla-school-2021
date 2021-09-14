## Требования
* .NET Core 3.1
* Microsoft SQL Server Express LocalDB
* Изменить название сервера в строке подключения к бд (см. appsettings.json)

## Запуск
В папке с проектом выполнить команды:  `dotnet restore`, `dotnet run`. Приложение будет запущено по адресу https://localhost:5001/.

## API
- Интерактивная документация (Swagger) после запуска приложения доступна по https://localhost:5001/index.html.
- OpenAPI-спецификация API, предоставляемого сервером, доступна по адресу https://localhost:5001/swagger/v1/swagger.json
