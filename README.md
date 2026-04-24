==========================================================
PROJECT: SwiftProcessor (MT103 Message Handler)
==========================================================

DESCRIPTION:
A high-performance .NET 10 Web API designed to automate the 
processing and storage of SWIFT MT103 financial messages.

CORE FEATURES:
1. File Upload: REST API endpoint accepting .txt SWIFT files.
2. Custom Parsing: Data extraction using Regular Expressions (Regex) 
   without external financial libraries (manual parsing logic).
3. Detailed Extraction:
   - Transaction: Reference (:20:), Value Date, Currency, Amount (:32A:).
   - Ordering Customer (Sender): IBAN and Name/Address (:50K:).
   - Beneficiary (Receiver): IBAN and Name/Address (:59:).
   - Bank Details: Receiver Bank BIC (:57A:).
   - Remittance & Charges: Purpose of Payment (:70:) and Charge details (:71A:).
4. Data Persistence: Automatic storage in a local SQLite database.
5. Logging: Full process traceability using NLog (Console & File).

TECH STACK:
- Language: C# / .NET 10
- Database: SQLite (via Microsoft.Data.Sqlite)
- Logging: NLog
- API Documentation: Swagger / OpenAPI

HOW TO RUN:
1. Open 'SwiftProcessor.sln' in Visual Studio 2022.
2. Press 'F5' to start the application.
3. The Swagger UI will automatically launch at:
   https://localhost:[PORT]/swagger/index.html
4. Use the POST /api/Swift/upload method to upload a sample file (e.g., EXAMPLE.txt).
5. The processed data is returned in JSON format and saved to 'swift_messages.db'.

LOGS & DATABASE:
- Database location: Root folder (swift_messages.db)
- Log file location: bin/Debug/net10.0/logs/
==========================================================
