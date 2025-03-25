# MyMvcApp-Contact-Database-Application

This repository contains the source code for the MyMvcApp-Contact-Database-Application, a .NET MVC application for managing contacts. This guide provides detailed instructions for deploying the application using an ARM template and setting up a GitHub Actions pipeline for continuous deployment.

## Prerequisites

- Azure Subscription
- GitHub Account
- .NET SDK 8.0.x
- Azure CLI

## Deployment

### 1. Create an Azure Resource Group

```sh
az group create --name MyResourceGroup --location <location>
```

### 2. Create an Azure Web App and SQL Database using ARM Template

Create an ARM template (`azuredeploy.json`) to define the resources needed for the application.

```json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "contentVersion": "1.0.0.0",
  "resources": [
    {
      "type": "Microsoft.Web/sites",
      "apiVersion": "2021-02-01",
      "name": "MyMvcApp-Contact-Database-Application",
      "location": "[resourceGroup().location]",
      "properties": {
        "serverFarmId": "[resourceId('Microsoft.Web/serverfarms', 'MyAppServicePlan')]"
      }
    },
    {
      "type": "Microsoft.Sql/servers",
      "apiVersion": "2021-02-01",
      "name": "mySqlServer",
      "location": "[resourceGroup().location]",
      "properties": {
        "administratorLogin": "sqladmin",
        "administratorLoginPassword": "P@ssw0rd!"
      },
      "resources": [
        {
          "type": "databases",
          "apiVersion": "2021-02-01",
          "name": "myDatabase",
          "properties": {
            "collation": "SQL_Latin1_General_CP1_CI_AS",
            "maxSizeBytes": "2147483648",
            "sampleName": "AdventureWorksLT"
          }
        }
      ]
    }
  ]
}
```

Deploy the ARM template:

```sh
az deployment group create --resource-group MyResourceGroup --template-file azuredeploy.json
```

### 3. Configure GitHub Secrets

Add the following secrets to your GitHub repository:

- `AZURE_WEBAPP_PUBLISH_PROFILE`: The content of the publish profile file.
- `AZURE_CREDENTIALS`: The JSON output from the Azure CLI command `az ad sp create-for-rbac --name "myApp" --role contributor --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group} --sdk-auth`.

### 4. GitHub Actions Workflow

The GitHub Actions workflow file (`.github/workflows/deploy.yml`) is already configured to build and deploy the application to Azure Web App.

```yaml
name: Deploy to Azure Web App

on:
  push:
    branches:
      - main

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
    - name: Checkout code
      uses: actions/checkout@v2

    - name: Setup .NET Core
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '8.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --configuration Release

    - name: Publish
      run: dotnet publish --configuration Release --output ./pubish

    - name: Upload artifact for deployment job
      uses: actions/upload-artifact@v4
      with:
        name: dotnet-app
        path: ./pubish

    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'MyMvcApp-Contact-Database-Application'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./pubish
        slot-name: production
      env:
        DOTNET_ROOT: /home/runner/.dotnet
```

## Code Documentation

### Controllers

#### UserController

The `UserController` is responsible for handling user-related actions such as listing users, displaying user details, creating, editing, and deleting users.

- **Index**: Returns a view with a list of users.
- **Details**: Returns a view with the details of a specific user.
- **Create**: Displays a form for creating a new user. Handles the form submission to add a new user.
- **Edit**: Displays a form for editing an existing user. Handles the form submission to update the user.
- **Delete**: Displays a confirmation view for deleting a user. Handles the form submission to delete the user.

### Models

#### User

The `User` model represents a user in the application. It contains properties such as `Id`, `Name`, and `Email`.

## Tests

The tests for the application are located in the `MyMvcApp.Tests` project. The tests use the xUnit framework.

### UserControllerTests

The `UserControllerTests` class contains unit tests for the `UserController`.

- **Index_ReturnsViewResult_WithListOfUsers**: Tests that the `Index` action returns a view with a list of users.
- **Details_ReturnsViewResult_WithUser**: Tests that the `Details` action returns a view with the details of a specific user.
- **Details_ReturnsNotFound_WhenUserNotFound**: Tests that the `Details` action returns a `NotFoundResult` when the user is not found.
- **Create_ReturnsViewResult**: Tests that the `Create` action returns a view.
- **Create_Post_AddsUserAndRedirects**: Tests that the `Create` action adds a new user and redirects to the `Index` action.
- **Edit_ReturnsViewResult_WithUser**: Tests that the `Edit` action returns a view with the details of a specific user.
- **Edit_ReturnsNotFound_WhenUserNotFound**: Tests that the `Edit` action returns a `NotFoundResult` when the user is not found.
- **Edit_Post_UpdatesUserAndRedirects**: Tests that the `Edit` action updates an existing user and redirects to the `Index` action.
- **Delete_ReturnsViewResult_WithUser**: Tests that the `Delete` action returns a view with the details of a specific user.
- **Delete_ReturnsNotFound_WhenUserNotFound**: Tests that the `Delete` action returns a `NotFoundResult` when the user is not found.
- **Delete_Post_RemovesUserAndRedirects**: Tests that the `Delete` action removes a user and redirects to the `Index` action.

## Conclusion

By following this guide, you will be able to deploy the MyMvcApp-Contact-Database-Application to Azure using an ARM template and set up a continuous deployment pipeline with GitHub Actions. If you encounter any issues, please refer to the official documentation or raise an issue in this repository.
