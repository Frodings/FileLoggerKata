﻿using System;
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

        private readonly IFileSystemOperations _fileOperations;
        private readonly IDateTimeWrapper _dateTimeWrapper;
               
        public FileLogger(IFileSystemOperations fileWrapper, IDateTimeWrapper dateTimeWrapper)
        {
            _fileOperations = fileWrapper;
            _dateTimeWrapper = dateTimeWrapper;
        }

        public void Log(string message)
        {
            string logTimeString = _dateTimeWrapper.GetNow().ToString("yyyy-MM-dd HH:mm:ss");
            string logText = $"{logTimeString} {message}";

            string filePath = GetLogFileName();

            if (filePath == _weekendFile)
                RenameWeekendFileIfNecessary();

            _fileOperations.AppendText(filePath, logText);
        }

        //public void Log_new(string message)
        //{
            
        //}

        private string GetLogFileName()
        {
            string fileName;
            DateTime now = _dateTimeWrapper.GetNow();

            if (IsWeekend(now))
            {
                fileName = _weekendFile;
            }
            else
            {
                string fileDateString = now.ToString("yyyyMMdd");
                fileName = $"log{fileDateString}.txt";
            }
            
            return fileName;
        }


        private bool IsWeekend(DateTime date)
        {
            return (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        }


        /// <summary>
        /// Checks if weekend file exist and when its modified - If its not modified current weekend, it is renamed.
        /// </summary>
        private void RenameWeekendFileIfNecessary()
        {
            if (_fileOperations.FileExist(_weekendFile))
            {
                DateTime modifiedDate = _fileOperations.GetFileModifiedDate(_weekendFile);
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
                        
                    _fileOperations.RenameFile(_weekendFile, newFileName);
                }
            }
        }
    }

    public interface IFileSystemOperations
    {
        void AppendText(string filePath, string text);

        bool FileExist(string filepath);

        DateTime GetFileModifiedDate(string filePath);

        bool RenameFile(string srcFilePath, string newFilePath);
    }


    public interface IDateTimeWrapper
    {
        DateTime GetNow();

    }

    
}
