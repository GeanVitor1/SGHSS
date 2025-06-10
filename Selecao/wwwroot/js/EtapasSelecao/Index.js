$("#filtrosEtapas").on("submit", function (e) {
    e.preventDefault();
    filtrarCandidatos();
});

function filtrarEtapas() {
    var dados = $("#filtrosEtapas").serialize();
    $.ajax({
        url: '/EtapasSelecao/BuscarEtapas',
        method: 'GET',
        data: dados,
        beforeSend: function () {
            showOverlay("#tbEtapas");
        },
        success: function (data) {
            $("#tableEtapas").empty().html(data);
        },
        complete: function () {
            hideOverlay("#tbEtapas");
        },
        error: function () {
            mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
        }
    });
}

function incluir() {
    $.ajax({
        url: '/EtapasSelecao/Incluir',
        method: 'GET',
        dataType: 'html',
        beforeSend: function () {
            showOverlay(".wrapper");
        },
        success: function (data) {
            hideOverlay(".wrapper");
            customSwal.fire({
                title: "<strong>Adicionar Etapa</strong>",
                icon: "question",
                iconHtml: "<i class=\"fas fa-tasks\"></i>",
                width: '80vw',
                html: data,
                showCancelButton: true,
                showCloseButton: true,
                focusConfirm: false,
                preConfirm: () => {
                    return new Promise((resolve, reject) => {

                        var dados = $("#formIncluir").serialize();
                        var form = document.getElementById("formIncluir");

                        if (form.checkValidity()) {
                            $.ajax({
                                url: '/EtapasSelecao/Incluir',
                                method: 'POST',
                                data: dados,
                                beforeSend: function () {
                                    showOverlay(".wrapper");
                                },
                                success: function (data) {
                                    if (data.resultado == "falha") {
                                        mensagem.error("Erro ao incluir candidato", data.mensagem, 10);
                                        resolve();
                                        return false;
                                    }
                                    mensagem.success("Etapa Incluída com Sucesso", "", 10);
                                    filtrarEtapas();
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
                        } else {
                            form.reportValidity();
                        }
                    });
                }
            });
        },
    });
}

$("#formIncluir").on('submit', function (e) {
    e.preventDefault();
});

function editar(id) {
    $.ajax({
        url: '/EtapasSelecao/Editar',
        method: 'GET',
        dataType: 'html',
        data: { id },
        beforeSend: function () {
            showOverlay(".wrapper");
        },
        success: function (data) {
            hideOverlay(".wrapper");
            customSwal.fire({
                title: "<strong>Adicionar Etapa</strong>",
                icon: "question",
                iconHtml: "<i class=\"fas fa-tasks\"></i>",
                width: '80vw',
                html: data,
                showCancelButton: true,
                showCloseButton: true,
                focusConfirm: false,
                preConfirm: () => {
                    return new Promise((resolve, reject) => {

                        var dados = $("#formIncluir").serialize();
                        var form = document.getElementById("formIncluir");

                        if (form.checkValidity()) {
                            $.ajax({
                                url: '/EtapasSelecao/Editar',
                                method: 'PUT',
                                data: dados,
                                beforeSend: function () {
                                    showOverlay(".wrapper");
                                },
                                success: function (data) {
                                    if (data.resultado == "falha") {
                                        mensagem.error("Erro ao editar etapa", data.mensagem, 10);
                                        resolve();
                                        return false;
                                    }
                                    mensagem.success("Etapa Editada com Sucesso", "", 10);
                                    filtrarEtapas();
                                    resolve();
                                },
                                complete: function () {
                                    hideOverlay(".wrapper");
                                },
                                error: function () {
                                    mensagem.error("Erro ao editar etapa", "Ocorreu um erro desconhecido", 10);
                                    Swal.enableButtons();
                                    reject();

                                }
                            });
                        } else {
                            form.reportValidity();
                        }
                    });
                }
            });
        },
    });
}

function alterarStatus(id, nome, acao) {
    let acaoConcluido = "";
    if (acao == "Excluir") {
        acaoConcluido = "Excluido";
    }
    if (acao == "Reativar") {
        acaoConcluido = "Reativado"
    }
    customSwal.fire({
        title: `<strong>${acao} Etapa</strong>`,
        html: `<h5>Deseja ${acao} a etapa<br>${nome}</h5>`,
        icon: "question",
        showCancelButton: true,
        showCloseButton: true,
        focusConfirm: false,
        preConfirm: () => {
            return new Promise((resolve, reject) => {
                $.ajax({
                    url: `/EtapasSelecao/AlterarStatus`,
                    method: 'PUT',
                    data: { id },
                    beforeSend: function () {
                        showOverlay("#tbEtapas");
                    },
                    success: function (data) {
                        if (data.mensagem) {
                            mensagem.error(`Erro ao ${acao} etapa`, data.mensagem, 10);
                            resolve();
                            return false;
                        }
                        mensagem.success(`Etapa ${acaoConcluido} com sucesso!`, "", 10);
                        filtrarEtapas();
                        resolve();
                    },
                    complete: function () {
                        showOverlay("#tbEtapas");
                    },
                    error: function () {
                        mensagem.error(`Erro ao ${acao} etapa`, "Ocorreu um erro desconhecido", 10);
                        Swal.enableButtons();
                        reject();
                    }
                });
            });
        },
    });
}