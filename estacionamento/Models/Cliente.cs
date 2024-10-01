using System;
using System.ComponentModel.DataAnnotations.Schema;
using estacionamento_dapper.Repositorios;
namespace estacionamento_dapper.Models;

[Table("clientes")]
public class Cliente
{
    [IgnoreInDapper]
    public int Id { get; set; } = default!;

    [Column("nome")]
    public string? Nome { get; set; }
    public string? Cpf { get; set; }
    public string? Telefone { get; set; }

    public Cliente() {}

    public Cliente(int id, string nome, string cpf, string telefone)
    {
        Id = id;
        Nome = nome;
        Cpf = cpf;
        Telefone = telefone;
    }
}

