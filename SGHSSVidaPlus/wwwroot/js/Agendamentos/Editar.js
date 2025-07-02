// Crie ou abra o arquivo: wwwroot/js/Agendamentos/editar.js

// Define a função gravarEdicao no escopo global
window.gravarEdicao = function () {
    var form = $('#formEditar');

    // Garante que o jQuery Unobtrusive Validation é acionado
    form.validate(); // Inicializa o validador se ainda não foi
    if (!form.valid()) {
        // Se a validação do cliente falhar, exibe os erros e não prossegue
        return;
    }

    var formData = {
        Id: $('#Id').val(),
        Descricao: $('#Descricao').val(),
        DataHoraAgendamento: $('#DataHoraAgendamento').val(),
        Local: $('#Local').val(),
        ProfissionalResponsavelId: $('#ProfissionalResponsavelId').val(),
        PacienteId: $('#PacienteId').val(),
        Observacoes: $('#Observacoes').val(),
        Status: $('#Status').val(),
        Encerrado: $('#Encerrado').is(':checked')
        // DataEncerramento: $('#DataEncerramento').val() // Incluir se houver um campo para isso na view e ViewModel
    };

    $.ajax({
        url: form.attr('action'),
        type: form.attr('method'),
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            // CORREÇÃO AQUI para o SweetAlert2
            if (typeof Swal !== 'undefined' && typeof Swal.fire === 'function') { // Se for SweetAlert2
                Swal.fire({ // Use Swal.fire() para SweetAlert2
                    title: "Sucesso!",
                    text: response.mensagem,
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "OK",
                    customClass: {
                        confirmButton: "btn btn-success"
                    }
                }).then(() => {
                    window.location.href = response.redirectUrl;
                });
            } else if (typeof swal !== 'undefined') { // Se for SweetAlert original
                swal("Sucesso!", response.mensagem, {
                    icon: "success",
                    buttons: {
                        confirm: {
                            className: 'btn btn-success'
                        }
                    },
                }).then(() => {
                    window.location.href = response.redirectUrl;
                });
            } else { // Fallback simples
                alert("Sucesso! " + response.mensagem);
                window.location.href = response.redirectUrl;
            }
        },
        error: function (xhr, status, error) {
            console.error("Erro na requisição AJAX:", error);
            var errorMessage = "Ocorreu um erro inesperado ao editar o agendamento.";
            if (xhr.responseJSON && xhr.responseJSON.mensagem) {
                errorMessage = xhr.responseJSON.mensagem;
            }

            // CORREÇÃO AQUI para o SweetAlert2
            if (typeof Swal !== 'undefined' && typeof Swal.fire === 'function') { // Se for SweetAlert2
                Swal.fire({ // Use Swal.fire() para SweetAlert2
                    title: "Erro!",
                    text: errorMessage,
                    icon: "error",
                    buttonsStyling: false,
                    confirmButtonText: "OK",
                    customClass: {
                        confirmButton: "btn btn-danger"
                    }
                });
            } else if (typeof swal !== 'undefined') { // Se for SweetAlert original
                swal("Erro!", errorMessage, {
                    icon: "error",
                    buttons: {
                        confirm: {
                            className: 'btn btn-danger'
                        }
                    },
                });
            } else { // Fallback simples
                alert("Erro: " + errorMessage);
            }
        }
    });
};

$(document).ready(function () {
    // Formatar a data e hora para o input type="datetime-local"
    var dataHoraInput = $('#DataHoraAgendamento');
    var rawValue = dataHoraInput.val();
    if (rawValue) {
        var formattedDateTime = moment(rawValue).format('YYYY-MM-DDTHH:mm');
        dataHoraInput.val(formattedDateTime);
    }

    // --- Lógica para os Modais de Seleção de Profissional e Paciente ---

    // Cacheie as URLs para evitar chamadas de jQuery repetidas
    var profissionalSelectUrl = $('#ProfissionalSearchBtn').data('ajax-url');
    var pacienteSelectUrl = $('#PacienteSearchBtn').data('ajax-url');

    // Botão de pesquisa de Profissional (assumindo que o ID é 'ProfissionalSearchBtn')
    $('#ProfissionalSearchBtn').on('click', function () {
        $('#modalProfissionais').modal('show');
        $.ajax({
            url: profissionalSelectUrl,
            type: 'GET',
            success: function (data) {
                $('#tabelaProfissionaisContainer').html(data);
            },
            error: function (xhr, status, error) {
                console.error("Erro ao carregar profissionais:", error);
                $('#tabelaProfissionaisContainer').html('<p class="text-danger">Erro ao carregar profissionais. Tente novamente.</p>');
            }
        });
    });

    // Botão de pesquisa de Paciente (assumindo que o ID é 'PacienteSearchBtn')
    $('#PacienteSearchBtn').on('click', function () {
        $('#modalPacientes').modal('show');
        $.ajax({
            url: pacienteSelectUrl,
            type: 'GET',
            success: function (data) {
                $('#tabelaPacientesContainer').html(data);
            },
            error: function (xhr, status, error) {
                console.error("Erro ao carregar pacientes:", error);
                $('#tabelaPacientesContainer').html('<p class="text-danger">Erro ao carregar pacientes. Tente novamente.</p>');
            }
        });
    });

    // Lógica para seleção de Profissional no modal (mantida)
    $(document).on('click', '.select-profissional-btn', function () {
        var profissionalId = $(this).data('id');
        var profissionalNome = $(this).data('nome');
        $('#ProfissionalResponsavelId').val(profissionalId);
        $('#ProfissionalResponsavelNomeDisplay').val(profissionalNome); // Atualiza o campo de exibição
        $('#modalProfissionais').modal('hide');
    });

    // Lógica para seleção de Paciente no modal (mantida)
    $(document).on('click', '.select-paciente-btn', function () {
        var pacienteId = $(this).data('id');
        var pacienteNome = $(this).data('nome');
        $('#PacienteId').val(pacienteId);
        $('#PacienteNomeDisplay').val(pacienteNome); // Atualiza o campo de exibição
        $('#modalPacientes').modal('hide');
    });
});