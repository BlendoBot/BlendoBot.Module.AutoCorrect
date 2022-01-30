# BlendoBot.Module.AutoCorrect
## Performs autocorrect on a message.
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/BlendoBot/BlendoBot.Module.AutoCorrect/Tests)

The currency converter command allows users to get a live conversion rate between two or more currencies.

## Discord Usage
- `?ac [message]`
  - Corrects typos in the given `message`.

## Config
This module optionally can use a [GrammarBot](https://www.grammarbot.io/) API key. The key should be in the config as:
```cfg
[AutoCorrect]
ApiKey=YOUR_API_KEY
```