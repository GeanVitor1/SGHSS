// wwwroot/js/Admin/Incluir.js

function gravarInclusao() {
    var form = $("#formIncluir");

    // Aciona a validação do jQuery Unobtrusive Validation
    // Se o form.valid() retornar false, os erros são exibidos e a função para.
    if (!form.valid()) {
        // Reporta a validade do formulário para exibir erros HTML5 nativos também
        form[0].reportValidity();
        return;
    }

    // Coleta todos os dados do formulário
    // serializeArray() é bom para dados simples. Para checkboxes e arrays complexos,
    // o processamento manual ou usar FormData pode ser mais robusto.
    // No seu caso, você já tem lógica para Permissoes e Admin, vamos focar nos campos de senha.
    var formData = form.serializeArray().reduce(function (obj, item) {
        // Esta parte pode sobrescrever seus tratamentos específicos para Admin/Permissoes
        // Se houver inputs com o mesmo 'name' mas de tipos diferentes, pode ser problemático.
        // É mais seguro pegar Admin e Permissoes separadamente, como você já faz.
        obj[item.name] = item.value;
        return obj;
    }, {});

    // Captura o estado do checkbox 'Admin' (se serializeArray não pegar corretamente)
    formData.Admin = $("#administradorCheck").is(":checked");

    // Pega os valores das senhas pelos IDs únicos
    formData.Password = $("#password").val();
    formData.ConfirmPassword = $("#confirmPassword").val();

    // Coleta as permissões (Claims) - esta lógica está boa
    var permissoes = [];
    form.find('input[name^="Permissoes"]').each(function () {
        var inputName = $(this).attr('name');
        var nameParts = inputName.match(/Permissoes\[(\d+)\]\.(.+)/);
        if (nameParts) {
            var idx = parseInt(nameParts[1]);
            var prop = nameParts[2];
            if (!permissoes[idx]) {
                permissoes[idx] = {};
            }
            if (prop === "IsSelected") {
                permissoes[idx][prop] = $(this).is(":checked");
            } else {
                permissoes[idx][prop] = $(this).val();
            }
        }
    });
    // Filtra para remover entradas nulas ou incompletas, se houver
    formData.Permissoes = permissoes.filter(p => p && p.Tipo && p.Valor);


    // O Ajax agora envia o formData
    $.ajax({
        url: '/Admin/Incluir',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        beforeSend: function () {
            // Se você tem a função showOverlay no site.js ou similar
            if (typeof showOverlay === 'function') showOverlay(".wrapper");
        },
        success: function (data) {
            // Se você tem a função hideOverlay no site.js ou similar
            if (typeof hideOverlay === 'function') hideOverlay(".wrapper");

            // Usando SweetAlert2 para as mensagens
            if (data.resultado === "sucesso") {
                Swal.fire("Sucesso!", data.mensagem || "Usuário incluído com sucesso!", "success")
                    .then(() => {
                        window.location.href = "/Admin"; // Redirecionamento
                    });
            } else {
                // Se o resultado for falha, a mensagem do servidor já vem detalhada
                Swal.fire("Erro!", data.mensagem || "Ocorreu um erro desconhecido ao tentar incluir o usuário.", "error");
            }
        },
        complete: function () {
            // Se você tem a função hideOverlay no site.js ou similar
            if (typeof hideOverlay === 'function') hideOverlay(".wrapper");
        },
        error: function (xhr, status, error) {
            // Se você tem a função hideOverlay no site.js ou similar
            if (typeof hideOverlay === 'function') hideOverlay(".wrapper");

            // Tenta pegar a mensagem de erro do servidor, se disponível
            let errorMessage = "Ocorreu um erro desconhecido na comunicação com o servidor.";
            if (xhr.responseJSON && xhr.responseJSON.mensagem) {
                errorMessage = xhr.responseJSON.mensagem;
            } else if (xhr.responseText) {
                errorMessage = xhr.responseText.substring(0, 200); // Limita o tamanho para não ser muito longo
            }
            Swal.fire("Erro!", errorMessage, "error");
            console.error("Erro AJAX ao incluir usuário: ", status, error, xhr.responseText);
        }
    });
}