using System;
using System.IO;

namespace FixDuplicates {
    internal class Program {

        /// <summary>
        /// Startup object.
        /// </summary>
        /// <param name="args">Not used.</param>
        static void Main(string[] args) {
            Console.Title = "OneDrive - Fix Duplicates";
            ToConsole(ConsoleColor.White, true,
                "----------------------------------------------------------------",
                "------ Delete duplicate OneDrive files from bad sync.     ------",
                "----------------------------------------------------------------",
                "- After confirming replacement of each file, the file is permanently removed.",
                "- If you replaced a file by mistake you can attempt to restore the file manually from the OneDrive recycle bin.",
                "- Y/N can be held down for repetitive actions."
                );
            RecursiveSearch(GetDirectory(), GetName());
            PreQuit();
        }

        /// <summary>
        /// EnumerateFiles in <paramref name="directoryInfo"/> and search for <paramref name="name"/> in all directories for possible duplicates. 
        /// </summary>
        /// <param name="directoryInfo"></param>
        /// <param name="name"></param>
        private static void RecursiveSearch(DirectoryInfo directoryInfo, string name) {
            foreach (FileInfo d in directoryInfo.EnumerateFiles("*-" + name + ".*", System.IO.SearchOption.AllDirectories)) {
                try {
                    Console.WriteLine();
                    FileInfo o = new FileInfo(d.FullName.Replace("-" + name + ".", "."));
                    if (o.Exists) {
                        ToConsole(ConsoleColor.White, true, "OneDrive copy: " + o.FullName);
                        ToConsole(ConsoleColor.Gray, false, "- Created: " + o.CreationTime);
                        ToConsole(o.CreationTime > d.CreationTime ? ConsoleColor.DarkGreen : o.CreationTime == d.CreationTime ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed, true, " *");
                        ToConsole(ConsoleColor.Gray, false, "- Access: " + o.LastAccessTime);
                        ToConsole(o.LastAccessTime > d.LastAccessTime ? ConsoleColor.DarkGreen : o.LastAccessTime == d.LastAccessTime ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed, true, " *");
                        ToConsole(ConsoleColor.Gray, false, "- Write: " + o.LastWriteTime);
                        ToConsole(o.LastWriteTime > d.LastWriteTime ? ConsoleColor.DarkGreen : o.LastWriteTime == d.LastWriteTime ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed, true, " *");
                        ToConsole(ConsoleColor.Gray, false, "- Bytes: " + o.Length);
                        ToConsole(o.Length > d.Length ? ConsoleColor.DarkGreen : o.Length == d.Length ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed, true, " *");
                        ToConsole(ConsoleColor.White, true, "Duplicate copy: " + d.FullName);
                        ToConsole(ConsoleColor.Gray, false, "- Created: " + d.CreationTime);
                        ToConsole(d.CreationTime > o.CreationTime ? ConsoleColor.DarkGreen : d.CreationTime == o.CreationTime ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed, true, " *");
                        ToConsole(ConsoleColor.Gray, false, "- Access: " + d.LastAccessTime);
                        ToConsole(d.LastAccessTime > o.LastAccessTime ? ConsoleColor.DarkGreen : d.LastAccessTime == o.LastAccessTime ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed, true, " *");
                        ToConsole(ConsoleColor.Gray, false, "- Write: " + d.LastWriteTime);
                        ToConsole(d.LastWriteTime > o.LastWriteTime ? ConsoleColor.DarkGreen : d.LastWriteTime == o.LastWriteTime ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed, true, " *");
                        ToConsole(ConsoleColor.Gray, false, "- Bytes: " + d.Length);
                        ToConsole(d.Length > o.Length ? ConsoleColor.DarkGreen : d.Length == o.Length ? ConsoleColor.DarkYellow : ConsoleColor.DarkRed, true, " *");
                        if (YesNo("Replace OneDrive copy with duplicated file? Y/N", "Expected Y/N")) {
                            o.Delete();
                            d.MoveTo(o.FullName);
                            ToConsole(ConsoleColor.Green, true, "Okay!");
                        }
                        else
                            ToConsole(ConsoleColor.Red, true, "Skipped!");
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Get root directory to recusively search.
        /// </summary>
        /// <returns></returns>
        private static DirectoryInfo GetDirectory() {
            while (true) {
                // Wrap in try-catch because DirectoryInfo can throw errors if a bad string was given.
                try {
                    Console.WriteLine();
                    ToConsole(ConsoleColor.White, true, "Set root directory to run duplicate search.");
                    ToConsole(ConsoleColor.DarkGray, true, "Example Path: C:\\Users\\User\\OneDrive", "Example Path: ..\\..\\..\\OneDrive\\Pictures");
                    ToConsole(ConsoleColor.DarkYellow, false, "Path: ");
                    string line = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) {
                        ToConsole(ConsoleColor.Red, true, "Cannot enter empty directory name, use \".\" for current directory.");
                        continue;
                    }
                    DirectoryInfo directoryInfo = Path.IsPathRooted(line) ? new DirectoryInfo(line) : new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, line));
                    if (!directoryInfo.Exists) {
                        ToConsole(ConsoleColor.Red, true, "Directory \"" + directoryInfo.FullName + "\" does not exist.");
                        continue;
                    }
                    ToConsole(ConsoleColor.Gray, true, string.Empty, "Search \"" + directoryInfo.FullName + "\" for all content, including sub directories.");
                    if (YesNo("Is this correct? Y/N", "Expected Y/N"))
                        return directoryInfo;
                }
                catch { }
            }
        }

        /// <summary>
        /// Get name for searching for file duplicates.
        /// </summary>
        /// <returns></returns>
        private static string GetName() {
            while (true) {
                Console.WriteLine();
                ToConsole(ConsoleColor.White, true, "Set name that is found in duplicate files.");
                ToConsole(ConsoleColor.DarkGray, true, "Example File: \"New Text Document-<NAME>.txt\"");
                ToConsole(ConsoleColor.DarkYellow, false, "Name: ");
                string line = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(line)) {
                    ToConsole(ConsoleColor.Red, true, "Cannot enter empty name!");
                    continue;
                }
                ToConsole(ConsoleColor.Gray, true, string.Empty, "Search for files that contain \"-" + line + "\".");
                ToConsole(ConsoleColor.DarkGray, true, "Example File: \"New Text Document-" + line + ".txt\"");
                if (YesNo("Is this correct? Y/N", "Expected Y/N"))
                    return line;
            }
        }

        /// <summary>
        /// Simple loop for yes or no questions.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool YesNo(string question, string error) {
            while (true) {
                ToConsole(ConsoleColor.DarkYellow, false, question + ": ");
                ConsoleKey key = Console.ReadKey().Key;
                Console.WriteLine();
                switch (key) {
                    case ConsoleKey.Y:
                    case ConsoleKey.T:
                    case ConsoleKey.NumPad1:
                    case ConsoleKey.D1:
                        return true;
                    case ConsoleKey.N:
                    case ConsoleKey.F:
                    case ConsoleKey.NumPad0:
                    case ConsoleKey.D0:
                        return false;
                    default:
                        if (!string.IsNullOrEmpty(error))
                            ToConsole(ConsoleColor.Red, true, error);
                        break;
                }
            }
        }

        /// <summary>
        /// Simple loop to prevent closing of window if user held down the Y or N keys.
        /// </summary>
        private static void PreQuit() {
            Console.WriteLine();
            ToConsole(ConsoleColor.DarkGreen, true, "All finished! It's safe to close this window.");
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }

        /// <summary>
        /// Helper to write to console with color.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="newLine"></param>
        /// <param name="lines"></param>
        private static void ToConsole(ConsoleColor color = ConsoleColor.Gray, bool newLine = true, params string[] lines) {
            Console.ForegroundColor = color;
            foreach (string line in lines)
                if (newLine)
                    Console.WriteLine(line);
                else
                    Console.Write(line);
            Console.ResetColor();
        }

    }
}
