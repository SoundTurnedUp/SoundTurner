namespace SoundTurner
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Menu menu = new Menu();
            menu.SoundMenu();
        }
    }
    class DisplayHelper
    {
        public static void DisplayMessage(string message, int displayMilliseconds = 1500, ConsoleColor colour = ConsoleColor.White)
        {
            Console.Clear();
            Console.ForegroundColor = colour;
            Console.WriteLine(message);
            Thread.Sleep(displayMilliseconds);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
        }
        public static void Pause()
        {
            Console.Write("Press any key to continue:");
            Console.ReadKey();
            Console.Clear();
        }
    }

    class Menu
    {
        string? choice;
        Queue? currentQueue;
        public void SoundMenu()
        {
            Queue queue = new Queue();
            currentQueue = queue;
            do
            {
                Console.Clear();
                Console.WriteLine("= MENU =");
                Console.WriteLine("1. Add song to queue");
                Console.WriteLine("2. Remove song from queue");
                Console.WriteLine("3. View the current queue");
                Console.WriteLine("Q. Quit");
                choice = Console.ReadLine();
                switch (choice.ToLower())
                {
                    case "1":
                    case "add":
                        Console.WriteLine("What song would you like to add?");
                        string newSong = Console.ReadLine();
                        currentQueue.EnQueue(newSong);
                        break;
                    case "2":
                    case "remove":
                        currentQueue.DeQueue();
                        break;
                    case "3":
                    case "view":
                        currentQueue.PrintQueue();
                        break;
                    case "q":
                    case "quit":
                        DisplayHelper.DisplayMessage("Quitting...");
                        break;
                    default:
                        DisplayHelper.DisplayMessage("Invalid input, try again.");
                        break;
                }
            } while (!choice.Equals("q", StringComparison.CurrentCultureIgnoreCase) && !choice.Equals("quit", StringComparison.CurrentCultureIgnoreCase));
        }
    }

    class Queue
    {
        private List<string> TheQueue;

        public Queue()
        {
            TheQueue = new List<string>();
        }

        public void PrintQueue()
        {
            Console.Clear();
            if (IsEmpty())
            {
                DisplayHelper.DisplayMessage("The queue is empty.");
                return;
            }
            Console.WriteLine("Current Queue:");

            foreach (var song in TheQueue)
            {
                Console.WriteLine(song);
            }
            DisplayHelper.Pause();
        }

        public void EnQueue(string song)
        {
            TheQueue.Add(song);
            DisplayHelper.DisplayMessage($"'{song}' has been added to the queue.");
        }
        public void DeQueue()
        {
            if (IsEmpty())
            {
                DisplayHelper.DisplayMessage("Song cannot be removed as the queue is empty!");
            }
            else
            {
                string removedSong = TheQueue[0];
                TheQueue.RemoveAt(0);
                DisplayHelper.DisplayMessage($"'{removedSong}' has been removed from the queue.");
            }
        }
        public bool IsEmpty()
        {
            return TheQueue.Count == 0;
        }
    }
}