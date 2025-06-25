$(document).ready(function () {
    // Inicialização da tabela DataTable
    $('#tbPacientes').DataTable({ // ID da tabela atualizado
        "language": { "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Portuguese-Brasil.json" },
        "ordering": false,
        "paging": false,
        "bFilter": true
    });

    // Submissão do formulário de filtros via AJAX
    $("#filtrosPacientes").submit(function (event) { // ID do formulário de filtros atualizado
        event.preventDefault(); // Impede a submissão padrão do formulário
        var formData = $(this).serialize(); // Coleta os dados do formulário de filtros

        $.get("/Pacientes/BuscarPacientes", formData, function (data) { // URL atualizada
            $("#tablePacientes").html(data); // Atualiza a partial view da tabela (ID atualizado)
            $('#tbPacientes').DataTable({ // Re-inicializa o DataTable após atualizar o HTML (ID atualizado)
                "language": { "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Portuguese-Brasil.json" },
                "ordering": false,
                "paging": false,
                "bFilter": true
            });
        });
    });
});

// Função para alterar o status do paciente (Ativar/Inativar)
function alterarStatus(pacienteId, nomePaciente, acao) { // Parâmetros atualizados
    Swal.fire({
        title: `${acao} Paciente`,
        text: `Deseja realmente ${acao.toLowerCase()} o paciente ${nomePaciente}?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: `Sim, ${acao.toLowerCase()}!`,
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Pacientes/AlterarStatus', // URL do método no controlador Pacientes
                method: 'POST',
                data: { id: pacienteId }, // Passa o ID do paciente
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (data) {
                    if (data.resultado === "sucesso") {
                        mensagem.success(data.mensagem || `Paciente ${acao.toLowerCase()} com sucesso!`, "", 10);
                        setTimeout(function () { location.reload(); }, 1500); // Recarrega a página
                    } else {
                        mensagem.error(data.mensagem || `Erro ao ${acao.toLowerCase()} paciente.`, "", 10);
                    }
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error(`Erro ao ${acao.toLowerCase()} paciente`, "Ocorreu um erro desconhecido", 10);
                }
            });
        }
    });
}

function verHistorico(pacienteId) {
    $.get(`/Pacientes/BuscarHistorico?id=${pacienteId}`, function (data) {
        $("#dadoHistorico").html(data);
        $("#modalHistorico").modal("show");
    });
}