using System;

namespace estacionamento_dapper.Models;

public class Veiculo
{
    public int Id { get; set; }
    public string Placa { get; set; }
    public string Modelo { get; set; }
    public string Marca { get; set; }
    public int ClienteId { get; set; }

    public Veiculo() {}

    public Veiculo(int id, string placa, string modelo, string marca, int clienteId)
    {
        Id = id;
        Placa = placa;
        Modelo = modelo;
        Marca = marca;
        ClienteId = clienteId;
    }
}

