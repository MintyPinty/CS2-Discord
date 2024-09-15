

# CS2-Discord


## Description
- Links a Discord Channel with CS2 In Game Chat
- !! WARNING !! THIS PLUGIN WILL HAVE TROUBLE WORKING IF THE SERVER HAS CUSTOM TAGS SUCH AS [CS2-TAGS](https://github.com/daffyyyy/CS2-Tags)

## Requirments
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/) **tested on v265**
- [Python](https://www.python.org/downloads/)

## Installation
### CS2 Plugin
 - Download the newest release from [Releases](https://github.com/MintyPinty/CS2-Discord/releases)
 - Make a folder in /plugins named /CS2-Discord.
 - Put the plugin files in to the new folder.
 - Restart your server.
### Discord Bot
 - Download the latest [Python Version](https://www.python.org/downloads/)
 - Open install.bat
 - Make a bot from the [Discord Developer Portal](https://discord.com/developers/applications)
 - Enable Message Content Intents (Check image below)
![image](https://github.com/user-attachments/assets/0baf1533-963e-46ab-bc8f-18adfe498f62)
 - Edit config.json
 - Open main.py


## Configuration
### CS2 Plugin
```json
{
  "prefix": "{Default}[{DarkBlue}DISCORD{Default}]", // Prefix in CS2 Game Chat
  "webhook_url": "https://discord.com/api/webhooks/", // Webhook URL
  "ConfigVersion": 1 // DO NOT CHANGE
}
```
### Discord Bot
```json
{
    "token": "", // Discord Bot Token
    "ip": "", // CS2 Server IP Address
    "port": 27015, // CS2 Server Port
    "password": "", // CS2 Server RCON Password
    "channel_id": 0 // Discord Channel ID
}
```

## Images
![image](https://github.com/user-attachments/assets/ebf8277e-daf4-4b83-b148-2c38b4a2df46)

![image](https://github.com/user-attachments/assets/6b355e93-50e2-4d1b-9088-55a7b89a7245)



## Credits
 - [cs2-Chat-Logger-GoldKingZ](https://github.com/oqyh/cs2-Chat-Logger-GoldKingZ/tree/main)
