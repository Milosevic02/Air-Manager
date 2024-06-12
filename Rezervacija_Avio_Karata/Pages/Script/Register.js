function Register(role=""){
    let name = $("#name")
    let surname = $("#surname")
    let username = $("#username")
    let password = $("#password")
    let email = $("#email")
    let birthDate = $("#dateOfBirth")

    let fields = [name,surname,username,password,email,birthDate,gender]

    let form = $("#registerForm")
    let data = convertFormToJSON(form)
    data = JSON.stringify(data)
    console.log(data)

}


function convertFormToJSON(form) {
    const array = $(form).serializeArray(); // Encodes the set of form elements as an array of names and values.
    const json = {};
    $.each(array, function () {
        json[this.name] = this.value || "";
    });
    return json;
}