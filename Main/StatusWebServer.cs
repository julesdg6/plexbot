using System.Net;
using System.Text;
using PlexBot.Utils;

namespace PlexBot.Main;

/// <summary>Simple HTTP server that provides a status page for monitoring container health and configuration</summary>
public class StatusWebServer : IHostedService, IDisposable
{
    private HttpListener? _listener;
    private CancellationTokenSource? _cts;
    private Task? _listenerTask;
    private readonly int _port;

    public StatusWebServer()
    {
        _port = EnvConfig.GetInt("STATUS_WEB_PORT", 8080);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{_port}/");
            _listener.Start();
            _cts = new CancellationTokenSource();
            
            Logs.Init($"Status web server started on port {_port}");
            
            _listenerTask = Task.Run(() => HandleRequestsAsync(_cts.Token), _cts.Token);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Logs.Error($"Failed to start status web server: {ex.Message}");
            return Task.CompletedTask;
        }
    }

    private async Task HandleRequestsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _listener != null && _listener.IsListening)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                _ = Task.Run(() => ProcessRequestAsync(context), cancellationToken);
            }
            catch (HttpListenerException)
            {
                // Listener was stopped
                break;
            }
            catch (Exception ex)
            {
                Logs.Error($"Error handling web request: {ex.Message}");
            }
        }
    }

    private async Task ProcessRequestAsync(HttpListenerContext context)
    {
        try
        {
            string response = GenerateStatusPage();
            byte[] buffer = Encoding.UTF8.GetBytes(response);
            
            context.Response.ContentType = "text/html; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.StatusCode = 200;
            
            await context.Response.OutputStream.WriteAsync(buffer);
            context.Response.Close();
        }
        catch (Exception ex)
        {
            Logs.Error($"Error processing web request: {ex.Message}");
            try
            {
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
            catch { }
        }
    }

    private string GenerateStatusPage()
    {
        // Check configuration status
        string discordToken = EnvConfig.Get("DISCORD_TOKEN", "");
        string plexUrl = EnvConfig.Get("PLEX_URL", "");
        string plexToken = EnvConfig.Get("PLEX_TOKEN", "");
        string lavalinkHost = EnvConfig.Get("LAVALINK_HOST", "localhost");
        string lavalinkPort = EnvConfig.Get("LAVALINK_SERVER_PORT", "2333");
        string lavalinkPassword = EnvConfig.Get("LAVALINK_SERVER_PASSWORD", "");
        
        bool hasDiscordToken = !string.IsNullOrWhiteSpace(discordToken) && discordToken != "YOUR_BOT_TOKEN_HERE";
        bool hasPlexUrl = !string.IsNullOrWhiteSpace(plexUrl) && plexUrl != "http://YOUR_PLEX_IP:32400" && plexUrl != "http://PUBLIC_URL:YOUR_PORT";
        bool hasPlexToken = !string.IsNullOrWhiteSpace(plexToken) && plexToken != "YOUR_PLEX_TOKEN_HERE";
        bool hasLavalinkPassword = !string.IsNullOrWhiteSpace(lavalinkPassword);
        
        bool isConfigured = hasDiscordToken && hasPlexUrl && hasPlexToken && hasLavalinkPassword;
        
        string statusColor = isConfigured ? "#28a745" : "#dc3545";
        string statusText = isConfigured ? "Configured" : "Needs Configuration";
        
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset='utf-8'>");
        html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1'>");
        html.AppendLine("    <title>PlexBot Status</title>");
        html.AppendLine("    <style>");
        html.AppendLine("        * { margin: 0; padding: 0; box-sizing: border-box; }");
        html.AppendLine("        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; background: #0d1117; color: #c9d1d9; padding: 20px; }");
        html.AppendLine("        .container { max-width: 900px; margin: 0 auto; }");
        html.AppendLine("        .header { text-align: center; margin-bottom: 30px; padding: 30px 0; border-bottom: 1px solid #30363d; }");
        html.AppendLine("        .header h1 { font-size: 2.5em; margin-bottom: 10px; color: #58a6ff; }");
        html.AppendLine("        .status-badge { display: inline-block; padding: 8px 16px; border-radius: 20px; font-weight: 600; font-size: 0.9em; margin-top: 10px; }");
        html.AppendLine("        .card { background: #161b22; border: 1px solid #30363d; border-radius: 6px; padding: 20px; margin-bottom: 20px; }");
        html.AppendLine("        .card h2 { font-size: 1.5em; margin-bottom: 15px; color: #f0f6fc; }");
        html.AppendLine("        .config-item { display: flex; justify-content: space-between; align-items: center; padding: 12px; margin: 8px 0; background: #0d1117; border-radius: 6px; border: 1px solid #30363d; }");
        html.AppendLine("        .config-label { font-weight: 500; color: #8b949e; }");
        html.AppendLine("        .config-value { font-family: 'Courier New', monospace; color: #c9d1d9; }");
        html.AppendLine("        .status-icon { display: inline-block; width: 12px; height: 12px; border-radius: 50%; margin-right: 8px; }");
        html.AppendLine("        .status-ok { background: #28a745; }");
        html.AppendLine("        .status-error { background: #dc3545; }");
        html.AppendLine("        .alert { padding: 15px; border-radius: 6px; margin-bottom: 20px; border-left: 4px solid; }");
        html.AppendLine("        .alert-danger { background: #2c1a1d; border-color: #dc3545; color: #f85149; }");
        html.AppendLine("        .alert-success { background: #1a2c1e; border-color: #28a745; color: #3fb950; }");
        html.AppendLine("        .alert h3 { margin-bottom: 10px; font-size: 1.1em; }");
        html.AppendLine("        .alert ul { margin-left: 20px; margin-top: 10px; }");
        html.AppendLine("        .alert li { margin: 5px 0; }");
        html.AppendLine("        code { background: #0d1117; padding: 2px 6px; border-radius: 3px; font-family: 'Courier New', monospace; }");
        html.AppendLine("        a { color: #58a6ff; text-decoration: none; }");
        html.AppendLine("        a:hover { text-decoration: underline; }");
        html.AppendLine("        .footer { text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #30363d; color: #8b949e; font-size: 0.9em; }");
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        html.AppendLine("    <div class='container'>");
        html.AppendLine("        <div class='header'>");
        html.AppendLine("            <h1>🎵 PlexBot</h1>");
        html.AppendLine($"            <div class='status-badge' style='background: {statusColor}; color: white;'>{statusText}</div>");
        html.AppendLine("        </div>");
        
        if (!isConfigured)
        {
            html.AppendLine("        <div class='alert alert-danger'>");
            html.AppendLine("            <h3>⚠️ Configuration Required</h3>");
            html.AppendLine("            <p>PlexBot requires the following environment variables to be configured:</p>");
            html.AppendLine("            <ul>");
            if (!hasDiscordToken) html.AppendLine("                <li><strong>DISCORD_TOKEN</strong> - Get from <a href='https://discord.com/developers/applications' target='_blank'>Discord Developer Portal</a></li>");
            if (!hasPlexUrl) html.AppendLine("                <li><strong>PLEX_URL</strong> - Your Plex server URL (e.g., http://192.168.1.100:32400)</li>");
            if (!hasPlexToken) html.AppendLine("                <li><strong>PLEX_TOKEN</strong> - See <a href='https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/' target='_blank'>Plex documentation</a></li>");
            if (!hasLavalinkPassword) html.AppendLine("                <li><strong>LAVALINK_SERVER_PASSWORD</strong> - Set a secure password for Lavalink</li>");
            html.AppendLine("            </ul>");
            html.AppendLine("            <p style='margin-top: 15px;'>In Unraid, configure these in the Docker container settings. See the <a href='https://github.com/julesdg6/plexbot/blob/main/unraid-template/UNRAID-SETUP-GUIDE.md' target='_blank'>Setup Guide</a> for details.</p>");
            html.AppendLine("        </div>");
        }
        else
        {
            html.AppendLine("        <div class='alert alert-success'>");
            html.AppendLine("            <h3>✅ Configuration Complete</h3>");
            html.AppendLine("            <p>All required settings are configured. PlexBot should be running.</p>");
            html.AppendLine("        </div>");
        }
        
        html.AppendLine("        <div class='card'>");
        html.AppendLine("            <h2>Configuration Status</h2>");
        
        html.AppendLine("            <div class='config-item'>");
        html.AppendLine("                <span class='config-label'>Discord Bot Token</span>");
        html.AppendLine($"                <span class='config-value'><span class='status-icon {(hasDiscordToken ? "status-ok" : "status-error")}'></span>{(hasDiscordToken ? "Configured" : "Not Set")}</span>");
        html.AppendLine("            </div>");
        
        html.AppendLine("            <div class='config-item'>");
        html.AppendLine("                <span class='config-label'>Plex Server URL</span>");
        // Mask the URL to hide internal network details, only show if it's set
        string maskedPlexUrl = hasPlexUrl ? "Configured (hidden for security)" : "Not Set";
        html.AppendLine($"                <span class='config-value'><span class='status-icon {(hasPlexUrl ? "status-ok" : "status-error")}'></span>{maskedPlexUrl}</span>");
        html.AppendLine("            </div>");
        
        html.AppendLine("            <div class='config-item'>");
        html.AppendLine("                <span class='config-label'>Plex Token</span>");
        html.AppendLine($"                <span class='config-value'><span class='status-icon {(hasPlexToken ? "status-ok" : "status-error")}'></span>{(hasPlexToken ? "Configured" : "Not Set")}</span>");
        html.AppendLine("            </div>");
        
        html.AppendLine("            <div class='config-item'>");
        html.AppendLine("                <span class='config-label'>Lavalink Host</span>");
        html.AppendLine($"                <span class='config-value'><span class='status-icon status-ok'></span>{lavalinkHost} (not validated)</span>");
        html.AppendLine("            </div>");
        
        html.AppendLine("            <div class='config-item'>");
        html.AppendLine("                <span class='config-label'>Lavalink Port</span>");
        html.AppendLine($"                <span class='config-value'><span class='status-icon status-ok'></span>{lavalinkPort} (not validated)</span>");
        html.AppendLine("            </div>");
        
        html.AppendLine("            <div class='config-item'>");
        html.AppendLine("                <span class='config-label'>Lavalink Password</span>");
        html.AppendLine($"                <span class='config-value'><span class='status-icon {(hasLavalinkPassword ? "status-ok" : "status-error")}'></span>{(hasLavalinkPassword ? "Configured" : "Not Set")}</span>");
        html.AppendLine("            </div>");
        
        html.AppendLine("        </div>");
        
        html.AppendLine("        <div class='card'>");
        html.AppendLine("            <h2>Optional Settings</h2>");
        
        string botPrefix = EnvConfig.Get("BOT_PREFIX", "!");
        string status = EnvConfig.Get("STATUS", "online");
        bool useModernPlayer = EnvConfig.GetBool("USE_MODERN_PLAYER", true);
        bool useStaticChannel = EnvConfig.GetBool("USE_STATIC_PLAYER_CHANNEL", false);
        string staticChannelId = EnvConfig.Get("STATIC_PLAYER_CHANNEL_ID", "Not Set");
        
        html.AppendLine("            <div class='config-item'>");
        html.AppendLine("                <span class='config-label'>Bot Prefix</span>");
        html.AppendLine($"                <span class='config-value'>{botPrefix}</span>");
        html.AppendLine("            </div>");
        
        html.AppendLine("            <div class='config-item'>");
        html.AppendLine("                <span class='config-label'>Bot Status</span>");
        html.AppendLine($"                <span class='config-value'>{status}</span>");
        html.AppendLine("            </div>");
        
        html.AppendLine("            <div class='config-item'>");
        html.AppendLine("                <span class='config-label'>Modern Player UI</span>");
        html.AppendLine($"                <span class='config-value'>{(useModernPlayer ? "Enabled" : "Disabled")}</span>");
        html.AppendLine("            </div>");
        
        html.AppendLine("            <div class='config-item'>");
        html.AppendLine("                <span class='config-label'>Static Player Channel</span>");
        html.AppendLine($"                <span class='config-value'>{(useStaticChannel ? $"Enabled ({staticChannelId})" : "Disabled")}</span>");
        html.AppendLine("            </div>");
        
        html.AppendLine("        </div>");
        
        html.AppendLine("        <div class='footer'>");
        html.AppendLine("            <p>PlexBot - Play your Plex music in Discord</p>");
        html.AppendLine("            <p><a href='https://github.com/julesdg6/plexbot' target='_blank'>GitHub</a> | <a href='https://github.com/julesdg6/plexbot/blob/main/unraid-template/UNRAID-SETUP-GUIDE.md' target='_blank'>Documentation</a></p>");
        html.AppendLine("        </div>");
        html.AppendLine("    </div>");
        html.AppendLine("</body>");
        html.AppendLine("</html>");
        
        return html.ToString();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _cts?.Cancel();
            _listener?.Stop();
            _listener?.Close();
            Logs.Info("Status web server stopped");
        }
        catch (Exception ex)
        {
            Logs.Error($"Error stopping status web server: {ex.Message}");
        }
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _cts?.Dispose();
        _listener?.Close();
        GC.SuppressFinalize(this);
    }
}
