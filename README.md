# 🏍️ MotoLogPro

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![MAUI](https://img.shields.io/badge/MAUI-Cross--Platform-blueviolet?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-Latest-239120?style=for-the-badge&logo=c-sharp)
![Status](https://img.shields.io/badge/Status-In%20Development-yellow?style=for-the-badge)

> **"Dal cacciavite al compilatore."**
> Un sistema di gestione officina Enterprise-grade costruito con .NET 10 e Clean Architecture.

## 💡 Il Progetto

**MotoLogPro** nasce dall'esigenza reale di unire la precisione meccanica con l'astrazione del software. Sviluppato da un Montatore Meccanico e Software Developer, questo progetto mira a simulare uno scenario aziendale completo per la gestione di flotte moto, interventi di manutenzione e clienti.

L'obiettivo tecnico è dimostrare l'applicazione di pattern architetturali avanzati e l'uso delle ultimissime tecnologie Microsoft (.NET 10) in un contesto distribuito (Mobile + Cloud).

## 🏗️ Architettura

La soluzione segue rigorosamente la **Clean Architecture** per garantire la separazione delle responsabilità (Separation of Concerns), scalabilità e testabilità. È suddivisa in 6 progetti distinti:

* **`MotoLogPro.Domain`**: Il cuore del sistema. Contiene le Entità (`Motorcycle`, `ApplicationUser`) e la logica di business pura. Nessuna dipendenza esterna.
* **`MotoLogPro.Shared`**: Libreria condivisa contenente i DTO (Data Transfer Objects) e i contratti che API e Client utilizzano per comunicare.
* **`MotoLogPro.Infrastructure`**: Gestione dell'accesso ai dati (EF Core), configurazione del Database (SQL Server) e integrazioni esterne.
* **`MotoLogPro.API`**: Backend ASP.NET Core Web API. Espone gli endpoint REST, gestisce l'autenticazione JWT e funge da gateway per il database.
* **`MotoLogPro.Client`**: Frontend Cross-Platform sviluppato in **.NET MAUI**. Gestisce la UI, la logica di presentazione (MVVM) e lo storage sicuro locale.
* **`MotoLogPro.Tests`**: Progetto xUnit per Unit Testing e Integration Testing.

## 🛠️ Stack Tecnologico

* **Framework:** .NET 10 (LTS)
* **Linguaggio:** C# 13
* **Frontend:** .NET MAUI (Android, iOS, Windows)
* **Backend:** ASP.NET Core Web API
* **Database:** SQL Server (LocalDB per sviluppo)
* **ORM:** Entity Framework Core (Code-First)
* **Autenticazione:** ASP.NET Core Identity + JWT Bearer Tokens
* **Sicurezza:** SecureStorage (Keychain/Keystore), Role-Based Access Control (RBAC)

## ✨ Funzionalità Chiave (In Sviluppo)

- [x] **Setup Architetturale:** Clean Architecture a 6 livelli configurata.
- [x] **Database:** Migrazioni EF Core e Relazioni 1:N (Utente -> Moto).
- [x] **Autenticazione:** Registrazione, Login e generazione JWT Token.
- [x] **Client Mobile:** Configurazione base MAUI e servizi HTTP.
- [ ] **Gestione Moto:** CRUD completo delle moto con decodifica VIN.
- [ ] **Dashboard:** Viste differenziate per Admin, Meccanici e Clienti.
- [ ] **Integrazione API:** Connessione a servizi esterni (es. NHTSA) per dati tecnici.

## 🚀 Come iniziare

Per eseguire il progetto in locale:

### Prerequisiti
* Visual Studio 2022 (Versione che supporta .NET 10 o Preview).
* .NET 10 SDK installato.
* SQL Server Express o LocalDB.

### Installazione

1.  **Clona la repository:**
    ```bash
    git clone [https://github.com/TUO-USERNAME/MotoLogPro.git](https://github.com/TUO-USERNAME/MotoLogPro.git)
    ```

2.  **Database Setup:**
    Apri la soluzione in Visual Studio, apri la *Console di Gestione Pacchetti* e lancia:
    ```powershell
    Update-Database -Project MotoLogPro.Infrastructure -StartupProject MotoLogPro.API
    ```

3.  **Avvia il Backend:**
    Imposta `MotoLogPro.API` come progetto di avvio ed esegui.
    * L'API sarà disponibile (di default) su `https://localhost:7196` (o porta simile).

4.  **Avvia il Client (Android/Windows):**
    * **Nota per Android:** L'emulatore utilizza `10.0.2.2` per connettersi al localhost del PC. Assicurati che `MauiProgram.cs` sia configurato correttamente.

## 🤝 Contribuisci & Feedback

Questo è un progetto open-source nato per passione e apprendimento. Feedback, Pull Request e suggerimenti sono benvenuti, specialmente su:
* Ottimizzazioni EF Core.
* Miglioramenti UI/UX in MAUI.
* Unit Test coverage.

---
*Developed with ❤️, passion and mechanical precision.*
