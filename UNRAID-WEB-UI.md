# Unraid Web UI Status Dashboard

## Overview

PlexBot now includes a built-in web interface that helps you configure and monitor your container on Unraid. This makes it easy to see what configuration is missing and fix deployment issues.

## Accessing the Web UI

After starting the PlexBot container in Unraid:

1. Click the **WebUI** button in your Docker dashboard (looks like a globe icon)
2. OR visit `http://YOUR_SERVER_IP:8080` in your web browser

The web UI is accessible on port **8080** by default.

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

If any required settings are missing, the web UI shows:
- ⚠️ Clear warning about what's missing
- 📝 Direct links to get required tokens
- 📚 Links to setup documentation
- 💡 Instructions for configuring in Unraid

## How to Fix Configuration Issues

If you see missing configuration warnings:

1. In Unraid, go to **Docker** tab
2. Click your PlexBot container
3. Click **Edit**
4. Fill in the missing environment variables shown in the web UI
5. Click **Apply** to restart the container
6. Refresh the web UI to verify all settings are green ✅

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

### Port Information

| Port | Purpose | Required |
|------|---------|----------|
| 8080 | Status Web UI | Yes (always visible) |
| 2333 | Lavalink (internal) | Advanced only |

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
<WebUI>http://[IP]:[PORT:8080]/</WebUI>
<Config Name="Status Web UI Port" Target="8080" Default="8080" Mode="tcp" 
        Description="Web interface port for monitoring container status and configuration." 
        Type="Port" Display="always" Required="true" Mask="false"/>
```

This makes the WebUI button appear automatically in Unraid's Docker dashboard.

## Files Modified

1. `Main/StatusWebServer.cs` - New web server implementation
2. `Main/BotHostedService.cs` - Graceful error handling  
3. `Main/ServiceRegistration.cs` - Register web server as hosted service
4. `Install/Docker/dockerfile.combined` - Expose port 8080, fix supervisor config
5. `unraid-template/plexbot-combined.xml` - Add WebUI and port configuration

## Summary

The web UI makes PlexBot much easier to configure on Unraid by:
- Providing instant feedback on configuration status
- Eliminating confusing crash loops
- Giving clear, actionable instructions
- Making the Unraid experience user-friendly

No more guessing what's wrong - just open the WebUI and it tells you exactly what to fix! 🎉
