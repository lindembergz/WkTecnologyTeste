


# WkTecnology.Portifolio

<div class="badge-base LI-profile-badge" data-locale="pt_BR" data-size="medium" data-theme="light" data-type="VERTICAL" data-vanity="lindemberg-cortez-a6ba42195" data-version="v1"><a class="badge-base__link LI-simple-link" href="https://br.linkedin.com/in/lindemberg-cortez-a6ba42195?trk=profile-badge">Lindemberg Cortez</a></div>

## Descrição

**WkTecnology.Portifolio** é um sistema de gerenciamento de produtos de veículos de alta performance, desenvolvido em **.NET 8**, com arquitetura moderna, integração com banco de dados **MySQL**, exposto via **API RESTfull**.  
O projeto é preparado para ser **resiliente** , **alta escalabilidade**, **validação robusta** e **integração com frontend Angular**.

---

## Arquitetura

- **WebAPI**: Camada de apresentação, responsável por expor os endpoints REST.
- **Aplicação**: Contém regras de negócio, serviços, DTOs e validadores (FluentValidation).
- **Domínio**: Entidades, interfaces de repositórios e lógica de domínio.
- **Infraestrutura**: Implementação dos repositórios, contexto do Entity Framework Core e integrações externas.
- **Core**: Objetos de valor, utilitários e contratos comuns.

- ![image](https://github.com/user-attachments/assets/3330cee1-e451-4b9d-9ce2-15065361d1d0)


### Principais Tecnologias

- .NET 8  
- Entity Framework Core (MySQL)  
- Redis (StackExchange.Redis)  
- FluentValidation  
- Swagger / OpenAPI  
- Docker (para bancos e cache)
- Angular + PrimeNG

---

## Requisitos

- .NET 8 SDK  
-  MySql 8+ 
- Node.js 15+ e Angular CLI (opcional, para rodar o frontend)


---

## Preparação do Ambiente FrontEnd

### Execução do projeto Angular na camada Presentation prosente na solução ( WkTecnology.Portifolio )

no terminal execute:

npm install

npm run start ou ng Server

---

## Preparação do Ambiente BackEnd

## 1. MySQL 

```bash

Se o MySQL não estiver instalado pode subir via docker.

(Requerido) docker run --name mysql8 -e MYSQL_ROOT_PASSWORD=RootPassword123 -e MYSQL_DATABASE=vehiclesalesdb -p 3306:3306 -d mysql:8.0
(Opcional) docker run --name redis6 -p 6379:6379 -d redis:6.0

Obs: caching com redis está funcional, mas desabilitado.

## Script tabelas ou execute a **Migration** presente na solução ( Portifolio.Infraestrutura )

CREATE TABLE `Products` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `Description` varchar(1000) NOT NULL,
  `Brand` varchar(100) NOT NULL,
  `Model` varchar(100) NOT NULL,
  `Year` int NOT NULL,
  `Color` varchar(50) NOT NULL,
  `Mileage` int NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `CategoryId` int NOT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Products_Brand_Model` (`Brand`,`Model`),
  KEY `IX_Products_CategoryId` (`CategoryId`),
  KEY `IX_Products_CreatedAt` (`CreatedAt`),
  KEY `IX_Products_IsActive` (`IsActive`),
  KEY `IX_Products_Name` (`Name`),
  CONSTRAINT `FK_Products_Categories_CategoryId` FOREIGN KEY (`CategoryId`) REFERENCES `Categories` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


CREATE TABLE `Categories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `Description` varchar(500) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT '1',
  `ParentCategoryId` int DEFAULT NULL,
  `CreatedAt` datetime(6) NOT NULL,
  `UpdatedAt` datetime(6) DEFAULT NULL,
  `IsDeleted` tinyint(1) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `IX_Categories_Name` (`Name`),
  KEY `IX_Categories_IsActive` (`IsActive`),
  KEY `IX_Categories_ParentCategoryId` (`ParentCategoryId`),
  CONSTRAINT `FK_Categories_Categories_ParentCategoryId` FOREIGN KEY (`ParentCategoryId`) REFERENCES `Categories` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


### 2. Configure o appsettings.json

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=vehiclesalesdb;User=root;Password=RootPassword123;",
  "Redis": "localhost:6379"
}


### 3. Restaure os pacotes e compile a solução

dotnet restore
dotnet build

###Execução
## 1. Execute as migrações

A aplicação executa as migrações automaticamente ao iniciar.

## 2. Inicie a API

dotnet run --project WkTecnology.WebAPI/WkTecnology.WebAPI.csproj

## 4 Acesse a documentação Swagger

Abra no navegador:

https://localhost:5284


linkedIn : https://www.linkedin.com/in/lindemberg-cortez-a6ba42195/
Email: lindemberg.cortez@gmail.com


              
