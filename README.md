# IRIS DCT Online Payments

## Overview
IRIS DCT Online Payments is a C# project designed to facilitate online transactions through the DIAS API. The project includes functionalities for creating RF payment codes, verifying payment details, displaying payment history, and processing payments. It also provides Swagger documentation for easy API access.

## Features
1. **Create RF Payment Codes**: Generate RF payment codes based on order amount, customer ID, and order ID.
2. **Verify Payment Details**: Check if the RF code and amount are correct.
3. **Paument Codes Information**: Information about the RF payment codes.
4. **Payment History**: Display the history of RF payments.
5. **Process Payments**: Accept or reject payments through the DIAS API.
6. **Swagger Documentation**: Access the API documentation through Swagger.

## Getting Started

### Prerequisites
- .NET 9
- DIAS API credentials

### Installation
1. Clone the repository:
    ```bash
    git clone https://github.com/nmethenitis/Iris_Payments.git
    ```
2. Navigate to the project directory:
    ```bash
    cd Iris_Payments
    ```
3. Restore the dependencies:
    ```bash
    dotnet restore
    ```

### Usage
1. **Settings**
    - Update the application settings with:
      1. API Key
      2. SMTP settings in order to get information emails about the payments
      3. DIAS username and password in order to accept the payments
      4. DIAS organization number
2. **Accessing Swagger Documentation**:
    - Start the application:
        ```bash
        dotnet run
        ```
    - Open your browser and navigate to `http://localhost:5000/swagger` to view the API documentation.

### API Endpoints
1. **Create RF Payment Code**:
    - **Endpoint**: `POST /api/v1/PaymentCode/Create`
    - **Request Body**:
        ```json
        {
          "amount": "<double>",
          "customerId": "<string>",
          "orderId": "<string>"
        }
        ```
2. **Check RF Payment Code**:
    - **Endpoint**: `POST /api/v1/PaymentCode/Check  `
    - **Request Body**:
        ```json
        {
          "amount": "<double>",
          "paymentCode": "<string>"
        }
        ```
3. **Information about RF Payment Code**:
    - **Endpoint**: `GET /api/v1/PaymentCode/Info/{code}  `
4. **Payment history about   RF Payment Code**:
    - **Endpoint**: `GET /api/v1/PaymentCode/History/{code}  `
5. **DIAS Payment Request**:
    - **Endpoint**: `POST /api/v1/PaymentCode/Request  `

## Contributing
Contributions are welcome! Please open an issue or submit a pull request.

## License
This project is licensed under the MIT License.

## Contact
For any inquiries, please contact nmethenitis@gmail.com.

