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
            // Após carregar o HTML, garantir que as linhas não estejam selecionadas por padrão e o botão desabilitado
            $('#btnSelecionarProfissional').prop('disabled', true);
            $('#tabelaProfissionaisContainer .selectable-row').removeClass('table-primary');
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
            // Após carregar o HTML, garantir que as linhas não estejam selecionadas por padrão e o botão desabilitado
            $('#btnSelecionarPaciente').prop('disabled', true);
            $('#tabelaPacientesContainer .selectable-row').removeClass('table-primary');
        },
        error: function (xhr, status, error) {
            container.html('<p class="text-danger">Erro ao carregar pacientes: ' + (xhr.responseText || error) + '</p>');
            console.error("Erro ao carregar pacientes:", error, xhr.responseText);
        }
    });
}

// Função para enviar o formulário de inclusão (sem alterações, pois não é o foco do problema atual)
function gravarInclusao() {
    var form = $("#formIncluir");

    if (!form[0].checkValidity()) {
        form[0].reportValidity();
        if (typeof $.validator !== 'undefined' && $.validator.unobtrusive && !$(form).valid()) {
            return;
        }
        return;
    }

    const profissionalId = $('#profissionalResponsavelIdHidden').val();
    const pacienteId = $('#pacienteIdHidden').val();

    if (!profissionalId || profissionalId === '0') {
        mensagem.error("Por favor, selecione um Profissional Responsável.", "", 5);
        return;
    }
    if (!pacienteId || pacienteId === '0') {
        mensagem.error("Por favor, selecione um Paciente.", "", 5);
        return;
    }

    var formData = {
        Descricao: $('#descricao').val(),
        DataHoraAgendamento: $('#dataHoraAgendamento').val(),
        ProfissionalResponsavelId: parseInt(profissionalId),
        Local: $('#local').val(),
        PacienteId: parseInt(pacienteId),
        Status: $('#statusAgendamento').val(),
        Observacoes: $('#observacoes').val()
    };

    if (typeof showOverlay === 'function') showOverlay(".wrapper");

    $.ajax({
        url: "/Agendamentos/Incluir",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (data) {
            if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
            if (data.resultado === "sucesso") {
                mensagem.success(data.mensagem || "Agendamento incluído com sucesso!", "", 10);
                setTimeout(function () {
                    if (data.redirectUrl) {
                        location.href = data.redirectUrl;
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

    // ==========================================================
    // DELEGAÇÃO DE EVENTOS PARA TABELAS DINÂMICAS (PROFISSIONAL)
    // ==========================================================
    // Anexa um ouvinte de eventos ao container do modal (que existe na carga inicial da página)
    // para cliques em '.selectable-row' dentro da tabela de profissionais.
    $('#modalSelecionarProfissional').on('click', '.selectable-row', function () {
        // Remove a classe de destaque de todas as linhas e adiciona à linha clicada
        $('#tabelaProfissionaisContainer .selectable-row').removeClass('table-primary');
        $(this).addClass('table-primary'); // Adiciona uma classe para visualmente indicar a seleção

        // Obtém os dados da linha selecionada.
        // Assumimos que o ID e o Nome estão em atributos data- do TR
        // Ou que você pode pegá-los de TDs específicos (ex: $(this).find('td:eq(0)').text() para ID)
        selectedProfissional = {
            id: $(this).data('id'),   // Certifique-se que o TR tenha data-id="SEU_ID"
            nome: $(this).data('nome') // Certifique-se que o TR tenha data-nome="SEU_NOME"
        };

        // Se o selectedProfissional foi populado, habilita o botão "Selecionar"
        if (selectedProfissional.id && selectedProfissional.nome) {
            $('#btnSelecionarProfissional').prop('disabled', false);
        } else {
            $('#btnSelecionarProfissional').prop('disabled', true);
            console.warn("Dados do profissional selecionado não encontrados (verifique data-id e data-nome na sua Partial View).");
        }
    });

    // ==========================================================
    // DELEGAÇÃO DE EVENTOS PARA TABELAS DINÂMICAS (PACIENTE)
    // ==========================================================
    // Anexa um ouvinte de eventos ao container do modal para cliques em '.selectable-row'
    // dentro da tabela de pacientes.
    $('#modalSelecionarPaciente').on('click', '.selectable-row', function () {
        // Remove a classe de destaque de todas as linhas e adiciona à linha clicada
        $('#tabelaPacientesContainer .selectable-row').removeClass('table-primary');
        $(this).addClass('table-primary');

        selectedPaciente = {
            id: $(this).data('id'),   // Certifique-se que o TR tenha data-id="SEU_ID"
            nome: $(this).data('nome') // Certifique-se que o TR tenha data-nome="SEU_NOME"
        };

        if (selectedPaciente.id && selectedPaciente.nome) {
            $('#btnSelecionarPaciente').prop('disabled', false);
        } else {
            $('#btnSelecionarPaciente').prop('disabled', true);
            console.warn("Dados do paciente selecionado não encontrados (verifique data-id e data-nome na sua Partial View).");
        }
    });


    // Evento de clique do botão "Selecionar" no modal de profissional
    $('#btnSelecionarProfissional').on('click', function () {
        if (selectedProfissional) {
            $('#profissionalResponsavelIdHidden').val(selectedProfissional.id);
            $('#profissionalResponsavelNome').val(selectedProfissional.nome);
            $('#modalSelecionarProfissional').modal('hide');
            // selectedProfissional é resetado no 'hidden.bs.modal'
        }
    });

    // Evento de clique do botão "Selecionar" no modal de paciente
    $('#btnSelecionarPaciente').on('click', function () {
        if (selectedPaciente) {
            $('#pacienteIdHidden').val(selectedPaciente.id);
            $('#pacienteNome').val(selectedPaciente.nome);
            $('#modalSelecionarPaciente').modal('hide');
            // selectedPaciente é resetado no 'hidden.bs.modal'
        }
    });

    // Limpa a seleção e desabilita o botão quando o modal é fechado
    // Este evento 'hidden.bs.modal' já está correto e será disparado quando o modal fechar,
    // seja pelo botão "Selecionar", pelo botão "Fechar" ou pelo 'X'.
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

    // ==========================================================
    // TRATAMENTO DO BOTÃO "FECHAR" (EXISTE O data-bs-dismiss no HTML?)
    // ==========================================================
    // Para o botão "Fechar" (e o 'X' de fechar no cabeçalho do modal) funcionar,
    // o elemento HTML deles deve ter o atributo 'data-bs-dismiss="modal"'.
    // Exemplo:
    // <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Fechar</button>
    // Se o seu botão "Fechar" não tem esse atributo, ele não vai fechar o modal.
    // Se ele já tiver, não precisa de JS adicional para o fechar.
});