function LoadAirllines(role){
    $.get("/api/GetAllAirllines",function(data){

        if(role != "Admin"){
            AirllineCards(data);
        }else{
            AdminTable(data);
        }
    })
}

function AirllineCards(data){
    let card = '<div class="row">';
    for(airlline in data){
        card += '<div class="card p-3 sec-color text-light" style="width: 22rem;margin-bottom:50px !important;margin-left:80px !important"><div class="card-body">';
        card += '<h5 class="card-title">'+ data[airlline].Name + '</h5><div class="form-group"><label for="address" class="sec-color text-light">Address:</label>';
        card += '<span id="address" class="form-control sec-color text-light" readonly>' + data[airlline].Address + '</span></div>';
        card += '<div class="form-group"><label for="contact" class="sec-color text-light">Contact Info:</label>'
        card += '<span id="contact" class="form-control sec-color text-light" readonly>' + data[airlline].ContactInfo + '</span></div></div><div class="card-body view-btn">';
        card += '<button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#airllineModal"><i class="fas fa-eye"></i> View Airline</button></div></div>'
    }
    card += '</div>';
    $("#airllineCard").html(card);
}

function AdminTable(data){
    let table = '<table class="table table-striped table-hover table-bordered fs-5"><thead><tr><th scope="col">#</th><th scope="col">Name</th><th scope="col">Address</th><th scope="col">Contact Info</th><th scope="col">Edit</th><th scope="col">Delete</th></tr></thead><tbody>';

    let counter = 0;
    for(airlline in data){
        counter ++;
        let row = '<td>' + counter.toString() + '</td>'; 
        row += '<td>' + data[airlline].Name + '</td>';
        row += '<td>' + data[airlline].Address + '</td>'; 
        row += '<td>' + data[airlline].ContactInfo + '</td>'; 
        row += '<td class="text-center">  <button type="button" class="btn btn-warning text-dark" data-bs-toggle="modal" data-bs-target="#editAirllineModal"><i class="fas fa-pen"></i> Edit</button></td>'; 
        row += '<td class="text-center">   <button type="button" class="btn btn-danger text-light" data-bs-toggle="modal" data-bs-target="#deleteModal"><i class="fas fa-trash"></i> Delete</button></td>'; 

        table += '<tr>' + row + '<tr/>';
    }

    table += '</tbody></table>';
    $('#airllineTable').html(table);
}

function AddAirlline(event){
    event.preventDefault(); 

    let form = $("#addAirlineForm");
    let data = convertFormToJSON(form);
    
    $.ajax({
        url: "/api/AddAirlline",
        type: "POST", 
        data: JSON.stringify(data),
        contentType: "application/json", 
        success: function (result) {
            console.log("success", result);
            window.location.href = "AirllineAdmin.html";
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.log("error", xhr.responseText);
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