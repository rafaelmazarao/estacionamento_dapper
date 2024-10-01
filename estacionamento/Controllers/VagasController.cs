using Dapper;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using estacionamento_dapper.Models;
using estacionamento_dapper.Repositorios;

namespace estacionamento_dapper.Controllers;

[Route("/vagas")]
public class VagasController : Controller
{
    private readonly IRepositorio<Vaga> _repo;

    public VagasController(IRepositorio<Vaga> repo)
    {
        _repo = repo;
    }
    public IActionResult Index()
    {   
        var vagas = _repo.ObterTodos();
        return View(vagas);
    }

    [HttpGet("Novo")]
    public IActionResult Novo()
    {   
        return View();
    }

    [HttpPost("Criar")]
    public IActionResult Criar([FromForm] Vaga vaga)
    { 
        _repo.Inserir(vaga);
        return Redirect("/vagas");
    }

    [HttpPost("{id}/Apagar")]
    public IActionResult Apagar([FromRoute] int id)
    { 
        _repo.Excluir(id);
        return Redirect("/vagas");
    }

    [HttpGet("{id}/Editar")]
    public IActionResult Editar([FromRoute] int id)
    { 
        var valor = _repo.ObterPorId(id);
        return View(valor);
    }

    [HttpPost("{id}/Alterar")]
    public IActionResult Alterar([FromRoute] int id, [FromForm] Vaga vaga )
    { 
       vaga.Id = id;             
       _repo.Atualizar(vaga);       
       return Redirect("/vagas");
    }
}
