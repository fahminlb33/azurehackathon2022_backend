# Evangelion01 - SNMPTN Profiling Backend [![Build and Deploy](https://github.com/fahminlb33/azurehackathon2022_backend/actions/workflows/master_evangelion01backend.yml/badge.svg)](https://github.com/fahminlb33/azurehackathon2022_backend/actions/workflows/master_evangelion01backend.yml)

> Backend and Data Import Tool

This repository contains an Azure Functions written in C# and data import tool to seed the server database with training data used in the [data repository](https://github.com/fahminlb33/azurehackathon2022_data).

## Running The Backend Locally

To run the backend locally, create a `appsetings.json` with the following keys and then run the project in Visual Studio 2022.

```json
{
  "DatabaseName": "evangelion01",
  "DatabaseConnectionString": "AccountEndpoint=;AccountKey=;",

  "TenantId": "55ab549d-699f-4738-a0f2-b6890e2eb808",
  "ApplicationId": "925790c8-225b-45ef-831f-c3e0a02f6030",
  "PolicyName": "B2C_1_signupsignin",
  "DomainName": "evangelion01msapac.onmicrosoft.com",
  "B2cDomainName": "evangelion01msapac.b2clogin.com",
  "RedirectUri": "http%3A%2F%2Flocalhost%3A7202%2Fapi%2Foauth2-redirect",

  "PredictionUri": "http://xxx.azurecontainer.io/score"
}

```

## Importing Data using DataTool

Get the following files from data repository:

- sn_grades_import.csv
- sn_students_import.csv

Place it in the same directroy with the DataTool executable, also copy the above `appsettings.json` file to this project root. Create two container in the Azure Cosmos DB named `grades` and `students`. Run the project and wait for the data upload to complete.

### Bulk Operations

When importing the students data, this project will not use bulk operation, but to import the grades data, this project will use bulk operation. We discovered this method when we tried to import 26k of records to Azure Cosmos DB using the sequential method and it was very slow. With bulk operation the import time can be reduced to only 3 minutes.
