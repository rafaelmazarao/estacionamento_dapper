using System;
using estacionamento_dapper.Models;

[TestClass]
public class ClienteTest
{    
    [TestMethod]
    public void TestandoPropriedadesDoModelCliente()
    {
        // Arrange
        var cliente = new Cliente();

        // Act
        cliente.Id = 1;
        cliente.Nome = "Rafael";
        cliente.Cpf = "070.291.390-11";

        // Assert
        Assert.AreEqual(1, cliente.Id);
        Assert.AreEqual("Rafael", cliente.Nome);
        Assert.AreEqual("070.291.390-11", cliente.Cpf);      
    }
}

