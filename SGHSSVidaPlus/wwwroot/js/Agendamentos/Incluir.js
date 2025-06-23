function gravarInclusao() {
    var form = $("#formIncluir");
    var formData = form.serialize(); // Serializa o formulário principal

    if (form[0].checkValidity()) {
        if (typeof showOverlay === 'function') showOverlay(".wrapper");

        $.ajax({
            url: "/Agendamentos/Incluir", // URL do controlador
            method: "POST",
            data: formData,
            success: function (data) {
                if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
                if (data.resultado === "sucesso") {
                    mensagem.success(data.mensagem || "Agendamento incluído com sucesso!", "", 10);
                    setTimeout(function () { location.href = "/Agendamentos/Index"; }, 1500); // Redireciona para a Index
                } else {
                    mensagem.error(data.mensagem || "Erro ao incluir agendamento.", "", 10);
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