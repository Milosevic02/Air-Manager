function ReserveFlight(event){
    event.preventDefault(); 

    let form = $("#reservationForm");
    let data = convertFormToJSON(form);
    data = JSON.stringify(data)
    $.ajax({
        url:'/api/CreateReservation',
        type:'POST',
        data:data,
        contentType: "application/json", 
        success:function(){
            LoadActiveFlights("Passenger");
            $('#reservationModal').modal('hide');
            $('#passengerToast .toast-body').text('Reservation created successfully.');
            $('#passengerToast').removeClass('text-bg-danger').addClass('text-bg-success');
            var toastEl = new bootstrap.Toast($('#passengerToast'));
            toastEl.show();
        },
        error:function(xhr){
            $('#reservationModal').modal('hide');
            var errorMessage = xhr.responseJSON ? xhr.responseJSON.Message : "An error occurred";
            $('#passengerToast .toast-body').text(errorMessage);
            $('#passengerToast').removeClass('text-bg-success').addClass('text-bg-danger');
            var toastEl = new bootstrap.Toast($('#passengerToast'));
            toastEl.show();
        }
    })
    

}

async function LoadCreatedReservationsPassenger() {
    try {
        const reservations = await $.get("/api/LoadCreatedReservations?role=\"Passenger\"");

        if (reservations.length > 0) {
            let table = '<table class="table table-striped table-hover table-bordered">';
            table += '<thead><tr>';
            table += '<th scope="col">#</th>';
            table += '<th scope="col">Airline</th>';
            table += '<th scope="col">Departure Destination</th>';
            table += '<th scope="col">Arrival Destination</th>';
            table += '<th scope="col">Departure Date</th>';
            table += '<th scope="col">Arrival Date</th>';
            table += '<th scope="col">Flight Status</th>';

            table += '<th scope="col">Reserved Seats</th>';
            table += '<th scope="col">Price</th>';
            table += '<th scope="col">Reservation Status</th>';
            table += '<th scope="col">Action</th>';
            table += '</tr></thead><tbody>';

            for (let i = 0; i < reservations.length; i++) {
                let row = '<tr>';
                row += '<td>' + (i + 1) + '</td>';

                const flight = await $.get('/api/GetFlightDetails?id=' + reservations[i].FlightId);

                row += '<td>' + flight.Airline + '</td>';
                row += '<td>' + flight.DepartureDestination + '</td>';
                row += '<td>' + flight.ArrivalDestination + '</td>';
                row += '<td>' + flight.DepartureDateAndTime + '</td>';
                row += '<td>' + flight.ArrivalDateAndTime + '</td>';
                let flightStatus = GetFlightStatus(flight.FlightStatus);
                row += '<td>' + flightStatus + '</td>';
                row += '<td>' + reservations[i].CountOfPassengers + '</td>';
                row += '<td>$' + reservations[i].Price + '</td>';
                let reservationStatus = GetReservationStatus(reservations[i].ReservationStatus);
                row += '<td>' + reservationStatus + '</td>';
                row += '<td> <button onclick="AddReservationIdOnModal(\'' + reservations[i].Id + '\', \'Passenger\')" type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#cancelModal"><i class="fas fa-times"></i> Cancel</button></div></div></td>';
                row += '</tr>';

                table += row;
            }

            table += '</tbody></table>';
            $("#reservationTableContainer").html(table);
        } else {
            $('#reservationTableContainer').html('<h1>No reservation available.</h1>');
        }
    } catch (error) {
        console.error('Error loading reservations:', error);
        $('#reservationTableContainer').html('<h1>Error loading reservations.</h1>');
    }
}






function GetReservationStatus(status) {
    let retVal = "";
    if (status === 0) {
        retVal = "Created";
    } else if (status === 1) {
        retVal = "Approved";
    } else if (status === 2) {
        retVal = "Rejected";
    }else if(status === 3){
        retVal = "Finished"
    }
    return retVal;
}

function GetFlightStatus(status) {
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

function AddReservationIdOnModal(id){
    $("#cancelFlightId").val(id);
}

function CancelReservation(role){
    let id = $("#cancelFlightId").val();
    $.ajax({
        url: '/api/CancelReservation?id=' + id,
        type: 'DELETE',
        success: function () {
            if(role != "Admin"){LoadCreatedReservationsPassenger();
            }else{
                LoadCreatedReservationsAdmin();
                LoadApprovedReservationsAdmin();
            }
            
            $('#cancelModal').modal('hide');
            $('#ReservationToast .toast-body').text('Flight canceled successfully.');
            $('#ReservationToast').removeClass('text-bg-danger').addClass('text-bg-success');
            var toastEl = new bootstrap.Toast($('#ReservationToast'));
            toastEl.show();

        },
        error:function(xhr){
            var errorMessage = xhr.responseJSON ? xhr.responseJSON.Message : "An error occurred";
            $('#cancelModal').modal('hide');
            $('#ReservationToast .toast-body').text(errorMessage);
            $('#ReservationToast').removeClass('text-bg-success').addClass('text-bg-danger');
            var toastEl = new bootstrap.Toast($('#ReservationToast'));
            toastEl.show();
        }
    });

}


async function LoadCreatedReservationsAdmin() {
    try {
        const reservations = await $.get("/api/LoadCreatedReservations?role=Admin");

        if (reservations.length > 0) {
            let table = '<table class="table table-striped table-hover table-bordered">';
            table += '<thead><tr>';
            table += '<th scope="col">#</th>';
            table += '<th scope="col">User</th>';
            table += '<th scope="col">Airline</th>';
            table += '<th scope="col">Departure Destination</th>';
            table += '<th scope="col">Arrival Destination</th>';
            table += '<th scope="col">Departure Date</th>';
            table += '<th scope="col">Arrival Date</th>';
            table += '<th scope="col">Flight Status</th>';
            table += '<th scope="col">Available Seats</th>';
            table += '<th scope="col">Reserved Seats</th>';

            table += '<th scope="col">Price</th>';
            table += '<th scope="col">Reservation Status</th>';
            table += '<th scope="col">Approve</th>';
            table += '<th scope="col">Reject</th>';
            table += '</tr></thead><tbody>';

            for (let i = 0; i < reservations.length; i++) {
                let row = '<tr>';
                row += '<td>' + (i + 1) + '</td>';

                const flight = await $.get('/api/GetFlightDetails?id=' + reservations[i].FlightId);
               
                row += '<td>' + reservations[i].User + '</td>';
                row += '<td>' + flight.Airline + '</td>';
                row += '<td>' + flight.DepartureDestination + '</td>';
                row += '<td>' + flight.ArrivalDestination + '</td>';
                row += '<td>' + flight.DepartureDateAndTime + '</td>';
                row += '<td>' + flight.ArrivalDateAndTime + '</td>';
                let flightStatus = GetFlightStatus(flight.FlightStatus);
                row += '<td>' + flightStatus + '</td>';
                row += '<td>' + flight.AvailableSeats + '</td>';
                row += '<td>' + reservations[i].CountOfPassengers + '</td>';
                row += '<td>$' + reservations[i].Price + '</td>';
                let reservationStatus = GetReservationStatus(reservations[i].ReservationStatus);
                row += '<td>' + reservationStatus + '</td>';
                row += '<td> <button onclick="ChangeReservationStatus(\'' + reservations[i].Id + '\', \'Approved\')" type="button" class="btn btn-success" ><i class="fas fa-check"></i> Approve</button></div></div></td>';
                row += '<td> <button onclick="ChangeReservationStatus(\'' + reservations[i].Id + '\', \'Rejected\')" type="button" class="btn btn-danger" ><i class="fas fa-times"></i> Reject</button></div></div></td>';

                row += '</tr>';

                table += row;
            }

            table += '</tbody></table>';
            $("#createdReservationTable").html(table);
        } else {
            $('#createdReservationTable').html('<h1 class="text-light" >No reservation available.</h1>');
        }
    } catch (error) {
        console.error('Error loading reservations:', error);
        $('#createdReservationTable').html('<h1>Error loading reservations.</h1>');
    }
}

function ChangeReservationStatus(id, action) {
    $.ajax({
        url: '/api/ChangeReservationStatus?id='+id+'&action='+action,
        type: 'POST',
        success: function() {
            LoadCreatedReservationsAdmin();
            LoadApprovedReservationsAdmin();
            $('#ReservationToast .toast-body').text('Reservation ' + action + ' successfully.');
            $('#ReservationToast').removeClass('text-bg-danger').addClass('text-bg-success');
            var toastEl = new bootstrap.Toast($('#ReservationToast'));
            toastEl.show();
        },
        error: function(xhr) {
            var errorMessage = xhr.responseJSON ? xhr.responseJSON.Message : "An error occurred";
            $('#ReservationToast .toast-body').text(errorMessage);
            $('#ReservationToast').removeClass('text-bg-success').addClass('text-bg-danger');
            var toastEl = new bootstrap.Toast($('#ReservationToast'));
            toastEl.show();
        }
    });
}


async function LoadApprovedReservationsAdmin() {
    try {
        const reservations = await $.get("/api/LoadApprovedReservations?role=Admin");

        if (reservations.length > 0) {
            let table = '<table class="table table-striped table-hover table-bordered">';
            table += '<thead><tr>';
            table += '<th scope="col">#</th>';
            table += '<th scope="col">User</th>';
            table += '<th scope="col">Airline</th>';
            table += '<th scope="col">Departure Destination</th>';
            table += '<th scope="col">Arrival Destination</th>';
            table += '<th scope="col">Departure Date</th>';
            table += '<th scope="col">Arrival Date</th>';
            table += '<th scope="col">Flight Status</th>';
            table += '<th scope="col">Available Seats</th>';
            table += '<th scope="col">Reserved Seats</th>';

            table += '<th scope="col">Price</th>';
            table += '<th scope="col">Reservation Status</th>';
            table += '<th scope="col">Cancel</th>';
            table += '</tr></thead><tbody>';

            for (let i = 0; i < reservations.length; i++) {
                let row = '<tr>';
                row += '<td>' + (i + 1) + '</td>';

                const flight = await $.get('/api/GetFlightDetails?id=' + reservations[i].FlightId);
               
                row += '<td>' + reservations[i].User + '</td>';
                row += '<td>' + flight.Airline + '</td>';
                row += '<td>' + flight.DepartureDestination + '</td>';
                row += '<td>' + flight.ArrivalDestination + '</td>';
                row += '<td>' + flight.DepartureDateAndTime + '</td>';
                row += '<td>' + flight.ArrivalDateAndTime + '</td>';
                let flightStatus = GetFlightStatus(flight.FlightStatus);
                row += '<td>' + flightStatus + '</td>';
                row += '<td>' + flight.AvailableSeats + '</td>';
                row += '<td>' + reservations[i].CountOfPassengers + '</td>';
                row += '<td>$' + reservations[i].Price + '</td>';
                let reservationStatus = GetReservationStatus(reservations[i].ReservationStatus);
                row += '<td>' + reservationStatus + '</td>';
                row += '<td> <button onclick="AddReservationIdOnModal(\'' + reservations[i].Id + '\', \'Passenger\')" type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#cancelModal"><i class="fas fa-times"></i> Cancel</button></div></div></td>';

                row += '</tr>';

                table += row;
            }

            table += '</tbody></table>';
            $("#approvedReservationTable").html(table);
        } else {
            $('#approvedReservationTable').html('<h1 class="text-light" >No reservation available.</h1>');
        }
    } catch (error) {
        console.error('Error loading reservations:', error);
        $('#approvedReservationTable').html('<h1>Error loading reservations.</h1>');
    }
}

function convertFormToJSON(form) {
    const array = $(form).serializeArray(); // Encodes the set of form elements as an array of names and values.
    const json = {};
    $.each(array, function () {
        json[this.name] = this.value || "";
    });
    return json;
}