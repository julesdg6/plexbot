# PlexBot unRAID Template

This directory contains the unRAID Community Applications templates for PlexBot and its companion Lavalink container.

## Installation on unRAID

### Option 1: Via Community Applications (Recommended - When Available)

Once these templates are added to the Community Applications repository:

1. Open the **Apps** tab in your unRAID web interface
2. Search for "PlexBot"
3. Click **Install** on both:
   - **PlexBot-Lavalink** (install this first)
   - **PlexBot**
4. Configure the required settings (see below)
5. Click **Apply**

### Option 2: Manual Template Installation

If the templates are not yet in Community Applications:

1. Download both template files:
   - `plexbot.xml`
   - `plexbot-lavalink.xml`

2. Copy them to your unRAID server at: `/boot/config/plugins/dockerMan/templates-user/`

3. In the unRAID web interface:
   - Go to **Docker** tab
   - Click **Add Container**
   - Select **PlexBot-Lavalink** from the template dropdown
   - Configure and start it first
   - Then add **PlexBot** container
   - Configure and start it

### Option 3: Template URL (Direct Install)

1. In unRAID Docker tab, click **Add Container**
2. Set **Template repositories** to:
   ```
   https://github.com/julesdg6/PlexBot
   ```
3. Click on template dropdown and search for PlexBot templates

## Required Configuration

### PlexBot-Lavalink Container (Install First)

| Setting | Default | Description |
|---------|---------|-------------|
| Lavalink Port | 2333 | Port for Lavalink server |
| Lavalink Password | youshallnotpass | Authentication password (change recommended) |
| Java Options | -Xmx2G | Memory allocation (adjust based on your server) |

### PlexBot Container

| Setting | Required | Description |
|---------|----------|-------------|
| Discord Bot Token | ✅ Yes | Get from https://discord.com/developers/applications |
| Plex URL | ✅ Yes | Your Plex server URL (e.g., http://192.168.1.100:32400) |
| Plex Token | ✅ Yes | Get from Plex: [Finding Your Token](https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/) |
| Lavalink Host | ✅ Yes | Set to `lavalink` (container name) |
| Lavalink Port | ✅ Yes | Set to `2333` (must match Lavalink container) |
| Lavalink Password | ✅ Yes | Must match the password set in Lavalink container |
| Use Modern Player | No | `true` for modern visual player, `false` for classic embed |
| Data Directory | ✅ Yes | Path for persistent data (default: `/mnt/user/appdata/plexbot/data`) |
| Logs Directory | ✅ Yes | Path for log files (default: `/mnt/user/appdata/plexbot/logs`) |

## Getting Your Discord Bot Token

1. Go to https://discord.com/developers/applications
2. Click **New Application**
3. Name it "PlexBot" and create
4. Go to **Bot** section
5. Click **Add Bot**
6. Under Token, click **Reset Token** and copy it
7. Enable these **Privileged Gateway Intents**:
   - Message Content Intent
   - Server Members Intent
   - Presence Intent
8. Go to **OAuth2** > **URL Generator**
9. Select scopes: `bot`, `applications.commands`
10. Select bot permissions: `Send Messages`, `Embed Links`, `Attach Files`, `Connect`, `Speak`, `Use Voice Activity`
11. Copy the generated URL and open it in browser to invite bot to your server

## Getting Your Plex Token

Follow the official Plex guide: https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/

Quick method:
1. Sign in to your Plex Web App
2. Open any item in your library
3. Click **Get Info** (or click the three dots and select **Get Info**)
4. Click **View XML**
5. Look for `X-Plex-Token` in the URL

## Network Configuration

Both containers need to be on the same Docker network to communicate. The templates automatically set this up using the `plexbot-network` bridge network.

If you experience connection issues:
1. Ensure both containers are running
2. Check that both containers use `--network=plexbot-network`
3. Verify the Lavalink Host in PlexBot is set to `lavalink` (the container name)

## Troubleshooting

### PlexBot won't start
- Ensure Lavalink container is running first
- Verify all required environment variables are set
- Check logs at `/mnt/user/appdata/plexbot/logs/`

### Can't connect to Lavalink
- Verify Lavalink container is running: `docker ps | grep lavalink`
- Check Lavalink password matches in both containers
- Ensure both containers are on the same network
- Verify port 2333 is not blocked by firewall

### Bot is online but won't respond
- Ensure bot has proper permissions in Discord server
- Check that Message Content Intent is enabled in Discord Developer Portal
- Verify Plex URL and token are correct
- Review logs for connection errors

### Audio quality issues
- Adjust Java memory in Lavalink: `-Xmx4G` for better performance
- Check your unRAID server has sufficient resources
- Verify network connectivity to Plex server

## Additional Resources

- [PlexBot Documentation](https://github.com/julesdg6/PlexBot)
- [PlexBot Discord Support](https://discord.com/invite/5m4Wyu52Ek)
- [Lavalink Documentation](https://lavalink.dev/)
- [unRAID Forums](https://forums.unraid.net/)

## Updates

The PlexBot container will check for updates on each restart. To update:
1. Stop the PlexBot container
2. Pull the latest image
3. Start the container again

Or use unRAID's built-in update feature in the Docker tab.

## Icon

The PlexBot icon is located at:
```
https://raw.githubusercontent.com/julesdg6/PlexBot/main/Images/plexbot-unraid-icon.png
```

## Support

For issues specific to the unRAID templates:
- [Open an issue on GitHub](https://github.com/julesdg6/PlexBot/issues)

For general PlexBot support:
- [Visit the Discord server](https://discord.com/invite/5m4Wyu52Ek)
