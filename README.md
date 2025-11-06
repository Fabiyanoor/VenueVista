# VenueVista

A hybrid multi-platform application built using [.NET MAUI](https://docs.microsoft.com/dotnet/maui) along with a complementary web application.  
This project aims to provide a seamless experience across mobile, desktop, and web platforms using a unified codebase.


---

##  About

**VenueVista** is a cross-platform application built with .NET MAUI for mobile and desktop, and ASP.NET for the web.  
It provides a unified solution that maintains shared logic across different platforms while offering optimized performance and user experience.

The project demonstrates how developers can use **.NET MAUI** and **ASP.NET Core** together to build a scalable, modern, and maintainable software ecosystem.

---

##  Features

-  Unified UI experience across Android, iOS, Windows, and macOS  
-  Shared business logic and data access layers  
-  Responsive web interface for administrative access  
-  Modular project architecture (separation of concerns)  
-  Easily extendable for future modules or services  
-  Built using modern .NET frameworks and tools  

---

## Architecture & Tech Stack

| Layer / Component         | Technology Used                              |
|----------------------------|---------------------------------------------|
| Frontend (Mobile/Desktop)  | .NET MAUI (C# + XAML)                       |
| Web Application            | ASP.NET Core MVC / Blazor                   |
| Shared Logic               | .NET Standard / .NET 6+ Class Libraries     |
| Data Layer / Services      | Entity Framework Core / REST API            |
| Platforms Supported        | Android, iOS, Windows, macOS, Web           |

---

##  Getting Started

### Prerequisites

Make sure you have the following installed before running the project:

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download)  
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (or newer) with **.NET MAUI workload** installed  
- A web browser (for the ASP.NET app)  
- Git (for version control)



## Directory Overview

- **MyApp/**: Contains the main .NET MAUI application with platform-specific implementations for iOS, Android, Windows, and macOS.

- **MyApp.Shared/**: Shared project containing business logic, data models, services, and other cross-platform components used by both the MAUI app and web application.

- **MyApp.Web/**: ASP.NET Core web application providing web-based access to the venue management system.

- **VenueVista.sln**: Visual Studio solution file that orchestrates all projects in the solution.

