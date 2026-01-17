# PlexBot Unraid Configuration Guide

This guide provides detailed information about configuring PlexBot on Unraid, answering all the questions you'll encounter when setting up the Docker container.

## Quick Reference: Unraid Template Answers

When adding PlexBot to Unraid, you'll be asked the following questions. Here are the correct answers:

### Container Basic Configuration

| Field | Value | Notes |
|-------|-------|-------|
| **Name** | `PlexBot-AllInOne` | Can be customized, but this is the recommended name |
| **Repository** | `ghcr.io/julesdg6/plexbot:combined` | DO NOT change - this is the Docker image location |
| **Registry URL** | `https://ghcr.io` | GitHub Container Registry |
| **Icon URL** | `https://raw.githubusercontent.com/julesdg6/plexbot/main/Images/plexbot-unraid-icon.png` | PlexBot logo |
| **WebUI** | *(leave empty)* | PlexBot has no web interface |
| **Extra Parameters** | *(leave empty for all-in-one)* | For separate containers: `--network=plexbot-network` |
| **Post Arguments** | *(leave empty)* | Not required |
| **Network Type** | `Bridge` | Standard Docker bridge network |
| **Console shell command** | `bash` | Shell for console access |
| **Privileged** | `false` | PlexBot doesn't need privileged access |

### Use Tailscale
**Answer:** No (leave unchecked)

PlexBot doesn't require Tailscale VPN networking.

## Deployment Options

PlexBot offers two deployment methods for Unraid:

### Option 1: All-In-One Container (⭐ RECOMMENDED)

**Template URL:** `https://raw.githubusercontent.com/julesdg6/plexbot/main/unraid-template/plexbot-combined.xml`

**Advantages:**
- ✅ Single container - easier to manage
- ✅ No network configuration needed
- ✅ Simpler setup process
- ✅ Perfect for most users

**Repository:** `ghcr.io/julesdg6/plexbot:combined`

### Option 2: Separate Containers (Advanced Users)

Requires two containers:

1. **PlexBot-Lavalink** (install first)
   - Template URL: `https://raw.githubusercontent.com/julesdg6/plexbot/main/unraid-template/plexbot-lavalink.xml`
   - Repository: `ghcr.io/lavalink-devs/lavalink:4`
   - Extra Parameters: `--network=plexbot-network`

2. **PlexBot** (install second)
   - Template URL: `https://raw.githubusercontent.com/julesdg6/plexbot/main/unraid-template/plexbot.xml`
   - Repository: `ghcr.io/julesdg6/plexbot:latest`
   - Extra Parameters: `--network=plexbot-network`

**Advantages:**
- ✅ Independent service management
- ✅ Dedicated resources per service
- ✅ More control over each component

## Step-by-Step Installation (All-In-One - Recommended)

### Step 1: Add the Container Template

1. Open your Unraid web interface
2. Navigate to the **Docker** tab
3. Click **Add Container** button
4. Scroll to the bottom and find **Template repositories** field
5. Paste this URL:
   ```
   https://raw.githubusercontent.com/julesdg6/plexbot/main/unraid-template/plexbot-combined.xml
   ```
6. Press Enter or click outside the field
7. The template will load automatically

### Step 2: Verify Basic Configuration

The template will auto-fill most fields. Verify these are correct:

- **Name:** `PlexBot-AllInOne` (or your preferred name)
- **Repository:** `ghcr.io/julesdg6/plexbot:combined`
- **Network Type:** `Bridge`
- **Privileged:** `No` (unchecked)

### Step 3: Configure Required Settings

You MUST configure these settings:

#### Discord Bot Token
- **Field:** Discord Bot Token
- **Required:** ✅ Yes
- **Where to get it:** https://discord.com/developers/applications
- **Steps:**
  1. Go to https://discord.com/developers/applications
  2. Click "New Application"
  3. Name it "PlexBot"
  4. Go to "Bot" section
  5. Click "Add Bot"
  6. Click "Reset Token" and copy it
  7. Enable these Privileged Gateway Intents:
     - Message Content Intent
     - Server Members Intent
     - Presence Intent
  8. Go to OAuth2 > URL Generator
  9. Select scopes: `bot`, `applications.commands`
  10. Select permissions: `Send Messages`, `Embed Links`, `Attach Files`, `Connect`, `Speak`, `Use Voice Activity`
  11. Copy the URL and invite bot to your server

#### Plex URL
- **Field:** Plex URL
- **Required:** ✅ Yes
- **Format:** `http://YOUR_SERVER_IP:32400`
- **Example:** `http://192.168.1.100:32400`
- **Notes:** 
  - Use your Unraid server's IP if Plex is on Unraid
  - Use `http://` not `https://` for local connections
  - Port is usually `32400`

#### Plex Token
- **Field:** Plex Token
- **Required:** ✅ Yes
- **Where to get it:** https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/
- **Quick method:**
  1. Open Plex Web App
  2. Open any media item
  3. Click "Get Info"
  4. Click "View XML"
  5. Look for `X-Plex-Token` in the URL
  6. Copy the token value

#### Lavalink Password
- **Field:** Lavalink Password
- **Required:** ✅ Yes
- **Default:** `youshallnotpass`
- **Recommendation:** Change this to a secure password
- **Note:** This is for internal communication only, not exposed externally

#### Data Directory
- **Field:** Data Directory
- **Required:** ✅ Yes
- **Default:** `/mnt/user/appdata/plexbot/data`
- **Recommendation:** Keep the default unless you have specific needs

#### Logs Directory
- **Field:** Logs Directory
- **Required:** ✅ Yes
- **Default:** `/mnt/user/appdata/plexbot/logs`
- **Recommendation:** Keep the default unless you have specific needs

### Step 4: Configure Optional Settings

These settings can be customized but have sensible defaults:

| Setting | Default | Options | Description |
|---------|---------|---------|-------------|
| Lavalink Host | `localhost` | Keep as `localhost` | Internal hostname for all-in-one |
| Lavalink Port | `2333` | Keep as `2333` | Internal port |
| Use Modern Player | `true` | `true` or `false` | Modern visual player vs classic embed |
| Bot Prefix | `!` | Any character | Prefix for text commands |
| Status | `online` | `online`, `idle`, `dnd`, `invisible` | Bot status in Discord |
| Java Memory | `-Xmx2G` | `-Xmx4G`, etc. | Memory for Lavalink (increase for better performance) |
| Use Static Player Channel | `false` | `true` or `false` | Enable dedicated player channel |
| Static Player Channel ID | *(empty)* | Discord channel ID | Only if static player is enabled |
| Logging Level | `INFO` | `DEBUG`, `INFO`, `WARN`, `ERROR` | Log verbosity |

### Step 5: Apply and Start

1. Review all settings
2. Click **Apply**
3. Unraid will download the Docker image (this may take a few minutes)
4. The container will start automatically
5. Check the logs to verify successful startup

### Step 6: Verify Installation

1. In Unraid Docker tab, find your PlexBot container
2. Click the container icon
3. Select **Logs**
4. Look for these success messages:
   - "Lavalink is ready!"
   - "PlexBot application started"
   - "Connected to Discord"

## Container Configuration Fields Explained

### Name
- **What it is:** The display name for your container in Unraid
- **Default:** `PlexBot-AllInOne`
- **Can you change it:** Yes, but keep it recognizable
- **Best practice:** Use the default or add your server name (e.g., `PlexBot-MainServer`)

### Overview
- **What it is:** Description shown in the Unraid UI
- **What's included:** Feature list, setup requirements, documentation links
- **Can you change it:** Not recommended
- **Note:** This is auto-populated by the template

### Additional Requirements
- **What it is:** Dependencies or prerequisites
- **For all-in-one:** None! Everything is included
- **For separate containers:** Requires Lavalink container to be running

### Repository
- **What it is:** The Docker image location
- **Value:** `ghcr.io/julesdg6/plexbot:combined`
- **Format:** `registry/username/image:tag`
- **⚠️ DO NOT CHANGE:** This must match the official image
- **Common error:** Using GitHub URL instead of Docker image name

### Registry URL
- **What it is:** The Docker registry hosting the image
- **Value:** `https://ghcr.io`
- **What it does:** Tells Unraid where to download the image from
- **Note:** GitHub Container Registry (ghcr.io) is free and public

### Icon URL
- **What it is:** The container icon shown in Unraid
- **Value:** `https://raw.githubusercontent.com/julesdg6/plexbot/main/Images/plexbot-unraid-icon.png`
- **Optional:** Yes, but makes the container easier to identify

### WebUI
- **What it is:** URL for web-based management interface
- **Value for PlexBot:** *(empty)*
- **Why empty:** PlexBot is configured via Discord, not a web UI
- **Note:** Some containers have WebUI (like Plex itself), PlexBot doesn't

### Extra Parameters
- **What it is:** Additional Docker run arguments
- **All-in-one value:** *(empty)*
- **Separate containers value:** `--network=plexbot-network`
- **Format:** Standard Docker CLI arguments
- **Example uses:** Custom networks, device mapping, capabilities

### Post Arguments
- **What it is:** Arguments passed to the container's entry point
- **Value for PlexBot:** *(empty)*
- **Why empty:** PlexBot doesn't require command-line arguments
- **Note:** Some containers use this for command-line options

### Network Type
- **What it is:** Docker networking mode
- **Value:** `Bridge`
- **Options:**
  - **Bridge** (default): Standard Docker networking
  - **Host:** Use host network stack (not recommended for PlexBot)
  - **Custom:** User-defined networks
- **Why Bridge:** Provides isolation while allowing necessary connections

### Use Tailscale
- **What it is:** Enables Tailscale VPN integration
- **Value for PlexBot:** No (unchecked)
- **When to use:** If you want remote access via Tailscale VPN
- **Note:** Not required for normal PlexBot operation

### Console Shell Command
- **What it is:** Shell used when accessing container console
- **Value:** `bash`
- **Options:** `bash`, `sh`, `shell`
- **Why bash:** More features and compatibility than basic `sh`

### Privileged
- **What it is:** Gives container root-level access to host system
- **Value for PlexBot:** `false` (unchecked)
- **Security:** Only enable if absolutely necessary
- **PlexBot needs:** No privileged access required

## Environment Variables

All configuration is done through environment variables. These are set in the Unraid template:

### Required Variables

| Variable | Example | Purpose |
|----------|---------|---------|
| `DISCORD_TOKEN` | `your-bot-token-here` | Discord bot authentication |
| `PLEX_URL` | `http://192.168.1.100:32400` | Plex server location |
| `PLEX_TOKEN` | `your-plex-token` | Plex authentication |
| `LAVALINK_SERVER_PASSWORD` | `youshallnotpass` | Lavalink security |

### Optional Variables

| Variable | Default | Purpose |
|----------|---------|---------|
| `LAVALINK_HOST` | `localhost` | Lavalink hostname |
| `LAVALINK_SERVER_PORT` | `2333` | Lavalink port |
| `USE_MODERN_PLAYER` | `true` | Player style |
| `BOT_PREFIX` | `!` | Command prefix |
| `STATUS` | `online` | Bot status |
| `_JAVA_OPTIONS` | `-Xmx2G` | Java memory |

## Storage Paths

The container needs these directories on your Unraid server:

### Data Directory
- **Container path:** `/app/data`
- **Host path:** `/mnt/user/appdata/plexbot/data`
- **Purpose:** Persistent bot data, settings, cache
- **Backup:** Recommended

### Logs Directory
- **Container path:** `/app/logs`
- **Host path:** `/mnt/user/appdata/plexbot/logs`
- **Purpose:** Application logs for troubleshooting
- **Backup:** Optional

### Source Code Directory (Optional)
- **Container path:** `/source`
- **Host path:** `/mnt/user/appdata/plexbot/source`
- **Purpose:** For development or auto-updates from git
- **Required:** No, only for advanced users

## Ports

### All-In-One Container

| Port | Protocol | Purpose | Expose to Host? |
|------|----------|---------|-----------------|
| 2333 | TCP | Lavalink (internal) | No (optional) |

**Note:** Port 2333 is only used internally. You can expose it for monitoring but it's not required.

### Separate Containers

**Lavalink Container:**
- Port 2333: Must be exposed (TCP)

**PlexBot Container:**
- No ports needed

## Troubleshooting

### Container Won't Start

**Check:**
1. All required variables are set (Discord Token, Plex URL, Plex Token)
2. Sufficient disk space in `/mnt/user/appdata/`
3. Container logs for specific error messages

**Common issues:**
- Missing or invalid Discord token
- Incorrect Plex URL format (should be `http://IP:32400`)
- Insufficient memory (increase Java memory if needed)

### Bot Shows as Offline in Discord

**Check:**
1. Discord token is correct
2. Bot has been invited to your Discord server
3. Required intents are enabled in Discord Developer Portal
4. Container is running (check Unraid Docker tab)

**Fix:**
1. Verify token in Discord Developer Portal
2. Re-invite bot using OAuth2 URL
3. Restart container

### Bot Won't Play Music

**Check:**
1. Lavalink is running (in container logs)
2. Plex URL is accessible from container
3. Plex token is valid
4. Bot has voice permissions in Discord

**Test Plex connection:**
```bash
# From Unraid terminal
docker exec PlexBot-AllInOne curl http://YOUR_PLEX_IP:32400
```

### Lavalink Connection Errors

**For All-In-One:**
- Ensure `LAVALINK_HOST` is set to `localhost`
- Verify `LAVALINK_SERVER_PASSWORD` is configured
- Check container logs for Lavalink startup errors

**For Separate Containers:**
- Ensure both containers use `--network=plexbot-network`
- Verify Lavalink container is running first
- Check `LAVALINK_HOST` is set to `lavalink` (container name)

### High Memory Usage

**Solution:**
- Adjust Java memory: Change `_JAVA_OPTIONS` to `-Xmx1G` (lower) or `-Xmx4G` (higher)
- Monitor with: `docker stats PlexBot-AllInOne`

### Container Keeps Restarting

**Check logs:**
```bash
docker logs PlexBot-AllInOne
```

**Common causes:**
- Configuration error (missing required variable)
- Application crash (check PlexBot logs)
- Out of memory (check system resources)

## Updating PlexBot

### Method 1: Unraid WebUI (Easiest)

1. Go to Docker tab
2. Find PlexBot container
3. Click container icon
4. Select "Force Update"
5. Wait for download
6. Container will restart automatically

### Method 2: Command Line

```bash
docker stop PlexBot-AllInOne
docker rm PlexBot-AllInOne
docker pull ghcr.io/julesdg6/plexbot:combined
# Then recreate from template
```

### Method 3: Auto-Update (Docker Compose)

Consider using Watchtower or similar tools for automatic updates.

## Advanced Configuration

### Custom Network

If you need custom networking:

1. Create network: `docker network create my-plexbot-net`
2. In Extra Parameters: `--network=my-plexbot-net`
3. Update other containers to use same network

### Resource Limits

Add to Extra Parameters:
```
--memory=4g --cpus=2
```

### Custom Java Options

For Lavalink tuning:
```
-Xmx4G -Xms2G -XX:+UseG1GC
```

## Security Best Practices

1. **Change default password:** Update `LAVALINK_SERVER_PASSWORD`
2. **Don't expose ports:** Unless monitoring, keep 2333 internal
3. **Secure tokens:** Never share Discord or Plex tokens
4. **Regular updates:** Keep container updated for security patches
5. **Backup data:** Regularly backup `/appdata/plexbot/data`

## Support and Resources

### Documentation
- [Main README](../README.md)
- [Docker Guide](../Docs/Setup/Docker-Guide.md)
- [Configuration Guide](../Docs/Setup/Configuration.md)
- [Troubleshooting Guide](../Docs/Guides/Troubleshooting.md)

### Community Support
- [GitHub Issues](https://github.com/julesdg6/plexbot/issues)
- [Discord Server](https://discord.com/invite/5m4Wyu52Ek)

### Unraid Resources
- [Unraid Forums](https://forums.unraid.net/)
- [Unraid Docker Documentation](https://wiki.unraid.net/Docker)

## FAQ

### Q: Do I need both containers or just one?
**A:** For most users, use the all-in-one container (`plexbot-combined.xml`). Only use separate containers if you need independent service management.

### Q: Can I use this with Unraid's Community Applications?
**A:** Yes! Search for "PlexBot" once the templates are added to the official repository. Until then, use the template URL method.

### Q: What's the difference between Repository and Registry URL?
**A:** 
- **Repository** = the full Docker image path (`ghcr.io/julesdg6/plexbot:combined`)
- **Registry URL** = just the registry domain (`https://ghcr.io`)

### Q: Why doesn't PlexBot have a WebUI?
**A:** PlexBot is a Discord bot - all interaction happens through Discord commands and the player interface. No web UI is needed.

### Q: Can I run multiple instances of PlexBot?
**A:** Yes, but each needs:
- Unique container name
- Unique Discord bot token
- Unique data directory

### Q: How much RAM does PlexBot need?
**A:** Minimum 2GB (default Java setting). For better performance with heavy use, allocate 4GB or more.

### Q: Where are the logs stored?
**A:** On your Unraid server at `/mnt/user/appdata/plexbot/logs/`

### Q: Can I use this with Plex running on a different server?
**A:** Yes! Just set `PLEX_URL` to your Plex server's IP and port.

### Q: Does this work with Emby or Jellyfin?
**A:** No, PlexBot is specifically designed for Plex Media Server.

### Q: What if I get "invalid reference format" error?
**A:** You're using the GitHub URL instead of the Docker image name. Use `ghcr.io/julesdg6/plexbot:combined`, NOT `https://github.com/julesdg6/plexbot`.

## Template Comparison

### All-In-One vs Separate Containers

| Feature | All-In-One | Separate |
|---------|-----------|----------|
| **Containers** | 1 | 2 |
| **Setup complexity** | Simple | Moderate |
| **Network config** | Automatic | Manual |
| **Resource sharing** | Shared | Dedicated |
| **Independent restart** | No | Yes |
| **Maintenance** | Easier | More control |
| **Recommended for** | Most users | Advanced users |

## Conclusion

This guide covers everything you need to configure PlexBot on Unraid. The key points:

1. **Use the all-in-one container** unless you have specific needs
2. **Template URL method** is the easiest installation approach
3. **Required fields:** Discord token, Plex URL, Plex token
4. **Network type:** Bridge (standard)
5. **No privileged access** needed
6. **No WebUI** - configure through Discord

For additional help, consult the documentation links or join the Discord support server.
