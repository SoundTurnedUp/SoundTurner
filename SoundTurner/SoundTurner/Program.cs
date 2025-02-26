using NAudio.Wave;

namespace SoundTurner
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "SoundTurner";
            Console.CursorVisible = false;
            
            Menu menu = new Menu();
            menu.SoundMenu();
        }
    }
    class ConsoleUI
    {
        private static readonly ConsoleColor DefaultForeground = Console.ForegroundColor;
        private static readonly ConsoleColor DefaultBackground = Console.BackgroundColor;
        public static void SetColour(ConsoleColor foreground, ConsoleColor background = ConsoleColor.Black)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }
        public static void ResetColour()
        {
            Console.ForegroundColor = DefaultForeground;
            Console.BackgroundColor = DefaultBackground;
        }
        public static void DisplayMessage(string message, int displayMilliseconds = 1500, ConsoleColor foregroundColor = ConsoleColor.White)
        {
            Console.Clear();
            Console.ForegroundColor = foregroundColor;

            DrawBox(message, 4, true, foregroundColor);

            Thread.Sleep(displayMilliseconds);
            ResetColour();
            Console.Clear();
        }
        public static void Pause()
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Press any key to continue...");
            ResetColour();
            Console.ReadKey(true);
            Console.Clear();
        }
        public static int SelectionMenu(string title, List<string> options, int startY = 2)
        {
            int currentSelection = 0;
            bool selectionMade = false;
            ConsoleKey key;

            while (!selectionMade)
            {
                Console.Clear();
                DrawBox(title, 1, true, ConsoleColor.DarkCyan);

                for (int i = 0; i < options.Count; i++)
                {
                    Console.SetCursorPosition(2, startY + i + 1);

                    if (i == currentSelection)
                    {
                        SetColour(ConsoleColor.Black, ConsoleColor.Blue);
                        Console.Write(" > " + options[i].PadRight(Console.WindowWidth - 6) + " ");
                        ResetColour();
                    }
                    else
                    {
                        Console.Write("   " + options[i]);
                    }
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        currentSelection = Math.Max(0, currentSelection - 1);
                        break;
                    case ConsoleKey.DownArrow:
                        currentSelection = Math.Min(options.Count - 1, currentSelection + 1);
                        break;
                    case ConsoleKey.Enter:
                        selectionMade = true;
                        break;
                }
            }

            return currentSelection;
        }

        public static string GetInput(string prompt)
        {
            Console.Clear();
            DrawBox(prompt, 1, true, ConsoleColor.Yellow);
            Console.SetCursorPosition(2, 4);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("> ");
            ResetColour();
            Console.CursorVisible = true;
            string input = Console.ReadLine();
            Console.CursorVisible = false;
            return input;
        }

        public static void DrawBox(string content, int padding = 2, bool centered = false, ConsoleColor titleColour = ConsoleColor.White)
        {
            int width = Console.WindowWidth - 4;

            string[] lines = content.Split('\n');
            int maxLineLength = lines.Max(line => line.Length);
            int boxWidth = Math.Min(width, maxLineLength + padding * 2);

            Console.SetCursorPosition(1, 1);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("╔");
            for (int i = 0; i < boxWidth; i++) 
                Console.Write("═");
            Console.WriteLine("╗");

            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(1, 2 + i);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("║");

                if (i == 0 && titleColour != ConsoleColor.White)
                    Console.ForegroundColor = titleColour;
                else
                    ResetColour();

                if (centered)
                {
                    int spaces = (boxWidth - lines[i].Length) / 2;
                    Console.Write(new string(' ', spaces) + lines[i] + new string(' ', boxWidth - spaces - lines[i].Length));
                }
                else
                {
                    Console.Write(new string(' ', padding) + lines[i].PadRight(boxWidth - padding * 2) + new string(' ', padding));
                }
                ResetColour();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("║");
            }
            Console.SetCursorPosition(1, 2 + lines.Length);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("╚");
            for (int i = 0; i < boxWidth; i++)
                Console.Write("═");
            Console.Write("╝");

            ResetColour();
            Console.WriteLine();
        }
        public static string ProgressBar(double percentage, int length = 20)
        {
            int filledLength = (int)Math.Round(length * percentage / 100);
            return "[" + new string('█', filledLength) + new string('░', length - filledLength) + "]";
        }
    }
    
    class MusicFileManager
    {
        private string MusicDirectory;

        public MusicFileManager(string directory = null)
        {
            MusicDirectory = directory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "SoundTurner");

            if (!Directory.Exists(MusicDirectory))
            {
                Directory.CreateDirectory(MusicDirectory);
            }
        }
        
        public List<Song> GetAllSongs()
        {
            List<Song> songs = new List<Song>();

            try
            {
                string[] supportedExtensions = { ".mp3", ".wav", ".aac", ".wma", ".m4a" };

                var files = Directory.GetFiles(MusicDirectory, "*.*", SearchOption.AllDirectories)
                    .Where(file => supportedExtensions.Contains(Path.GetExtension(file)
                    .ToLower()));

                foreach (string file in files)
                {
                    try
                    {
                        string title = Path.GetFileNameWithoutExtension(file);
                        string artist = "Unknown";

                        TimeSpan duration = GetAudioDuration(file);

                        songs.Add(new Song(title, artist, file, duration));
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error accessing music directory: {e.Message}");
            }

            return songs;
        }

        public List<Song> SearchSongs(string query)
        {
            return GetAllSongs()
                .Where(s => s.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.Artist.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public TimeSpan GetAudioDuration(string filePath)
        {
            try
            {
                using (var audioFile = new AudioFileReader(filePath))
                {
                    return audioFile.TotalTime;
                }
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }
    }

    class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string FilePath { get; set; }
        public TimeSpan Duration { get; set; }

        public Song(string title, string artist = "", string filePath = "", TimeSpan? duration = null)
        {
            Title = title;
            Artist = artist ?? "";
            FilePath = filePath ?? "";
            Duration = duration ?? TimeSpan.Zero;
        }

        public override string ToString()
        {
            string display = Title;
            if (!string.IsNullOrEmpty(Artist)) display += " - " + Artist;
            if (Duration != TimeSpan.Zero) display += $" ({Duration.Minutes}:{Duration.Seconds:D2})";

            return display;
        }
    }

    class AudioPlayer : IDisposable
    {
        private IWavePlayer OutputDevice;
        private AudioFileReader AudioFile;
        private CancellationTokenSource CancellationToken;
        private Task PlaybackMonitorTask;

        public bool IsPlaying { get; private set; }
        public Song? CurrentSong { get; private set; }
        public double Volume { get; set; } = 0.75;
        public double PlaybackPosition => AudioFile?.CurrentTime.TotalSeconds ?? 0;
        public double TotalDuration => AudioFile?.TotalTime.TotalSeconds ?? 0;
        public double PlaybackPercentage => TotalDuration > 0 ? (PlaybackPosition / TotalDuration * 100) : 0;

        public event EventHandler PlaybackFinished;

        public AudioPlayer()
        {
            try
            {
                OutputDevice = new WaveOutEvent();
                ((WaveOutEvent)OutputDevice).DesiredLatency = 200;
            }
            catch (Exception e)
            {
                ConsoleUI.DisplayMessage($"Audio initialization error: {e.Message}\nPlayback may not work on this system.", 3000, ConsoleColor.Red);
            }
        }


        public bool Play(Song song)
        {
            if (song == null || string.IsNullOrEmpty(song.FilePath) || !File.Exists(song.FilePath))
            {
                ConsoleUI.DisplayMessage("Cannot play: invalid or missing file!", 1500, ConsoleColor.Red);
                return false;
            }

            try
            {
                Stop();

                AudioFile = new AudioFileReader(song.FilePath);
                AudioFile.Volume = (float)Volume;

                OutputDevice.Init(AudioFile);
                OutputDevice.Play();

                CurrentSong = song;
                IsPlaying = true;

                StartPlaybackMonitor();

                ConsoleUI.DisplayMessage($"Now playing: {song}", 1500);
                return true;
            }
            catch (Exception e)
            {
                ConsoleUI.DisplayMessage($"Playback error: {e.Message}", 2000, ConsoleColor.Red);
                CleanupResources();
                return false;
            }
        }

        public void Pause()
        {
            if (IsPlaying && OutputDevice != null)
            {
                OutputDevice.Pause();
                IsPlaying = false;
                ConsoleUI.DisplayMessage("Playback paused", 1000);
            }
        }

        public void Resume()
        {
            if (CurrentSong != null && !IsPlaying && OutputDevice != null)
            {
                OutputDevice.Play();
                IsPlaying = true;
                ConsoleUI.DisplayMessage("Playback resumed", 1000);
            }
        }

        public void Stop()
        {
            if (OutputDevice != null)
            {
                CancelPlaybackMonitor();

                if (IsPlaying)
                {
                    OutputDevice.Stop();
                    ConsoleUI.DisplayMessage("Playback stopped", 1000);
                }

                CleanupResources();

                IsPlaying = false;
                CurrentSong = null;
            }
        }

        public void SetVolume(double volume)
        {
            volume = Math.Clamp(volume, 0, 1);
            if (AudioFile != null)
            {
                AudioFile.Volume = (float)volume;
            }
        }

        private void StartPlaybackMonitor()
        {
            CancelPlaybackMonitor();

            CancellationToken = new CancellationTokenSource();
            PlaybackMonitorTask = Task.Run(async () =>
            {
                while (!CancellationToken.Token.IsCancellationRequested)
                {
                    if (OutputDevice.PlaybackState == PlaybackState.Stopped && IsPlaying)
                    {
                        IsPlaying = false;
                        PlaybackFinished?.Invoke(this, EventArgs.Empty);
                        break;
                    }

                    await Task.Delay(500, CancellationToken.Token);
                }
            }, CancellationToken.Token);
        }
        
        private void CancelPlaybackMonitor()
        {
            CancellationToken?.Cancel();

            try
            {
                PlaybackMonitorTask?.Wait(500);
            }
            catch (AggregateException)
            {
                //expected
            }

            CancellationToken?.Dispose();
            CancellationToken = null;
            PlaybackMonitorTask = null;
        }

        private void CleanupResources()
        {
            if (AudioFile != null)
            {
                AudioFile.Dispose();
                AudioFile = null;
            }
        }

        public void Dispose()
        {
            Stop();

            if (OutputDevice != null)
            {
                OutputDevice.Dispose();
                OutputDevice = null;
            }
        }
    }

    class Menu
    {
        private Queue currentQueue;
        private AudioPlayer player;
        private MusicFileManager fileManager;
        private bool autoPlay = true;

        public Menu()
        {
            currentQueue = new Queue();
            player = new AudioPlayer();
            fileManager = new MusicFileManager();

            player.PlaybackFinished += (sender, e) =>
            {
                if (autoPlay && !currentQueue.IsEmpty())
                {
                    PlayNext();
                }
            };
        }
        public void SoundMenu()
        {
            bool running = true;

            while (running)
            {
                List<string> options = new List<string>()
                {
                    "Add song to queue",
                    "Remove song from queue",
                    "View the current queue",
                    "Browse music library",
                    "Play next song",
                    "Pause/Resume playback",
                    "Stop playback",
                    "Adjust volume",
                    "Autoplay: " + (autoPlay ? "ON" : "OFF"),
                    "Quit SoundTurner"
                };

                string title = "SoundTurner\nMusic Player";
                if (player.CurrentSong != null)
                {
                    string playState = player.IsPlaying ? "▶" : "⏸";
                    double position = player.PlaybackPosition;
                    double duration = player.TotalDuration;

                    title += $"\n\nNow Playing: {player.CurrentSong} [{playState}]";
                    title += $"\n{TimeSpan.FromSeconds(position):mm\\:ss} / {TimeSpan.FromSeconds(duration):mm\\:ss}";
                    title += $"\n{ConsoleUI.ProgressBar(player.PlaybackPercentage)}";
                }

                int selection = ConsoleUI.SelectionMenu(title, options);

                switch (selection)
                {
                    case 0: //add song
                        AddSong();
                        break;
                    case 1: //remove song
                        RemoveSong();
                        break;
                    case 2: //view queue
                        currentQueue.PrintQueue();
                        break; 
                    case 3: //browse library
                        BrowseLibrary();
                        break;
                    case 4: //play next
                        PlayNext();
                        break;
                    case 5: //pause/resume
                        if (player.IsPlaying) player.Pause();
                        else player.Resume();
                        break;
                    case 6: //stop
                        player.Stop();
                        break;
                    case 7: //adjust volume
                        AdjustVolume();
                        break;
                    case 8: //toggle autoplay
                        autoPlay = !autoPlay;
                        ConsoleUI.DisplayMessage($"Autoplay: {(autoPlay ? "ON" : "OFF")}", 1000);
                        break;
                    case 9: //quit
                        player.Dispose();
                        ConsoleUI.DisplayMessage("Thanks for using SoundTurner!\nQuitting...", 1500, ConsoleColor.DarkCyan);
                        running = false;
                        break;
                }
            }
        }
        
        private void AddSong()
        {
            List<string> options = new List<string>()
            {
                "Enter song details manually",
                "Browse music library",
                "Search music library",
                "Cancel"
            };

            int selection = ConsoleUI.SelectionMenu("How would you like to add a song?", options);

            switch (selection)
            {
                case 0: //manual entry
                    EnterSongManually();
                    break;
                case 1: //browse library
                    BrowseAndAdd();
                    break;
                case 2: //search library
                    SearchAndAdd();
                    break;
            }
        }

        private void EnterSongManually()
        {
            string title = ConsoleUI.GetInput("Enter song title:");
            if (string.IsNullOrWhiteSpace(title))
                return;

            string artist = ConsoleUI.GetInput("Enter artist name:");

            Song newSong = new Song(title, artist);
            currentQueue.EnQueue(newSong);
        }

        private void BrowseAndAdd()
        {
            List<Song> songs = fileManager.GetAllSongs();

            if (songs.Count == 0)
            {
                ConsoleUI.DisplayMessage("No songs found in the music library.\nAdd songs to your music folder and try again.", 2000, ConsoleColor.Red);
                return;
            }

            List<string> options = songs.Select(s => s.ToString()).ToList();
            options.Add("Cancel");

            int selection = ConsoleUI.SelectionMenu("Select a song to add to queue", options);

            if (selection < songs.Count)
            {
                currentQueue.EnQueue(songs[selection]);
            }
        }

        private void SearchAndAdd()
        {
            string query = ConsoleUI.GetInput("Enter search term");
            if (string.IsNullOrWhiteSpace(query)) return;

            List<Song> results = fileManager.SearchSongs(query);

            if (results.Count == 0)
            {
                ConsoleUI.DisplayMessage($"no songs found matching '{query}'", 1500, ConsoleColor.Red);
                return;
            }

            List<string> options = results.Select(s => s.ToString()).ToList();
            options.Add("Cancel");

            int selection = ConsoleUI.SelectionMenu($"Search results for '{query}':", options);

            if (selection < results.Count)
            {
                currentQueue.EnQueue(results[selection]);
            }
        }
        private void RemoveSong()
        {
            if (currentQueue.IsEmpty())
            {
                ConsoleUI.DisplayMessage("Song cannot be removed as queue is empty!", 1500, ConsoleColor.Red);
                return;
            }

            List<Song> songs = currentQueue.GetAllSongs();
            List<string> options = songs.Select(s => s.ToString()).ToList();
            options.Add("Cancel");

            int selection = ConsoleUI.SelectionMenu("Select a song to remove:", options);

            if (selection < songs.Count)
            {
                currentQueue.RemoveSongAt(selection);
                ConsoleUI.DisplayMessage($"'{songs[selection]}' has been removed from the queue!", 1500);
            }
        }

        private void BrowseLibrary()
        {
            List<Song> songs = fileManager.GetAllSongs();

            if (songs.Count == 0)
            {
                ConsoleUI.DisplayMessage("No songs found in the music library.\nAdd songs to your music folder and try again.", 2000, ConsoleColor.Red);
                return;
            }

            List<string> options = songs.Select(s => s.ToString()).ToList();
            options.Add("Back to main menu");

            int selection = ConsoleUI.SelectionMenu("Music library", options);

            if (selection < songs.Count)
            {
                List<string> songOptions = new List<string>()
                {
                    "Play now",
                    "Add to queue",
                    "Back to library"
                };

                int action = ConsoleUI.SelectionMenu($"Selected: {songs[selection]}", songOptions);

                switch (action)
                {
                    case 0: //play now
                        player.Play(songs[selection]);
                        break;
                    case 1: //Add to queue
                        currentQueue.EnQueue(songs[selection]);
                        break;
                }
            }
        }
        private void PlayNext()
        {
            if (currentQueue.IsEmpty())
            {
                ConsoleUI.DisplayMessage("Queue is empty! Add some songs first.", 1500, ConsoleColor.Red);
                return;
            }

            Song nextSong = currentQueue.DeQueue();
            if (nextSong != null)
            {
                player.Play(nextSong);
            }
        }

        private void AdjustVolume()
        {
            bool adjusting = true;
            int volume = (int)(player.Volume * 100);

            while (adjusting)
            {
                Console.Clear();
                string volumeBar = ConsoleUI.ProgressBar(volume, 30);
                ConsoleUI.DrawBox($"Volume Control\n\n{volumeBar} {volume}%\n\nUse left/right arrows to adjust volume\nPress enter to save", 2, true, ConsoleColor.DarkCyan);

                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        volume = Math.Min(100, volume - 5);
                        player.SetVolume(volume / 100.0);
                        break;
                    case ConsoleKey.RightArrow:
                        volume = Math.Min(100, volume + 5);
                        player.SetVolume(volume / 100.0);
                        break;
                    case ConsoleKey.Enter:
                        adjusting = false;
                        break;
                }
            }
        }
    }

    class Queue
    {
        private List<Song> TheQueue;

        public Queue()
        {
            TheQueue = new List<Song>();
        }

        public void PrintQueue()
        {
            if (IsEmpty())
            {
                ConsoleUI.DisplayMessage("The queue is empty, add some songs first!", 1500, ConsoleColor.Red);
                return;
            }

            string queueContent = "Current Queue:\n\n";
            for (int i = 0; i < TheQueue.Count; i++)
            {
                queueContent += $"{i + 1}. {TheQueue[i]}\n";
            }

            Console.Clear();
            ConsoleUI.DrawBox(queueContent, 2, false);
            ConsoleUI.Pause();
        }

        public void EnQueue(Song song)
        {
            TheQueue.Add(song);
            ConsoleUI.DisplayMessage($"'{song}' has been added to the queue.", 1500);
        }
        public Song DeQueue()
        {
            if (IsEmpty())
            {
                ConsoleUI.DisplayMessage("Song cannot be removed as the queue is empty!", 1500, ConsoleColor.Red);
                return null;
            }
            Song nextSong = TheQueue[0];
            TheQueue.RemoveAt(0);
            return nextSong;
        }
        public void RemoveSongAt(int index)
        {
            if (index >= 0 && index < TheQueue.Count)
            {
                TheQueue.RemoveAt(index);
            }
        }
        public List<Song> GetAllSongs()
        {
            return new List<Song>(TheQueue);
        }
        public bool IsEmpty()
        {
            return TheQueue.Count == 0;
        }
    }
}