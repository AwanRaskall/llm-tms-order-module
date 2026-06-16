# LLM Models Reference

## How Model Selection Works

The user selects a model on the **Configuration** page. The selection is saved to RavenDB as a `ConfigurationModel` document. On each extraction, `MessageProcessor` reads the saved model name and routes the request to the appropriate service.

```
Configuration page
      ↓
ConfigurationModel in RavenDB
      ↓
MessageProcessor.ProcessMessageAsync()
      switch(modelName)
      ├── OpenRouter models -> HttpServiceOpenRouter
      └── Ollama models -> HttpServiceOllama
```

---

## OpenRouter Models (Cloud service)

Requires an API key from [openrouter.ai](https://openrouter.ai).
Add the key to your secret configuration file `appsettings.Development.json`:

```json
"OpenRouter": {
    "ApiKey": "your-api-key-here"
}
```

Free tier has daily request limits. Note that free models available on
OpenRouter change frequently - new models are added and old ones may be
removed. The models below were selected during development of this project
based on availability and extraction quality at that time.

| Config key | Model identifier |
|---|---|
| `openrouter-free` | `openrouter/free` |
| `gpt-oss-120b` | `openai/gpt-oss-120b:free` |
| `gpt-oss-20b` | `openai/gpt-oss-20b:free` | 
| `gemma-4-26b-a4b-it` | `google/gemma-4-26b-a4b-it:free` |
| `nemotron3` | `nvidia/nemotron-3-super-120b-a12b:free` | 

---

## Ollama Models (Local service)

Runs entirely on your machine. No API key, no request limits, works offline.

To use Ollama models you need to:
1. Download and install Ollama from [ollama.com](https://ollama.com)
2. Pull the models you want to use:
```bash
ollama pull mistral:7b
ollama pull llama3.2:3b
ollama pull granite3.2:2b
```
3. Add the Ollama base URL to `appsettings.Development.json`


| Config key | Model identifier | RAM required |
|---|---|---|
| `mistral` | `mistral:7b` | ~4 GB |
| `llama` | `llama3.2:3b` | ~2 GB |
| `granite` | `granite3.2:2b` | ~2 GB |

---

## API Parameters

Both LLM providers accept parameters for controlling model behaviour,
but with different formats.


**For OpenRouter** parameters are sent at the top level of the request body:

```json
{
  "model": "openrouter/free",
  "temperature": 0.5,
  "max_tokens": 500,
  "messages": [
    { "role": "system", "content": "Return ONLY valid JSON." },
    { "role": "user",   "content": "..." }
  ]
}
```

**Ollama** `/api/generate` requires `temperature` and token limit nested
inside an `options` object:

```json
{
  "model": "mistral:7b",
  "prompt": "...",
  "stream": false,
  "options": {
    "temperature": 0.5,
    "num_predict": 500
  }
}
```

---

## How to Add a New Model

### 1. Add a new case in `MessageProcessor.cs`

```csharp
case "my-new-model":
    model = "provider/model-name:free";  // OpenRouter
    return await _openRouterService.ExtractDataFromText(model, prompt);

// OR for Ollama:
case "my-local-model":
    model = "modelname:tag";
    return await _ollamaService.ExtractDataFromText(model, prompt);
```
If adding an Ollama model, don't forget to pull it on your device first.

### 2. Add the model to Configuration page

In `Views/Configuration/Index.cshtml`, add a new radio button option
in the appropriate service section (OpenRouter or Ollama).

### 3. No other changes needed

The selected model key is stored in RavenDB and read by `MessageProcessor`
at extraction time. No redeployment required after adding the model.

---

## Extraction Prompt Design

The prompt in `PromptBuilder.cs` instructs the LLM to:

1. Act as a logistics expert
2. Return **only** a valid JSON object with exactly 8 keys
3. Follow strict field extraction rules - no hallucination, no guessing
4. Leave fields empty if the information is not found
5. Move ambiguous or vague location data (regions, directions) to `Notes`
6. Never use transport mode words (land, road, rail) as `Transport` value
7. Validate output before responding

Key validation rules enforced in the prompt:
- `Transport` must be a concrete vehicle type (truck, van, ship) - not a mode
- `DepPoint` / `ArrPoint` must be specific locations - not regions or directions
- Dates can be inferred from context but never invented
