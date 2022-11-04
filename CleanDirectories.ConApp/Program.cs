namespace CleanDirectories.ConApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = string.Empty;
            var dropFolders = new string[] { "\\bin", "\\obj", "\\target" };

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("********************************************");
            Console.WriteLine("*          Clean directories...            *");
            Console.WriteLine("********************************************");
            Console.WriteLine();

            if (args.Length == 1)
            {
                path = args[0];
            }

            while (string.IsNullOrEmpty(path))
            {
                Console.Clear();
                Console.WriteLine("********************************************");
                Console.WriteLine("*          Clean directories...            *");
                Console.WriteLine("********************************************");
                Console.WriteLine();
                Console.WriteLine($"Drop folders: {string.Join(", ", dropFolders)}");
                Console.WriteLine();
                Console.Write("Path: ");

                var input = Console.ReadLine();

                if (Directory.Exists(input))
                {
                    path = input;
                }
            }

            PrintBusyProgress();
            CleanDirectories(path, dropFolders);
            runBusyProgress = false;
        }
        private static readonly bool canBusyPrint = true;
        private static bool runBusyProgress = false;
        private static void PrintBusyProgress()
        {
            var sign = "\\";

            Console.WriteLine();
            runBusyProgress = true;
            Task.Factory.StartNew(async () =>
            {
                while (runBusyProgress)
                {
                    if (canBusyPrint)
                    {
                        if (Console.CursorLeft > 0)
                            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

                        Console.Write($".{sign}");
                        sign = sign == "\\" ? "/" : "\\";
                    }
                    await Task.Delay(250).ConfigureAwait(false);
                }
            });
        }
        private static void PrintDirectory(string path)
        {
            static string CreateIndent(int level)
            {
                string result = string.Empty;

                for (int i = 0; i < level; i++)
                {
                    result += " ";
                }
                return result;
            }
            static void PrintDirectory(DirectoryInfo di, int level)
            {
                string indent = CreateIndent(level);

                foreach (var item in di.GetFiles())
                {
                    Console.WriteLine($"{indent}{item}");
                }
                foreach (var item in di.GetDirectories())
                {
                    Console.WriteLine($"{indent}{item.Name}");
                    PrintDirectory(item, level + 1);
                }
            }

            DirectoryInfo di = new DirectoryInfo(path);

            Console.WriteLine(path);

            PrintDirectory(di, 0);
        }
        private static void CleanDirectories(string path, params string[] dropFolders)
        {
            static int CleanDirectories(DirectoryInfo dirInfo, params string[] dropFolders)
            {
                int result = 0;

                try
                {
                    result = dirInfo.GetFiles().Length;
                    foreach (var item in dirInfo.GetDirectories())
                    {
                        int fileCount = CleanDirectories(item, dropFolders);

                        try
                        {
                            if (fileCount == 0)
                            {
                                item.Delete();
                            }
                            else if ((dropFolders.FirstOrDefault(df => item.FullName.EndsWith(df))) != null)
                            {
                                fileCount = 0;
                                item.Delete(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error in {System.Reflection.MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
                        }
                        result += fileCount;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in {System.Reflection.MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");
                }
                return result;
            }

            CleanDirectories(new DirectoryInfo(path), dropFolders);
        }
    }
}