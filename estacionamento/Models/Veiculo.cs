using System;
using System.ComponentModel.DataAnnotations.Schema;
using estacionamento_dapper.Repositorios;

namespace estacionamento_dapper.Models;

[Table("veiculos")]
public class Veiculo
{
    [IgnoreInDapper]
    public int Id { get; set; }
    public string Placa { get; set; }
    public string Modelo { get; set; }
    public string Marca { get; set; }
    public int ClienteId { get; set; }
    
     [IgnoreInDapper]
    public Cliente cliente{ get; set; } = default!;

    public Veiculo() {}
}

