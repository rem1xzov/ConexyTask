# ConexyTask

**ConexyTask** — веб-приложение для управления задачами, созданное для хакатона. Бэкенд на ASP.NET Core 9.0, база данных PostgreSQL, фронтенд на чистом HTML/CSS/JavaScript.
Доступен по ссылке https://conexytask.onrender.com

---

## Технологии

- **ASP.NET Core 9.0** — API и бизнес-логика
- **Entity Framework Core 9.0** — ORM
- **PostgreSQL** — база данных
- **HTML5 / CSS3 / JavaScript** — фронтенд
- **Font Awesome** — иконки

---

## Возможности

- Создание, просмотр, редактирование и удаление задач
- Установка приоритета (сложность + срочность)
- Установка дедлайна
- Встроенный таймер (помидорный) в карточке задачи
- Автоматическое определение URL API при локальном запуске и на хостинге
- Сохранение данных в localStorage при недоступности сервера
- Адаптивный дизайн

---

## Установка и запуск

### Требования
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/) (или Docker)
- Git

### Шаги

1. Клонируйте репозиторий:
   ```bash
   git clone https://github.com/ваш-username/conexytask.git
   cd conexytask
   "ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=conexy_db;Username=postgres;Password=ваш_пароль"
}
dotnet ef database update
dotnet run
