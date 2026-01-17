# Unraid Web UI Status Dashboard

## Overview

PlexBot now includes a built-in web interface that helps you configure and monitor your container on Unraid. This makes it easy to see what configuration is missing and fix deployment issues.

## Accessing the Web UI

After starting the PlexBot container in Unraid:

1. Click the **WebUI** button in your Docker dashboard (looks like a globe icon)
2. OR visit `http://YOUR_SERVER_IP:4303` in your web browser

The web UI is accessible on port **4303** by default. Port 4303 lives in the private/dynamic range, so it is rarely claimed by other services; if you prefer, you can remap the host port to another rarely-used number such as 1210.

## What the Web UI Shows

### Configuration Status Dashboard

The web interface displays:

✅ **Required Configuration**
- Discord Bot Token status
- Plex Server URL status (masked for security)
- Plex Token status
- Lavalink Password status

⚙️ **Optional Settings**
- Bot Prefix
- Bot Status (online/idle/dnd/invisible)
- Modern Player UI (enabled/disabled)
- Static Player Channel settings

### Status Indicators

- 🟢 **Green dot** = Setting is configured correctly
- 🔴 **Red dot** = Setting is missing or invalid

### Configuration Help

- If any required settings are missing, the web UI shows:
  - ⚠️ Clear warning about what's missing
  - 📝 Direct links to get required tokens
  - 📚 Links to setup documentation
  - 💡 Instructions for configuring in Unraid
  
  The container also logs a configuration summary on startup (`docker logs plexbot`), for example `Configuration summary: DISCORD_TOKEN=configured, PLEX_URL=http://..., PLEX_TOKEN=configured...`, so you can cross-check that the values Unraid is passing matched what you entered.

When you edit the container in Unraid, click **Show more settings...** (or the **Add another Path, Port, Variable, Label or Device** link) to reveal the environment variables such as `DISCORD_TOKEN`, `PLEX_URL`, `PLEX_TOKEN`, and `LAVALINK_SERVER_PASSWORD`. The status dashboard highlights which of those required fields still need values.

## How to Fix Configuration Issues

### Missing Configuration

If you see missing configuration warnings:

1. In Unraid, go to **Docker** tab
2. Click your PlexBot container
3. Click **Edit**
4. Fill in the missing environment variables shown in the web UI
5. Click **Apply** to restart the container
6. Refresh the web UI to verify all settings are green ✅

### Port Conflicts

If port 4303 or 2333 is already in use by another application:

1. In Unraid, go to **Docker** tab
2. Click your PlexBot container  
3. Click **Edit**
4. Scroll down to the **Port Mappings** section
5. Change the **Host Port** (left side) to an available port (e.g., 4304, 4305, etc.)
6. Keep the **Container Port** (right side) at the default value
7. Click **Apply** to restart the container
8. Access the web UI using the new port: `http://YOUR_SERVER_IP:[New Host Port]`

## Solved Problem: No More Crash Loops!

### Before This Fix

The container would crash repeatedly if DISCORD_TOKEN wasn't set:
```
WARN exited: plexbot (exit status 1; not expected)
INFO spawned: 'plexbot' with pid 78
WARN exited: plexbot (exit status 1; not expected)
INFO gave up: plexbot entered FATAL state, too many start retries too quickly
```

Users couldn't tell what was wrong without digging through logs.

### After This Fix

- ✅ Container starts successfully even without full configuration
- ✅ Web UI clearly shows what needs to be configured
- ✅ Helpful error messages in the logs
- ✅ No crash loops - everything runs gracefully
- ✅ One-click access to configuration status via WebUI button

## Technical Details

### Port Configuration

All port numbers are fully configurable in the Unraid UI to avoid conflicts with other applications:

| Port Type | Default | Configurable | Description |
|-----------|---------|--------------|-------------|
| **Status Web UI Host Port** | 4303 | Yes (always visible) | The port on your Unraid server to access the web interface |
| **Status Web UI Container Port** | 4303 | Yes (advanced) | The port inside the container where the web server runs |
| **Lavalink Host Port** | 2333 | Yes (advanced) | The port on your Unraid server for Lavalink (typically not exposed) |
| **Lavalink Container Port** | 2333 | Yes (advanced, via LAVALINK_SERVER_PORT) | The port inside the container where Lavalink runs |

**How Port Mapping Works:**
- **Host Port** → **Container Port** (e.g., 4303 → 4303)
- If port 4303 is already in use on your server, change the **Host Port** to something else (e.g., 4304)
- The container port can usually stay at the default unless you have specific requirements
- The web interface will be accessible at `http://YOUR_SERVER_IP:[Host Port]`

### Environment Variables

The web UI validates these required variables:
- `DISCORD_TOKEN` - Your Discord bot token
- `PLEX_URL` - Your Plex server URL
- `PLEX_TOKEN` - Your Plex authentication token  
- `LAVALINK_SERVER_PASSWORD` - Password for Lavalink

And displays these optional variables:
- `BOT_PREFIX` - Command prefix (default: `!`)
- `STATUS` - Bot status (default: `online`)
- `USE_MODERN_PLAYER` - Modern vs classic player UI
- `USE_STATIC_PLAYER_CHANNEL` - Enable static player channel
- `STATIC_PLAYER_CHANNEL_ID` - Channel ID for static player

### Security

- The web interface masks sensitive information:
  - Plex URLs are shown as "Configured (hidden for security)"
  - Tokens show only "Configured" or "Not Set" status
- No passwords or tokens are displayed in plain text
- The web UI is read-only - it doesn't modify configuration

## Unraid Template Changes

The `plexbot-combined.xml` template now includes:

```xml
<WebUI>http://[IP]:[PORT:4303]/</WebUI>
<Config Name="Status Web UI Port" Target="4303" Default="4303" Mode="tcp" 
        Description="Web interface port for monitoring container status and configuration." 
        Type="Port" Display="always" Required="true" Mask="false"/>
```

This makes the WebUI button appear automatically in Unraid's Docker dashboard.

## Files Modified

1. `Main/StatusWebServer.cs` - New web server implementation
2. `Main/BotHostedService.cs` - Graceful error handling  
3. `Main/ServiceRegistration.cs` - Register web server as hosted service
4. `Install/Docker/dockerfile.combined` - Expose port 4303, fix supervisor config
5. `unraid-template/plexbot-combined.xml` - Add WebUI and port configuration

## Summary

The web UI makes PlexBot much easier to configure on Unraid by:
- Providing instant feedback on configuration status
- Eliminating confusing crash loops
- Giving clear, actionable instructions
- Making the Unraid experience user-friendly

No more guessing what's wrong - just open the WebUI and it tells you exactly what to fix! 🎉
