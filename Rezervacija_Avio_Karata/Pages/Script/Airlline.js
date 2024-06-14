function LoadAirllines(){
    $.get("/api/GetAllAirllines",function(data){
        let table = '<table class="table table-striped table-hover table-bordered fs-5">';
        table += '<table class="table table-striped table-hover table-bordered fs-5"><thead><tr><th scope="col">#</th><th scope="col">Name</th><th scope="col">Address</th><th scope="col">Contact Info</th><th scope="col">Edit</th><th scope="col">Delete</th></tr></thead><tbody>';

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
    })
}