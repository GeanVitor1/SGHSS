// Funções para carregar e exibir os modais de inclusão de item
function novaFormacaoAcademica() {
    $.get("/ProfissionalSaude/TelaNovaFormacaoAcademica", function (data) {
        $("#modalNovaFormacaoAcademica .modal-body").html(data);
        $("#modalNovaFormacaoAcademica").modal("show");
    });
}

function novoCursoCertificacao() {
    $.get("/ProfissionalSaude/TelaNovoCursoCertificacao", function (data) {
        $("#modalNovoCursoCertificacao .modal-body").html(data);
        $("#modalNovoCursoCertificacao").modal("show");
    });
}

// Funções para salvar os dados adicionados nos modais
function salvarNovaFormacaoAcademica() {
    var form = $("#formIncluirFormacaoAcademica");
    var formData = form.serialize();

    $.post("/ProfissionalSaude/IncluirFormacaoAcademica", formData, function (response) {
        if (response && response.resultado && response.resultado.toLowerCase() === "sucesso") {
            $("#modalNovaFormacaoAcademica").modal("hide");

            // Verifica se o backend retornou o HTML completo e atualizado da tabela
            // Esta é a parte crucial para manter os dados antigos.
            if (response.partialHtml) {
                $("#formacaoTable").html(response.partialHtml); // Substitui o conteúdo da tabela pelo HTML COMPLETO
            } else {
                // Fallback: Se o backend não retornar o partialHtml,
                // faz uma nova requisição GET para obter a tabela completa.
                // Idealmente, o backend SEMPRE deve retornar partialHtml no POST de inclusão.
                $.get("/ProfissionalSaude/ObterFormacoesAcademicaProfissionalSaude", function (partialHtml) {
                    $("#formacaoTable").html(partialHtml);
                });
            }

            form[0].reset(); // Limpa o formulário do modal
            mensagem.success(response.mensagem || "Formação acadêmica adicionada com sucesso!", "", 10);

        } else {
            // Exibe mensagem de erro se a operação não for bem-sucedida
            mensagem.error(response && response.mensagem ? response.mensagem : "Erro desconhecido ao adicionar formação acadêmica.", "", 10);
        }
    }).fail(function (jqXHR) {
        // Captura erros de comunicação ou erros HTTP do servidor
        mensagem.error("Erro ao salvar formação acadêmica: " + jqXHR.responseText, "", 10);
    });
}

function salvarNovoCursoCertificacao() {
    var form = $("#formIncluirCursoCertificacao");
    var formData = form.serialize();

    $.post("/ProfissionalSaude/IncluirCursoCertificacao", formData, function (response) {
        if (response && response.resultado && response.resultado.toLowerCase() === "sucesso") {
            $("#modalNovoCursoCertificacao").modal("hide");
            // Verifica se o backend retornou o HTML da tabela
            if (response.partialHtml) {
                $("#cursoTable").html(response.partialHtml); // ATUALIZA A TABELA COM O HTML RECEBIDO NO JSON
            } else {
                // Caso o backend não retorne, mantém o $.get como fallback,
                // mas a ideia é que o backend sempre retorne o HTML.
                $.get("/ProfissionalSaude/ObterCursosCertificacoesProfissionalSaude", function (partialHtml) {
                    $("#cursoTable").html(partialHtml);
                });
            }
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
                url: '/ProfissionalSaude/RemoverFormacaoAcademica',
                method: 'POST',
                data: { titulo: titulo, instituicao: instituicao },
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (response) {
                    if (response.resultado === "sucesso") {
                        mensagem.success(response.mensagem || "Formação removida com sucesso!", "", 10);
                        $.get("/ProfissionalSaude/ObterFormacoesAcademicaProfissionalSaude", function (partialHtml) {
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
                url: '/ProfissionalSaude/RemoverCursoCertificacao',
                method: 'POST',
                data: { titulo: titulo, duracaoHoras: duracaoHoras, instituicao: instituicao },
                beforeSend: function () {
                    if (typeof showOverlay === 'function') showOverlay(".wrapper");
                },
                success: function (response) {
                    if (response.resultado === "sucesso") {
                        mensagem.success(response.mensagem || "Curso/Certificação removido(a) com sucesso!", "", 10);
                        $.get("/ProfissionalSaude/ObterCursosCertificacoesProfissionalSaude", function (partialHtml) {
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
            url: "/ProfissionalSaude/Incluir",
            method: "POST",
            data: formData,
            success: function (data) {
                if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                if (data.resultado === "sucesso") {
                    mensagem.success(data.mensagem || "Profissional de saúde incluído com sucesso!", "", 10);
                    setTimeout(function () { location.href = "/ProfissionalSaude/Index"; }, 1500);
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