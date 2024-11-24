using System.IO.Compression;

Console.WriteLine("Installer has been initiated!");

string url = "https://samanqaydi.com/houragency.zip";
string extractPath = @"C:\Moghim";

await DownloadAndExtractZipFile(url, extractPath);

static async Task DownloadAndExtractZipFile(string url, string extractPath)
{
    using (HttpClient client = new HttpClient())
    {
        using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;
            using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                          zipStream = new MemoryStream())
            {
                byte[] buffer = new byte[8192];
                long totalRead = 0;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    zipStream.Write(buffer, 0, bytesRead);
                    totalRead += bytesRead;

                    if (totalBytes.HasValue)
                    {
                        Console.SetCursorPosition(0, 1);
                        Console.WriteLine($"Download progress: {totalRead * 100 / totalBytes.Value}%");
                    }
                }

                zipStream.Seek(0, SeekOrigin.Begin);
                using (ZipArchive archive = new ZipArchive(zipStream))
                {
                    archive.ExtractToDirectory(extractPath, true);
                }
            }
        }
    }

    string sourceFilePath = Path.Combine(extractPath, "Agency\\Agency.lnk");
    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    string destinationFilePath = Path.Combine(desktopPath, "Agency.lnk");

    if (File.Exists(sourceFilePath))
    {
        File.Copy(sourceFilePath, destinationFilePath, true);
        Console.WriteLine("Shortcut copied to desktop successfully.");
    }
    else
    {
        Console.WriteLine("Shortcut file not found.");
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Moghim Installed successfully");
    Console.ResetColor();
    Console.WriteLine("Press any key to quit");
    Console.ReadKey();
}