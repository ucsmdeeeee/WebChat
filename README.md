
# WebChat

## Описание проекта

**WebChat** — это веб-приложение, созданное с использованием ASP.NET Core 8.0. Приложение включает функционал управления пользователями, чатами, логами и предоставляет удобный интерфейс для работы с данными.

## Основные функции


## Структура проекта

```
WebApplication13/
├── WebApplication13/
│   ├── Controllers/        # Контроллеры MVC
│   ├── Data/               # Классы для работы с данными
│   ├── Jobs/               # Фоновые задания
│   ├── Migrations/         # Миграции базы данных
│   ├── Models/             # Модели данных
│   ├── Services/           # Логика сервисов
│   ├── Utils/              # Утилиты и вспомогательные классы
│   ├── Views/              # Представления MVC
│       ├── Admin/
│       ├── Home/
│       └── Shared/
│            # Файлы конфигурации проекта
├── TestProject/            # Проект с тестами
├── LogsConsole/            # Консоль для просмотра логов
└── README.md               # Текущая документация
```

## Технологии

- **Frontend**:
    - Bootstrap: для стилизации.
    - jQuery: для клиентской логики.

- **Backend**:
    - ASP.NET Core 8.0.
    - Entity Framework Core: для работы с базой данных.

## Настройка проекта

1. Установите .NET SDK версии 8.0 или выше.
2. Настройте базу данных:
    ```bash
    dotnet ef database update
    ```
3. Установите зависимости через npm (если требуется):
    ```bash
    npm install
    ```

## Запуск проекта
1. Установите свои параматры в appsettings.json

2. Выполните команду:
    ```bash
    dotnet run
    ```
3. Приложение будет доступно по адресу [https://localhost:7296](https://localhost:7296).

## Тестирование

Проект включает модульные тесты. Для их выполнения используйте:
```bash
dotnet test
```

## Роли пользователей

- **Администратор**:
    - Имеет доступ к логам.

- **Пользователь**:
    - Может участвовать в чатах, отправлять и получать сообщения.
