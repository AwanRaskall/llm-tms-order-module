# LogiFlow - TMS Order Extraction Module

A portfolio project demonstrating an AI-powered transport order extraction system.

The module introduces a language-model-driven interpretation layer that extracts structured transport attributes from unstructured email messages and generates validated draft records prior to official system registration

Users upload email files (`.eml` / `.msg`), and the system uses an LLM to automatically extract 8 structured order fields and save them to a cloud database.

## Key Features

- **Drag-and-drop upload** of Gmail `.eml` and Outlook `.msg` email files
- **LLM extraction** of 8 structured transport order fields from raw email text
- **Two LLM providers**: OpenRouter (cloud) and Ollama (local) 
- **Editable form** to review and correct extracted data before saving
- **RavenDB Cloud** persistence with server-side filtering across 7 fields
- **Responsive table** with horizontal scroll for Shipment Requests (saved transport data)
- **Configuration page** to switch between models without redeployment

---

## Contents
- [UI Overview](#UI-Overview)
- [Tech Stack](#tech-Stack)
- [Project Structure](#project-structure)
- [Requirements](#requirements)
- [Available LLM Models](#available-LLM-Models)
- [Getting Started](#getting-started)
- [License](#license)

---

## UI Overview

### Home Page
![Home page](docs/screenshots/home.png)
![Home page Cards](docs/screenshots/home2.png)

### Order Extractor - upload email file and extract the data
![Order Extractor](docs/screenshots/extractor.png)

### Shipment Requests - saved orders table with filters
![Shipment Requests](docs/screenshots/shipment-requests.png)

### Configuration - select LLM model to run
![Configuration](docs/screenshots/configuration.png)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | C#, ASP.NET Core 8, MVC |
| Frontend | Vue 2 (via CDN), Razor (.cshtml) |
| Database | RavenDB Cloud (free tier) |
| Cloud LLM | OpenRouter API (free tier) |
| Local LLM | Ollama |
| Email parsing | MsgReader |

## Project Structure

- `OrderModule.Web` - ASP.NET Core MVC web application: controllers, views, models
- `OrderModule.Application` - Business logic: LLM services, extraction, persistence
- `OrderModule.RavenDB` - Database indexes and DocumentStore initialization

-> Full architecture details (layer responsibilities, request flow and solution structure): [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)

## Requirements

### OrderModule.Application
| Package | Purpose | Command |
|---|---|---|
| `RavenDB.Client` | RavenDB document store, sessions, queries | `dotnet add package RavenDB.Client --version 7.2.2` |
| `MsgReader` | Parsing `.eml` and `.msg` email files | `dotnet add package MsgReader --version 6.0.11` |
| `Configuration.Abstractions` | `IConfiguration` interface in class library | `dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 10.0.8` |
| `Logging.Abstractions` | `ILogger<T>` interface in class library | `dotnet add package Microsoft.Extensions.Logging.Abstractions --version 10.0.8` |

### OrderModule.RavenDB
| Package | Purpose | Command |
|---|---|---|
| `RavenDB.Client` | DocumentStore initialization and index deployment | `dotnet add package RavenDB.Client --version 7.2.2` |
| `Configuration.Abstractions` | `IConfiguration` interface in class library | `dotnet add package Microsoft.Extensions.Configuration.Abstractions --version 10.0.8` |
| `Configuration.Binder` | Reading config values via `configuration["key"]` | `dotnet add package Microsoft.Extensions.Configuration.Binder --version 10.0.8` |

### OrderModule.Web
| Package | Purpose | Command |
|---|---|---|
| `System.Text.Encoding.CodePages` | Windows-1252 encoding for correct `.msg` parsing | `dotnet add package System.Text.Encoding.CodePages --version 10.0.8` |

- After installing `System.Text.Encoding.CodePages`, register it as the **first line** of `Program.cs`: `Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);`
- `Microsoft.Extensions` packages are included automatically in the Web project via `Microsoft.NET.Sdk.Web` - no installation needed.
- Vue 2, Tabler Icons and Google Fonts are referenced via CDN in `_Layout.cshtml` and load automatically - no installation needed.

---

## Available LLM Models

**OpenRouter (cloud):**

| Model name |  Why chosen |
|---|---|
| `openrouter-free` | Auto-selects best available free model |
| `gpt-oss-120b` | Large 120B model - best at following complex structured JSON instructions and extracting data from unstructured text |
| `gpt-oss-20b` | Smaller 20B model - faster response, good balance of speed and accuracy for straightforward emails |
| `gemma-4-26b-a4b-it` | Instruction-tuned variant - strong at following strict output format rules and keyword extraction |
| `nemotron3` | Nvidia 120B model - excels at reasoning tasks and handling ambiguous or incomplete text |

**Ollama (local):**

| Model name |  Why chosen |
|---|---|
| `mistral` | Well-balanced instruction-following model - reliable JSON output and good at identifying key logistics terms in email text |
| `llama3.2` | Lightweight and fast - efficient for structured output extraction, good choice for machines with limited RAM |
| `granite3.2` | IBM's model designed for enterprise text extraction - particularly strong at identifying structured data fields in business documents |

-> More about chosen LLM models: [docs/LLM_MODELS.md](docs/LLM_MODELS.md)

---

## Getting Started

1. Clone the repository
```bash
git clone https://github.com/your-username/llm-tms-order-module.git
cd llm-tms-order-module
```

2. Install the required packages

Install into their respective projects as listed in the [Requirements](#requirements) section.

3. Configure secrets

Create `OrderModule.Web/appsettings.Development.json` (add folder in `.gitignore`):
```json
{
  "OpenRouter": {
    "ApiKey": "your-openrouter-api-key"
  },
  "Ollama": {
    "BaseUrl": "http://localhost:11434"
  },
  "RavenDB": {
    "Urls": ["https://your-instance.ravendb.cloud"],
    "DatabaseName": "OrderModule",
    "CertPath": "path/to/certificate.pfx",
    "CertPassword": ""
  }
}
```

For a detailed guide on configuring RavenDB, Ollama and OpenRouter see [docs/SETUP.md](docs/SETUP.md)

4. Run the Application
```bash
cd OrderModule.Web
dotnet run
```

Navigate to `https://localhost:*****` (the port shown in the terminal)

---


## Future Improvements

**LLM Self-Validation Cycle** — add a secondary prompt after initial extraction to instruct the model to verify its own output against the JSON schema before returning results, reducing structural errors and missing fields.

**Prompt Engineering** — continue iterating on prompt rules to reduce cases where LLMs ignore specific extraction constraints.

**Email API Integration** — replace manual file upload with direct corporate mail API integration for automatic detection and ingestion of transport-related emails, moving toward event-driven processing.

**Master Data Integration** — connect to a warehouse master database to validate and normalize extracted locations against corporate address records, reducing typographical errors and improving data consistency.

**Extraction Quality Monitoring** — introduce logging of extraction deviations, periodic performance audits and model behavior assessments to ensure long-term reliability across varying operational conditions.

---
## License

This project is distributed under the Apache License 2.0.

- You are free to use, modify, and distribute this code for both personal and commercial purposes;
- Attribution is required: include the original copyright notice and license in any copy or substantial portion of the project - Anastasiia Bakhmutova (2026);
- Modified versions must carry a notice stating that changes were made.

[Full license text](https://github.com/AwanRaskall/llm-tms-order-module/blob/main/LICENSE)