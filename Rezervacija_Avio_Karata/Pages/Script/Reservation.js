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

async function LoadCreatedReservations() {
    try {
        const reservations = await $.get("/api/LoadCreatedReservations");

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
                row += '<td>' + reservations[i].Price + '</td>';
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

function GetFlightStatus(status) {
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

function GetReservationStatus(status) {
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

function CancelReservation(){
    let id = $("#cancelFlightId").val();
    $.ajax({
        url: '/api/CancelReservation?id=' + id,
        type: 'DELETE',
        success: function () {
            LoadCreatedReservations();
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



function convertFormToJSON(form) {
    const array = $(form).serializeArray(); // Encodes the set of form elements as an array of names and values.
    const json = {};
    $.each(array, function () {
        json[this.name] = this.value || "";
    });
    return json;
}