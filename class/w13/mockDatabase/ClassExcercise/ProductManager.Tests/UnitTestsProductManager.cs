namespace ProductManager.Tests;

using ProductManagerApp;
using System.Data;
using Moq;

[TestClass]
public class UnitTestsProductManager
{
    [TestMethod]
    [TestCategory("UnitTest")]
    public void UnitTestGetProductsByCategory()
    {
        // Arrange
        var mockConnection = new Mock<IDbConnection>();
        var mockCommand    = new Mock<IDbCommand>();
        var mockParams     = new Mock<IDataParameterCollection>();
        var mockParam      = new Mock<IDbDataParameter>();
        var mockReader     = new Mock<IDataReader>();

        var readCallCount = 0;
        mockReader.Setup(r => r.Read()).Returns(() => readCallCount++ == 0);
        mockReader.Setup(r => r.GetInt32(0)).Returns(1);
        mockReader.Setup(r => r.GetString(1)).Returns("TEST_Android Galaxy 15");
        mockReader.Setup(r => r.GetString(2)).Returns("Tech");
        mockReader.Setup(r => r.GetDecimal(3)).Returns(999.99m);

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockCommand.Setup(c => c.CreateParameter()).Returns(mockParam.Object);
        mockCommand.Setup(c => c.Parameters).Returns(mockParams.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var productManager = new ProductManager(mockConnection.Object);

        // Act
        var results = productManager.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("Tech", results[0].Category);
        Assert.AreEqual("TEST_Android Galaxy 15", results[0].Name);
    }
}