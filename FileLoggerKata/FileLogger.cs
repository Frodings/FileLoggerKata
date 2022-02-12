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

            _fileWrapper.AppendText(filePath, text);
        }

        private string GetCurrentLogFilePath()
        {
            string currentDate = _dateTimeWrapper.GetNowDateString();
            string fileName = $"log{currentDate}.txt";

            return fileName;
        }
    }

    public interface IFileWrapper
    {
        void AppendText(string filePath, string text);
    }

    // TODO klasse som implementerer dette interfacet må også testes
    // det må testes at strengene returneres på korrekt format 

    public interface IDateTimeWrapper
    {
        string GetNowString();//DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

        string GetNowDateString();//DateTime.Now.TosTring(yyyyMMdd)
    }

    //public class FileWrapper : IFileWrapper
    //{
    //    private readonly string _path;

    //    public FileWrapper(string filePath)
    //    {
    //        _path = filePath;
    //    }

    //    public void AppendText(string text)
    //    {
    //        throw new NotImplementedException();
    //    }

    //}
}
