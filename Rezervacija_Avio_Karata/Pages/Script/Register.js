
function Register(event) {
    event.preventDefault();
    let form = $("#registerForm")
    let data = convertFormToJSON(form)
    data = JSON.stringify(data)

    $.ajax({
        url: "/api/RegisterUser",
        type: "POST",
        data: data,
        contentType: "application/json",
        success: function (result) {
            // Show success toast
            window.location.href = "Login.html"
            
        },
        error: function(xhr) {
            var errorMessage = xhr.responseJSON ? xhr.responseJSON.Message : "An error occurred";
            $('#registerToast .toast-body').text(errorMessage);
            $('#registerToast').removeClass('text-bg-success').addClass('text-bg-danger');
            var toastEl = new bootstrap.Toast($('#registerToast'));
            toastEl.show();
        }
    });


}

function convertFormToJSON(form) {
    const array = $(form).serializeArray(); // Encodes the set of form elements as an array of names and values.
    const json = {};
    $.each(array, function () {
        json[this.name] = this.value || "";
    });
    return json;
}