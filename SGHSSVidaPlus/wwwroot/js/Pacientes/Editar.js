// Funções para carregar e exibir os modais de inclusão de item (incluídas para o contexto de Edição)
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

// Funções para salvar os dados adicionados nos modais (incluídas para o contexto de Edição)
function salvarNovoContato() {
    var form = $("#formIncluirContato"); // ID do formulário dentro da partial _NovoContato
    var contatoData = form.serialize();

    $.post("/Pacientes/IncluirContato", contatoData, function (response) {
        if (response && response.resultado && response.resultado.toLowerCase() === "sucesso") {
            $("#modalNovoContato").modal("hide");
            $("#contatotable").html(response.partialHtml);
            form[0].reset();
            mensagem.success(response.mensagem || "Contato adicionado com sucesso!", "", 10);
        } else {
            mensagem.error(response && response.mensagem ? response.mensagem : "Erro desconhecido ao adicionar contato.", "", 10);
        }
    }).fail(function (jqXHR) {
        mensagem.error("Erro na comunicação com o servidor: " + jqXHR.statusText, "", 10);
    });
}

function salvarNovoHistorico() {
    var form = $("#formIncluirHistorico"); // ID do formulário dentro da partial _NovoHistorico
    var historicoData = form.serialize();

    $.post("/Pacientes/IncluirHistorico", historicoData, function (response) {
        if (response && response.resultado && response.resultado.toLowerCase() === "sucesso") {
            $("#modalNovoHistorico").modal("hide");
            $("#historicoTable").html(response.partialHtml);
            form[0].reset();
            mensagem.success(response.mensagem || "Registro de histórico adicionado com sucesso!", "", 10);
        } else {
            mensagem.error(response && response.mensagem ? response.mensagem : "Erro desconhecido ao adicionar histórico.", "", 10);
        }
    }).fail(function (jqXHR) {
        mensagem.error("Erro na comunicação com o servidor: " + jqXHR.statusText, "", 10);
    });
}

// Funções para remover itens das listas (incluídas para o contexto de Edição)
function removerContato(contato, tipo) {
    Swal.fire({
        title: "Remover Contato?",
        text: `Deseja realmente remover o contato ${contato} (${tipo})?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sim, remover!",
        cancelButtonText: "Cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Pacientes/RemoverContato',
                method: 'POST',
                data: { contato: contato, tipo: tipo },
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (response) {
                    if (response.resultado === "sucesso") {
                        mensagem.success(response.mensagem || "Contato removido com sucesso!", "", 10);
                        $("#contatotable").html(response.partialHtml);
                    } else {
                        mensagem.error(response.mensagem || "Erro ao remover contato.", "", 10);
                    }
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error("Erro na comunicação com o servidor ao remover contato.", "", 10);
                }
            });
        }
    });
}

function removerHistorico(titulo, dataEvento) {
    Swal.fire({
        title: "Remover Histórico?",
        text: `Deseja realmente remover o registro "${titulo}" de ${dataEvento}?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sim, remover!",
        cancelButtonText: "Cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Pacientes/RemoverHistorico',
                method: 'POST',
                data: { titulo: titulo, dataEvento: dataEvento },
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (response) {
                    if (response.resultado === "sucesso") {
                        mensagem.success(response.mensagem || "Registro de histórico removido com sucesso!", "", 10);
                        $("#historicoTable").html(response.partialHtml);
                    } else {
                        mensagem.error(response.mensagem || "Erro ao remover registro de histórico.", "", 10);
                    }
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error("Erro na comunicação com o servidor ao remover histórico.", "", 10);
                }
            });
        }
    });
}

// Função principal para gravar edição do paciente
function gravarEdicao() {
    var form = $("#formEditar");
    var formData = form.serialize();

    if (form[0].checkValidity()) {
        if (typeof showOverlay === 'function') showOverlay(".wrapper");

        $.ajax({
            url: "/Pacientes/Editar",
            method: "POST",
            data: formData,
            success: function (data) {
                if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                if (data.resultado === "sucesso") {
                    mensagem.success(data.mensagem || "Paciente editado com sucesso!", "", 10);
                    setTimeout(function () { location.href = "/Pacientes/Index"; }, 1500);
                } else {
                    mensagem.error(data.mensagem || "Erro ao editar paciente.", "", 10);
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