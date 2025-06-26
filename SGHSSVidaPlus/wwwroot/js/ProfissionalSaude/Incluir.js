// Funções para carregar e exibir os modais de inclusão de item
function novaFormacaoAcademica() {
    $.get("/ProfissionaisSaude/TelaNovaFormacaoAcademica", function (data) {
        $("#modalNovaFormacaoAcademica .modal-body").html(data);
        $("#modalNovaFormacaoAcademica").modal("show");
    });
}

function novoCursoCertificacao() {
    $.get("/ProfissionaisSaude/TelaNovoCursoCertificacao", function (data) {
        $("#modalNovoCursoCertificacao .modal-body").html(data);
        $("#modalNovoCursoCertificacao").modal("show");
    });
}

// Funções para salvar os dados adicionados nos modais
function salvarNovaFormacaoAcademica() {
    var form = $("#formIncluirFormacaoAcademica");
    var formData = form.serialize();

    $.post("/ProfissionaisSaude/IncluirFormacaoAcademica", formData, function (response) {
        if (response && response.resultado && response.resultado.toLowerCase() === "sucesso") {
            $("#modalNovaFormacaoAcademica").modal("hide");
            $.get("/ProfissionaisSaude/ObterFormacoesAcademicaProfissionalSaude", function (partialHtml) {
                $("#formacaoTable").html(partialHtml);
            });
            form[0].reset();
            mensagem.success(response.mensagem || "Formação acadêmica adicionada com sucesso!", "", 10);
        } else {
            mensagem.error(response && response.mensagem ? response.mensagem : "Erro desconhecido ao adicionar formação acadêmica.", "", 10);
        }
    }).fail(function (jqXHR) {
        mensagem.error("Erro ao salvar formação acadêmica: " + jqXHR.responseText, "", 10);
    });
}

function salvarNovoCursoCertificacao() {
    var form = $("#formIncluirCursoCertificacao");
    var formData = form.serialize();

    $.post("/ProfissionaisSaude/IncluirCursoCertificacao", formData, function (response) {
        if (response && response.resultado && response.resultado.toLowerCase() === "sucesso") {
            $("#modalNovoCursoCertificacao").modal("hide");
            $.get("/ProfissionaisSaude/ObterCursosCertificacoesProfissionalSaude", function (partialHtml) {
                $("#cursoTable").html(partialHtml);
            });
            form[0].reset();
            mensagem.success(response.mensagem || "Curso/Certificação adicionado(a) com sucesso!", "", 10);
        } else {
            mensagem.error(response && response.mensagem ? response.mensagem : "Erro desconhecido ao adicionar curso/certificação.", "", 10);
        }
    }).fail(function (jqXHR) {
        mensagem.error("Erro ao salvar curso/certificação: " + jqXHR.responseText, "", 10);
    });
}

// Funções para remover itens das listas (chamadas de dentro das partials)
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
            $.ajax({
                url: '/ProfissionaisSaude/RemoverFormacaoAcademica',
                method: 'POST',
                data: { titulo: titulo, instituicao: instituicao },
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (response) {
                    if (response.resultado === "sucesso") {
                        mensagem.success(response.mensagem || "Formação removida com sucesso!", "", 10);
                        $.get("/ProfissionaisSaude/ObterFormacoesAcademicaProfissionalSaude", function (partialHtml) {
                            $("#formacaoTable").html(partialHtml);
                        });
                    } else {
                        mensagem.error(response.mensagem || "Erro ao remover formação.", "", 10);
                    }
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error("Erro na comunicação ao remover formação.", "", 10);
                }
            });
        }
    });
}

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
            $.ajax({
                url: '/ProfissionaisSaude/RemoverCursoCertificacao',
                method: 'POST',
                data: { titulo: titulo, duracaoHoras: duracaoHoras, instituicao: instituicao },
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (response) {
                    if (response.resultado === "sucesso") {
                        mensagem.success(response.mensagem || "Curso/Certificação removido(a) com sucesso!", "", 10);
                        $.get("/ProfissionaisSaude/ObterCursosCertificacoesProfissionalSaude", function (partialHtml) {
                            $("#cursoTable").html(partialHtml);
                        });
                    } else {
                        mensagem.error(response.mensagem || "Erro ao remover curso/certificação.", "", 10);
                    }
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error("Erro na comunicação ao remover curso/certificação.", "", 10);
                }
            });
        }
    });
}

// Função principal para gravar inclusão do profissional
function gravarInclusao() {
    var form = $("#formIncluir");
    var formData = form.serialize();

    if (form[0].checkValidity()) {
        if (typeof showOverlay === 'function') showOverlay(".wrapper");

        $.ajax({
            url: "/ProfissionaisSaude/Incluir",
            method: "POST",
            data: formData,
            success: function (data) {
                if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                if (data.resultado === "sucesso") {
                    mensagem.success(data.mensagem || "Profissional de saúde incluído com sucesso!", "", 10);
                    setTimeout(function () { location.href = "/ProfissionaisSaude/Index"; }, 1500);
                } else {
                    mensagem.error(data.mensagem || "Erro ao incluir profissional de saúde.", "", 10);
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