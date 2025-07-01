$(document).ready(function () {
    // Inicializa��o da tabela DataTable
    $('#tbProfissionalSaude').DataTable({ // ID da tabela atualizado
        "language": { "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Portuguese-Brasil.json" },
        "ordering": false,
        "paging": false,
        "bFilter": true
    });

    // Submiss�o do formul�rio de filtros via AJAX
    $("#filtrosProfissionalSaude").submit(function (event) { // ID do formul�rio de filtros atualizado
        event.preventDefault(); // Impede a submiss�o padr�o do formul�rio
        var formData = $(this).serialize(); // Coleta os dados do formul�rio de filtros

        $.get("/ProfissionalSaude/BuscarProfissionais", formData, function (data) { // URL atualizada
            $("#tbProfissionalSaude").html(data); // Atualiza a partial view da tabela (ID atualizado)
            $('#ProfissionalSaude').DataTable({ // Re-inicializa o DataTable ap�s atualizar o HTML (ID atualizado)
                "language": { "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Portuguese-Brasil.json" },
                "ordering": false,
                "paging": false,
                "bFilter": true
            });
        });
    });
});

// Fun��o para alterar o status do profissional de sa�de (Ativar/Inativar)
function alterarStatus(profissionalId, nomeProfissional, acao) { // Par�metros atualizados
    Swal.fire({
        title: `${acao} Profissional`,
        text: `Deseja realmente ${acao.toLowerCase()} o profissional ${nomeProfissional}?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: `Sim, ${acao.toLowerCase()}!`,
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/ProfissionalSaude/AlterarStatus', // URL do m�todo no controlador ProfissionaisSaude
                method: 'POST',
                data: { id: profissionalId }, // Passa o ID do profissional
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (data) {
                    if (data.resultado === "sucesso") {
                        mensagem.success(data.mensagem || `Profissional ${acao.toLowerCase()} com sucesso!`, "", 10);
                        setTimeout(function () { location.reload(); }, 1500); // Recarrega a p�gina
                    } else {
                        mensagem.error(data.mensagem || `Erro ao ${acao.toLowerCase()} profissional.`, "", 10);
                    }
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error(`Erro ao ${acao.toLowerCase()} profissional`, "Ocorreu um erro desconhecido", 10);
                }
            });
        }
    });
}