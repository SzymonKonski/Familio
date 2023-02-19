# Familio
Familio is a MyFamily type system for remote communication between family members (chat), setting schedules and tasks, and location sharing.

# Architecture
The application is divided into four layers: domains, applications, infrastructure and presentation. The project center has a core that consists of a domain layer and application.

## Domain layer
The domain layer contains types for domain facilities. This layer is located in the very center of the entire application, it is not dependent on the other layers or external components. All other layers are indirectly dependent on it. The project that reproduces this layer contains all domain types such as: entities, value objects, enum types.

## Application layer
The application layer contains all the logic of the application. This layer depends on the domain, but there is no dependence to other layers. This layer defines interfaces that are implemented by the outer layers, thanks to which together with the domain layer they form a core that is isolated from external dependencies. In the project, which reflects the layer of application, the CQRS pattern has been implemented, thanks to which every business case of use is represented by a single command or query.

## Infrastructure layer
The infrastructure layer contains services and classes that are responsible for access to external resources such as: services for sending emails, connection with Azure Blob Storage, connection to the cloud database, etc. Services that use these resources implement interfaces defined in the application layer. This layer depends directly on the application layer.

## Presentation layer
The presentation layer contains end points used by client applications. Webapi data is returned in JSON format. The Internet API was written in the Framework ASP.NET Core 6.0. The presentation layer depends on both the application layer and infrastructure, but the dependence on the infrastructure layer is only to ensure the dependecy inversion in the project.

## Database
The server module connects to the MSSQL database, which was launched in the Azure cloud.

## Communication
Communication between the server and client applications takes place in two ways: using the HTTP protocol, which is used to perform most of the actions in the system and using the Signalr library, which is used to real time notifications.

## Authentication
The authentication in the server module has been implemented using the Microsoft.Aspnetcore.Identity library. On the basis of the email address sent by the User and password, an access token and refresh token are generated and then are returned to the client application.

# Tech Stack
* .NET 6.0
* ASP.NET CORE 6.0
* Microcosft SQL Server
* SignalR
* Entity Framework Core 6.0
* AutoMapper
* FluentValidation
* SendGrid
* Azure.Storage.Blobs
* NUnit
* FlutenAssertions

# Tools used:
* Visual Studio 2022
* Visual Studio Code
* Source Tree
* Microsoft SQL Server Management Studio 18

# Design Patterns used
* CQRS
* Mediator

# Tests
* Unit tests
* Integration tests
* Performance tests with NBomber
