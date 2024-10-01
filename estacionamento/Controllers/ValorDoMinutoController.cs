using Dapper;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using estacionamento_dapper.Models;
using estacionamento_dapper.Repositorios;

namespace estacionamento_dapper.Controllers;

[Route("/valores")]
public class ValorDoMinutoController : Controller
{
    private readonly IRepositorio<ValorDoMinuto> _repo;

    public ValorDoMinutoController(IRepositorio<ValorDoMinuto> repo)
    {
        _repo = repo;
    }
    public IActionResult Index()
    {   
        var valores = _repo.ObterTodos();
        return View(valores);
    }

    [HttpGet("Novo")]
    public IActionResult Novo()
    {   
        return View();
    }

    [HttpPost("Criar")]
    public IActionResult Criar([FromForm] ValorDoMinuto valorDoMinuto)
    { 
        _repo.Inserir(valorDoMinuto);
        return Redirect("/valores");
    }

    [HttpPost("{id}/Apagar")]
    public IActionResult Apagar([FromRoute] int id)
    { 
        _repo.Excluir(id);
        return Redirect("/valores");
    }

    [HttpGet("{id}/Editar")]
    public IActionResult Editar([FromRoute] int id)
    { 
        var valor = _repo.ObterPorId(id);
        return View(valor);
    }

    [HttpPost("{id}/Alterar")]
    public IActionResult Alterar([FromRoute] int id, [FromForm] ValorDoMinuto valorDoMinuto )
    { 
       valorDoMinuto.Id = id;             
       _repo.Atualizar(valorDoMinuto);       
       return Redirect("/valores");
    }
    
}
