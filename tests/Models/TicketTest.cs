using System;
using estacionamento_dapper.Models;

[TestClass]
public class TicketTest
{    
    [TestMethod]
    public void TestandoMetodoValorTotal()
    {
        // Arrange
        var cliente = new Cliente { Id = 1, Nome = "Rafael" };
        var veiculo = new Veiculo { Id = 1, Marca = "Fiat", Modelo = "Uno", ClienteId = cliente.Id, Placa = "AAA000" };
        var ticket = new Ticket { DataEntrada = DateTime.Now.AddHours(-1), Id = 1, VagaId = 1, VeiculoId = veiculo.Id };
        var valorDoMinuto = new ValorDoMinuto{ Minutos = 1, Valor = 1 };

        // Act
        var ValorTotal = ticket.ValorTotal(valorDoMinuto);

        // Assert
        Assert.AreEqual(60, ValorTotal);          
    }

    public void TestandoValorPagoDoTicket()
    {
        // Arrange
        var cliente = new Cliente { Id = 1, Nome = "Rafael" };
        var veiculo = new Veiculo { Id = 1, Marca = "Fiat", Modelo = "Uno", ClienteId = cliente.Id, Placa = "AAA000" };
        var ticket = new Ticket { DataEntrada = DateTime.Now.AddHours(-1), Id = 1, VagaId = 1, VeiculoId = veiculo.Id };
        var valorDoMinuto = new ValorDoMinuto{ Minutos = 1, Valor = 1 };
        var ValorTotalDesejado = ticket.ValorTotal(valorDoMinuto);

        // Act
        ticket.Pago(valorDoMinuto);

        // Assert
        Assert.AreEqual(ValorTotalDesejado, ticket.Valor);
        Assert.IsNotNull(ticket.DataSaida);          
    }
}

