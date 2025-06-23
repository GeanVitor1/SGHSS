$(document).ready(function () {
    // Inicialização da tabela DataTable
    $('#tbAgendamentos').DataTable({ // ID da tabela atualizado
        "language": { "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Portuguese-Brasil.json" },
        "ordering": false,
        "paging": false,
        "bFilter": true
    });

    // Submissão do formulário de filtros via AJAX
    $("#filtrosAgendamentos").submit(function (event) {
        event.preventDefault(); // Impede a submissão padrão do formulário
        var formData = $(this).serialize(); // Coleta os dados do formulário de filtros

        $.get("/Agendamentos/BuscarAgendamentos", formData, function (data) { // URL atualizada
            $("#tableAgendamentos").html(data); // Atualiza a partial view da tabela
            $('#tbAgendamentos').DataTable({ // Re-inicializa o DataTable após atualizar o HTML
                "language": { "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Portuguese-Brasil.json" },
                "ordering": false,
                "paging": false,
                "bFilter": true
            });
        });
    });
});

// Função para alterar o status do agendamento (Encerrar/Reabrir)
function alterarStatus(agendamentoId, descricao, acao) { // Parâmetros atualizados
    Swal.fire({
        title: `${acao} Agendamento`,
        text: `Deseja realmente ${acao.toLowerCase()} o agendamento: ${descricao}?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: `Sim, ${acao.toLowerCase()}!`,
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Agendamentos/AlterarStatus', // URL do método no controlador Agendamentos
                method: 'POST',
                data: { id: agendamentoId }, // Passa o ID do agendamento
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (data) {
                    if (data.resultado === "sucesso") {
                        mensagem.success(data.mensagem || `Agendamento ${acao.toLowerCase()} com sucesso!`, "", 10);
                        setTimeout(function () { location.reload(); }, 1500); // Recarrega a página
                    } else {
                        mensagem.error(data.mensagem || `Erro ao ${acao.toLowerCase()} agendamento.`, "", 10);
                    }
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error(`Erro ao ${acao.toLowerCase()} agendamento`, "Ocorreu um erro desconhecido", 10);
                }
            });
        }
    });
}