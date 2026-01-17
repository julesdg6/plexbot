# Quick Start: PlexBot All-In-One Container

This guide will help you get PlexBot up and running on Unraid in just a few minutes using the all-in-one container.

## What You Need

Before starting, gather these items:

1. **Discord Bot Token**
   - Create at: https://discord.com/developers/applications
   - Enable "Message Content Intent" in Bot settings
   
2. **Plex Server URL**
   - Example: `http://192.168.1.100:32400`
   
3. **Plex Authentication Token**
   - Get it here: https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/

## Installation Steps

### 1. Open Unraid Docker Page
- Log into your Unraid web interface
- Click on the **Docker** tab
- Click **Add Container**

### 2. Load the Template
At the bottom of the Add Container page:
- Find the **Template repositories** field
- Paste this URL:
  ```
  https://raw.githubusercontent.com/julesdg6/plexbot/main/unraid-template/plexbot-combined.xml
  ```
- Press **Enter** or click outside the field
- The template will load automatically

### 3. Fill in Required Settings

The template will show several fields. Fill in these **required** ones:

| Field | What to Enter |
|-------|---------------|
| **Discord Bot Token** | Your Discord bot token from step "What You Need" |
| **Plex URL** | Your Plex server URL (e.g., `http://192.168.1.100:32400`) |
| **Plex Token** | Your Plex authentication token |
| **Lavalink Password** | Change from default `youshallnotpass` to something secure |

### 4. Review Optional Settings

These have good defaults, but you can customize:

| Field | Default | Description |
|-------|---------|-------------|
| **Use Modern Player** | true | Set to false for classic Discord embed style |
| **Bot Prefix** | ! | Text command prefix (slash commands always work) |
| **Data Directory** | /mnt/user/appdata/plexbot/data | Where PlexBot stores data |
| **Logs Directory** | /mnt/user/appdata/plexbot/logs | Where logs are saved |

### 5. Start the Container

- Click **Apply** at the bottom
- Wait for the container to download and start
- This may take a few minutes on first run

### 6. Verify It's Working

1. **Check Container Status**
   - In the Docker tab, PlexBot-AllInOne should show as "Started"
   
2. **View Logs** (click the container icon → Logs)
   - You should see:
     ```
     Lavalink is ready!
     Starting PlexBot application...
     ```

3. **Check Discord**
   - Your bot should appear online in Discord
   - Try the `/play` command to test

## Common Issues

### Container won't start
- **Check logs** for error messages
- Verify all required fields are filled in
- Ensure Discord token is valid

### Bot shows offline in Discord
- Verify Discord token is correct
- Check that "Message Content Intent" is enabled in Discord Developer Portal
- Review container logs for connection errors

### Can't play music
- Verify Plex URL is accessible from your Unraid server
- Check that Plex token is valid
- Make sure bot has proper permissions in Discord voice channel

### Need more help?
- View detailed logs: `/mnt/user/appdata/plexbot/logs/`
- Check the [full documentation](./README-COMBINED.md)
- [Open an issue on GitHub](https://github.com/julesdg6/plexbot/issues)
- [Join the Discord server](https://discord.com/invite/5m4Wyu52Ek)

## Next Steps

Once your bot is running:

1. **Invite it to your server** using the OAuth2 URL from Discord Developer Portal
2. **Join a voice channel** in Discord
3. **Try these commands**:
   - `/play <song name>` - Play music from Plex or YouTube
   - `/queue` - View the current queue
   - `/skip` - Skip to next track
   - `/pause` - Pause playback
   - `/resume` - Resume playback

Enjoy your music! 🎵
