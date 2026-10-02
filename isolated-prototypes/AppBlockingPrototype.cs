using System.Diagnostics;

namespace AppBlockingTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter application to block: ");
            string? search = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(search))
            {
                Console.WriteLine("Invalid application name.");
                return;
            }

            Process[] matches = Process.GetProcesses()
                .Where(p => !string.IsNullOrWhiteSpace(p.MainWindowTitle) &&
                    (p.ProcessName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                     p.MainWindowTitle.Contains(search, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            if (matches.Length == 0)
            {
                Console.WriteLine($"No running application matching \"{search}\" found.");
                return;
            }

            Console.WriteLine("\nMatches found:");

            for (int i = 0; i < matches.Length; i++)
                Console.WriteLine($"{i + 1}. {matches[i].MainWindowTitle} ({matches[i].ProcessName})");

            Console.Write("\nSelect application: ");

            if (!int.TryParse(Console.ReadLine(), out int selection) ||
                selection < 1 || selection > matches.Length)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            string blockedProcess = matches[selection - 1].ProcessName;

            Console.WriteLine($"\nBlocking {blockedProcess}. Press Ctrl+C to stop.");

            while (true)
            {
                foreach (Process process in Process.GetProcessesByName(blockedProcess))
                {
                    try
                    {
                        Console.WriteLine($"Killing {blockedProcess} (PID {process.Id})");
                        process.Kill();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed: {ex.Message}");
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}