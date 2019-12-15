using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Obsidian.TestCore
{
    public static class FileEx
    {
        public static void WriteAllText(string path, string contents)
        {
            // generate a temp filename
            var tempPath = Path.GetTempFileName();

            // get the bytes
            var data = Encoding.UTF8.GetBytes(contents);

            // write the data to a temp file
            using (var tempFile = File.Create(tempPath, 4096, FileOptions.WriteThrough))
                tempFile.Write(data, 0, data.Length);

            // replace the contents
            File.Replace(tempPath, path, null);
        }
    }
}
