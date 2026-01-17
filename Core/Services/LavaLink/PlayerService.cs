using PlexBot.Core.Discord.Embeds;
using PlexBot.Core.Exceptions;
using PlexBot.Core.Models.Media;
using PlexBot.Core.Models.Players;
using PlexBot.Core.Services;
using PlexBot.Utils;

namespace PlexBot.Core.Services.LavaLink;

/// <summary>Comprehensive service that manages audio playback in Discord voice channels, handling player lifecycle, track queueing, and providing rich metadata integration with Plex</summary>
/// <remarks>Constructs the player service with necessary dependencies and loads configuration from environment variables to ensure consistent playback settings</remarks>
/// <param name="audioService">The Lavalink audio service that provides the underlying audio streaming capabilities</param>
public class PlayerService(VisualPlayerStateManager stateManager, IAudioService audioService, VisualPlayer visualPlayer, IServiceProvider serviceProvider)
    : IPlayerService
{
    // Configuration constants
    private const float DefaultVolume = 0.2f;
    private const int BatchSize = 3;
    private const int TrackDelayMs = 100;
    private const int BatchDelayMs = 300;
    private const int RetryDelayMs = 500;

    private readonly TimeSpan _inactivityTimeout = TimeSpan.FromMinutes(EnvConfig.GetDouble("PLAYER_INACTIVITY_TIMEOUT", 2.0));

    /// <inheritdoc />
    public async Task<QueuedLavalinkPlayer?> GetPlayerAsync(IDiscordInteraction interaction, bool connectToVoiceChannel = true,
        CancellationToken cancellationToken = default)
    {
        // Check if the user is in a voice channel
        if (interaction.User is not IGuildUser user || user.VoiceChannel == null)
        {
            await interaction.FollowupAsync("You must be in a voice channel to use the music player.", ephemeral: true);
            return null;
        }
        try
        {
            // Get guild and channel information
            ulong guildId = user.Guild.Id;
            ulong voiceChannelId = user.VoiceChannel.Id;
            // Determine channel behavior based on connectToVoiceChannel parameter
            PlayerChannelBehavior channelBehavior = connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None;
            PlayerRetrieveOptions retrieveOptions = new(channelBehavior);
            // Create player options
            CustomPlayerOptions playerOptions = new()
            {
                DisconnectOnStop = false,
                SelfDeaf = true,
                // Get text channel based on interaction type
                TextChannel = interaction is SocketInteraction socketInteraction
                    ? socketInteraction.Channel as ITextChannel
                    : null,
                InactivityTimeout = _inactivityTimeout,
                DefaultVolume = DefaultVolume,
            };
            // Wrap options for DI
            var optionsWrapper = Options.Create(playerOptions);
            // Retrieve or create the player
            PlayerResult<CustomLavaLinkPlayer> result = await audioService.Players
            .RetrieveAsync<CustomLavaLinkPlayer, CustomPlayerOptions>(guildId, voiceChannelId,
                (properties, token) => ValueTask.FromResult(new CustomLavaLinkPlayer(properties, serviceProvider)),
                optionsWrapper, retrieveOptions, cancellationToken).ConfigureAwait(false);
            // Handle retrieval failures
            if (!result.IsSuccess)
            {
                string errorMessage = result.Status switch
                {
                    PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
                    PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected to a voice channel.",
                    _ => "An unknown error occurred while trying to retrieve the player."
                };
                await interaction.FollowupAsync(errorMessage, ephemeral: true);
                return null;
            }
            // Set volume if it's a new player
            if (result.Status == PlayerRetrieveStatus.Success)
            {
                await result.Player.SetVolumeAsync(DefaultVolume, cancellationToken);
                Logs.Debug($"Created new player for guild {guildId} with volume {DefaultVolume * 100:F0}%");
            }
            return result.Player;
        }
        catch (Exception ex)
        {
            Logs.Error($"Error getting player: {ex.Message}");
            throw new PlayerException($"Failed to get player: {ex.Message}", "Connect", ex);
        }
    }

    /// <inheritdoc />
    public async Task PlayTrackAsync(
        IDiscordInteraction interaction,
        Track track,
        CancellationToken cancellationToken = default)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(interaction, true, cancellationToken);

        if (player == null)
        {
            Logs.Warning("Failed to get player for playback");
            return;
        }
        try
        {
            // Load the track through Lavalink
            LavalinkTrack? lavalinkTrack = await LoadTrackAsync(track, cancellationToken);
            if (lavalinkTrack == null)
            {
                Logs.Error($"Failed to load track: {track.Title} from URL: {track.PlaybackUrl}");
                await interaction.FollowupAsync($"Failed to load track: {track.Title}", ephemeral: true);
                return;
            }
            // Create custom queue item with rich metadata
            CustomTrackQueueItem queueItem = CreateQueueItem(track, lavalinkTrack, interaction.User.Username);
            // Set playback options
            TrackPlayProperties playProperties = new()
            {
                NoReplace = true // If something's already playing, add to queue instead of replacing
            };
            // Play the track
            Logs.Debug($"Playing track: {track.Title} by {track.Artist} (requested by {interaction.User.Username})");
            await player.PlayAsync(queueItem, playProperties, cancellationToken);
            await interaction.FollowupAsync($"Playing: {track.Title} by {track.Artist}", ephemeral: true);
        }
        catch (Exception ex)
        {
            Logs.Error($"Error playing track: {ex.Message}");
            throw new PlayerException($"Failed to play track: {ex.Message}", "Play", ex);
        }
    }

    /// <inheritdoc />
    public async Task AddToQueueAsync(IDiscordInteraction interaction, IEnumerable<Track> tracks,
    CancellationToken cancellationToken = default)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(interaction, true, cancellationToken);
        if (player == null)
        {
            Logs.Warning("Failed to get player for queueing");
            return;
        }
        try
        {
            IUserMessage originalResponse = await interaction.GetOriginalResponseAsync();
            ITextChannel? channel = originalResponse.Channel as ITextChannel;
            stateManager.CurrentPlayerChannel = channel ?? throw new InvalidOperationException("CurrentPlayerChannel is not set");
            List<Track> trackList = tracks.ToList();
            int totalCount = trackList.Count;
            Logs.Debug($"Adding {totalCount} tracks to queue");
            // Process tracks in smaller batches
            int successCount = 0;
            bool firstTrackProcessed = false;
            // Send a preliminary message for long playlists
            if (totalCount > 10)
            {
                await interaction.ModifyOriginalResponseAsync(msg =>
                {
                    msg.Embed = DiscordEmbedBuilder.Info("Processing Tracks", $"Processing {totalCount} tracks. This may take a moment...");
                });
            }
            // Keep track of failed tracks for a retry
            List<Track> failedTracks = [];
            // Process in batches
            for (int batchStart = 0; batchStart < totalCount; batchStart += BatchSize)
            {
                // Get the current batch
                int currentBatchSize = Math.Min(BatchSize, totalCount - batchStart);
                List<Track> batch = trackList.Skip(batchStart).Take(currentBatchSize).ToList();
                Logs.Debug($"Processing batch {batchStart / BatchSize + 1} of {Math.Ceiling((double)totalCount / BatchSize)} ({batch.Count} tracks)");
                // Process one track at a time within the batch - more reliable than parallel in small batches
                foreach (Track track in batch)
                {
                    try
                    {
                        LavalinkTrack? lavalinkTrack = await LoadTrackAsync(track, cancellationToken);
                        if (lavalinkTrack == null)
                        {
                            Logs.Warning($"Failed to load track: {track.Title} - will retry later");
                            failedTracks.Add(track);
                            continue;
                        }
                        Logs.Debug($"Successfully loaded track: {track.Title ?? lavalinkTrack.Title}");
                        // Create queue item
                        CustomTrackQueueItem queueItem = CreateQueueItem(track, lavalinkTrack, interaction.User.Username);
                        // Play first track or add to queue
                        if (!firstTrackProcessed && player.State != PlayerState.Playing && player.State != PlayerState.Paused)
                        {
                            Logs.Debug($"Playing first track: {queueItem.Title} by {queueItem.Artist}");
                            await player.PlayAsync(queueItem, cancellationToken: cancellationToken);
                            firstTrackProcessed = true;
                        }
                        else
                        {
                            await player.Queue.AddAsync(queueItem, cancellationToken);
                        }
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        Logs.Error($"Error processing track {track.Title}: {ex.Message}");
                        failedTracks.Add(track);
                    }
                    // Small delay between individual tracks for stability
                    await Task.Delay(TrackDelayMs, cancellationToken);
                }
                // Longer delay between batches
                if (batchStart + BatchSize < totalCount)
                {
                    await Task.Delay(BatchDelayMs, cancellationToken);
                }
            }
            // Try to recover failed tracks (one retry attempt)
            if (failedTracks.Count > 0 && successCount > 0)  // Only retry if we had some successes
            {
                Logs.Debug($"Attempting to recover {failedTracks.Count} failed tracks");

                foreach (Track track in failedTracks)
                {
                    try
                    {
                        // Wait a bit longer before retry
                        await Task.Delay(RetryDelayMs, cancellationToken);
                        Logs.Debug($"Retrying track: {track.Title}");

                        LavalinkTrack? lavalinkTrack = await LoadTrackAsync(track, cancellationToken);
                        if (lavalinkTrack == null)
                        {
                            Logs.Warning($"Failed to load track on retry: {track.Title}");
                            continue;
                        }

                        // Create queue item and add to queue
                        CustomTrackQueueItem queueItem = CreateQueueItem(track, lavalinkTrack, interaction.User.Username);
                        await player.Queue.AddAsync(queueItem, cancellationToken);
                        successCount++;
                        // Pause between retries
                        await Task.Delay(BatchDelayMs, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Logs.Error($"Error retrying track {track.Title}: {ex.Message}");
                    }
                }
            }
            // Final message
            if (successCount > 0)
            {
                string message = $"Added {successCount} of {totalCount} tracks to the queue";
                await interaction.ModifyOriginalResponseAsync(msg =>
                {
                    msg.Embed = DiscordEmbedBuilder.Success("Tracks Added", message);
                });
            }
            else
            {
                await interaction.ModifyOriginalResponseAsync(msg =>
                {
                    msg.Embed = DiscordEmbedBuilder.Error("Failed to Add Tracks", "No tracks were added to the queue.");
                });
            }
        }
        catch (Exception ex)
        {
            Logs.Error($"Error adding tracks to queue: {ex.Message}");
            throw new PlayerException($"Failed to add tracks to queue: {ex.Message}", "Queue", ex);
        }
    }

    /// <inheritdoc />
    public async Task<string> TogglePauseResumeAsync(IDiscordInteraction interaction,
    CancellationToken cancellationToken = default)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(interaction, false, cancellationToken);
        if (player == null)
        {
            Logs.Warning("Failed to get player for pause/resume");
            throw new PlayerException("No active player found", "Pause");
        }
        try
        {
            string result;
            // Toggle state based on current state
            if (player.State == PlayerState.Paused)
            {
                await player.ResumeAsync(cancellationToken);
                Logs.Debug($"Playback resumed by {interaction.User.Username}");
                result = "Resumed";
            }
            else if (player.State == PlayerState.Playing)
            {
                await player.PauseAsync(cancellationToken);
                Logs.Debug($"Playback paused by {interaction.User.Username}");
                result = "Paused";
            }
            else
            {
                throw new PlayerException("No track is currently playing", "Pause");
            }
            // Update player UI if it's our custom player
            if (player is CustomLavaLinkPlayer customPlayer)
            {
                ButtonContext context = new()
                {
                    Player = customPlayer,
                    Interaction = interaction
                };
                ComponentBuilder components = DiscordButtonBuilder.Instance.BuildButtons(ButtonFlag.VisualPlayer, context);
                await visualPlayer.AddOrUpdateVisualPlayerAsync(components);
            }
            return result;
        }
        catch (Exception ex) when (ex is not PlayerException)
        {
            Logs.Error($"Error toggling pause/resume: {ex.Message}");
            throw new PlayerException($"Failed to toggle pause/resume: {ex.Message}", "Pause", ex);
        }
    }

    /// <inheritdoc />
    public async Task SkipTrackAsync(IDiscordInteraction interaction, CancellationToken cancellationToken = default)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(interaction, false, cancellationToken);
        if (player == null)
        {
            Logs.Warning("Failed to get player for skip");
            throw new PlayerException("No active player found", "Skip");
        }
        try
        {
            if (player.State != PlayerState.Playing && player.State != PlayerState.Paused)
            {
                throw new PlayerException("No track is currently playing", "Skip");
            }
            // Get current track info for the message
            string trackTitle = "the current track";
            if (player is CustomLavaLinkPlayer customPlayer && customPlayer.CurrentItem is CustomTrackQueueItem currentTrack)
            {
                trackTitle = currentTrack.Title ?? "the current track";
            }
            // Skip the current track - default to 1 track to fix argument type error
            await player.SkipAsync(1, cancellationToken);
            Logs.Debug($"Track skipped by {interaction.User.Username}");
            await interaction.FollowupAsync($"Skipped {trackTitle}.", ephemeral: true);
        }
        catch (Exception ex) when (ex is not PlayerException)
        {
            Logs.Error($"Error skipping track: {ex.Message}");
            throw new PlayerException($"Failed to skip track: {ex.Message}", "Skip", ex);
        }
    }

    /// <inheritdoc />
    public async Task SetRepeatModeAsync(IDiscordInteraction interaction, TrackRepeatMode repeatMode,
        CancellationToken cancellationToken = default)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(interaction, false, cancellationToken);
        if (player == null)
        {
            Logs.Warning("Failed to get player for setting repeat mode");
            throw new PlayerException("No active player found", "Repeat");
        }
        try
        {
            // Set the repeat mode
            player.RepeatMode = repeatMode;
            string modeDescription = repeatMode switch
            {
                TrackRepeatMode.None => "Repeat mode disabled",
                TrackRepeatMode.Track => "Now repeating current track",
                TrackRepeatMode.Queue => "Now repeating the entire queue",
                _ => "Unknown repeat mode"
            };
            if (player is CustomLavaLinkPlayer customPlayer)
            {
                // Update player UI if it's our custom player
                ButtonContext context = new()
                {
                    Player = customPlayer,
                    Interaction = interaction
                };
                ComponentBuilder components = DiscordButtonBuilder.Instance.BuildButtons(ButtonFlag.VisualPlayer, context);
                await visualPlayer.AddOrUpdateVisualPlayerAsync(components, true); // Update Visual Player image
            }
            Logs.Debug($"Repeat mode set to {repeatMode} by {interaction.User.Username}");
        }
        catch (Exception ex)
        {
            Logs.Error($"Error setting repeat mode: {ex.Message}");
            throw new PlayerException($"Failed to set repeat mode: {ex.Message}", "Repeat", ex);
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(IDiscordInteraction interaction, bool disconnect = false,
        CancellationToken cancellationToken = default)
    {
        QueuedLavalinkPlayer? player = await GetPlayerAsync(interaction, false, cancellationToken);
        if (player == null)
        {
            Logs.Warning("Failed to get player for stop");
            throw new PlayerException("No active player found", "Stop");
        }
        try
        {
            // Stop playback
            await player.StopAsync(cancellationToken);
            // Clear the queue
            await player.Queue.ClearAsync(cancellationToken);
            // Disconnect if requested
            if (disconnect)
            {
                await player.DisconnectAsync(cancellationToken);
                Logs.Debug($"Player stopped and disconnected by {interaction.User.Username}");
            }
            else
            {
                Logs.Debug($"Player stopped by {interaction.User.Username}");
            }
        }
        catch (Exception ex)
        {
            Logs.Error($"Error stopping player: {ex.Message}");
            throw new PlayerException($"Failed to stop player: {ex.Message}", "Stop", ex);
        }
    }

    /// <summary>Creates a CustomTrackQueueItem from track metadata and LavalinkTrack.
    /// Centralizes the logic for building queue items with proper metadata prioritization.</summary>
    /// <param name="track">The source track with Plex metadata</param>
    /// <param name="lavalinkTrack">The Lavalink track instance</param>
    /// <param name="requestedBy">Username of the user who requested the track</param>
    /// <returns>A configured CustomTrackQueueItem</returns>
    private static CustomTrackQueueItem CreateQueueItem(Track track, LavalinkTrack lavalinkTrack, string requestedBy)
    {
        return new CustomTrackQueueItem
        {
            // Prioritize Plex metadata over Lavalink metadata
            Title = track.Title ?? lavalinkTrack.Title ?? "Unknown Title",
            Artist = track.Artist ?? lavalinkTrack.Author ?? "Unknown Artist",
            Album = track.Album,
            ReleaseDate = track.ReleaseDate,
            Artwork = track.ArtworkUrl ?? lavalinkTrack.ArtworkUri?.ToString() ?? "",
            Url = track.PlaybackUrl,
            ArtistUrl = track.ArtistUrl,
            Duration = track.DurationDisplay,
            Studio = track.Studio,
            RequestedBy = requestedBy,
            Reference = new TrackReference(lavalinkTrack)
        };
    }

    /// <summary>Loads a track through Lavalink with proper search mode based on source system.
    /// Handles YouTube tracks with fallback to search mode if direct loading fails.</summary>
    /// <param name="track">The track to load</param>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The loaded LavalinkTrack or null if loading failed</returns>
    private async Task<LavalinkTrack?> LoadTrackAsync(Track track, CancellationToken cancellationToken)
    {
        Logs.Debug($"Loading track: {track.Title}");

        // Handle YouTube tracks differently - they may need search mode fallback
        if (track.SourceSystem.Equals("youtube", StringComparison.OrdinalIgnoreCase))
        {
            // Try direct URL first
            TrackLoadOptions directOptions = new() { SearchMode = TrackSearchMode.None };
            LavalinkTrack? lavalinkTrack = await audioService.Tracks.LoadTrackAsync(
                track.PlaybackUrl,
                directOptions,
                cancellationToken: cancellationToken);

            // Fallback to YouTube search if direct load fails
            if (lavalinkTrack == null)
            {
                TrackLoadOptions searchOptions = new() { SearchMode = TrackSearchMode.YouTube };
                lavalinkTrack = await audioService.Tracks.LoadTrackAsync(
                    track.PlaybackUrl,
                    searchOptions,
                    cancellationToken: cancellationToken);
            }

            return lavalinkTrack;
        }

        // For Plex and other tracks, use direct loading only
        TrackLoadOptions loadOptions = new() { SearchMode = TrackSearchMode.None };
        return await audioService.Tracks.LoadTrackAsync(
            track.PlaybackUrl,
            loadOptions,
            cancellationToken: cancellationToken);
    }
}
