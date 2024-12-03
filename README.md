# Restaurant Management API

A robust Restaurant Management System built with **.NET Core** following **DDD (Domain-Driven Design)** principles. This API handles various restaurant operations such as order management, ticketing, menu management, and customer feedback. It integrates **JWT Authentication**, **Entity Framework (EF)**, and **RabbitMQ** for messaging and ticket status updates.

## Features

- **Order Management**: Customers can view the menu, place orders, and track their order statuses.
- **Ticketing System**: Automatically generates tickets when an order is placed. If a ticket is delayed, the system changes the ticket's status using RabbitMQ.
- **Delivery Service**: Customers can choose delivery options with estimated delivery times.
- **Role-based Authorization**: Secure access to specific routes based on user roles, including customers, chefs, and managers.
- **RabbitMQ Integration**: Used for monitoring ticket delays and notifying the system to update ticket status.
- **ACID-Compliant Queries**: All database operations are designed to follow ACID principles ensuring data integrity.
- **DDD Principles**: The project follows Domain-Driven Design (DDD) with services and repositories to manage business logic and persistence.
- **Fully Unit Tested Services**: All service layer methods are fully unit tested to ensure correctness and reliability.

## Technologies Used

- **.NET Core**: Framework used for building the API.
- **Entity Framework**: ORM for database operations, managing models, and queries.
- **JWT Authentication**: For secure user authentication and authorization.
- **RabbitMQ**: Message broker used to manage ticket status updates based on delays.
- **SQL Server**: Used as the database for storing data related to orders, tickets, menu items, and user information.
- **Swagger**: API documentation for testing and exploration.

## Installation

Follow these steps to set up the project locally:

### 1. Clone the repository

```bash
git clone https://github.com/mojtabafarzaneh/RestaurantManagement.git
```
