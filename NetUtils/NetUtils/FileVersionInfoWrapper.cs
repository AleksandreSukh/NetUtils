using System;
using NetUtils;

namespace TextLoggerNet.Loggers
{
    [Serializable]
    public class FileVersionInfoWrapper : IFileVersionInfoWrapper
    {
        FileVersionInfoWrapper()//For Serialization
        { }


        FileVersionInfoWrapper(int fileMajorPart, int fileMinorPart, int fileBuildPart, int filePrivatePart)//I Made this private to request for versionStringInConstructor (To write sortable string in sessionLog)
        {
            FileMajorPart = fileMajorPart;
            FileMinorPart = fileMinorPart;
            FileBuildPart = fileBuildPart;
            FilePrivatePart = filePrivatePart;
        }
        public FileVersionInfoWrapper(int fileMajorPart, int fileMinorPart, int fileBuildPart, int filePrivatePart, string versionString)
            : this(fileMajorPart, fileMinorPart, fileBuildPart, filePrivatePart)
        {
            _stringValue = versionString;
        }
        public int FileMajorPart { get; set; }
        public int FileMinorPart { get; set; }
        public int FileBuildPart { get; set; }
        public int FilePrivatePart { get; set; }
        //public string FileVersion => $"{FileMajorPart}.{FileMinorPart}.{FileBuildPart}.{FilePrivatePart}";
        public string FileVersion => _stringValue ?? FileVersionWithoutZeros;
        public string FileVersionWithoutZeros => $"{FileMajorPart}.{FileMinorPart}.{FileBuildPart}.{FilePrivatePart}";
        readonly string _stringValue;
    }
}