// === Incluir.js Corrigido e Funcionando ===

// Abrir modal para nova formação acadêmica
function novaFormacaoAcademica() {
    $.get("/ProfissionalSaude/TelaNovaFormacaoAcademica", function (html) {
        $("#modalNovaFormacaoAcademica .modal-body").html(html);
        $("#modalNovaFormacaoAcademica").modal("show");
    });
}

// Abrir modal para novo curso/certificação
function novoCursoCertificacao() {
    $.get("/ProfissionalSaude/TelaNovoCursoCertificacao", function (html) {
        $("#modalNovoCursoCertificacao .modal-body").html(html);
        $("#modalNovoCursoCertificacao").modal("show");
    });
}

// Salvar formação acadêmica
function salvarNovaFormacaoAcademica() {
    var form = $("#formIncluirFormacaoAcademica");
    var formData = form.serialize();

    $.post("/ProfissionalSaude/IncluirFormacaoAcademica", formData, function (response) {
        if (response.resultado === "sucesso") {
            $("#modalNovaFormacaoAcademica").modal("hide");
            $("#formacaoTable").html(response.partialHtml);
            mensagem.success(response.mensagem, "", 10);
        } else {
            mensagem.error(response.mensagem, "", 10);
        }
    }).fail(function () {
        mensagem.error("Erro ao salvar formação acadêmica.", "", 10);
    });
}

// Salvar curso/certificação
function salvarNovoCursoCertificacao() {
    var form = $("#formIncluirCursoCertificacao");
    var formData = form.serialize();

    $.post("/ProfissionalSaude/IncluirCursoCertificacao", formData, function (response) {
        if (response.resultado === "sucesso") {
            $("#modalNovoCursoCertificacao").modal("hide");
            $("#cursoTable").html(response.partialHtml);
            mensagem.success(response.mensagem, "", 10);
        } else {
            mensagem.error(response.mensagem, "", 10);
        }
    }).fail(function () {
        mensagem.error("Erro ao salvar curso/certificação.", "", 10);
    });
}

// Remover formação
function removerFormacaoAcademica(titulo, instituicao) {
    Swal.fire({
        title: "Remover Formação?",
        text: `Deseja remover a formação "${titulo}" da "${instituicao}"?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sim, remover!",
        cancelButtonText: "Cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/ProfissionalSaude/RemoverFormacaoAcademica", { titulo, instituicao }, function (response) {
                if (response.resultado === "sucesso") {
                    $("#formacaoTable").html(response.partialHtml);
                    mensagem.success(response.mensagem, "", 10);
                } else {
                    mensagem.error(response.mensagem, "", 10);
                }
            });
        }
    });
}

// Remover curso
function removerCursoCertificacao(titulo, duracaoHoras, instituicao) {
    Swal.fire({
        title: "Remover Curso?",
        text: `Deseja remover o curso "${titulo}" (${duracaoHoras}h) da "${instituicao}"?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sim, remover!",
        cancelButtonText: "Cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/ProfissionalSaude/RemoverCursoCertificacao", { titulo, duracaoHoras, instituicao }, function (response) {
                if (response.resultado === "sucesso") {
                    $("#cursoTable").html(response.partialHtml);
                    mensagem.success(response.mensagem, "", 10);
                } else {
                    mensagem.error(response.mensagem, "", 10);
                }
            });
        }
    });
}

// Gravar inclusão do profissional
function gravarInclusao() {
    var form = $("#formIncluir");
    var formData = form.serialize();

    if (form[0].checkValidity()) {
        $.post("/ProfissionalSaude/Incluir", formData, function (data) {
            if (data.resultado === "sucesso") {
                mensagem.success("Profissional incluído com sucesso!", "", 10);
                setTimeout(() => window.location.href = "/ProfissionalSaude/Index", 1500);
            } else {
                mensagem.error(data.mensagem, "", 10);
            }
        }).fail(function () {
            mensagem.error("Erro ao incluir profissional.", "", 10);
        });
    } else {
        form[0].reportValidity();
    }
}
