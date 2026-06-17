# Setup Guide

## RavenDB Cloud

1. Create a free account at [cloud.ravendb.net](https://cloud.ravendb.net)
2. Create a new cloud node (free tier: 1 node, 1 GB)
3. Download the `.pfx` client certificate
4. Place the certificate file in a `certificate/` folder at the solution root (add folder in `.gitignore`)
5. Create a new database named "OrderModule"

---

## OpenRouter API Key

1. Create a free account at [openrouter.ai](https://openrouter.ai)
2. Go to **Keys** and create a new API key
3. Free tier includes access to multiple models. Choose `input modalities` - text, `Prompt pricing` - free

---

## Ollama

1. Download and install from [ollama.com](https://ollama.com)
2. Ollama runs as a background service on `http://localhost:11434`
3. Pull the models you want to use:

```bash
ollama pull mistral:7b
ollama pull llama3.2:3b
ollama pull granite3.2:2b
```

4. Verify installation:
```bash
ollama list
```

5. Test that the server is running - open in browser:
```
http://localhost:11434
```
Should respond: "Ollama is running"

---

## Configuration File

Create `OrderModule.Web/appsettings.Development.json` (add folder in `.gitignore`):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "OpenRouter": {
    "ApiKey": "sk-or-v1-your-key-here"
  },
  "Ollama": {
    "BaseUrl": "http://localhost:11434"
  },
  "RavenDB": {
    "Urls": ["https://your-instance.ravendb.cloud"],
    "DatabaseName": "OrderModule",
    "CertPath": "..\\certificate\\your-certificate.pfx",
    "CertPassword": ""
  }
}
```