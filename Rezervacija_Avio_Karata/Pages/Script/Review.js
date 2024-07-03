function LoadCreatedReview(){
    $.get("/api/GetCreatedReview",function(data){
        if(data.length > 0){
            let table = '<table class="table table-striped table-hover table-bordered fs-5"><thead><tr><th scope="col">#</th><th scope="col">User</th><th scope="col">Airlline</th><th scope="col">Title</th><th scope="col">Image</th><th scope="col">Description</th><th scope="col">Approve</th><th scope="col">Reject</th></tr></thead><tbody>';

            let counter = 0;
            for(review in data){
                counter++;
                let row = '<td>' + counter.toString() + '</td>'; 
                row += '<td>' + data[review].Reviewer + '</td>';
                row += '<td>' + data[review].Airline + '</td>'; 
                row += '<td>' + data[review].Title + '</td>'; 
                row += '<td>' + data[review].Image + '</td>'; 
                row += '<td>' + data[review].Description + '</td>'; 
    
                row += '<td class="text-center">  <button onclick ="ChangeStatus(\'' + data[review].Id + '\', \'Approved\')" type="button" class="btn btn-success text-light"><i class="fas fa-check"></i></button></td>'; 
                row += '<td class="text-center">   <button onclick="ChangeStatus(\'' + data[review].Id + '\', \'Rejected\')" type="button" class="btn btn-danger text-light"><i class="fas fa-times"></i></button></td>'; 
    
                table += '<tr>' + row + '<tr/>';
            }
    
            table += '</tbody></table>';
            $('#reviewTable').html(table);
        }else{
            $('#reviewTable').html('<h1 class="text-light">No review available.</h1>');
        }

    })
}

function ChangeStatus(id,action){
    $.ajax({
        url: '/api/ChangeReviewStatus?id='+id+'&action='+action,
        type: 'POST',
        success: function() {
            LoadCreatedReview()
            $('#ReviewToast .toast-body').text('Review ' + action + ' successfully.');
            $('#ReviewToast').removeClass('text-bg-danger').addClass('text-bg-success');
            var toastEl = new bootstrap.Toast($('#ReviewToast'));
            toastEl.show();
        },
        error: function(xhr) {
            var errorMessage = xhr.responseJSON ? xhr.responseJSON.Message : "An error occurred";
            $('#ReviewToast .toast-body').text(errorMessage);
            $('#ReviewToast').removeClass('text-bg-success').addClass('text-bg-danger');
            var toastEl = new bootstrap.Toast($('#ReviewToast'));
            toastEl.show();
        }
    });
}