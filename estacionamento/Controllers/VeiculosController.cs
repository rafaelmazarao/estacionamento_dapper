using Dapper;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using estacionamento_dapper.Models;
using estacionamento_dapper.Repositorios;

namespace estacionamento_dapper.Controllers;

[Route("/veiculos")]
public class VeiculosController : Controller
{
    private readonly IDbConnection _cnn;
    private readonly IRepositorio<Veiculo> _repo;

    public VeiculosController(IDbConnection cnn)
    {
        _cnn = cnn;
        _repo = new RepositorioDapper<Veiculo>(_cnn);
    }
    public IActionResult Index()
    {   
        var sql = @"select  v.*, c.* from veiculos v
                    inner join clientes c on c.id = v.clienteId";
        
        var veiculos = _cnn.Query<Veiculo, Cliente, Veiculo>(sql, (veiculo, cliente) => {
            veiculo.cliente = cliente;
            return veiculo;
        }, splitOn: "Id");

        return View(veiculos);
    }

    [HttpGet("Novo")]
    public IActionResult Novo()
    {   
        return View();
    }

    [HttpPost("Criar")]
    public IActionResult Criar([FromForm] Veiculo veiculo)
    { 
        _repo.Inserir(veiculo);
        return Redirect("/veiculos");
    }

    [HttpPost("{id}/Apagar")]
    public IActionResult Apagar([FromRoute] int id)
    { 
        _repo.Excluir(id);
        return Redirect("/veiculos");
    }

    [HttpGet("{id}/Editar")]
    public IActionResult Editar([FromRoute] int id)
    { 
        var valor = _repo.ObterPorId(id);
        return View(valor);
    }

    [HttpPost("{id}/Alterar")]
    public IActionResult Alterar([FromRoute] int id, [FromForm] Veiculo veiculo )
    { 
       veiculo.Id = id;             
       _repo.Atualizar(veiculo);       
       return Redirect("/veiculos");
    }
    
}
