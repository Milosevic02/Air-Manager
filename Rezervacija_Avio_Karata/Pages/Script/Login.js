function Login(){
    let form = $("#loginForm")
    let data = convertFormToJSON(form)

    $.ajax({
        url:"/api/LoginUser",
        type:"GET",
        data:{username:data.username,password:data.password},
        contentType:"text/plain",
        complete: function(result){
            if(result.status == 400){
                console.log("Error when log in")
            }else if(result == 200){
                console.log("Success Log in")
                window.location.href = "Index.html"
            }else{
                console.log("Unknown error when log in")
            }
        }
    })

}



function convertFormToJSON(form) {
    const array = $(form).serializeArray(); // Encodes the set of form elements as an array of names and values.
    const json = {};
    $.each(array, function () {
        json[this.name] = this.value || "";
    });
    return json;
}