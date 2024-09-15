from rcon.source import Client
import discord
from discord import app_commands
import json
import os

def load_config():
    if os.path.exists("config.json"):
        with open("config.json", 'r') as f:
            return json.load(f)
    else:
        return {}
    
config = load_config()

BOT_TOKEN = config.get('token')
ip = config.get('ip')
port = config.get('port')
password = config.get('password')
channel_id = config.get('channel_id')

class aclient(discord.Client):
    def __init__(self):
        super().__init__(intents = discord.Intents.all())

        self.synced = False 
        

    async def on_ready(self):
        await self.wait_until_ready()
        if not self.synced: 
            await tree.sync() 
            self.synced = True
        await self.change_presence(activity=discord.Game(name="Made by mintyx"))

        print(f"Discord2CS BOT Loaded as {self.user}.")

    async def on_message(self, message):
        author_name = message.author.global_name
        chnl_id = message.channel.id
        msg_content = message.content
        
        if chnl_id == channel_id:
            try:
                sendMessage(author_name, msg_content)
            except Exception as e:
                print("An error occured!")
                print(e)

client = aclient()
tree = app_commands.CommandTree(client)

    
def sendMessage(name, message):

    with Client(ip, port, passwd=password) as client:
        msg = "{DarkBlue}" + name + ": {Default}" + message
        response = client.run(f"print {msg}")
        print(response)

                
client.run(BOT_TOKEN)

