
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
            console.log("success");
            // Show success toast
            window.location.href = "Login.html"
            var successToast = new bootstrap.Toast(document.getElementById('successToast'));
            successToast.show();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log("error");
            // Show error toast
            var errorToast = new bootstrap.Toast(document.getElementById('errorToast'));
            errorToast.show();
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