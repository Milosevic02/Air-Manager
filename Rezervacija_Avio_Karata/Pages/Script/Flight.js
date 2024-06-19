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
            row += '<td class="text-center">' + data[flight].AvailableSeats + '</td>'; 
            row += '<td class="text-center">' + data[flight].OccupiedSeats + '</td>'; 
            row += '<td>' + data[flight].Price + '</td>'; 
            let status = GetStatus(data[flight].FlightStatus);
            row += '<td>' + status + '</td>'; 

            row += '<td class="text-center">  <button onclick ="GetFlightInfo('+ data[flight].Id + ')" type="button" class="btn btn-warning text-dark edit-flight-btn" data-bs-toggle="modal" data-bs-target="#editModal"><i class="fas fa-pen"></i> Edit</button></td>'; 
            row += '<td class="text-center">   <button onclick="AddIdToDeleteModal(' + data[flight].Id + ')" type="button" class="btn btn-danger text-light" data-bs-toggle="modal" data-bs-target="#deleteModal"><i class="fas fa-trash"></i> Delete</button></td>'; 

            table += '<tr>' + row + '<tr/>';
        }

        table += '</tbody></table>';
        $('#flightTable').html(table);
    })
}

function AddIdToDeleteModal(flightId){
    $('#deleteFlightId').val(flightId)
}

function DeleteFlight() {
    let id = $('#deleteFlightId').val();
    $.ajax({
        url: '/api/DeleteFlight?id=' + id,
        type: 'DELETE',
        success: function () {
            LoadFlights();
            $('#deleteModal').modal('hide');
            $('#FlightsToast .toast-body').text('Flight deleted successfully.');
            $('#FlightsToast').removeClass('text-bg-danger').addClass('text-bg-success');
            var toastEl = new bootstrap.Toast($('#FlightsToast'));
            toastEl.show();
        },
        error: function (xhr) {
            $('#deleteModal').modal('hide');
            var errorMessage = xhr.responseJSON ? xhr.responseJSON.Message : "An error occurred";
            $('#FlightsToast .toast-body').text(errorMessage);
            $('#FlightsToast').removeClass('text-bg-success').addClass('text-bg-danger');
            var toastEl = new bootstrap.Toast($('#FlightsToast'));
            toastEl.show();
        }
    });
}

function GetFlightInfo(flightId){
    $.get('/api/GetFlightDetails?id=' + flightId, function (flightDetails) {
        $('#editFlightId').val(flightDetails.Id);
        $('#editAirline').val(flightDetails.Airline);
        $('#editDepartureDateAndTime').val(flightDetails.DepartureDateAndTime);
        $('#editArrivalDateAndTime').val(flightDetails.ArrivalDateAndTime);
        $('#editAvailableSeats').val(flightDetails.AvailableSeats);
        $('#editOccupiedSeats').val(flightDetails.OccupiedSeats);
        $('#editPrice').val(flightDetails.Price);
        let statusValue = GetStatus(flightDetails.FlightStatus);
        $('#editFlightStatus').val(statusValue);
    });
}

function GetStatus(status) {
    let retVal = "";
    if (status === 0) {
        retVal = "Active";
    } else if (status === 1) {
        retVal = "Cancelled";
    } else if (status === 2) {
        retVal = "Completed";
    }
    return retVal;
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
        success: function () {
            $('#addModal').modal('hide');
            LoadFlights();
            $('#FlightsToast .toast-body').text('Flight added successfully.');
            $('#FlightsToast').removeClass('text-bg-danger').addClass('text-bg-success');
            var toastEl = new bootstrap.Toast($('#FlightsToast'));
            toastEl.show();
        },
        error: function (xhr) {
            $('#addModal').modal('hide');
            var errorMessage = xhr.responseJSON ? xhr.responseJSON.Message : "An error occurred";
            $('#FlightsToast .toast-body').text(errorMessage);
            $('#FlightsToast').removeClass('text-bg-success').addClass('text-bg-danger');
            var toastEl = new bootstrap.Toast($('#FlightsToast'));
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


function LoadActiveFlights(role){
    $.get("/api/GetActiveFlights",function(data){
        if (role != "Passenger") {
            IndexTable(data);
        } else {
            PassengerTable(data);
        }
    })
}



function IndexTable(data) {
    let table = '<table class="table table-striped table-hover table-bordered fs-5"><thead><tr><th scope="col">#</th><th scope="col">Airlline</th><th scope="col">Departure Destination</th><th scope="col">Arrival Destination</th><th scope="col">Departure Date</th><th scope="col">Arrival Date</th><th scope="col">Available Seats</th><th scope="col">Occupied Seats</th><th scope="col">Price</th><th scope="col">Status</th></tr></thead><tbody>';

    let counter = 0;
    for (flight in data) {
        counter++;
        let row = '<td>' + counter.toString() + '</td>';
        row += '<td><a class="text-light" href="#" data-bs-toggle="modal" data-bs-target="#airllineModal">' + data[flight].Airline + '</a></td>';
        row += '<td>' + data[flight].DepartureDestination + '</td>';
        row += '<td>' + data[flight].ArrivalDestination + '</td>';
        row += '<td>' + data[flight].DepartureDateAndTime + '</td>';
        row += '<td>' + data[flight].ArrivalDateAndTime + '</td>';
        row += '<td class="text-center">' + data[flight].AvailableSeats + '</td>';
        row += '<td class="text-center">' + data[flight].OccupiedSeats + '</td>';
        row += '<td>' + data[flight].Price + '</td>';
        let status = GetStatus(data[flight].FlightStatus);
        row += '<td>' + status + '</td>';


        table += '<tr>' + row + '<tr/>';
    }

    table += '</tbody></table>';
    $('#flightTable').html(table);
}


function PassengerTable(data) {
    let table = '<table class="table table-striped table-hover table-bordered fs-5"><thead><tr><th scope="col">#</th><th scope="col">Airlline</th><th scope="col">Departure Destination</th><th scope="col">Arrival Destination</th><th scope="col">Departure Date</th><th scope="col">Arrival Date</th><th scope="col">Available Seats</th><th scope="col">Occupied Seats</th><th scope="col">Price</th><th scope="col">Status</th><th>Action</th></tr></thead><tbody>';

    let counter = 0;
    for (flight in data) {
        counter++;
        let row = '<td>' + counter.toString() + '</td>';
        row += '<td><a class="text-light" href="#" data-bs-toggle="modal" data-bs-target="#airllineModal">' + data[flight].Airline + '</a></td>';
        row += '<td>' + data[flight].DepartureDestination + '</td>';
        row += '<td>' + data[flight].ArrivalDestination + '</td>';
        row += '<td>' + data[flight].DepartureDateAndTime + '</td>';
        row += '<td>' + data[flight].ArrivalDateAndTime + '</td>';
        row += '<td class="text-center">' + data[flight].AvailableSeats + '</td>';
        row += '<td class="text-center">' + data[flight].OccupiedSeats + '</td>';
        row += '<td>' + data[flight].Price + '</td>';
        let status = GetStatus(data[flight].FlightStatus);
        row += '<td>' + status + '</td>';

        row += '<td class="text-center">   <button type="button" class="btn btn-primary text-light" data-bs-toggle="modal" data-bs-target="#reservationModal">Reserve</button></td>'; 

        table += '<tr>' + row + '<tr/>';
    }

    table += '</tbody></table>';
    $('#flightTable').html(table);
}