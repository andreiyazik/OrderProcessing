# Order Processing API

A proof-of-concept .NET 9 Web API for managing and processing customer orders with inventory verification and status transitions.

---

## 1. General API Information

The API allows clients to:
- Place new orders (with one or more items)
- Confirm orders after inventory check
- Retrieve order details
- Automatically expire stale inventory reservations via background processing
- Utilize in-memory caching for frequent lookups

---

## 2. Setup Instructions

### 2.a. Apply Migrations

1. Set the database connection string as an environment variable:

```powershell
$Env:ConnectionStrings__OrdersDb="Server=.;Database=Orders;Integrated Security=SSPI;TrustServerCertificate=True;"
```

2. Apply EF Core migrations:

```bash
dotnet ef database update --project OrderProcessing.Infrastructure
```

> Ensure the connection string is available to the infrastructure project.

### 2.b. Use Scalar to Test API

1. Run the application:
```bash
dotnet run --project OrderProcessing.API
```

2. Navigate to the Scalar UI in your browser:
```
https://localhost:<port>/docs
```

> Scalar provides a clean UI for testing endpoints and reviewing OpenAPI specs.

---

## 3. Explanation of Key Design Choices

- **Domain-Driven Design (DDD)**: Encapsulates business logic inside aggregates.
- **MediatR**: Decouples command/query handling from controllers.
- **AutoMapper**: Simplifies model mapping.
- **Entity Framework Core**: Used for data persistence.
- **Strategy Pattern**: Handles different order update behaviors.
- **Background Services**: Periodically clean up expired inventory reservations.
- **MemoryCache**: Optimizes performance for frequent GET calls.

---

## 4. Hosting & Deployment Design

### 4.a. Hosting Approaches

#### Docker (Containerized)

- **Pros**: Portability, easy scaling, environment consistency, fast CI/CD.
- **Cons**: Requires orchestration tools like Kubernetes for complex deployments.

#### Kubernetes (K8s)

- **Pros**: Excellent for large-scale deployments, built-in auto-scaling and self-healing, service discovery.
- **Cons**: Steeper learning curve, more infrastructure overhead.

#### Virtual Machines (VMs)

- **Pros**: Full control over environment, familiar to traditional ops teams.
- **Cons**: Manual scaling, more overhead for setup, slower deployments.

#### Serverless (e.g., Azure Functions)

- **Pros**: Cost-efficient for low/variable workloads, auto-scaled by provider.
- **Cons**: Limited execution time, state must be externalized, cold-start latency.

### 4.b. Reliability and Scalability

#### 4.c.1. Handling Traffic Spikes

- Use **horizontal scaling** via Kubernetes or Azure App Service Plans.
- Apply **caching** for common GET operations.
- Queue background tasks (e.g., order confirmations) via **Azure Service Bus** or **RabbitMQ**.

#### 4.c.2. Fault Tolerance

- Retry logic in HTTP clients and database commands.
- Use **circuit breakers** with libraries like Polly.
- Health checks for containers (`/health` endpoints) integrated with load balancers.

#### 4.c.3. Scaling Techniques

- **Horizontal Scaling**: Add more containers or pods behind a load balancer. Works well for stateless services like APIs.
- **Vertical Scaling**: Increase resources (CPU/RAM) on nodes or VMs. Limited by hardware caps and not as resilient as horizontal.

---

## 5. Security Considerations

### 5.a. Potential Security Risks

- Unauthenticated access to sensitive endpoints.
- Insecure transport (HTTP instead of HTTPS).
- Data exposure via error messages.
- Unprotected background processing or admin interfaces.

### 5.b. Authentication & Authorization Approaches

#### Role-Based Access Control (RBAC)

- Assigns roles (e.g., Admin, User, Manager) to users.
- Simple to implement with policies or attributes.
- **Pros**: Easy to manage, widely supported.
- **Cons**: Lacks flexibility, can become hard to scale with large role sets.

#### Attribute-Based Access Control (ABAC)

- Grants access based on attributes (e.g., department, order owner, status).
- More dynamic and fine-grained than RBAC.
- **Pros**: High flexibility, context-aware.
- **Cons**: More complex to implement and maintain. Requires clear attribute strategy.

##### Proposed Approach

Use **RBAC** for general role enforcement and **ABAC** for fine-grained control at the domain level (e.g., only allow access to orders created by the requesting user).

### 5.c. Data Protection & Secure Communication

- Enforce HTTPS using middleware.
- Store sensitive settings (like connection strings or service keys) in secure configuration stores (e.g., Azure Key Vault).
- Use TLS 1.2+ and database encryption (TDE or column-level).

---

## 6. How to Run Tests

### Integration Test Setup with Docker

1. Pull and run SQL Server using Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Your_password123" -p 14333:1433 --name sqlserver_test -d mcr.microsoft.com/mssql/server:2022-latest
```

2. Use this connection string in your test environment:

```json
Server=localhost,14333;Database=OrderTestDb;User=sa;Password=Your_password123;TrustServerCertificate=True;
```

3. Run integration tests:

```bash
dotnet test OrderProcessing.IntegrationTests
```

> Integration tests will apply migrations, seed data, and interact with the real SQL Server running in Docker.

---

## License

MIT