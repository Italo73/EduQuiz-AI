# 🎓 EduQuiz AI

Generador de evaluaciones de selección múltiple potenciado por **Claude AI**.
Desarrollado con **.NET 8 + C#** (backend) y **Vue 3** (frontend).

---

## Requisitos previos

- .NET SDK 8.0 → https://dotnet.microsoft.com/download
- Node.js 18+  → https://nodejs.org
- API Key de Claude → https://console.anthropic.com

---

## Pasos para levantar el proyecto

### 1. Poner la API Key

Edita el archivo: backend/EduQuizAI.API/appsettings.json

Reemplaza TU_API_KEY_AQUI con tu clave real:
  "Claude": { "ApiKey": "sk-ant-..." }

### 2. Backend (Terminal 1)

  cd backend/EduQuizAI.API
  dotnet restore
  dotnet run

API disponible en: http://localhost:5000

### 3. Frontend (Terminal 2)

  cd frontend
  npm install
  npm run dev

App disponible en: http://localhost:5173