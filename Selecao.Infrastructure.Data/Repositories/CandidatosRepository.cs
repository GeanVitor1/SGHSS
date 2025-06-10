using Microsoft.EntityFrameworkCore;
using Selecao.Domain.Entities;
using Selecao.Domain.Interfaces.Repository;
using Selecao.Infrastructure.Data.Context;
using Selecao.Infrastructure.Data.Repository;

namespace Selecao.Infrastructure.Data.Repositories
{
    public class CandidatosRepository : RepositoryBase<Candidato>, ICandidatosRepository
    {
        public CandidatosRepository(SelecaoContext context) : base(context) { }

        public async Task<List<Candidato>> BuscarCandidatos(CandidatoParams parametros)
        {
            var query = _context.Candidatos.AsQueryable();

            if (parametros.InfoExtras)
            {
                query = query.Include(c => c.Contatos)
                    .Include(c => c.Formacao)
                    .Include(c => c.Cursos)
                    .Include(c => c.Experiencias);
            }

            if (parametros.Id != 0)
                query = query.Where(c => c.Id.ToString().Contains(parametros.Id.ToString()));

            if (!string.IsNullOrWhiteSpace(parametros.Nome))
                query = query.Where(c => c.Nome.Contains(parametros.Nome));

            if (parametros.DataInclusaoInicio != DateTime.MinValue && parametros.DataInclusaoFim != DateTime.MinValue)
            {
                parametros.DataInclusaoFim = parametros.DataInclusaoFim.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.DataInclusao >= parametros.DataInclusaoInicio && c.DataInclusao <= parametros.DataInclusaoFim);
            }

            if (parametros.DataNascimentoInicio != DateTime.MinValue && parametros.DataNascimentoFim != DateTime.MinValue)
            {
                parametros.DataNascimentoFim = parametros.DataNascimentoFim.Date.AddDays(1).AddTicks(-1);
                query = query.Where(c => c.DataNascimento >= parametros.DataNascimentoInicio && c.DataNascimento <= parametros.DataNascimentoFim);
            }

            if (!string.IsNullOrWhiteSpace(parametros.Endereço))
                query = query.Where(c => c.Logradouro.Contains(parametros.Endereço) || c.Bairro.Contains(parametros.Endereço));

            if (!string.IsNullOrWhiteSpace(parametros.EstadoCivil))
                query = query.Where(c => c.EstadoCivil == parametros.EstadoCivil);

            if (!string.IsNullOrWhiteSpace(parametros.UsuarioInclusao))
                query = query.Where(c => c.EstadoCivil == parametros.UsuarioInclusao);


            switch (parametros.Status)
            {
                case "1":
                    query = query.Where(c => !c.Selecionado);
                    query = query.Where(c => !c.Banido);
                    break;

                case "2":
                    query = query.Where(c => c.Selecionado);
                    query = query.Where(c => !c.Banido);
                    break;

                case "3":
                    query = query.Where(c => !c.Selecionado);
                    query = query.Where(c => c.Banido);
                    break;
            }

            query = query.OrderByDescending(c => c.Id);

            if (parametros.TotalRegistros != 0)
                query = query.Take(parametros.TotalRegistros);

            return await query.ToListAsync();
        }

        public async Task Editar(Candidato candidato)
        {
            // Buscando o candidato no banco de dados
            var candidatoBd = (await BuscarCandidatos(new CandidatoParams(){ Id = candidato.Id, InfoExtras = true })).FirstOrDefault();

            //Tratando os  contatos
            var contatosParaRemover = candidatoBd.Contatos.Where(c => !candidato.Contatos.Any(cn => cn.Contato == c.Contato && cn.Tipo == c.Tipo)).ToList();
            var contatosParaAdicionar = candidato.Contatos.Where(c => !candidatoBd.Contatos.Any(cn => cn.Contato == c.Contato && cn.Tipo == c.Tipo)).ToList();            
            _context.CandidatoContatos.RemoveRange(contatosParaRemover);

            foreach (var contato in contatosParaAdicionar)
            {
                contato.CandidatoId = candidatoBd.Id;
                _context.Entry(contato).State = EntityState.Added;
            }

            //trantando as formações
            var formacaoParaRemover = candidatoBd.Formacao.Where(f => !candidato.Formacao.Any(fn => fn.Titulo == f.Titulo && fn.Area == f.Area)).ToList();
            var formacaoParaAdicionar = candidato.Formacao.Where(f => !candidatoBd.Formacao.Any(fn => fn.Titulo == f.Titulo && fn.Area == f.Area)).ToList();
            _context.CandidatoFormacoes.RemoveRange(formacaoParaRemover);

            foreach(var formacao in formacaoParaAdicionar)
            {
                formacao.CandidatoId = candidatoBd.Id;
                _context.Entry(formacao).State = EntityState.Added;
            }

            //tratando os cursos
            var cursoParaRemover = candidatoBd.Cursos.Where(c => !candidato.Cursos.Any(cn => cn.Titulo == c.Titulo && cn.DuracaoHoras == c.DuracaoHoras && cn.InstituicaoEnsino == c.InstituicaoEnsino)).ToList();
            var cursoParaAdicionar = candidato.Cursos.Where(c => !candidatoBd.Cursos.Any(cn => cn.Titulo == c.Titulo && cn.DuracaoHoras == c.DuracaoHoras && cn.InstituicaoEnsino == c.InstituicaoEnsino)).ToList();
            _context.CandidatoCursos.RemoveRange(cursoParaRemover);

            foreach(var curso in cursoParaAdicionar)
            {
                curso.CandidatoId = candidatoBd.Id;
                _context.Entry(curso).State = EntityState.Added;
            }

            //tratando experiencias
            var experienciaParaRemover = candidatoBd.Experiencias.Where(e => !candidato.Experiencias.Any(en => en.Empregador == e.Empregador && en.Inicio == e.Inicio && en.Cargo == e.Cargo)).ToList();
            var experienciaParaAdicionar = candidato.Experiencias.Where(e => !candidatoBd.Experiencias.Any(en => en.Empregador == e.Empregador && en.Inicio == e.Inicio && en.Cargo == e.Cargo)).ToList();
            _context.CandidatoExperiencias.RemoveRange(experienciaParaRemover);

            foreach(var experiencia in experienciaParaAdicionar)
            {
                experiencia.CandidatoId = candidatoBd.Id;
                _context.Entry(experiencia).State = EntityState.Added;
            }


            // Desanexar a entidade candidatoBd para evitar conflito de rastreamento
            _context.Entry(candidatoBd).State = EntityState.Detached;

            // Atualizar o candidatoBd com os dados de candidato
            candidatoBd.Nome = candidato.Nome;
            candidatoBd.DataNascimento = candidato.DataNascimento;
            candidatoBd.Logradouro = candidato.Logradouro;
            candidatoBd.Bairro = candidato.Bairro;
            candidatoBd.EstadoCivil = candidato.EstadoCivil;
            candidatoBd.Selecionado = candidato.Selecionado;
            candidatoBd.Banido = candidato.Banido;
            candidatoBd.Contatos = null;
            
            // Agora, reanexamos o candidatoBd para realizar a persistência das alterações
            _context.Candidatos.Update(candidatoBd);

            // Salvar as alterações no banco de dados
            await _context.SaveChangesAsync();
        }


    }
}
