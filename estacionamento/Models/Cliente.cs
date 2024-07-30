using System;
namespace estacionamento_dapper.Models;

public class Cliente
{
    public int Id { get; set; } = default!;
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

