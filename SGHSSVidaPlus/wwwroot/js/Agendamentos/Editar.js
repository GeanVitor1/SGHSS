// wwwroot/js/Agendamentos/editar.js

// Variáveis globais para armazenar a seleção temporária para edição
let selectedProfissionalEdicao = null;
let selectedPacienteEdicao = null;

// =========================================================
// FUNÇÕES PARA ABRIR MODAIS (ESPECÍFICAS PARA EDIÇÃO)
// =========================================================

function abrirModalProfissionaisEdicao() {
    $('#modalSelecionarProfissional').modal('show'); // ID do modal
    const container = $('#tabelaProfissionaisContainer');
    container.html('<p class="text-center">Carregando profissionais...</p>');

    $.ajax({
        url: '/Agendamentos/ObterProfissionaisParaSelecao',
        type: 'GET',
        success: function (html) {
            container.html(html);
            $('#btnSelecionarProfissional').prop('disabled', true);
            $('#tabelaProfissionaisContainer .selectable-row').removeClass('table-primary');
        },
        error: function (xhr, status, error) {
            container.html('<p class="text-danger">Erro ao carregar profissionais: ' + (xhr.responseText || error) + '</p>');
            console.error("Erro ao carregar profissionais:", error, xhr.responseText);
        }
    });
}

function abrirModalPacientesEdicao() {
    $('#modalSelecionarPaciente').modal('show'); // ID do modal
    const container = $('#tabelaPacientesContainer');
    container.html('<p class="text-center">Carregando pacientes...</p>');

    $.ajax({
        url: '/Agendamentos/ObterPacientesParaSelecao',
        type: 'GET',
        success: function (html) {
            container.html(html);
            $('#btnSelecionarPaciente').prop('disabled', true);
            $('#tabelaPacientesContainer .selectable-row').removeClass('table-primary');
        },
        error: function (xhr, status, error) {
            container.html('<p class="text-danger">Erro ao carregar pacientes: ' + (xhr.responseText || error) + '</p>');
            console.error("Erro ao carregar pacientes:", error, xhr.responseText);
        }
    });
}

// =========================================================
// FUNÇÃO PARA ENVIAR O FORMULÁRIO DE EDIÇÃO
// =========================================================
window.gravarEdicao = function () {
    var form = $("#formEditar");

    if (!form[0].checkValidity()) {
        form[0].reportValidity();
        if (typeof $.validator !== 'undefined' && $.validator.unobtrusive && !$(form).valid()) {
            return;
        }
        return;
    }

    // Verifique se os IDs ocultos foram preenchidos
    const profissionalId = $('#profissionalResponsavelIdHidden').val();
    const pacienteId = $('#pacienteIdHidden').val();

    if (!profissionalId || profissionalId === '0') {
        Swal.fire("Erro", "Por favor, selecione um Profissional Responsável.", "error"); // Use Swal.fire
        return;
    }
    if (!pacienteId || pacienteId === '0') {
        Swal.fire("Erro", "Por favor, selecione um Paciente.", "error"); // Use Swal.fire
        return;
    }

    var formData = {
        Id: $('#Id').val(),
        Descricao: $('#Descricao').val(),
        DataHoraAgendamento: $('#DataHoraAgendamento').val(),
        ProfissionalResponsavelId: parseInt(profissionalId),
        Local: $('#Local').val(),
        PacienteId: parseInt(pacienteId),
        Status: $('#Status').val(),
        Observacoes: $('#Observacoes').val()
    };

    // if (typeof showOverlay === 'function') showOverlay(".wrapper"); // Se você tiver essa função no site.js

    $.ajax({
        url: "/Agendamentos/Editar", // URL da ação de edição
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (data) {
            // if (typeof hideOverlay === 'function') hideOverlay(".wrapper"); // Se você tiver essa função no site.js

            if (data.resultado === "sucesso") {
                Swal.fire("Sucesso!", data.mensagem || "Agendamento editado com sucesso!", "success")
                    .then(() => {
                        if (data.redirectUrl) {
                            location.href = data.redirectUrl;
                        } else {
                            location.href = "/Agendamentos/Index"; // Fallback
                        }
                    });
            } else {
                Swal.fire("Erro!", data.mensagem || "Erro ao editar agendamento.", "error");
            }
        },
        error: function (xhr, status, error) {
            // if (typeof hideOverlay === 'function') hideOverlay(".wrapper"); // Se você tiver essa função no site.js
            const errorMessage = xhr.responseJSON && xhr.responseJSON.mensagem ? xhr.responseJSON.mensagem : "Erro na comunicação com o servidor.";
            Swal.fire("Erro!", errorMessage, "error");
            console.error("Erro AJAX: ", status, error, xhr.responseText);
        }
    });
}

// =========================================================
// CÓDIGO QUE DEVE RODAR APÓS O DOCUMENTO ESTAR PRONTO
// =========================================================
$(document).ready(function () {
    // Formatar a data e hora para o input type="datetime-local"
    var dataHoraInput = $('#DataHoraAgendamento');
    var rawValue = dataHoraInput.val();
    if (rawValue) {
        var formattedDateTime = moment(rawValue).format('YYYY-MM-DDTHH:mm');
        dataHoraInput.val(formattedDateTime);
    }

    // Inicializa o estado dos botões "Selecionar" no modal
    $('#btnSelecionarProfissional').prop('disabled', true);
    $('#btnSelecionarPaciente').prop('disabled', true);

    // ==========================================================
    // DELEGAÇÃO DE EVENTOS PARA TABELAS DINÂMICAS (PROFISSIONAL)
    // ==========================================================
    $('#modalSelecionarProfissional').on('click', '.selectable-row', function () {
        $('#tabelaProfissionaisContainer .selectable-row').removeClass('table-primary');
        $(this).addClass('table-primary');

        selectedProfissionalEdicao = {
            id: $(this).data('id'),
            nome: $(this).data('nome')
        };

        if (selectedProfissionalEdicao.id && selectedProfissionalEdicao.nome) {
            $('#btnSelecionarProfissional').prop('disabled', false);
        } else {
            $('#btnSelecionarProfissional').prop('disabled', true);
            console.warn("Dados do profissional selecionado não encontrados (verifique data-id e data-nome na sua Partial View).");
        }
    });

    // ==========================================================
    // DELEGAÇÃO DE EVENTOS PARA TABELAS DINÂMICAS (PACIENTE)
    // ==========================================================
    $('#modalSelecionarPaciente').on('click', '.selectable-row', function () {
        $('#tabelaPacientesContainer .selectable-row').removeClass('table-primary');
        $(this).addClass('table-primary');

        selectedPacienteEdicao = {
            id: $(this).data('id'),
            nome: $(this).data('nome')
        };

        if (selectedPacienteEdicao.id && selectedPacienteEdicao.nome) {
            $('#btnSelecionarPaciente').prop('disabled', false);
        } else {
            $('#btnSelecionarPaciente').prop('disabled', true);
            console.warn("Dados do paciente selecionado não encontrados (verifique data-id e data-nome na sua Partial View).");
        }
    });


    // Evento de clique do botão "Selecionar" no modal de profissional
    $('#btnSelecionarProfissional').on('click', function () {
        if (selectedProfissionalEdicao) {
            $('#profissionalResponsavelIdHidden').val(selectedProfissionalEdicao.id);
            $('#ProfissionalResponsavelNomeDisplay').val(selectedProfissionalEdicao.nome); // Atualiza campo de display
            $('#modalSelecionarProfissional').modal('hide');
        }
    });

    // Evento de clique do botão "Selecionar" no modal de paciente
    $('#btnSelecionarPaciente').on('click', function () {
        if (selectedPacienteEdicao) {
            $('#pacienteIdHidden').val(selectedPacienteEdicao.id);
            $('#PacienteNomeDisplay').val(selectedPacienteEdicao.nome); // Atualiza campo de display
            $('#modalSelecionarPaciente').modal('hide');
        }
    });

    // Limpa a seleção e desabilita o botão quando o modal é fechado
    $('#modalSelecionarProfissional').on('hidden.bs.modal', function () {
        selectedProfissionalEdicao = null;
        $('#btnSelecionarProfissional').prop('disabled', true);
        $('#tabelaProfissionaisContainer .selectable-row').removeClass('table-primary');
    });

    $('#modalSelecionarPaciente').on('hidden.bs.modal', function () {
        selectedPacienteEdicao = null;
        $('#btnSelecionarPaciente').prop('disabled', true);
        $('#tabelaPacientesContainer .selectable-row').removeClass('table-primary');
    });

    // Garante que o input text não seja editável manualmente
    $('#ProfissionalResponsavelNomeDisplay').attr('readonly', true);
    $('#PacienteNomeDisplay').attr('readonly', true);
});