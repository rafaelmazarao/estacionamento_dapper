using Dapper;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using estacionamento_dapper.Models;
using estacionamento_dapper.Repositorios;
using Microsoft.AspNetCore.Mvc.Rendering;
using estacionamento_dapper.DTO;

namespace estacionamento_dapper.Controllers;

[Route("/tickets")]
public class TicketsController : Controller
{
    private readonly IDbConnection _cnn;
    private readonly IRepositorio<Ticket> _repo;     

    public TicketsController(IDbConnection cnn )
    {
        _cnn = cnn;
        _repo = new RepositorioDapper<Ticket>(_cnn);       
    }
    public IActionResult Index()
    {   
         var sql = @"select t.*, v.*, c.*, vg.* from tickets t
                    inner join veiculos v on v.id = t.veiculoId
                    inner join clientes c on c.id = v.clienteId
                    inner join vagas vg on vg.id = t.vagaId                    
                    order by t.id desc";
        
        var tickets = _cnn.Query<Ticket, Veiculo, Cliente, Vaga, Ticket>(sql, (ticket, veiculo, cliente, vaga) => {
            ticket.veiculo = veiculo;
            veiculo.cliente = cliente;
            ticket.vaga = vaga;
            return ticket;
        }, splitOn: "Id, Id, Id");

        ViewBag.ValorDoMinuto = _cnn.QueryFirstOrDefault<ValorDoMinuto>("select * from valores order by id desc limit 1");
        
        return View(tickets);
    }

    [HttpGet("Novo")]
    public IActionResult Novo()
    {   
        preencheVagasViewBag();
        return View();
    }

    [HttpPost("Criar")]
    public IActionResult Criar([FromForm] TicketDTO ticketDTO)
    { 
        Cliente cliente = buscarOuCadastrarClientePorDTO(ticketDTO);
        Veiculo veiculo = buscarOuCadastrarVeiculoPorDTO(ticketDTO, cliente);

        var ticket = new Ticket();
        ticket.VeiculoId = veiculo.Id;
        ticket.DataEntrada = DateTime.Now;
        ticket.VagaId = ticketDTO.VagaId;

        _repo.Inserir(ticket);
        alteraStatusVaga(ticket.VagaId, true);

        return Redirect("/tickets");
    }

    [HttpPost("{id}/Apagar")]
    public IActionResult Apagar([FromRoute] int id)
    { 
        var ticket = _repo.ObterPorId(id);
        alteraStatusVaga(ticket.VagaId, false);
        _repo.Excluir(id);
        return Redirect("/tickets");
    }

    [HttpPost("{id}/Pago")]
    public IActionResult Pago([FromRoute] int id)
    { 
        var sql = @"select t.*, v.*, c.*, vg.* from tickets t
                    inner join veiculos v on v.id = t.veiculoId
                    inner join clientes c on c.id = v.clienteId
                    inner join vagas vg on vg.id = t.vagaId
                    where t.id = @id";
        
        Ticket? ticket = _cnn.Query<Ticket, Veiculo, Cliente, Vaga, Ticket>(sql, (ticket, veiculo, cliente, vaga) => {
            ticket.veiculo = veiculo;
            veiculo.cliente = cliente;
            ticket.vaga = vaga;
            return ticket;
        }, new {id}, splitOn: "Id, Id, Id").FirstOrDefault();

        if (ticket != null)
        {
            var valorDoMinuto = _cnn.QueryFirstOrDefault<ValorDoMinuto>("select * from valores order by id desc limit 1");
            valorDoMinuto = valorDoMinuto ?? new ValorDoMinuto();            
            ticket.Pago(valorDoMinuto);
            _repo.Atualizar(ticket);        
            alteraStatusVaga(ticket.VagaId, false);
        }
      
        return Redirect("/tickets");
    }

    private void preencheVagasViewBag()
    {
        var sql = @"select * from vagas where ocupado = false";        
        var vagas = _cnn.Query<Vaga>(sql);
        ViewBag.Vagas = new SelectList(vagas, "Id", "CodigoLocalizacao");
    }

    private Cliente buscarOuCadastrarClientePorDTO(TicketDTO ticketDTO)
    {   
        Cliente? cliente = null;

        if (!string.IsNullOrWhiteSpace(ticketDTO.Cpf))
        {
            var sql = @"select * from clientes where Cpf = @Cpf";               
            cliente = _cnn.QueryFirstOrDefault<Cliente>(sql, new Cliente {Cpf = ticketDTO.Cpf});            
        }

        if (cliente != null) 
            return cliente;

        cliente = new Cliente(); 
        cliente.Nome = ticketDTO.Nome;
        cliente.Cpf = ticketDTO.Cpf;

        var query = $"INSERT INTO clientes (Nome, Cpf) VALUES (@Nome, @Cpf); SELECT LAST_INSERT_ID();";
        cliente.Id = _cnn.ExecuteScalar<int>(query, cliente);

        return cliente;
    }

    private Veiculo buscarOuCadastrarVeiculoPorDTO(TicketDTO ticketDTO, Cliente cliente)
    {   
        Veiculo? veiculo = null;

        if (!string.IsNullOrWhiteSpace(ticketDTO.Placa))
        {
            var sql = @"select * from veiculos where Placa = @Placa and ClienteId = @clienteId";               
            veiculo = _cnn.QueryFirstOrDefault<Veiculo>(sql, new Veiculo { Placa = ticketDTO.Placa, ClienteId = cliente.Id });            
        }

        if (veiculo != null) 
            return veiculo;

        veiculo = new Veiculo(); 
        veiculo.Placa = ticketDTO.Placa;
        veiculo.Marca = ticketDTO.Marca;
        veiculo.Modelo = ticketDTO.Modelo;
        veiculo.ClienteId = cliente.Id;       

        var query = $"INSERT INTO veiculos (Placa, Marca, Modelo, ClienteId) VALUES (@Placa, @Marca, @Modelo, @ClienteId); SELECT LAST_INSERT_ID();";
        veiculo.Id = _cnn.ExecuteScalar<int>(query, veiculo);

        return veiculo;
    }

    private void alteraStatusVaga(int Id, bool ocupado)
    { 
        var query = $"UPDATE vagas set ocupado = @ocupado where Id = @Id;";
        _cnn.Execute(query, new {Id, ocupado});        
    }
    
}
