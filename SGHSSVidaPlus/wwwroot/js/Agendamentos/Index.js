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
// Exemplo de como sua função alterarStatus pode parecer em site.js
// (Pode ter pequenas variações)

function alterarStatus(id, nomeOuDescricao, acao) {
    let titulo = '';
    let mensagem = '';
    let url = '';
    let confirmText = '';
    let successMessage = '';

    if (acao === 'Inativar') {
        titulo = 'Inativar ' + nomeOuDescricao + '?';
        mensagem = 'Esta ação irá inativar o registro. Deseja continuar?';
        url = '/Pacientes/AlterarStatus'; // <<-- ESTA É A URL QUE ESTÁ SENDO CHAMADA PARA PACIENTES
        confirmText = 'Sim, inativar!';
        successMessage = 'Registro inativado com sucesso!';
    } else if (acao === 'Reativar') {
        titulo = 'Reativar ' + nomeOuDescricao + '?';
        mensagem = 'Esta ação irá reativar o registro. Deseja continuar?';
        url = '/Pacientes/AlterarStatus'; // <<-- ESTA É A URL QUE ESTÁ SENDO CHAMADA PARA PACIENTES
        confirmText = 'Sim, reativar!';
        successMessage = 'Registro reativado com sucesso!';
    } else if (acao === 'Encerrar') { // NOVO CASO PARA AGENDAMENTO
        titulo = 'Encerrar Agendamento: ' + nomeOuDescricao + '?';
        mensagem = 'Esta ação irá encerrar o agendamento. Deseja continuar?';
        url = '/Agendamentos/EncerrarAgendamento'; // <<-- URL CORRETA PARA AGENDAMENTO
        confirmText = 'Sim, encerrar!';
        successMessage = 'Agendamento encerrado com sucesso!';
    } else if (acao === 'Reabrir') { // NOVO CASO PARA AGENDAMENTO
        titulo = 'Reabrir Agendamento: ' + nomeOuDescricao + '?';
        mensagem = 'Esta ação irá reabrir o agendamento. Deseja continuar?';
        url = '/Agendamentos/ReabrirAgendamento'; // <<-- Você precisará deste método no Controller e Service
        confirmText = 'Sim, reabrir!';
        successMessage = 'Agendamento reaberto com sucesso!';
    } else {
        console.error("Ação desconhecida: " + acao);
        return;
    }

    Swal.fire({ // Usando SweetAlert2
        title: titulo,
        text: mensagem,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: confirmText,
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'POST', // Geralmente é POST para alteração de status
                contentType: 'application/json',
                data: JSON.stringify(id), // Envia apenas o ID
                success: function (response) {
                    if (response.resultado === "sucesso") {
                        Swal.fire('Sucesso!', response.mensagem || successMessage, 'success')
                            .then(() => {
                                // Redireciona para atualizar a tabela
                                location.reload(); // Ou para uma URL específica: location.href = '/Agendamentos/Index';
                            });
                    } else {
                        Swal.fire('Erro!', response.mensagem || 'Ocorreu um erro ao alterar o status.', 'error');
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Erro AJAX ao alterar status:", error, xhr.responseText);
                    let errorMessage = 'Erro na comunicação com o servidor ao alterar o status.';
                    if (xhr.responseJSON && xhr.responseJSON.mensagem) {
                        errorMessage = xhr.responseJSON.mensagem;
                    }
                    Swal.fire('Erro!', errorMessage, 'error');
                }
            });
        }
    });
}
