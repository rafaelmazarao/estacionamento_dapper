using Dapper;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using estacionamento_dapper.Models;
using estacionamento_dapper.Repositorios;

namespace estacionamento_dapper.Controllers;

[Route("/clientes")]
public class ClientesController : Controller
{
    private readonly IRepositorio<Cliente> _repo;

    public ClientesController(IRepositorio<Cliente> repo)
    {
        _repo = repo;
    }
    public IActionResult Index()
    {   
        var clientes = _repo.ObterTodos();
        return View(clientes);
    }

    [HttpGet("Novo")]
    public IActionResult Novo()
    {   
        return View();
    }

    [HttpPost("Criar")]
    public IActionResult Criar([FromForm] Cliente cliente)
    { 
        _repo.Inserir(cliente);
        return Redirect("/clientes");
    }

    [HttpPost("{id}/Apagar")]
    public IActionResult Apagar([FromRoute] int id)
    { 
        _repo.Excluir(id);
        return Redirect("/clientes");
    }

    [HttpGet("{id}/Editar")]
    public IActionResult Editar([FromRoute] int id)
    { 
        var valor = _repo.ObterPorId(id);
        return View(valor);
    }

    [HttpPost("{id}/Alterar")]
    public IActionResult Alterar([FromRoute] int id, [FromForm] Cliente cliente )
    { 
       cliente.Id = id;             
       _repo.Atualizar(cliente);       
       return Redirect("/clientes");
    }
    
}
