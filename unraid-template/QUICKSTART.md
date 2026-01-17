# PlexBot unRAID Templates - Quick Reference

## 📦 What's Included

This directory contains everything needed to run PlexBot on unRAID:

- **plexbot.xml** - Main PlexBot container template
- **plexbot-lavalink.xml** - Companion Lavalink audio server template
- **lavalink.application.yml** - Optional Lavalink configuration file
- **README.md** - Detailed installation and configuration guide
- **SUBMISSION.md** - Guide for submitting to Community Applications

## 🚀 Quick Start

### For unRAID Users:

**Method 1: Direct Template URLs** (Easiest)

1. Install Lavalink first:
   - Docker tab → Add Container
   - Template URL: `https://raw.githubusercontent.com/julesdg6/PlexBot/main/unraid-template/plexbot-lavalink.xml`
   - Configure password (default: `youshallnotpass`)
   - Apply

2. Install PlexBot:
   - Docker tab → Add Container
   - Template URL: `https://raw.githubusercontent.com/julesdg6/PlexBot/main/unraid-template/plexbot.xml`
   - Set required fields:
     - Discord Bot Token
     - Plex URL
     - Plex Token
   - Match Lavalink password from step 1
   - Apply

**Method 2: Add Repository to Community Applications**

1. Apps → App Install Options
2. Add repository URL: `https://github.com/julesdg6/PlexBot`
3. Search for "PlexBot" in Apps tab
4. Install PlexBot-Lavalink first, then PlexBot

## 📋 Required Configuration

| Setting | Where to Get It |
|---------|----------------|
| Discord Bot Token | https://discord.com/developers/applications |
| Plex URL | Your Plex server address (e.g., `http://192.168.1.100:32400`) |
| Plex Token | https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/ |

## 🔧 Files in This Directory

```
unraid-template/
├── plexbot.xml                    # PlexBot container template
├── plexbot-lavalink.xml           # Lavalink container template
├── lavalink.application.yml       # Optional Lavalink config
├── README.md                      # Full installation guide
├── SUBMISSION.md                  # Community Applications submission guide
└── QUICKSTART.md                  # This file
```

## ✅ Features

- ✨ No .env file required - uses environment variables
- 🔄 Auto-updates when source code changes (optional)
- 📊 Persistent data and logs
- 🌐 Automatic network configuration
- 🎨 Modern visual player or classic embed style
- 🎵 YouTube and Plex music streaming
- ⚡ Slash commands with autocomplete

## 🆘 Troubleshooting

**Container won't start?**
- Make sure Lavalink is running first
- Check all required fields are filled
- View logs: `/mnt/user/appdata/plexbot/logs/`

**Can't connect to Lavalink?**
- Verify password matches in both containers
- Check both containers are on `plexbot-network`
- Default host should be `lavalink`

**Bot doesn't respond?**
- Enable Message Content Intent in Discord Developer Portal
- Verify bot has proper permissions in your server
- Check Plex URL and token are correct

## 📚 More Information

- [Full README](./README.md) - Detailed installation and configuration
- [Main Documentation](../README.md) - PlexBot features and commands
- [Discord Support](https://discord.com/invite/5m4Wyu52Ek) - Get help from the community
- [GitHub Issues](https://github.com/julesdg6/PlexBot/issues) - Report bugs

## 🎯 Next Steps After Installation

1. Invite bot to your Discord server
2. Use `/help` command to see all available commands
3. Try `/search` to find and play music
4. Configure player style in environment variables
5. Optionally set up a static player channel

## 🔐 Security Notes

- Keep your Discord token and Plex token secure
- Use strong passwords for Lavalink
- Don't share your .env file or environment variables
- Consider using masked variables for sensitive data in unRAID

---

For detailed instructions, see [README.md](./README.md)
