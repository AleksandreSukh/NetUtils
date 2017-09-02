using System.IO;

namespace NetUtils
{
    public class PathHelper
    {
        public static bool IsDirectory(string path)
        {
            try { return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory; }
            catch { return false; }
        }
    }
}