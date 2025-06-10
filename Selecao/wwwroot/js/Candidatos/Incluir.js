$("#incluirCandidato").on('submit', function (e) {
    e.preventDefault();
    incluirCandidatos();
});

function incluirContatos() {
    $.ajax({
        url: '/Candidatos/TelaNovoContato',
        method: 'GET',
        dataType: 'html',
        beforeSend: function () {
            showOverlay(".wrapper");
        },
        success: function (data) {
            hideOverlay(".wrapper");
            customSwal.fire({
                title: "<strong>Adicionar Contato</strong>",
                icon: "question",
                iconHtml: "<i class=\"fas fa-phone\"></i>",
                width: '80vw',
                html: data,
                showCancelButton: true,
                showCloseButton: true,
                focusConfirm: false,
                preConfirm: () => {
                    return new Promise((resolve, reject) => {

                        dados = {
                            Contato: $("#Contato").val(),
                            Tipo: $("#tipo").val(),
                            isWhatsApp: $("#flexSwitchCheckDefault").val()
                        }

                        if (dados.Contato == "") {
                            mensagem.error("Erro ao adicionar contato", "O Contato não pode ser vazio", 10);
                            reject();
                            Swal.enableButtons();
                            return
                        }

                        if (dados.Tipo == "") {
                            mensagem.error("Erro ao adicionar contato", "É preciso informar o tipo do contato", 10);
                            reject();
                            Swal.enableButtons();
                            return
                        }

                        $.ajax({
                            url: '/Candidatos/IncluirContato',
                            method: 'POST',
                            data: dados,
                            beforeSend: function () {
                                showOverlay(".wrapper");
                            },
                            success: function (data) {
                                if (data.mensagem) {
                                    mensagem.error("Erro ao adicionar contato", data.mensagem, 10);
                                    resolve();
                                    return false;
                                }
                                $("#contatotable").empty().html(data);
                                resolve();
                            },
                            complete: function () {
                                hideOverlay(".wrapper");
                            },
                            error: function () {
                                mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
                                Swal.enableButtons();
                                reject();
                            }
                        });
                    });
                },
            });
        },
    });
}

function removerContato(contato, tipo) {
    $.ajax({
        url: '/Candidatos/RemoverContato',
        method: 'POST',
        dataType: 'html',
        data: { contato, tipo },
        beforeSend: function () {
            showOverlay();
        },
        success: function (data) {
            $("#contatotable").empty().html(data);
            mensagem.success("Contato removido com sucesso!", "", 10);
        },
        complete: function () {
            hideOverlay();
        },
        error: function () {
            mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
        }
    });
}


//Formação
function novaFormacao() {
    $.ajax({
        url: '/Candidatos/TelaNovaFormacao',
        method: 'GET',
        dataType: 'html',
        beforeSend: function () {
            showOverlay();
        },
        success: function (data) {
            customSwal.fire({
                title: "<strong>Adicionar Formação</strong>",
                icon: "question",
                iconHtml: "<i class=\"fas fa-user-graduate\"></i>",
                width: '80vw',
                html: data,
                showCancelButton: true,
                showCloseButton: true,
                focusConfirm: false,
                preConfirm: () => {
                    return new Promise((resolve, reject) => {
                        var form = document.getElementById("formIncluirFormacao");
                        if (form.checkValidity()) {
                            // Se o formulário é válido, realiza o AJAX
                            var dados = $("#formIncluirFormacao").serialize();
                            $.ajax({
                                url: '/Candidatos/IncluirFormacao',
                                method: 'POST',
                                data: dados,
                                beforeSend: function () {
                                    showOverlay(".wrapper");
                                },
                                success: function (data) {
                                    if (data.mensagem) {
                                        mensagem.error(
                                            "Erro ao incluir formação do candidato",
                                            data.mensagem,
                                            10
                                        );
                                        resolve();
                                        return false;
                                    }
                                    $("#formacaoTable").empty().html(data);
                                    resolve();
                                },
                                complete: function () {
                                    hideOverlay(".wrapper");
                                },
                                error: function () {
                                    mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
                                    Swal.enableButtons();
                                    reject();
                                },
                            });
                        } else {
                            form.reportValidity();
                            Swal.enableButtons();
                            reject();
                        }
                    });
                },
            });
        },
        complete: function () {
        },
    });
}

function removerFormacao(titulo, area) {
    $.ajax({
        url: '/Candidatos/RemoverFormacao',
        method: 'POST',
        data: { titulo, area },
        beforeSend: function () {
            showOverlay(".wrapper");
        },
        success: function (data) {
            mensagem.success("Formação removida com sucesso!", "", 10);
            $("#formacaoTable").empty().html(data);
        },
        complete: function () {
            hideOverlay(".wrapper");
        },
        error: function () {
            mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
        },
    });
}

//Cursos

function novoCurso() {
    $.ajax({
        url: '/Candidatos/TelaNovoCurso',
        method: 'GET',
        dataType: 'html',
        beforeSend: function () {
            showOverlay();
        },
        success: function (data) {
            customSwal.fire({
                title: "<strong>Adicionar Curso</strong>",
                icon: "question",
                iconHtml: "<i class=\"fas fa-award\"></i>",
                width: '80vw',
                html: data,
                showCancelButton: true,
                showCloseButton: true,
                focusConfirm: false,
                preConfirm: () => {
                    return new Promise((resolve, reject) => {
                        var dados = $("#formIncluirCurso").serialize();

                        var form = document.getElementById("formIncluirCurso");
                        if (form.checkValidity()) {
                            $.ajax({
                                url: '/Candidatos/IncluirCurso',
                                method: 'POST',
                                data: dados,
                                beforeSend: function () {
                                    showOverlay(".wrapper");
                                },
                                success: function (data) {
                                    if (data.mensagem) {
                                        mensagem.error("Erro ao incluir formação do candidato", data.mensagem, 10);
                                        resolve();
                                        return false;
                                    }

                                    $("#cursoTable").empty().html(data);
                                    resolve();
                                },
                                complete: function () {
                                    hideOverlay(".wrapper");
                                },
                                error: function () {
                                    mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
                                },
                            });

                        } else {
                            form.reportValidity();
                            Swal.enableButtons();
                            reject();
                        }
                    });
                },
            });
        }
    });
}

function removerCurso(titulo, duracaoHoras, instituicao) {
    $.ajax({
        url: '/Candidatos/RemoverCurso',
        method: 'POST',
        data: { titulo, duracaoHoras, instituicao },
        beforeSend: function () {
            showOverlay(".wrapper");
        },
        success: function (data) {
            mensagem.success("Curso removido com sucesso!", "", 10);
            $("#cursoTable").empty().html(data);
        },
        complete: function () {
            hideOverlay(".wrapper");
        },
        error: function () {
            mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
        },
    });
}

// Nova Experiencia
function novaExperiencia() {
    $.ajax({
        url: '/Candidatos/TelaNovaExperiencia',
        method: 'GET',
        dataType: 'html',
        beforeSend: function () {
            showOverlay();
        },
        success: function (data) {
            customSwal.fire({
                title: "<strong>Adicionar Experiência</strong>",
                icon: "question",
                iconHtml: "<i class=\"fas fa-user-clock\"></i>",
                width: '80vw',
                html: data,
                showCancelButton: true,
                showCloseButton: true,
                focusConfirm: false,
                preConfirm: () => {
                    return new Promise((resolve, reject) => {
                        var dados = $("#formIncluirExperiencia").serialize();

                        var form = document.getElementById("formIncluirExperiencia");
                        if (form.checkValidity()) {
                            $.ajax({
                                url: '/Candidatos/IncluirExperiencia',
                                method: 'POST',
                                data: dados,
                                beforeSend: function () {
                                    showOverlay(".wrapper");
                                },
                                success: function (data) {
                                    if (data.mensagem) {
                                        mensagem.error("Erro ao incluir formação do candidato", data.mensagem, 10);
                                        resolve();
                                        return false;
                                    }

                                    $("#experienciaTable").empty().html(data);
                                    resolve();
                                },
                                complete: function () {
                                    hideOverlay(".wrapper");
                                },
                                error: function () {
                                    mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
                                },
                            });

                        } else {
                            form.reportValidity();
                            Swal.enableButtons();
                            reject();
                        }
                    });
                },
            });
        }
    });
}

function calcularDuracao() {
    var inicio = $('#Inicio').val();
    var termino = $('#Termino').val();

    if (inicio && termino) {
        var dataInicio = new Date(inicio);
        var dataTermino = new Date(termino);

        var diff = dataTermino - dataInicio;

        if (diff < 0) {
            mensagem.error("Data de término não pode ser antes da data de início", 10);
            return;
        }

        var anos = Math.floor(diff / (365.25 * 24 * 60 * 60 * 1000));
        var meses = Math.floor((diff % (365.25 * 24 * 60 * 60 * 1000)) / (30 * 24 * 60 * 60 * 1000));

        var compAnos = "";
        var compMeses = "";
        if (anos != 1) {
            compAnos = "s"
        }
        if (meses != 1) {
            compMeses = "es"
        }

        $('#Duracao').val(anos + ' ano' + compAnos + ' e ' + meses + ' mes' + compMeses);
    }
    else {
        $('#Duracao').val('');
    }
}

function removerExperiencia(empregador, inicio, cargo) {
    $.ajax({
        url: '/Candidatos/RemoverExperiencia',
        method: 'POST',
        data: { empregador, inicio, cargo },
        beforeSend: function () {
            showOverlay(".wrapper");
        },
        success: function (data) {
            mensagem.success("Experiência removido com sucesso!", "", 10);
            $("#experienciaTable").empty().html(data);
        },
        complete: function () {
            hideOverlay(".wrapper");
        },
        error: function () {
            mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
        },
    });
}



//Gravar Conato
function gravarInclusao() {
    var dados = $("#formIncluir").serialize();

    var form = document.getElementById("formIncluir");
    if (form.checkValidity()) {
        $.ajax({
            url: '/Candidatos/Incluir',
            method: 'POST',
            data: dados,
            beforeSend: function () {
                showOverlay(".wrapper");
            },
            success: function (data) {
                if (data.resultado == "falha") {
                    mensagem.error("Erro ao incluir candidato", data.mensagem, 10);
                    console.log(data.mensagem);
                    return false;
                }

                window.location.href = "/Candidatos";
            },
            complete: function () {
                hideOverlay(".wrapper");
            },
            error: function () {
                mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
            }
        });
    } else {
        form.reportValidity();
    }
}

function gravarEdicao() {
    var dados = $("#formEditar").serializeArray();

    //dados = dados.filter(field => !field.name.includes("_Invariant"));

    var form = document.getElementById("formEditar");

    if (form.checkValidity()) {
        $.ajax({
            url: '/Candidatos/Editar',
            method: 'POST',
            data: $.param(dados),
            beforeSend: function () {
                showOverlay(".wrapper");
            },
            success: function (data) {
                if (data.resultado == "falha") {
                    mensagem.error("Erro ao incluir candidato", data.mensagem, 10);
                    console.log(data.mensagem);
                    return false;
                }

                window.location.href = "/Candidatos";
            },
            complete: function () {
                hideOverlay(".wrapper");
            },
            error: function () {
                mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
            }
        });
    } else {
        form.reportValidity();
    }
}