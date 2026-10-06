# Cinema System Management

CinemaSystem is a unified hybrid platform that connects the customer transactional flow and the internal administrative operations to a single, centralized database. Developed by Alin Maslov, the application solves real-time reservation challenges under high concurrency and modernizes the user experience through a private, AI-driven virtual assistant.

## Features

**Real-Time Concurrency Control**: Implements a custom SeatHold mechanism that temporarily locks selected seats as asynchronous background tasks during checkout, effectively preventing double-booking collisions.

**Native AI Assistant**: Features a conversational virtual assistant that formulates personalized movie recommendations by dynamically analyzing the user's age, profile preferences, and the current database context.

**Automated Ticketing Pipeline**: Assembles digital PDF tickets directly in memory, secures them with unique QR codes, and dispatches them asynchronously to the customer via SMTP.

**Administrative Management**: Provides a secure, role-based dashboard for staff to handle movie catalogs, schedule showtimes (with automatic conflict prevention), manage inventory, and monitor financial KPIs.

**Dynamic Loyalty System**: Automatically updates loyalty points per transaction and applies percentage-based discounts to the cart subtotal based on membership tiers.

## Technologies Used

**Backend**: Developed on the .NET platform using C# and the ASP.NET Core framework.

**Database & Persistance**: Microsoft SQL Server managed via Entity Framework Core utilizing a Code-First methodology.

**Frontend**: Built with HTML5, Tailwind CSS, JavaScript, AJAX, and Razor Views to deliver a fluid, asynchronous user interface without full page reloads.

**Artificial Intelligence**: Powered locally by the Meta Llama3 large language model, orchestrated through the Ollama execution engine to guarantee data privacy.

**Third-Party Integrations**: QuestPDF for document generation, QRCoder for validation assets, and MailKit for modern electronic messaging.

## System Architecture

The application is structured on an N-Tier layered architecture to ensure scalability and a strict separation of concerns. The project is divided into four distinct modules:

* **CinemaSystem.Web (Presentation)**: Handles HTTP routing, controller logic, and user interface rendering using the Model-View-Controller (MVC) pattern.

* **CinemaSystem.DataAccess**: Abstracts physical database interactions using the generic Repository and Unit of Work design patterns to guarantee atomic database transactions.

* **CinemaSystem.Models (Domain)**: Contains the POCO domain entities, Data Transfer Objects (DTOs), and data validation annotations.

* **CinemaSystem.Utility**: Encapsulates cross-cutting services such as AI processing, PDF rendering, and email dispatch.
