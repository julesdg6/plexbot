# Unraid Container Configuration - Quick Reference

When configuring PlexBot as a Docker container in Unraid, you'll be asked these questions. Here are the answers:

## Basic Container Fields

### Name:
**Answer:** `PlexBot-AllInOne`

- This is the display name in your Unraid Docker tab
- You can customize this if you want (e.g., `PlexBot-MyServer`)

### Overview:
**Answer:** *(Auto-filled by template)*

- This is the description shown in Unraid
- No action needed - the template provides this

### Additional Requirements:
**Answer:** None for all-in-one container

- All-in-one container: Everything included, no dependencies
- Separate containers: Requires Lavalink container (see separate guide)

### Repository:
**Answer:** `ghcr.io/julesdg6/plexbot:combined`

- ⚠️ **IMPORTANT:** This is the Docker image name, NOT a GitHub URL
- DO NOT use `https://github.com/julesdg6/plexbot`
- Format: `registry/username/image:tag`

### Registry URL:
**Answer:** `https://ghcr.io`

- This tells Unraid where to download the image from
- GitHub Container Registry (free and public)

### Icon URL:
**Answer:** `https://raw.githubusercontent.com/julesdg6/plexbot/main/Images/plexbot-unraid-icon.png`

- This is the icon shown in Unraid Docker tab
- Makes the container easy to identify

### WebUI:
**Answer:** *(Leave empty)*

- PlexBot has no web interface
- All interaction is through Discord

### Extra Parameters:
**Answer:** *(Leave empty for all-in-one)*

- All-in-one container: Not needed
- Separate containers: Use `--network=plexbot-network`

### Post Arguments:
**Answer:** *(Leave empty)*

- PlexBot doesn't require command-line arguments

### Network Type:
**Answer:** `Bridge`

- Standard Docker bridge networking
- Other options exist but Bridge is correct for PlexBot

### Use Tailscale:
**Answer:** `No` (unchecked)

- PlexBot doesn't require Tailscale VPN
- Only enable if you specifically need Tailscale

### Console shell command:
**Answer:** `bash`

- Shell used when accessing container console
- `bash` provides more features than basic `sh`

### Privileged:
**Answer:** `false` (unchecked)

- PlexBot doesn't need root-level host access
- Leaving unchecked is more secure

## Installation Methods

### Method 1: Template URL (Recommended)

1. Unraid Docker tab → Add Container
2. Scroll to bottom → Template repositories field
3. Paste: `https://raw.githubusercontent.com/julesdg6/plexbot/main/unraid-template/plexbot-combined.xml`
4. Press Enter
5. Template loads automatically with all correct values

### Method 2: Manual Template File

1. Download `plexbot-combined.xml`
2. Copy to `/boot/config/plugins/dockerMan/templates-user/`
3. Unraid Docker tab → Add Container
4. Select "PlexBot-AllInOne" from dropdown

### Method 3: Community Applications (When Available)

1. Apps tab in Unraid
2. Search "PlexBot"
3. Click Install
4. Configure settings

## Required Configuration (You Must Set These)

After adding the template, configure these values:

### 1. Discord Bot Token
- **Get from:** https://discord.com/developers/applications
- **Required:** ✅ Yes
- **Example:** `MTIzNDU2Nzg5MDEyMzQ1Njc4OQ.GAbCdE.fGhIjKlMnOpQrStUvWxYz`

### 2. Plex URL
- **Required:** ✅ Yes
- **Format:** `http://YOUR_IP:32400`
- **Example:** `http://192.168.1.100:32400`

### 3. Plex Token
- **Get from:** https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/
- **Required:** ✅ Yes
- **Example:** `aBcDeFgHiJkLmNoPqRsTuV`

### 4. Lavalink Password
- **Default:** `youshallnotpass`
- **Required:** ✅ Yes
- **Recommendation:** Change to a secure password

### 5. Data Directory
- **Default:** `/mnt/user/appdata/plexbot/data`
- **Required:** ✅ Yes
- **Recommendation:** Keep default

### 6. Logs Directory
- **Default:** `/mnt/user/appdata/plexbot/logs`
- **Required:** ✅ Yes
- **Recommendation:** Keep default

## Common Mistakes to Avoid

### ❌ Using GitHub URL as Repository
**Wrong:** `https://github.com/julesdg6/plexbot`
**Right:** `ghcr.io/julesdg6/plexbot:combined`

### ❌ Wrong Plex URL Format
**Wrong:** `192.168.1.100:32400` or `https://192.168.1.100:32400`
**Right:** `http://192.168.1.100:32400`

### ❌ Forgetting to Enable Discord Intents
- Must enable Message Content Intent in Discord Developer Portal
- Without this, bot won't respond to commands

### ❌ Using Wrong Network Type
**Wrong:** Host or Custom (for beginners)
**Right:** Bridge

### ❌ Enabling Privileged Mode
- PlexBot doesn't need this
- Less secure if enabled

## Verification Steps

After applying the container configuration:

1. **Check container starts:** Unraid Docker tab shows container as "Started"
2. **View logs:** Click container → Logs
3. **Look for success messages:**
   - "Lavalink is ready!"
   - "PlexBot application started"
   - "Connected to Discord"
4. **Test in Discord:** Bot should show as online

## Quick Troubleshooting

### Container won't start
- Check all required variables are filled in
- Verify Discord token is valid
- Ensure sufficient disk space

### Bot offline in Discord
- Verify Discord token is correct
- Check bot was invited to server
- Confirm intents are enabled in Discord Developer Portal

### Can't play music
- Verify Plex URL is correct format
- Check Plex token is valid
- Ensure bot has voice permissions in Discord

## Need More Help?

- **Full Guide:** [UNRAID-SETUP-GUIDE.md](./UNRAID-SETUP-GUIDE.md)
- **GitHub Issues:** https://github.com/julesdg6/plexbot/issues
- **Discord Support:** https://discord.com/invite/5m4Wyu52Ek
- **Unraid Forums:** https://forums.unraid.net/

## Summary

The key answers for Unraid Docker configuration:

```
Name: PlexBot-AllInOne
Repository: ghcr.io/julesdg6/plexbot:combined
Registry URL: https://ghcr.io
Icon URL: https://raw.githubusercontent.com/julesdg6/plexbot/main/Images/plexbot-unraid-icon.png
WebUI: (empty)
Extra Parameters: (empty)
Post Arguments: (empty)
Network Type: Bridge
Console shell: bash
Privileged: false
Use Tailscale: No
```

Then configure your Discord token, Plex URL, Plex token, and you're ready to go!
