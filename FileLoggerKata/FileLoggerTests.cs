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

        [Test]
        public void Log_WhenDayIsWeekday_WritesMessage()
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

        [Test]
        public void Log_WhenDayIsWeekend_LogFileNameIsWeekend()
        {
            IFileSystemOperations mockFileSysOps = Substitute.For<IFileSystemOperations>();
            var testTime = new DateTime(2022, 2, 13, 21, 21, 1);
            var logger = CreateFileLogger(testTime, mockFileSysOps);

            logger.Log("Dette er en test");

            mockFileSysOps.Received().AppendText("weekend.txt", "2022-02-13 21:21:01 Dette er en test");
        }

        // ! Det er kanskje feil navngivning på disse metodene, siden det er Log() som faktisk kalles
        // og dette er inngangen for Unit under test..
        // navn kunne vært Log_CallsRenameFile_
        [TestCase("2022-02-13 18:13:01", "2022-02-06 21:15:13", "weekend-20220205.txt")]
        [TestCase("2022-02-12 18:13:01", "2022-01-01 15:06:22", "weekend-20220101.txt")]
        public void RenameWeekendFileIfNecessary_WhenFileIsOld_RenameFileIsCalled(DateTime logTime, DateTime exisitingFileModifiedDate, string expectedFileName)
        {
            IFileSystemOperations mockFileSysOps = Substitute.For<IFileSystemOperations>();

            mockFileSysOps.GetFileModifiedDate(Arg.Any<string>()).Returns(exisitingFileModifiedDate);
            mockFileSysOps.FileExist(Arg.Any<string>()).Returns(true);

            FileLogger logger = CreateFileLogger(logTime, mockFileSysOps);
            logger.Log("Dette er en test");

            mockFileSysOps.Received().RenameFile(Arg.Any<string>(), expectedFileName);
        }


        [TestCase("2022-02-13 14:04:01", "2022-02-12 21:32:22")]
        public void RenameWeekendFileIfNecessary_WhenFileIsNew_RenameFileIsNotCalled(DateTime logTime, DateTime existingFileModifiedDate)
        {
            IFileSystemOperations mockFileSysOps = Substitute.For<IFileSystemOperations>();
            mockFileSysOps.GetFileModifiedDate(Arg.Any<string>()).Returns(existingFileModifiedDate);
            mockFileSysOps.FileExist(Arg.Any<string>()).Returns(true);

            FileLogger logger = CreateFileLogger(logTime, mockFileSysOps);
            logger.Log("Dette er en test");

            mockFileSysOps.DidNotReceive().RenameFile(Arg.Any<string>(), Arg.Any<string>());
        }
    
    }

    // TODO
    // lag klasse for IDateTimeWrapper
    // test denne

    //public void NowIsWeekend_WhenNowIsNotWeekend_ReturnsFalse
    //public void NowIsWeekend_WhenNowIsWeekend_ReturnsTrue

    //Hvordan assert mot mock vha NSubstitute
    //-bruke Received() for å sjekke at angitt metode kalles opp som forventet
    //mockObject.Received().MetodeSomSkalSjekkes([forventede parametere])
}
