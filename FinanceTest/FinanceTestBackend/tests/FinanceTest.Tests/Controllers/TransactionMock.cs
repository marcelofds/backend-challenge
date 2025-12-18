using Microsoft.AspNetCore.Http;
using Moq;

namespace FinanceTest.Tests.Controllers;

public static class TransactionMock
{
    public static Mock<IFormFile> CreateMockFile(string fileName, long length)
    {
        var fileMock = new Mock<IFormFile>();
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write("Dummy content");
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(length);

        return fileMock;
    }
}