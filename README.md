# MovieVault

A personal movie and TV tracking REST API. Users authenticate with JWT and can search TMDB for titles, manage watchlists, mark favourites, write reviews, and rate content on a 1–10 scale.

**Course:** Service Oriented Architecture — 2026  
**Team:** Nejazi Shabani · Alban Zulfija

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 / ASP.NET Core |
| Database | SQL Server (LocalDB for development) |
| ORM | Entity Framework Core 8 |
| Authentication | JWT Bearer |
| Object mapping | AutoMapper |
| Logging | Serilog (console + rolling file) |
| API docs | Swashbuckle / Swagger UI |
| Password hashing | BCrypt.Net-Next |
| External data | TMDB REST API v3 |
| Unit tests | xUnit + Moq + FluentAssertions |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server LocalDB (ships with Visual Studio) or any SQL Server instance
- A free [TMDB API key](https://www.themoviedb.org/settings/api)

---

## Setup

### 1. Clone the repository

```bash
git clone https://github.com/<your-org>/MovieVault.git
cd MovieVault
```

### 2. Configure secrets

Open `MovieVault.API/appsettings.json` and fill in:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MovieVault;Trusted_Connection=True;"
},
"JwtSettings": {
  "Key": "<your-32+-char-secret-key>"
},
"TmdbSettings": {
  "ApiKey": "<your-tmdb-api-key>"
}
```

> Never commit real secrets. Copy `appsettings.json` to `appsettings.Development.json` and add the latter to `.gitignore` for local overrides.

### 3. Apply database migrations

```bash
dotnet ef database update --project MovieVault.API
```

### 4. Run the API

```bash
dotnet run --project MovieVault.API
```

The API starts on `https://localhost:7xxx` / `http://localhost:5xxx` (see `Properties/launchSettings.json`).  
Swagger UI is available at `https://localhost:<port>/swagger` in Development mode.

---

## Running Tests

```bash
dotnet test MovieVault.Tests
```

25 unit tests covering Auth, Watchlist, Review, Favorite, and Rating services.

---

## TMDB API Key

1. Create a free account at [themoviedb.org](https://www.themoviedb.org/)
2. Go to **Settings → API** and request an API key (choose *Developer*)
3. Copy the **API Key (v3 auth)** value into `appsettings.json` under `TmdbSettings:ApiKey`

---

## Database Setup

The project uses EF Core Code-First migrations.

```bash
# Create the database and apply all migrations
dotnet ef database update --project MovieVault.API

# Add a new migration after model changes
dotnet ef migrations add <MigrationName> --project MovieVault.API

# Roll back the last migration
dotnet ef migrations remove --project MovieVault.API
```

The default connection string targets **SQL Server LocalDB** (`(localdb)\mssqllocaldb`), which is included with Visual Studio. To use a full SQL Server instance, update `DefaultConnection` in `appsettings.json`.

---

## API Endpoints

All authenticated endpoints require the header:  
`Authorization: Bearer <token>`

Tokens are obtained from `POST /api/auth/login` or `POST /api/auth/register`.

### Auth — `/api/auth`

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | No | Register a new account |
| POST | `/api/auth/login` | No | Log in, receive JWT |
| GET | `/api/auth/me` | Yes | Get current user profile |

### Movies (TMDB) — `/api/movies`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/movies/search?query=&page=1&mediaType=multi` | No | Search TMDB |
| GET | `/api/movies/popular?mediaType=movie&page=1` | No | Popular titles |
| GET | `/api/movies/trending?mediaType=movie&timeWindow=week&page=1` | No | Trending titles |
| GET | `/api/movies/genre/{genreId}?mediaType=movie&page=1` | No | Browse by genre |
| GET | `/api/movies/movie/{tmdbId}` | No | Movie detail |
| GET | `/api/movies/tv/{tmdbId}` | No | TV show detail |

### Watchlists — `/api/watchlists`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/watchlists` | Yes | List user's watchlists |
| POST | `/api/watchlists` | Yes | Create watchlist |
| GET | `/api/watchlists/{id}` | Yes | Get watchlist by id |
| PUT | `/api/watchlists/{id}` | Yes | Update watchlist |
| DELETE | `/api/watchlists/{id}` | Yes | Delete watchlist |
| GET | `/api/watchlists/{id}/items` | Yes | List items in watchlist |
| POST | `/api/watchlists/{id}/items` | Yes | Add item to watchlist |
| DELETE | `/api/watchlists/{id}/items/{itemId}` | Yes | Remove item |
| PATCH | `/api/watchlists/{id}/items/{itemId}/toggle` | Yes | Toggle watched status |

### Favorites — `/api/favorites`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/favorites?page=1&pageSize=10` | Yes | List favourites (paged) |
| POST | `/api/favorites` | Yes | Add to favourites |
| DELETE | `/api/favorites/{id}` | Yes | Remove favourite |
| GET | `/api/favorites/check/{tmdbId}` | Yes | Check if title is favourited |
| DELETE | `/api/favorites/clear` | Yes | Clear all favourites |
| GET | `/api/favorites/count` | Yes | Count of favourites |

### Reviews — `/api/reviews`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/reviews/movie/{tmdbId}?page=1&pageSize=10` | No | Reviews for a title |
| GET | `/api/reviews/user/{userId}` | No | Reviews by a user |
| GET | `/api/reviews/{id}` | No | Single review |
| POST | `/api/reviews` | Yes | Write a review |
| PUT | `/api/reviews/{id}` | Yes | Update own review |
| DELETE | `/api/reviews/{id}` | Yes | Delete own review |

### Ratings — `/api/ratings`

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/ratings/movie/{tmdbId}` | No | All ratings for a title |
| GET | `/api/ratings/movie/{tmdbId}/average` | No | Average score for a title |
| GET | `/api/ratings/my/{tmdbId}` | Yes | Your rating for a title |
| POST | `/api/ratings` | Yes | Rate a title (1–10) |
| PUT | `/api/ratings/{id}` | Yes | Update rating |
| DELETE | `/api/ratings/{id}` | Yes | Delete rating |

---

## Git Branching Strategy

The project follows **GitHub Flow**:

| Branch | Purpose |
|---|---|
| `main` | Always deployable; protected — direct pushes prohibited |
| `feature/<name>` | One branch per feature or task, branched off `main` |
| `fix/<name>` | Bug fixes, same lifecycle as feature branches |

Workflow:

1. Branch off `main`: `git checkout -b feature/watchlist-api`
2. Commit focused, working increments
3. Open a Pull Request targeting `main`
4. At least one code review required before merge
5. Squash-merge or rebase to keep `main` history linear
6. Delete the feature branch after merge

---

## Project Structure

```
MovieVault/
├── MovieVault.API/
│   ├── Controllers/        # HTTP layer — routing and status codes only
│   ├── Data/               # EF DbContext + migrations
│   ├── Helpers/            # JwtTokenGenerator, QueryParameters, PagedResult
│   ├── Integrations/TMDB/  # TMDB HTTP client + response models
│   ├── Mappings/           # AutoMapper profiles
│   ├── Middleware/         # ExceptionHandlingMiddleware
│   ├── Models/
│   │   ├── Domain/         # EF entity classes
│   │   └── DTOs/           # Request / response shapes
│   ├── Repositories/
│   │   ├── Interfaces/     # Repository contracts
│   │   └── Implementations/# EF Core implementations
│   └── Services/
│       ├── Interfaces/     # Service contracts (business logic boundary)
│       └── Implementations/# Concrete service classes
└── MovieVault.Tests/       # xUnit unit tests
```
