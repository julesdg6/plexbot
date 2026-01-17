# PlexBot All-In-One Container for Unraid

This is a simplified, single-container deployment option for Unraid users who prefer everything in one container.

## What's Included

This combined container includes:
- **PlexBot**: The Discord music bot application
- **Lavalink**: The audio streaming server (runs internally)

Both services run together in a single Docker container using supervisor to manage the processes.

## Installation

### Option 1: Template URL (Easiest)

1. In your Unraid web interface, go to the **Docker** tab
2. Click **Add Container**
3. At the bottom of the form, find the **Template repositories** field
4. Paste this URL:
   ```
   https://raw.githubusercontent.com/julesdg6/plexbot/main/unraid-template/plexbot-combined.xml
   ```
5. Press Enter or click outside the field - the template will load automatically
6. Fill in the required settings (see below)
7. Click **Apply**

### Option 2: Manual Template Installation

1. Download the template file: `plexbot-combined.xml`
2. Copy it to your Unraid server at: `/boot/config/plugins/dockerMan/templates-user/`
3. In the Unraid web interface:
   - Go to **Docker** tab
   - Click **Add Container**
   - Select **PlexBot-AllInOne** from the template dropdown
   - Configure and click **Apply**

## Required Configuration

| Setting | Required | Description |
|---------|----------|-------------|
| Discord Bot Token | ✅ Yes | Get from https://discord.com/developers/applications |
| Plex URL | ✅ Yes | Your Plex server URL (e.g., http://192.168.1.100:32400) |
| Plex Token | ✅ Yes | Get from Plex (see below) |
| Lavalink Password | ✅ Yes | Password for Lavalink (default: youshallnotpass) |
| Data Directory | ✅ Yes | Path for persistent data (default: `/mnt/user/appdata/plexbot/data`) |
| Logs Directory | ✅ Yes | Path for log files (default: `/mnt/user/appdata/plexbot/logs`) |

### Optional Settings

| Setting | Default | Description |
|---------|---------|-------------|
| Lavalink Host | localhost | Internal hostname (keep as localhost) |
| Lavalink Port | 2333 | Internal port (keep as 2333) |
| Use Modern Player | true | Modern visual player vs classic embed |
| Bot Prefix | ! | Prefix for text commands |
| Status | online | Bot status in Discord |
| Java Memory | -Xmx2G | Memory for Lavalink (increase for better performance) |

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

## Advantages of All-In-One Container

✅ **Simpler Setup**: Only one container to configure and manage
✅ **No Network Configuration**: Lavalink runs internally on localhost
✅ **Fewer Resources**: Shared container overhead
✅ **Easier Troubleshooting**: All logs in one place

## Comparison: All-In-One vs Separate Containers

### All-In-One (This Template)
- **Pros**: Simpler, fewer containers, easier to manage
- **Cons**: Both services restart together, shares resources
- **Best for**: Most users, especially those new to Docker/Unraid

### Separate Containers (Original Templates)
- **Pros**: Independent service restarts, dedicated resources per service
- **Cons**: More complex, requires network configuration
- **Best for**: Advanced users who want fine-grained control

## Troubleshooting

### Container won't start
- Verify all required environment variables are set
- Check logs: Docker tab → PlexBot-AllInOne container → Logs
- Ensure sufficient server resources (especially RAM for Java/Lavalink)

### Bot is online but won't play music
- Check that Message Content Intent is enabled in Discord Developer Portal
- Verify Plex URL and token are correct
- Review logs at `/mnt/user/appdata/plexbot/logs/`

### Lavalink connection errors
- Ensure `LAVALINK_HOST` is set to `localhost`
- Verify `LAVALINK_SERVER_PASSWORD` is configured
- Check container logs for Lavalink startup errors

### Performance issues
- Increase Java memory: Set `_JAVA_OPTIONS` to `-Xmx4G` or higher
- Ensure your Unraid server has sufficient RAM
- Monitor CPU usage in Unraid dashboard

## Viewing Logs

To view logs for both services:

1. **Via Unraid Dashboard**:
   - Go to Docker tab
   - Click on PlexBot-AllInOne container
   - Click **Logs**

2. **Via File System**:
   - PlexBot logs: `/mnt/user/appdata/plexbot/logs/`
   - Lavalink logs: `/var/log/lavalink.out.log` (inside container)
   - Supervisor logs: `/var/log/supervisor/supervisord.log` (inside container)

3. **Via Docker Command** (SSH to Unraid):
   ```bash
   docker logs PlexBot-AllInOne
   ```

## Updates

To update the container:
1. Stop the PlexBot-AllInOne container
2. In Docker tab, click the container name
3. Click **Force Update**
4. Start the container again

## Support

For issues or questions:
- [Open an issue on GitHub](https://github.com/julesdg6/plexbot/issues)
- [Visit the Discord server](https://discord.com/invite/5m4Wyu52Ek)

## Building the Image Yourself

If you want to build the combined image yourself:

```bash
cd /path/to/plexbot
docker build -f Install/Docker/dockerfile.combined -t plexbot:combined .
```

Then in your Unraid template, change the Repository to `plexbot:combined`.
