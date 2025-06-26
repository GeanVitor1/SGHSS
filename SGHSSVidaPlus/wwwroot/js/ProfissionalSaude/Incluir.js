// Fun��es para carregar e exibir os modais de inclus�o de item
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

// Fun��es para salvar os dados adicionados nos modais
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
            mensagem.success(response.mensagem || "Forma��o acad�mica adicionada com sucesso!", "", 10);
        } else {
            mensagem.error(response && response.mensagem ? response.mensagem : "Erro desconhecido ao adicionar forma��o acad�mica.", "", 10);
        }
    }).fail(function (jqXHR) {
        mensagem.error("Erro ao salvar forma��o acad�mica: " + jqXHR.responseText, "", 10);
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
            mensagem.success(response.mensagem || "Curso/Certifica��o adicionado(a) com sucesso!", "", 10);
        } else {
            mensagem.error(response && response.mensagem ? response.mensagem : "Erro desconhecido ao adicionar curso/certifica��o.", "", 10);
        }
    }).fail(function (jqXHR) {
        mensagem.error("Erro ao salvar curso/certifica��o: " + jqXHR.responseText, "", 10);
    });
}

// Fun��es para remover itens das listas (chamadas de dentro das partials)
function removerFormacaoAcademica(titulo, instituicao) {
    Swal.fire({
        title: "Remover Forma��o?",
        text: `Deseja remover a forma��o "${titulo}" da "${instituicao}"?`,
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
                        mensagem.success(response.mensagem || "Forma��o removida com sucesso!", "", 10);
                        $.get("/ProfissionaisSaude/ObterFormacoesAcademicaProfissionalSaude", function (partialHtml) {
                            $("#formacaoTable").html(partialHtml);
                        });
                    } else {
                        mensagem.error(response.mensagem || "Erro ao remover forma��o.", "", 10);
                    }
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error("Erro na comunica��o ao remover forma��o.", "", 10);
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
                        mensagem.success(response.mensagem || "Curso/Certifica��o removido(a) com sucesso!", "", 10);
                        $.get("/ProfissionaisSaude/ObterCursosCertificacoesProfissionalSaude", function (partialHtml) {
                            $("#cursoTable").html(partialHtml);
                        });
                    } else {
                        mensagem.error(response.mensagem || "Erro ao remover curso/certifica��o.", "", 10);
                    }
                },
                complete: function () {
                    if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                },
                error: function () {
                    mensagem.error("Erro na comunica��o ao remover curso/certifica��o.", "", 10);
                }
            });
        }
    });
}

// Fun��o principal para gravar inclus�o do profissional
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
                    mensagem.success(data.mensagem || "Profissional de sa�de inclu�do com sucesso!", "", 10);
                    setTimeout(function () { location.href = "/ProfissionaisSaude/Index"; }, 1500);
                } else {
                    mensagem.error(data.mensagem || "Erro ao incluir profissional de sa�de.", "", 10);
                }
            },
            error: function () {
                if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                mensagem.error("Erro na comunica��o com o servidor.", "", 10);
            }
        });
    } else {
        form[0].reportValidity();
    }
}