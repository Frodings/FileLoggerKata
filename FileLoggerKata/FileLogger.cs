using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLoggerKata
{
    public class FileLogger
    {
        private readonly string _weekendFile = "weekend.txt";

        private readonly IFileWrapper _fileWrapper;
        private readonly IDateTimeWrapper _dateTimeWrapper;
               
        public FileLogger(IFileWrapper fileWrapper, IDateTimeWrapper dateTimeWrapper)
        {
            _fileWrapper = fileWrapper;
            _dateTimeWrapper = dateTimeWrapper;
        }

        public void Log(string message)
        {
            string text = $"{_dateTimeWrapper.GetNowString()} {message}";
            string filePath = GetCurrentLogFilePath();

            if (filePath == _weekendFile)
                RenameWeekendFileIfNecessary();

            _fileWrapper.AppendText(filePath, text);
        }

        private string GetCurrentLogFilePath()
        {
            string fileName;
            if (!_dateTimeWrapper.NowIsWeekend())
            {
                string currentDate = _dateTimeWrapper.GetNowDateString();
                fileName = $"log{currentDate}.txt";
            }
            else
            {
                fileName = _weekendFile;
            }
            
            return fileName;
        }

        /// <summary>
        /// Checks if weekend file exist and when its modified - If its not modified current weekend, it is renamed.
        /// </summary>
        private void RenameWeekendFileIfNecessary()
        {
            if (_fileWrapper.FileExist(_weekendFile))
            {
                DateTime modifiedDate = _fileWrapper.GetFileModifiedDate(_weekendFile);
                TimeSpan interval = _dateTimeWrapper.GetNow().Date - modifiedDate.Date;

                if (interval.TotalDays >= 2) // not modified this weekend
                {
                    string newFileName;
                    if (modifiedDate.DayOfWeek == DayOfWeek.Saturday)
                        newFileName = $"weekend-{modifiedDate.ToString("yyyyMMdd")}.txt";
                    else
                    {
                        DateTime saturday = modifiedDate.AddDays(- ((int)modifiedDate.DayOfWeek + 1));
                        newFileName = $"weekend-{saturday.ToString("yyyyMMdd")}.txt";
                    }
                        
                    _fileWrapper.RenameFile(_weekendFile, newFileName);
                }
            }
        }
    }

    public interface IFileWrapper
    {
        void AppendText(string filePath, string text);

        bool FileExist(string filepath);

        DateTime GetFileModifiedDate(string filePath);

        bool RenameFile(string srcFilePath, string newFilePath);
    }

    // TODO en klasse som implementerer dette interfacet må også testes
    // det må testes at strengene returneres på korrekt format 

    public interface IDateTimeWrapper
    {
        DateTime GetNow();

        string GetNowString();//DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        string GetNowDateString();//DateTime.Now.TosTring(yyyyMMdd)

        bool NowIsWeekend();

    }

    
}
