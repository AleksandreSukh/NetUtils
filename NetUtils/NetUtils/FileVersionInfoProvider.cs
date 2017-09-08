using System;
using System.Diagnostics;
using TextLoggerNet.Loggers;

namespace NetUtils
{
    public interface IFileVersionInfoProviderEx : IFileVersionInfoProvider
    {
        IFileVersionInfoWrapper GetVersionInfo(string path);
    }
    public class FileVersionInfoProviderEx : FileVersionInfoProvider, IFileVersionInfoProviderEx
    {
        IFileVersionInfoWrapper FromVersion(Version info)
        {
            return new FileVersionInfoWrapper(
                info.Major,
                info.Minor,
                info.Build,
                info.Revision,
                $"{info.Major}.{info.Minor}.{info.Build}.{info.Revision}");
        }
    }
    public interface IFileVersionInfoWrapper
    {
        int FileMajorPart { get; set; }
        int FileMinorPart { get; set; }
        int FileBuildPart { get; set; }
        int FilePrivatePart { get; set; }
        string FileVersion { get; }
        string FileVersionWithoutZeros { get; }
    }
    public interface IFileVersionInfoProvider
    {
        IFileVersionInfoWrapper FromVersionInfo(FileVersionInfo info);
    }

    public class FileVersionInfoProvider : IFileVersionInfoProvider
    {
        public IFileVersionInfoWrapper GetVersionInfo(string path)
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(path);
            var foundVersion = new Version(
                fileVersionInfo.FileMajorPart,
                fileVersionInfo.FileMinorPart,
                fileVersionInfo.FileBuildPart,
                fileVersionInfo.FilePrivatePart
            );

            return FromVersion(foundVersion);
        }
        public IFileVersionInfoWrapper FromVersionInfo(FileVersionInfo info)
        {
            return new FileVersionInfoWrapper(
                info.FileMajorPart,
                info.FileMinorPart,
                info.FileBuildPart,
                info.FilePrivatePart,
                info.FileVersion);
        }
        IFileVersionInfoWrapper FromVersion(Version info)
        {
            return new FileVersionInfoWrapper(
                info.Major,
                info.Minor,
                info.Build,
                info.Revision,
                $"{info.Major}.{info.Minor}.{info.Build}.{info.Revision}");
        }
    }
}