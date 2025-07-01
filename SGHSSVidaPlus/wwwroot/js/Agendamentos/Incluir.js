// Variáveis globais para armazenar a seleção temporária
let selectedProfissional = null;
let selectedPaciente = null;

// =========================================================
// FUNÇÕES PARA ABRIR MODAIS - DEVEM ESTAR NO ESCOPO GLOBAL
// PARA SEREM ACESSÍVEIS PELO ATRIBUTO 'onclick' NO HTML
// =========================================================

// Função para abrir o modal de profissionais e carregar a partial view
function abrirModalProfissionais() {
    $('#modalSelecionarProfissional').modal('show');
    const container = $('#tabelaProfissionaisContainer');
    container.html('<p class="text-center">Carregando profissionais...</p>'); // Mensagem de carregamento

    $.ajax({
        url: '/Agendamentos/ObterProfissionaisParaSelecao', // Ação que retorna a Partial View
        type: 'GET',
        success: function (html) {
            container.html(html); // Insere o HTML da Partial View no container
        },
        error: function (xhr, status, error) {
            container.html('<p class="text-danger">Erro ao carregar profissionais: ' + (xhr.responseText || error) + '</p>');
            console.error("Erro ao carregar profissionais:", error, xhr.responseText);
        }
    });
}

// Função para abrir o modal de pacientes e carregar a partial view
function abrirModalPacientes() {
    $('#modalSelecionarPaciente').modal('show');
    const container = $('#tabelaPacientesContainer');
    container.html('<p class="text-center">Carregando pacientes...</p>'); // Mensagem de carregamento

    $.ajax({
        url: '/Agendamentos/ObterPacientesParaSelecao', // Ação que retorna a Partial View
        type: 'GET',
        success: function (html) {
            container.html(html); // Insere o HTML da Partial View no container
        },
        error: function (xhr, status, error) {
            container.html('<p class="text-danger">Erro ao carregar pacientes: ' + (xhr.responseText || error) + '</p>');
            console.error("Erro ao carregar pacientes:", error, xhr.responseText);
        }
    });
}

// Função para enviar o formulário de inclusão
function gravarInclusao() {
    var form = $("#formIncluir");

    // Dispara a validação HTML5 nativa
    if (!form[0].checkValidity()) {
        form[0].reportValidity();
        // Se houver validações adicionais do jQuery Unobtrusive, eles também serão disparados aqui
        if (typeof $.validator !== 'undefined' && $.validator.unobtrusive && !$(form).valid()) {
            return; // Impede o envio se a validação unobtrusive falhar
        }
        return; // Impede o envio se a validação HTML5 falhar
    }

    // Pega os IDs dos campos hidden
    const profissionalId = $('#profissionalResponsavelIdHidden').val();
    const pacienteId = $('#pacienteIdHidden').val();

    // Validação adicional para garantir que um profissional e paciente foram selecionados
    if (!profissionalId || profissionalId === '0') { // Verifica se é vazio ou 0
        mensagem.error("Por favor, selecione um Profissional Responsável.", "", 5);
        return;
    }
    if (!pacienteId || pacienteId === '0') { // Verifica se é vazio ou 0
        mensagem.error("Por favor, selecione um Paciente.", "", 5);
        return;
    }

    // Se você estiver enviando JSON para o controller, monte o objeto de dados
    var formData = {
        Descricao: $('#descricao').val(),
        DataHoraAgendamento: $('#dataHoraAgendamento').val(),
        ProfissionalResponsavelId: parseInt(profissionalId), // Converte para int
        Local: $('#local').val(),
        PacienteId: parseInt(pacienteId), // Converte para int
        Status: $('#statusAgendamento').val(),
        Observacoes: $('#observacoes').val()
    };

    if (typeof showOverlay === 'function') showOverlay(".wrapper");

    $.ajax({
        url: "/Agendamentos/Incluir", // URL do controlador
        method: "POST",
        contentType: "application/json", // Informa ao servidor que estamos enviando JSON
        data: JSON.stringify(formData), // Converte o objeto JavaScript para uma string JSON
        success: function (data) {
            if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
            if (data.resultado === "sucesso") {
                mensagem.success(data.mensagem || "Agendamento incluído com sucesso!", "", 10);
                setTimeout(function () {
                    if (data.redirectUrl) {
                        location.href = data.redirectUrl; // Usa a URL de redirecionamento do servidor
                    } else {
                        location.href = "/Agendamentos/Index";
                    }
                }, 1500);
            } else {
                mensagem.error(data.mensagem || "Erro ao incluir agendamento.", "", 10);
            }
        },
        error: function (xhr, status, error) {
            if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
            const errorMessage = xhr.responseJSON && xhr.responseJSON.mensagem ? xhr.responseJSON.mensagem : "Erro na comunicação com o servidor.";
            mensagem.error(errorMessage, "", 10);
            console.error("Erro AJAX: ", status, error, xhr.responseText);
        }
    });
}


// =========================================================
// CÓDIGO QUE DEVE RODAR APÓS O DOCUMENTO ESTAR PRONTO
// =========================================================
$(document).ready(function () {
    // Inicializa o estado dos botões "Selecionar"
    $('#btnSelecionarProfissional').prop('disabled', true);
    $('#btnSelecionarPaciente').prop('disabled', true);

    // Não precisa mais anexar eventos de click aos botões de abrir modal aqui,
    // pois eles já usam o atributo onclick no HTML.

    // Evento de clique do botão "Selecionar" no modal de profissional
    $('#btnSelecionarProfissional').on('click', function () {
        if (selectedProfissional) {
            $('#profissionalResponsavelIdHidden').val(selectedProfissional.id);
            $('#profissionalResponsavelNome').val(selectedProfissional.nome);
            $('#modalSelecionarProfissional').modal('hide');
            selectedProfissional = null; // Reseta a seleção
            $('#btnSelecionarProfissional').prop('disabled', true); // Desabilita o botão novamente
        }
    });

    // Evento de clique do botão "Selecionar" no modal de paciente
    $('#btnSelecionarPaciente').on('click', function () {
        if (selectedPaciente) {
            $('#pacienteIdHidden').val(selectedPaciente.id);
            $('#pacienteNome').val(selectedPaciente.nome);
            $('#modalSelecionarPaciente').modal('hide');
            selectedPaciente = null; // Reseta a seleção
            $('#btnSelecionarPaciente').prop('disabled', true); // Desabilita o botão novamente
        }
    });

    // Limpa a seleção e desabilita o botão quando o modal é fechado
    $('#modalSelecionarProfissional').on('hidden.bs.modal', function () {
        selectedProfissional = null;
        $('#btnSelecionarProfissional').prop('disabled', true);
        $('#tabelaProfissionaisContainer .selectable-row').removeClass('table-primary');
    });

    $('#modalSelecionarPaciente').on('hidden.bs.modal', function () {
        selectedPaciente = null;
        $('#btnSelecionarPaciente').prop('disabled', true);
        $('#tabelaPacientesContainer .selectable-row').removeClass('table-primary');
    });

    // Garante que o input text não seja editável manualmente
    $('#profissionalResponsavelNome').attr('readonly', true);
    $('#pacienteNome').attr('readonly', true);
});