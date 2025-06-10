$("#filtrosCandidatos").on("submit", function (e) {
    e.preventDefault();
    filtrarCandidatos();
});

function filtrarCandidatos() {
    var dados = $("#filtrosCandidatos").serialize();
    $.ajax({
        url: '/Candidatos/BuscarCandidatos',
        method: 'GET',
        data: dados,
        beforeSend: function () {
            showOverlay("#tbCandidatos");
        },
        success: function (data) {
            $("#tableCandidatos").empty().html(data);
        },
        complete: function () {
            hideOverlay("#tbCandidatos");
        },
        error: function () {
            mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
        }
    });
}

function verCurriculo(id) {
    $.ajax({
        url: '/Candidatos/BuscarCurriculo',
        method: 'GET',
        data: id,
        beforeSend: function () {
            showOverlay(".wrapper");
        },
        success: function (data) {
            $("#dadoCurriculo").empty().html(data);
            $("#modalCurriculo").modal("show");
        },
        complete: function () {
            hideOverlay(".wrapper");
        },
        error: function () {
            mensagem.error("Erro ao incluir contato", "Ocorreu um erro desconhecido", 10);
        }
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
        title: `<strong>${acao} Candidato</strong>`,
        html: `<h5>Deseja ${acao} o candidato<br>${nome}</h5>`,
        icon: "question",
        showCancelButton: true,
        showCloseButton: true,
        focusConfirm: false,
        preConfirm: () => {
            return new Promise((resolve, reject) => {
                $.ajax({
                    url: '/Candidatos/AlterarStatus',
                    method: 'POST',
                    data: { id },
                    beforeSend: function () {
                        showOverlay(".wrapper");
                    },
                    success: function (data) {
                        if (data.mensagem) {
                            mensagem.error(`Erro ao ${acao} candidato`, data.mensagem, 10);
                            resolve();
                            return false;
                        }
                        mensagem.success(`Candidato ${acaoConcluido} com Sucesso`,"", 10)
                        filtrarCandidatos();
                        resolve();
                    },
                    complete: function () {
                        hideOverlay(".wrapper");
                    },
                    error: function () {
                        mensagem.error(`Erro ao ${acao} candidato`, "Ocorreu um erro desconhecido", 10);
                        Swal.enableButtons();
                        reject();
                    }
                });
            });
        },
    });
}