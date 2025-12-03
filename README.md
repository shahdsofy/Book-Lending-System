# 📚 Book Lending System API

A RESTful API for a **Book Lending System** where users can register, log in, and borrow or return books from a shared catalog.  
This project is built using **Clean Architecture**, **CQRS**, and **MediatR** with modern **ASP.NET Core** technologies.

---

## 🚀 Technologies & Tools

- ✅ ASP.NET Core Web API
- ✅ Clean Architecture
- ✅ CQRS (Command Query Responsibility Segregation)
- ✅ MediatR
- ✅ Fluent Validation
- ✅ Entity Framework Core
- ✅ SQL Server
- ✅ ASP.NET Core Identity
- ✅ AutoMapper
- ✅ Hangfire (Background Jobs)
- ✅ Swagger (API Documentation)
- ✅ XUnit & NSubstitute (Unit Testing)
- ✅ In-Memory Database for Testing

---

## 🏗️ Project Architecture

The project follows **Clean Architecture** combined with **CQRS & MediatR**, divided into the following layers:

- **Domain** → Core business entities & interfaces  
- **Application** → Commands, Queries, Handlers, DTOs & Business Logic  
- **Infrastructure** → Database, Identity, Repositories, Hangfire  
- **API** → Controllers & Endpoints  
- **Tests** → Unit Tests using XUnit & NSubstitute  

---

## 🔄 CQRS & MediatR Implementation

- ✅ Commands handle:
  - Create, Update, Delete operations
- ✅ Queries handle:
  - Read-only operations
- ✅ All requests pass through **MediatR**
- ✅ Controllers do **not** directly call services
- ✅ Improves:
  - Separation of concerns
  - Scalability
  - Testability
  - Maintainability

---

## 🔐 Core Features

### 1️⃣ User Authentication
- Register & Login using **ASP.NET Core Identity**
- Role-based authorization:
  - **Admin**
  - **Member**

---

### 2️⃣ Book Catalog Management
- ✅ Admin can:
  - Add books
  - Update books
  - Delete books
- ✅ Members can:
  - View available books
  - Search books

---

### 3️⃣ Book Borrowing System
- ✅ Members can:
  - Borrow **one book at a time**
  - Return borrowed books
- ✅ Due Date:
  - Automatically set to **7 days** after borrowing
- ✅ Book availability updates automatically

---

### 4️⃣ Hangfire Background Job
- ✅ Runs **daily**
- ✅ Checks overdue borrowed books
- ✅ Sends reminders (via logs or simulated email)

---

### 5️⃣ Swagger API Documentation
- ✅ Auto-generated documentation
- ✅ Full API testing via browser



### 📌 Project Highlights

✅ Clean Code & SOLID Principles

✅ CQRS & MediatR Pattern Implementation

✅ Secure authentication & authorization

✅ Background jobs with Hangfire

✅ Fully documented APIs via Swagger

✅ Strong test coverage using Unit Tests

✅ Scalable & Maintainable architecture

---------------------------------------------------------------








