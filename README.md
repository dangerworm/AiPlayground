# Quick Start Guide

## LLMs

The application is set up to use Azure OpenAI. As I'm not sharing my key,
this will require you to deploy a model of your own. Once that's done, fill out
the user secrets (`secrets.json`) in the following format:

```
{
  "AzureOpenAiDeploymentKey": "",
  "AzureOpenAiDeploymentName": "",
  "AzureOpenAiEndpoint": "",
  "AzureOpenAiHost": "",
  "AzureOpenAiModel": ""
}
```

### vLLM

The default setup is to run mistral model `mistralai/Mistral-7B-Instruct-v0.3`.
If you're happy with this, simply run the following command in the root folder:

`docker compose up -d`

If, however, you want to run your own choices of model:

- Choose the model(s) you wish to run from [Hugging Face](huggingface.co).
- Add your hub token to `.env` (e.g. `HUGGING_FACE_HUB_TOKEN=hf_...`)
- Add the model name(s) to the `models.txt` file in `./llm`
- Choose either the Powershell or Bash script and run it:
  - `./llm/run_models.ps1` 
  - `./llm/run_models.sh` 

### Ollama

- Download and install [Ollama](https://ollama.com/)
- Choose the model(s) you wish to run from [the library](https://ollama.com/library).
- Open a terminal
- For each model you've chosen, run the following:
  - `ollama pull <model>`
  - `ollama run <model>`

## API

If you have Visual Studio, open the solution file at
`./api/AiPlayground.Api.sln` and run it.

Otherwise, make sure the dotnet runtime for .NET 9.0 is installed.
Then build and run the solution:

`dotnet run --project ./api/AiPlayground.Api/AiPlayground.Api.csproj`

## UI
Download and install Node.js. Make sure both `node -v` and `npm -v`
return version numbers on the command line.

- Change directory into `./ui`
- Open a terminal
- Type `npm run dev`