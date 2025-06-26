# **OrderDelivery Backend Solution**

## **Project Overview**
OrderDelivery is a robust backend solution for an innovative order delivery platform. It aims to seamlessly connect local merchants (e.g., supermarkets, restaurants) with a network of freelance drivers (bicycles, motorcycles, tuk-tuks) to facilitate efficient and flexible last-mile delivery services to customers. This project emphasizes scalability, reliability, and clear separation of concerns using Clean Architecture.

---

## **Objectives**
- Enable local merchants to expand their reach through flexible delivery options.
- Provide freelance drivers with opportunities for flexible earnings.
- Ensure efficient and timely delivery of orders.
- Offer a reliable and scalable backend infrastructure for growth.
- Enhance customer satisfaction through real-time tracking and reliable service.

---

## **Key Features**
- **User Management**: Comprehensive management for Merchants, Drivers, and Admin users, including secure authentication and authorization.
- **Order Lifecycle Management**: From creation by merchants, through driver assignment and real-time status updates, to successful delivery.
- **Driver Matching & Assignment**: Intelligent system for assigning orders to available and suitable drivers based on proximity and other criteria.
- **Real-time Location Tracking**: Accurate GPS tracking for drivers and delivery locations to provide real-time updates.
- **Notification System**: Instant push notifications for critical order updates to all relevant parties.
- **Rating & Review System**: Allows merchants to rate drivers based on delivery performance and service quality.
- **Admin Panel**: Tools for administrators to oversee users, manage orders, and monitor system health and reports.

---

## **Technologies Used**
- **Backend**: C# (.NET SDK 8)
- **Web Framework**: ASP.NET Core Web API
- **Database**: SQL Server with Entity Framework Core
- **Architecture**: Clean Architecture (App.Api, App.Application, App.Domain, App.Infrastructure)
- **ORM**: Entity Framework Core

---

## **Installation & Setup**
1. **Clone the repository**:
    ```bash
    git clone https://github.com/WaelAlfnan/OrderDelivery.git
    ```
2. **Set up the database**:
    - Ensure SQL Server is running and accessible.
    - Update the database connection string in `appsettings.json` within the `App.Api` or `App.Infrastructure` project (depending on where you centralize configuration).
    - Navigate to the `App.Infrastructure` project directory in your terminal.
    - Run Entity Framework Core migrations to create the database schema:
    ```bash
    dotnet ef database update --project App.Infrastructure --startup-project App.Api
    ```
3. **Restore NuGet Packages**:
    - Open the solution in Visual Studio or run `dotnet restore` from the solution root directory:
    ```bash
    dotnet restore
    ```
4. **Run the application**:
    - Set `App.Api` as the startup project in Visual Studio and press F5.
    - Alternatively, navigate to the `App.Api` project directory in your terminal and run:
    ```bash
    dotnet run
    ```
    - The API will typically be available at `https://localhost:7239` (check the console output for the exact port).

---

## **System Architecture**
The system is built upon **Clean Architecture** principles, promoting separation of concerns, testability, and maintainability:

### **App.Api (Presentation/Entry Point)**
- Acts as the interface for external clients.
- Contains Web API controllers, handling HTTP requests and responses.
- Responsible for deserializing requests, routing, and serializing responses.
- Contains Dependency Injection setup.

### **App.Application (Application Core)**
- Implements the application's specific business rules and use cases.
- Contains application services, commands, queries, and DTOs (Data Transfer Objects).
- Orchestrates interactions between the Domain and Infrastructure layers.

### **App.Domain (Domain Core)**
- The heart of the application, completely independent of other layers.
- Contains core business entities (e.g., User, Order, Merchant, Driver), value objects, domain events, and business rules.
- Defines interfaces for repositories and external services that the domain needs.

### **App.Infrastructure (Data Access & External Services)**
- Provides concrete implementations for interfaces defined in `App.Domain`.
- Handles data persistence (e.g., Entity Framework Core DbContext and Repositories for SQL Server).
- Manages integration with external services (e.g., notification services, payment gateways, if applicable).

---

## **Database Design**
### **Entity-Relationship Diagram (ERD)**
The ERD visually represents the entities within our system and their relationships, illustrating the core data model for merchants, drivers, orders, and users.

![ERD](/Media/OrderDeliveryERD.png)

### **Logical Schema**
The logical schema outlines the tables, columns, data types, and relationships in our SQL Server database, ensuring data integrity and efficient data retrieval.

![Logical Schema](/Media/OrderDeliverySchema.png)

---

## **Project Timeline**

### **Development Phases**
| Phase | Task | Duration |
|-------|------|----------|
| **Phase 1** | Define Entities & Enums, Initial ERD & Schema Design | 2 Days |
| **Phase 2** | Setup Clean Architecture Project Structure | X Days |
| **Phase 3** | Implement Core Domain Entities & Interfaces | X Weeks |
| **Phase 4** | Database Setup & Initial Migrations (EF Core) | X Weeks |
| **Phase 5** | Implement User Management (Authentication, Authorization) | X Weeks |
| **Phase 6** | Develop Order Management Logic & APIs | X Weeks |
| **Phase 7** | Integrate Location Tracking & Notifications | X Weeks |
| **Phase 8** | Testing, Debugging & Optimization | X Weeks |

**Project Duration**: *June 2025 - August 2025*

---

## **Requirements**

### **Functional Requirements**
- Secure User Authentication (Login, Registration) for all user types (Merchant, Driver, Admin).
- Merchant capabilities to create, view, and manage their orders.
- Driver capabilities to view, accept/reject, and update order delivery status.
- Real-time location updates for drivers during active deliveries.
- Push notifications for critical events (new orders, status changes).
- Rating system for merchants to rate drivers post-delivery.
- Admin functionalities for comprehensive platform oversight.

### **Non-Functional Requirements**
- **Reliability**: High availability for backend services (e.g., 99.9% uptime).
- **Performance**: API response times within acceptable limits (< 300ms for critical operations).
- **Scalability**: Ability to handle increasing load of users and orders.
- **Security**: Robust authentication and authorization, data encryption, protection against common web vulnerabilities.
- **Maintainability**: Clean code, well-defined architecture, comprehensive documentation.

---

## **Key Performance Indicators (KPIs)**
| KPI | Target | Description |
|-----|--------|-------------|
| **API Response Time** | < 300ms | Average response time for core API endpoints. |
| **System Uptime** | > 99.9% | Overall availability of backend services. |
| **Order Assignment Rate** | > 95% | Percentage of orders successfully assigned to a driver. |
| **Delivery Completion Rate** | > 98% | Percentage of assigned orders successfully delivered. |
| **Driver Acceptance Rate** | > 80% | Percentage of new order requests accepted by drivers. |
| **Average Delivery Time** | [Target specific time] | Average time from order creation to delivery. |

---

## **Security Measures**
- **Authentication & Authorization**: Utilizes ASP.NET Core Identity for secure user management and JWT (JSON Web Tokens) for API authorization.
- **Password Hashing**: Passwords stored as cryptographically secure hashes.
- **Input Validation**: Rigorous server-side validation to prevent injection attacks (SQL, XSS).
- **Data Encryption**: Data in transit (SSL/TLS) and data at rest (database encryption, if applicable).
- **Secure Coding Practices**: Adherence to OWASP security guidelines and secure coding standards.

---

## **Testing Strategy**
### **Unit Testing**
- Extensive unit tests for `App.Domain` and `App.Application` business logic.
- Ensures individual components function correctly in isolation.

### **Integration Testing**
- Tests interactions between `App.Application` and `App.Infrastructure` (e.g., database interactions).
- Verifies API endpoint functionality end-to-end (without a frontend).

### **Performance Testing (Future)**
- Load testing to evaluate system behavior under high concurrency.
- Stress testing to determine system breaking points.

---

## **Team Members (Backend - .NET)**
| Team Member | Role | Responsibility |
|-------------|------|----------------|
| **[Wael Bahaa Alfnan](https://github.com/WaelAlfnan)** | Backend Developer | Core API Development, Database Integration, Architecture Design |
| **[Dina Gamal Hawas](https://github.com/Dina-Hawas)** | Backend Developer | Business Logic Implementation, API Endpoints, Testing |

---

## **Project Documentation**
For comprehensive project details including detailed requirements, technical specifications, and API documentation (e.g., Swagger/OpenAPI), refer to:

- [https://localhost:7239/swagger](https://localhost:7239/swagger/index.html)

---

## **Repository Structure**

OrderDelivery/
├── .github/          # GitHub Actions workflows (CI/CD)
├── App.Api/          # API layer, entry point
├── App.Application/  # Application logic, use cases
├── App.Domain/       # Domain entities, enums, interfaces
│   ├── Entities/
│   ├── Enums/
│   ├── Extensions/
│   └── Interfaces/
├── App.Infrastructure/ # Data access, concrete implementations
├── Tests/            # Unit and Integration Tests (optional, but recommended)
├── .gitignore
├── OrderDelivery.sln # Visual Studio Solution file
└── README.md


---

## **Support & Contact**
For questions, issues, or contributions:
- **Issues**: [GitHub Issue Tracker](https://github.com/WaelAlfnan/OrderDelivery/issues/new)
- **Repository**: [OrderDelivery GitHub Repository](https://github.com/WaelAlfnan/OrderDelivery)

---

## **Conclusion**
The OrderDelivery Backend Solution represents a robust and scalable foundation for modern delivery services. Developed with Clean Architecture and .NET 8, it is designed for high performance, security, and future extensibility, aiming to streamline logistics between local businesses and the freelance driver community.
