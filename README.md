# 🎓 EduQuiz AI

Generador de evaluaciones de selección múltiple potenciado por **Claude AI**.  
Desarrollado con **.NET 10 + C#** (backend) y **Vue 3** (frontend).

---

## Requisitos previos

- .NET SDK 10 → https://dotnet.microsoft.com/download
- Node.js 18+ → https://nodejs.org
- API Key de Claude → https://console.anthropic.com

---

## Configuración

Edita el archivo `backend/EduQuizAI.API/appsettings.json` y reemplaza `TU_API_KEY_AQUI` con tu clave real de Claude:
```json
{
  "Claude": {
    "ApiKey": "TU_API_KEY_AQUI"
  }
}
```

---

## Cómo correr el proyecto

### Terminal 1 — Backend
```bash
cd backend/EduQuizAI.API
dotnet restore
dotnet run
```

API disponible en: http://localhost:5000

### Terminal 2 — Frontend
```bash
cd frontend
npm install
npm run dev
```

App disponible en: http://localhost:5173

---

## Stack tecnológico

| Capa | Tecnología |
|---|---|
| Backend | .NET 10, C#, ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Base de datos | SQLite |
| IA | Claude API (claude-sonnet-4-20250514) |
| Frontend | Vue 3, Vite |