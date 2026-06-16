# LogiFlow - TMS Order Extraction Module

A portfolio project demonstrating an AI-powered transport order extraction system.

The module introduces a language-model-driven interpretation layer that extracts structured transport attributes from unstructured email messages and generates validated draft records prior to official system registration

Users upload email files (`.eml` / `.msg`), and the system uses an LLM to automatically extract 8 structured order fields and save them to a cloud database.

---

## Key Features

- **Drag-and-drop upload** of Gmail `.eml` and Outlook `.msg` email files
- **LLM extraction** of 8 structured transport order fields from raw email text
- **Two LLM providers**: OpenRouter (cloud) and Ollama (local) 
- **Editable form** to review and correct extracted data before saving
- **RavenDB Cloud** persistence with server-side filtering across 7 fields
- **Responsive table** with horizontal scroll for Shipment Requests (saved transport data)
- **Configuration page** to switch between models without redeployment

---

## Project Structure

- `OrderModule.Web` - ASP.NET Core MVC web application: controllers, views, models
- `OrderModule.Application` - Business logic: LLM services, extraction, persistence
- `OrderModule.RavenDB` - Database indexes and DocumentStore initialization

-> Full architecture details (layer responsibilities, request flow and solution structure): [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)

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
| `llama` | Lightweight and fast - efficient for structured output extraction, good choice for machines with limited RAM |
| `granite` | IBM's model designed for enterprise text extraction - particularly strong at identifying structured data fields in business documents |

-> More about chosen LLM models: [docs/LLM_MODELS.md](docs/LLM_MODELS.md)

---
