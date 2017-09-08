using System.IO;
using System.Reflection;

namespace NetUtils
{
    public class ExeLocationInfo : IExeLocationInfo
    {
        public string ExeDirectory => Path.GetDirectoryName(ExeFullPath);
        static string _exeFullPath;
        //NOTE! Checked and it is safe to Lazy-load location it will keep value when process was launched
        public string ExeFullPath
            => _exeFullPath ?? (_exeFullPath = Assembly.GetEntryAssembly().Location);
        public string Exename => Path.GetFileName(ExeFullPath);

    }
}