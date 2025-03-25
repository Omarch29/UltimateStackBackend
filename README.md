# REPM - Real Estate Property Manager 🏡

**REPM** (Real Estate Property Manager) is a backend API built for managing real estate properties, leases, users, and related operations. This project is designed not only to serve as a solid foundation for real-world property management platforms, but also as a technical showcase and learning tool for advanced software architecture patterns. 🌟

## 🧠 Overview of the API

The REPM API provides a comprehensive solution for managing real estate assets, facilitating interactions between users, properties, and leases. It utilizes modern technologies to ensure efficient data handling and robust architecture, making it suitable for both development and production environments. 🚀

## 🔧 Technologies and Patterns Used

This project is structured around modern .NET backend principles and architecture patterns:

- **.NET 9** 🖥️
- **GraphQL** using HotChocolate 🍫
- **Domain-Driven Design (DDD)** to model complex real-world behaviors 🌍
- **Clean Architecture** to keep concerns separated and maintainable 🧩
- **CQRS (Command Query Responsibility Segregation)** to separate read/write operations 📊
- **Mediator Pattern** via MediatR for decoupling command and query handling 🔄
- **Repository Pattern** for abstracting data access 📂
- **Unit of Work Pattern** to manage transactional consistency ⚖️

## 📦 Project Structure

This solution is organized into several projects to enforce clear separation of concerns. Each project contains its own `README.md` with more detailed information:

- `REPM.Domain` — Entities, Value Objects, Domain Events, and Exceptions 📜
- `REPM.Application` — Commands, Queries, DTOs, and Interfaces 📁
- `REPM.Infrastructure` — Repositories, DbContext, and external integrations 🔌
- `REPM.API` — GraphQL setup and API entry point 🌐

## 🎯 Goal of This Project

The main goal of REPM is to put into practice advanced architectural patterns and serve as a **template** or **starting point** for robust backend systems. It aims to balance maintainability, scalability, and domain expressiveness while embracing .NET's powerful ecosystem. 💪

## 🚀 Basic Use Cases

1️⃣ A property owner lists a property for rent.  
2️⃣ A renter requests to lease a property.  
3️⃣ The lease is created with a valid start & end date.  
4️⃣ The renter makes a payment.  
5️⃣ The system ensures no overdue payments.  
6️⃣ An owner unlists a property if it’s no longer available.  

---

> For implementation details and logic behind each layer, check the `README.md` inside each project folder. The detailed logic will be explained in each individual project's README. 📚
