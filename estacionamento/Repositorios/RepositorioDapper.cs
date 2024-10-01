using Dapper;
using estacionamento_dapper.Repositorios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data; 
using System.Linq;
using System.Reflection;

namespace estacionamento_dapper.Repositorios;

public class RepositorioDapper<T> : IRepositorio<T>
{
    private readonly IDbConnection _conexao;
    private readonly string _nomeTabela;

    public RepositorioDapper(IDbConnection conexao)
    {
        _conexao = conexao;
        _nomeTabela = ObterNomeTabela();
    }

    private string ObterNomeTabela()
    {
        var tipo = typeof(T);
        var atributoTabela = tipo.GetCustomAttribute<TableAttribute>();
        if (atributoTabela != null)
        {
            return atributoTabela.Name;
        }
        return tipo.Name;
    }

    public void Atualizar(T entidade)
    {
        var campos = ObterCamposUpdate(entidade);
        var sql = $"UPDATE {_nomeTabela} SET {campos} WHERE Id = @Id";
        _conexao.Execute(sql, entidade);
    }

    public void Excluir(int id)
    {
        var sql = $"DELETE FROM {_nomeTabela} WHERE Id = @Id";
        _conexao.Execute(sql, new { Id = id });
    }

    public void Inserir(T entidade)
    {
        var campos = ObterCamposInsert(entidade);
        var valores = ObterValoresInsert(entidade);
        var sql = $"INSERT INTO {_nomeTabela} ({campos}) VALUES ({valores})";
        _conexao.Execute(sql, entidade);
    }

    public T ObterPorId(int id)
    {
        var sql = $"SELECT * FROM {_nomeTabela} WHERE Id = @Id";
        return _conexao.QueryFirst<T>(sql, new { id });
    }

    public IEnumerable<T> ObterTodos()
    {
        var sql = $"SELECT * FROM {_nomeTabela}";
        return _conexao.Query<T>(sql);
    }

    private string ObterCamposInsert(T entidade)
    {
        var tipo = typeof(T);
        var propriedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !Attribute.IsDefined(p, typeof(IgnoreInDapperAttribute)));

        var nomesCampos = propriedades.Select(p => 
        {
            var colunaName = p.GetCustomAttribute<ColumnAttribute>()?.Name;
            return colunaName ?? p.Name;
        });
        return string.Join(", ", nomesCampos);
    }

    private string ObterValoresInsert(T entidade)
    {
        var tipo = typeof(T);
        var propriedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !Attribute.IsDefined(p, typeof(IgnoreInDapperAttribute)));

        var nomesCampos = propriedades.Select(p => 
        {
            var colunaName = p.GetCustomAttribute<ColumnAttribute>()?.Name;
            return colunaName ?? p.Name;
        });

        return string.Join(", ", nomesCampos.Select(p => $"@{p}"));
    }

    private string ObterCamposUpdate(T entidade)
    {
        var tipo = typeof(T);
        var propriedades = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !Attribute.IsDefined(p, typeof(IgnoreInDapperAttribute)));

        var nomesCampos = propriedades.Select(p => 
        {
            var colunaName = p.GetCustomAttribute<ColumnAttribute>()?.Name;
            return $"{colunaName ?? p.Name} = @{p.Name}";
        });
        
        return string.Join(", ", nomesCampos);
    }
}