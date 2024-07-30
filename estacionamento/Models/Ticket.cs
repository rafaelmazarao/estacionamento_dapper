using System;

namespace estacionamento_dapper.Models;

public class Ticket
{
    public int Id { get; set;}
    public DateTime DataEntrada { get; set; }
    public DateTime? DataSaida { get; set; }
    public float Valor { get; set; }
    public int VeiculoId { get; set; }
    public int VagaId { get; set; }
}