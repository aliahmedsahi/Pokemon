# 🧠 Pokémon Web App – Full Stack Application (ASP.NET Core + Vue 3)

A simple full-stack web application that allows users to **search for Pokémon by name or ID**, fetch their details from the public [PokéAPI](https://pokeapi.co/), store them in a SQL database, and view recent entries. The solution includes:

- 🔐 User registration, login, and logout (ASP.NET Identity)
- 🔍 Pokémon search with caching and local storage
- 🎨 Vue 3 frontend for a responsive and user-friendly interface
- 🧪 Unit-tested backend using xUnit and Moq
- 📦 Separate frontend/backend projects (with CORS setup)

---

## 📂 Repositories

- 🔗 **Main Repo (API + Frontend)**: [GitHub – aliahmedsahi/Pokemon](https://github.com/aliahmedsahi/Pokemon/tree/main)
- 📁 **Vue Frontend Project**: [pokemon-vue](https://github.com/aliahmedsahi/Pokemon/tree/main/pokemon-vue)

---

## 🔧 Technologies Used

- ASP.NET Core 7 (Web API)
- Entity Framework Core (In-Memory or SQL Server)
- ASP.NET Core Identity (with cookie-based auth)
- PokéAPI (https://pokeapi.co/)
- Vue 3 (Vite + Composition API)
- xUnit, Moq for backend testing

---

## 🚀 How to Run Locally

### 🔙 Backend Setup (`https://localhost:7009`)

1. Clone the repo and open the backend solution.
2. Run the following to install dependencies:
   ```bash
   dotnet restore
   dotnet build
   dotnet run
   ```
3. API Swagger: `https://localhost:7009/swagger`

✅ **Default Admin User** auto-created:
```txt
Username: admin
Password: 1q2w3E*
```

---

### 🎨 Frontend Setup (`http://localhost:5173`)

1. Navigate to `pokemon-vue` folder:
   ```bash
   cd pokemon-vue
   npm install
   npm run dev
   ```
2. Visit `http://localhost:5173/` in your browser.

🌐 The frontend calls the backend API via CORS (`https://localhost:7009`).

---

## 🧪 Testing

Backend tests:
```bash
cd PokemonWebAPI.Tests
dotnet test
```

Uses:
- `EF InMemory` DB
- `Moq` for mocking HTTP calls
- `xUnit` assertions

---

## 🔐 Authentication

- Register or login using the UI
- Cookie-based auth for all Pokémon API calls
- Logout via frontend logout button
- Admin is seeded on first run

---

## 🔍 Usage Features

- Search Pokémon by name or ID
- Displays: name, sprite, types, height, weight, abilities
- Show 15 most recent Pokémon stored locally

---

## 🧱 Design Decisions

- Caching via DB: avoid repeat PokéAPI calls
- Clear separation of frontend/backend
- Cookie auth (can extend to JWT)
- Clean architecture with future scalability in mind

---

## 📸 Screenshots

![image](https://github.com/user-attachments/assets/ec4c806b-9b9f-4c2a-bffa-06a097af11ca)

![image](https://github.com/user-attachments/assets/6d74a1c8-5008-4074-bd26-90acd725613d)
 
![image](https://github.com/user-attachments/assets/822fcde0-6d97-43f0-a19b-297d339f7bff)



## 🙌 Credits

- [PokéAPI](https://pokeapi.co/)
- Vue.js
- ASP.NET Team
- Moq, xUnit

---

Enjoy building and catching 'em all! 🎮# Pokemon

