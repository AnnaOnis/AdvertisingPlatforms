# Рекламные площадки: Сервис подбора по локациям

Веб-сервис для управления рекламными площадками и поиска по локациям.

## 📋 Оглавление
- [Рекламные площадки: Сервис подбора по локациям](#рекламные-площадки-сервис-подбора-по-локациям)
  - [📋 Оглавление](#-оглавление)
  - [⚙️ Требования](#️-требования)
  - [🚀 Запуск](#-запуск)
  - [🌐 Использование API](#-использование-api)
  - [](#)
  - [🧪 Тестирование](#-тестирование)
  - [📖 Документация](#-документация)
  - [🛠 Разработка](#-разработка)
  
---

## ⚙️ Требования
- .NET 6 SDK
- IDE (Visual Studio 2022/Rider/VSCode)

---

## 🚀 Запуск
```bash
# 1. Клонировать репозиторий
git clone https://github.com/AnnaOnis/AdvertisingPlatforms.git
cd AdvertisingPlatforms

# 2. Восстановить зависимости
dotnet restore

# 3. Запустить приложение
dotnet run --project AdvertisingPlatforms
```
Приложение будет доступно по адресу: http://localhost:5000

---

## 🌐 Использование API

- **Через Swagger UI**
    Откройте в браузере: http://localhost:5000/swagger
##
- **Примеры запросов через curl**
    - Загрузка данных:
      ```bash
      curl -X POST -F "file=@data.txt" http://localhost:5000/api/advplatforms/upload
      ```
    - Поиск площадок:
      ```bash
      curl "http://localhost:5000/api/advplatforms/search?location=/ru/msk"
      ```
---

## 🧪 Тестирование

```bash
# Запуск всех тестов
dotnet test
```
---

## 📖 Документация

**Методы API**
|  Метод  | Путь    | Описание |
|---------|---------|---------|
| POST  | 	/api/advplatforms/upload  | Загрузка данных из файла  |
| GET| /api/advplatforms/search?location=/yuor/location | Поиск по локации  |

---

## 🛠 Разработка

**Структура проекта**
```
AdPlatformsService/
├── Controllers/       # API контроллер
├── Models/            # Модель данных
├── Parser/            # Парсер файла с данными
├── Samples/           # Пример файла с данными для тестирования
└── Services/          # Бизнес-логика

AdvPlatformsTests/
├── AdvPlatformServiceTests.cs      # Юнит-тесты сервиса
└── PlatformsFileParserTests.cs    # Юнит-тесты парсера
```