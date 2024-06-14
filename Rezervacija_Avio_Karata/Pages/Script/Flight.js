function LoadFlights(){
    $.get("/api/GetAllFlights",function(data){
        let table = '<table class="table table-striped table-hover table-bordered fs-5"><thead><tr><th scope="col">#</th><th scope="col">Airlline</th><th scope="col">Departure Destination</th><th scope="col">Arrival Destination</th><th scope="col">Departure Date</th><th scope="col">Arrival Date</th><th scope="col">Available Seats</th><th scope="col">Occupied Seats</th><th scope="col">Price</th><th scope="col">Status</th><th scope="col">Edit</th><th scope="col">Delete</th></tr></thead><tbody>';

        let counter = 0;
        for(flight in data){
            counter++;
            let row = '<td>' + counter.toString() + '</td>'; 
            row += '<td>' + data[flight].Airline + '</td>';
            row += '<td>' + data[flight].DepartureDestination + '</td>'; 
            row += '<td>' + data[flight].ArrivalDestination + '</td>'; 
            row += '<td>' + data[flight].DepartureDateAndTime + '</td>'; 
            row += '<td>' + data[flight].ArrivalDateAndTime + '</td>'; 
            row += '<td>' + data[flight].AvailableSeats + '</td>'; 
            row += '<td>' + data[flight].OccupiedSeats + '</td>'; 
            row += '<td>' + data[flight].Price + '</td>'; 
            row += '<td>' + data[flight].Status + '</td>'; 

            row += '<td class="text-center">  <button type="button" class="btn btn-warning text-dark" data-bs-toggle="modal" data-bs-target="#editModal"><i class="fas fa-pen"></i> Edit</button></td>'; 
            row += '<td class="text-center">   <button type="button" class="btn btn-danger text-light" data-bs-toggle="modal" data-bs-target="#deleteModal"><i class="fas fa-trash"></i> Delete</button></td>'; 

            table += '<tr>' + row + '<tr/>';
        }

        table += '</tbody></table>';
        $('#flightTable').html(table);
    })
}

function AddFlight(event){
    event.preventDefault(); 

    let form = $("#addFlightForm");
    let data = convertFormToJSON(form);
    
    $.ajax({
        url: "/api/addFlight",
        type: "POST", 
        data: JSON.stringify(data),
        contentType: "application/json", 
        success: function (result) {
            console.log("success", result);
            window.location.href = "Admin.html";
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

