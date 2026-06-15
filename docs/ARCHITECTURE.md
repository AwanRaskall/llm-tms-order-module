# Architecture

## Layer Responsibilities

**OrderModule.Web**
Presentation layer. Controllers receive HTTP requests, call Application services, map domain models to ViewModels and return views or JSON responses.

Vue 2 handles frontend interactivity: drag-and-drop upload, loading state, form binding and save notification - without SPA complexity.

**OrderModule.Application**
Business logic layer. Contains all LLM integration, email parsing, data extraction and RavenDB persistence. Has no dependency on the Web layer.

**OrderModule.RavenDB**
Data access layer. Defines the DocumentStore singleton, X.509 certificate authentication and static indexes deployed at startup.

---

## Request Flow - Order Extraction

```
User uploads .eml/.msg file
        ↓
OrderExtractorController.ParseSummary()
        ↓
ExtractTextService.ExtractText()       <- routes by extension
    .eml -> MsgReader MIME parser
    .msg -> MsgReader Outlook parser
        ↓
PromptBuilder.BuildExtractionPrompt()  <- prompt for LLM
        ↓
MessageProcessor.ProcessMessageAsync()
        ↓
    ConfigurationReadService           <- reads selected model from RavenDB
        ↓
    switch(modelName)
        ├── OpenRouter models -> HttpServiceOpenRouter -> openrouter.ai/api/v1
        └── Ollama models -> HttpServiceOllama -> localhost:11434
        ↓
Normalizer.ExtractJson()               <- strips markdown/think tags from LLM output
        ↓
JsonSerializer.Deserialize<ExtractedSummary>()
        ↓
Normalizer.ConvertDate()               <- normalizes dates to yyyy-MM-dd
        ↓
ViewModelSummary returned as JSON      <- Vue updates editable form
```

---

## Solution Structure

```
OrderModule.sln
├── OrderModule.Web
│   ├── Controllers
│   │   ├── HomeController.cs
│   │   ├── OrderExtractorController.cs
│   │   └── ConfigurationController.cs
│   ├── Views
│   │   ├── Home/Index.cshtml
│   │   ├── Configuration/Index.cshtml
│   │   └── OrderExtractor
│   │       ├── Index.cshtml
│   │       └── ShipmentRequests.cshtml
│   ├── Models
│   │   ├── OrderExtractorViewModels.cs
│   │   ├── ShipmentRequestViewModels.cs
│   │   └── ConfigViewModel.cs
│   └── Program.cs
│
├── OrderModule.Application
│   ├── Features
│   │   ├── OrderExtractorService
│   │   │   ├── MessageProcessor.cs
│   │   │   ├── Models/ExtractedSummary.cs
│   │   │   └── Utils
│   │   │       ├── Normalizer.cs
│   │   │       └── PromptBuilder.cs
│   │   ├── ShipmentRequests
│   │   │   ├── Models/ShipmentRequest.cs
│   │   │   ├── ShipmentRequestService.cs
│   │   │   └── ShipmentRequestReadService.cs
│   │   └── Configuration
│   │       ├── Models/ConfigurationModel.cs
│   │       ├── ConfigurationService.cs
│   │       └── ConfigurationReadService.cs
│   ├── DomainServices/EmailExtraction
│   │   ├── Services
│   │   │   ├── ExtractTextService.cs
│   │   │   ├── OllamaService.cs
│   │   │   └── HttpServiceOllama.cs
│   │   └── Models/OllamaResponse.cs
│   ├── ExternalServices/OpenRouter
│   │   ├── OpenRouterService.cs
│   │   ├── HttpServiceOpenRouter.cs
│   │   ├── OpenRouterRequest.cs
│   │   └── OpenRouterResponse.cs
│   └── Interfaces
│       ├── ILLMService.cs
│       ├── IHttpService.cs
│       └── IModelResponse.cs
│
└── OrderModule.RavenDB
    ├── Connection/RavenDbStore.cs
    └── Indexes/ShipmentRequests_ByFilters.cs
```
