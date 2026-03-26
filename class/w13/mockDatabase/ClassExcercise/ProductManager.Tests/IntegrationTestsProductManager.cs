namespace ProductManager.Tests;

using ProductManagerApp;
using Npgsql;
using System.Linq;

[TestClass]
public class IntegrationTestsProductManager
{
    private const string ConnectionString = 
        "Host=localhost;Port=5431;Username=postgres;Password=mysecretpassword;Database=postgres";

    private const string TestProductName1 = "TEST_Android Galaxy 15";
    private const string TestProductName2 = "TEST_MacBook Pro";
    private const string TestProductName3 = "TEST_Margherita Pizza";

    [TestCleanup]
    public void Cleanup()
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM products WHERE name LIKE 'TEST_%'";
        cmd.ExecuteNonQuery();
    }

    [TestMethod]
    [TestCategory("Integration")]
    public void TestGetProductsByCategory()
    {
        // Arrange
        using var connection = new NpgsqlConnection(ConnectionString);
        connection.Open();
        using var cmd = connection.CreateCommand();
        
        cmd.CommandText = $"INSERT INTO products (name, category, price) VALUES ('{TestProductName1}', 'Tech'::product_category, 999.99)";
        cmd.ExecuteNonQuery();

        cmd.CommandText = $"INSERT INTO products (name, category, price) VALUES ('{TestProductName2}', 'Tech'::product_category, 799.99)";
        cmd.ExecuteNonQuery();

        cmd.CommandText = $"INSERT INTO products (name, category, price) VALUES ('{TestProductName3}', 'Food'::product_category, 12.99)";
        cmd.ExecuteNonQuery();

        var productManager = new ProductManager();

        // Act
        var results = productManager.GetProductsByCategory("Tech");

        // Assert
        Assert.IsTrue(results.All(p => p.Category == "Tech"));
        Assert.IsTrue(results.Any(p => p.Name == TestProductName1));
        Assert.IsTrue(results.Any(p => p.Name == TestProductName2));
    }
}