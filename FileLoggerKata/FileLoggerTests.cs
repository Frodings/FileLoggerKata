using System;
using NSubstitute;
using NUnit.Framework;

namespace FileLoggerKata
{
    [TestFixture]
    public class FileLoggerTests
    {

        private FileLogger CreateFileLogger(DateTime logTime, IFileSystemOperations fileWrapper)
        {
            IDateTimeWrapper stubDateTime = Substitute.For<IDateTimeWrapper>();
            stubDateTime.GetNow().Returns(logTime);

            var logger = new FileLogger(fileWrapper, stubDateTime);

            return logger;
        }

        private IFileSystemOperations CreateFakeFileSystemOperations(DateTime fileModifiedTime)
        {
            IFileSystemOperations mockFileSysOps = Substitute.For<IFileSystemOperations>();
            mockFileSysOps.GetFileModifiedDate(Arg.Any<string>()).Returns(fileModifiedTime);
            mockFileSysOps.FileExist(Arg.Any<string>()).Returns(true);

            return mockFileSysOps;
        }


        [Test]
        public void Log_WhenDayIsWeekday_LogsMessageToFile()
        {
            // arrange
            IFileSystemOperations mockFileSysOps = Substitute.For<IFileSystemOperations>();
            var testTime = new DateTime(2022, 02, 11, 8, 9, 1);
            FileLogger logger = CreateFileLogger(testTime, mockFileSysOps);

            // act
            logger.Log("Dette er en test");

            //assert - bruker NSubstitute sin Received() for å sjekke at mockFileSysOps.AppendText() er kalt opp som angitt 
            string expectedFileName = $"log{testTime.ToString("yyyyMMdd")}.txt";
            string expectedLogMessage = $"{testTime.ToString("yyyy-MM-dd HH:mm:ss")} Dette er en test";
            mockFileSysOps.Received().AppendText(expectedFileName, expectedLogMessage);
        }

        [TestCase("2022-02-13 18:13:01", "2022-02-06 21:15:13")]
        [TestCase("2022-02-20 02:03:01", "2022-02-20 02:03:00")]
        public void Log_WhenDayIsWeekend_LogsMessageToFile(DateTime logTime, DateTime existingFileModifiedDate)
        {
            IFileSystemOperations mockFileSysOps = CreateFakeFileSystemOperations(existingFileModifiedDate);
           
            var logger = CreateFileLogger(logTime, mockFileSysOps);

            string logMessage = "Dette er en test";
            logger.Log(logMessage);

            string expectedFileName = "weekend.txt";
            string expectedLogMessage = $"{logTime.ToString("yyyy-MM-dd HH:mm:ss")} {logMessage}";
            mockFileSysOps.Received().AppendText(expectedFileName, expectedLogMessage);
        }

        [TestCase("2022-02-13 18:13:01", "2022-02-06 21:15:13", "weekend-20220205.txt")]
        [TestCase("2022-02-12 18:13:01", "2022-01-01 15:06:22", "weekend-20220101.txt")]
        public void Log_WhenWeekendAndOldLogfile_CallsRenameFile(DateTime logTime, DateTime existingFileModifiedDate, string expectedFileName)
        {
            IFileSystemOperations mockFileSysOps = CreateFakeFileSystemOperations(existingFileModifiedDate);

            FileLogger logger = CreateFileLogger(logTime, mockFileSysOps);
            logger.Log("Dette er en test");

            mockFileSysOps.Received().RenameFile(Arg.Any<string>(), expectedFileName);
        }


        [TestCase("2022-02-13 14:04:01", "2022-02-12 21:32:22")]
        [TestCase("2022-02-05 00:01:00", "2022-02-05 00:00:00")]
        public void Log_WhenWeekendAndFileIsNew_RenameFileIsNotCalled(DateTime logTime, DateTime existingFileModifiedDate)
        {
            IFileSystemOperations mockFileSysOps = CreateFakeFileSystemOperations(existingFileModifiedDate);

            FileLogger logger = CreateFileLogger(logTime, mockFileSysOps);
            logger.Log("Dette er en test");

            mockFileSysOps.DidNotReceive().RenameFile(Arg.Any<string>(), Arg.Any<string>());
        }
    
    }

}
