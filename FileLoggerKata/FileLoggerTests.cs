using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;

namespace FileLoggerKata
{
    [TestFixture]
    public class FileLoggerTests
    {

        [Test]
        public void Log_RandomMessage_WritesMessage()
        {
            // arrange
            //stub datetime
            IDateTimeWrapper stubDateTime = Substitute.For<IDateTimeWrapper>();
            stubDateTime.GetNowString().Returns("2022-02-12 08:09:01");
            stubDateTime.GetNowDateString().Returns("20220212");

            IFileWrapper mockFileWrapper = Substitute.For<IFileWrapper>();

            // act
            var logger = new FileLogger(mockFileWrapper, stubDateTime);
            logger.Log("Dette er en test");

            //assert
            // assert against mock, that the method was called with expected text

            mockFileWrapper.Received().AppendText("log20220212.txt", "2022-02-12 08:09:01 Dette er en test");
        }
    }
}
