# Staff.Io-Back
Backend-часть проекта **Staff.IO** на **ASP.NET Core 8 (Web API)**.

## Что умеет проект

API для управления сотрудниками и внутренними процессами:
- авторизация и сессии;
- управление сотрудниками и профилями;
- администрирование (доступы, перемещения, удаление/обновление);
- аналитика и история изменений;
- расходы;
- работа с файлами (Backblaze B2).

## Стек

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server
- Swagger (в `Development`)

- ## Требования

- SDK: **.NET 8**
- БД: **SQL Server**

- ## Конфигурация

Основные настройки находятся в:
- `StaffIo.Web/StaffIo.Web/appsettings.json`

Обязательные параметры:

```json
{
  "DataContext": "<connection string to SQL Server>",
  "JwtOptions": {
    "SecretKey": "<secret>",
    "ExpiresHours": "12",
    "NameToken": "token"
  }
}
```

Дополнительно (если хотите переопределить значения по умолчанию для файлового сервиса):

```json
{
  "BackBazeB2Options": {
    "ApplicationKeyId": "<key-id>",
    "ApplicationKey": "<application-key>",
    "AuthPatch": "https://api.backblazeb2.com/b2api/v4/b2_authorize_account",
    "BasketId": "<bucket-id>"
  }
}
```

## Основные API-модули

Контроллеры находятся в `StaffIo.Web/StaffIo.Web/Controllers`:
- `AuthorizationController`
- `EmployeesController`
- `AdminController`
- `AnalyticsController`
- `ExpensesController`
- `HistoryController`
