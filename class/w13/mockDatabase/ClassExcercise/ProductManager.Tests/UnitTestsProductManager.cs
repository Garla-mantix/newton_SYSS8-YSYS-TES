namespace ProductManager.Tests;

using ProductManagerApp;
using System.Data;
using System.Linq;
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
        
        var products = new List<(int Id, string Name, string Category, decimal Price)>
        {
            (1, "TEST_Android Galaxy 15", "Tech", 999.99m),
            (2, "TEST_iPhone 15",         "Tech", 799.99m),
            (3, "TEST_Margherita Pizza",  "Food",  12.99m)
        };

        var readCallCount = 0;
        mockReader.Setup(r => r.Read()).Returns(() => readCallCount++ < products.Count);
        mockReader.Setup(r => r.GetInt32(0)).Returns(() => products[readCallCount - 1].Id);
        mockReader.Setup(r => r.GetString(1)).Returns(() => products[readCallCount - 1].Name);
        mockReader.Setup(r => r.GetString(2)).Returns(() => products[readCallCount - 1].Category);
        mockReader.Setup(r => r.GetDecimal(3)).Returns(() => products[readCallCount - 1].Price);

        mockCommand.Setup(c => c.ExecuteReader()).Returns(mockReader.Object);
        mockCommand.Setup(c => c.CreateParameter()).Returns(mockParam.Object);
        mockCommand.Setup(c => c.Parameters).Returns(mockParams.Object);
        mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);

        var productManager = new ProductManager(mockConnection.Object);

        // Act
        var results = productManager.GetProductsByCategory("Tech");

        // Assert
        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.All(p => p.Category == "Tech"));
        Assert.IsTrue(results.Any(p => p.Name == "TEST_Android Galaxy 15"));
        Assert.IsTrue(results.Any(p => p.Name == "TEST_iPhone 15"));
        Assert.IsFalse(results.Any(p => p.Name == "TEST_Margherita Pizza"));
    }
}