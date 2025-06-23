// Funções para carregar e exibir os modais de inclusão de item
function incluirContatos() {
    $.get("/Pacientes/TelaNovoContato", function (data) {
        $("#modalNovoContato .modal-body").html(data);
        $("#modalNovoContato").modal("show");
    });
}

function novoHistorico() {
    $.get("/Pacientes/TelaNovoHistorico", function (data) {
        $("#modalNovoHistorico .modal-body").html(data);
        $("#modalNovoHistorico").modal("show");
    });
}

// Funções para salvar os dados adicionados nos modais
function salvarNovoContato() {
    var form = $("#formIncluirContato");
    var contatoData = form.serialize();

    $.post("/Pacientes/IncluirContato", contatoData, function (response) {
        // A resposta agora contém { resultado: "sucesso", mensagem: "...", partialHtml: "..." }
        if (response && response.resultado && response.resultado.toLowerCase() === "sucesso") {
            $("#modalNovoContato").modal("hide"); // Fecha o modal
            $("#contatotable").html(response.partialHtml); // ATUALIZA A TABELA COM O HTML RECEBIDO NO JSON
            form[0].reset();
            mensagem.success(response.mensagem || "Contato adicionado com sucesso!", "", 10);
        } else {
            // Se a resposta for JSON mas não tiver "resultado: sucesso"
            mensagem.error(response && response.mensagem ? response.mensagem : "Erro desconhecido ao adicionar contato.", "", 10);
        }
    }).fail(function (jqXHR) {
        // Erro na requisição AJAX (rede, servidor não respondeu com 200 OK)
        mensagem.error("Erro na comunicação com o servidor: " + jqXHR.statusText, "", 10);
    });
}

function salvarNovoHistorico() {
    var form = $("#formIncluirHistorico");
    var historicoData = form.serialize();

    $.post("/Pacientes/IncluirHistorico", historicoData, function (response) {
        if (response && response.resultado && response.resultado.toLowerCase() === "sucesso") {
            $("#modalNovoHistorico").modal("hide"); // Fecha o modal
            $("#historicoTable").html(response.partialHtml); // ATUALIZA A TABELA COM O HTML RECEBIDO NO JSON
            form[0].reset();
            mensagem.success(response.mensagem || "Registro de histórico adicionado com sucesso!", "", 10);
        } else {
            mensagem.error(response && response.mensagem ? response.mensagem : "Erro desconhecido ao adicionar histórico.", "", 10);
        }
    }).fail(function (jqXHR) {
        mensagem.error("Erro na comunicação com o servidor: " + jqXHR.statusText, "", 10);
    });
}

// ... (restante das funções do Incluir.js, como gravarInclusao, que não foram alteradas)
function gravarInclusao() {
    var form = $("#formIncluir");
    var formData = form.serialize();

    if (form[0].checkValidity()) {
        if (typeof showOverlay === 'function') showOverlay(".wrapper");

        $.ajax({
            url: "/Pacientes/Incluir",
            method: "POST",
            data: formData,
            success: function (data) {
                if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                if (data.resultado === "sucesso") {
                    mensagem.success(data.mensagem || "Paciente incluído com sucesso!", "", 10);
                    setTimeout(function () { location.href = "/Pacientes/Index"; }, 1500);
                } else {
                    mensagem.error(data.mensagem || "Erro ao incluir paciente.", "", 10);
                }
            },
            error: function () {
                if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                mensagem.error("Erro na comunicação com o servidor.", "", 10);
            }
        });
    } else {
        form[0].reportValidity();
    }
}