
function LoadProfile() {
    $.get("/api/GetCurrentUser", function (data) {

        $("#username").val(data.Username);
        $("#oldUsername").val(data.Username);
        $("#password").val(data.Password);
        $("#email").val(data.Email);
        $("#name").val(data.Name);
        $("#surname").val(data.Surname);
        $("#dateOfBirth").val(data.DateOfBirth);
        var genderValue = data.Gender == 0 ? "male" : "female";
        $("#gender").val(genderValue);
    })

}

function LoadUsers(){
    var searchByName = $("#searchByName").val();
    var searchBySurname = $("#searchBySurname").val();
    var lowerDateOfBirth = $("#lowerDateOfBirth").val();
    var upperDateOfBirth = $("#upperDateOfBirth").val();
    var sortByName = $("input[name='sortByNameOption']:checked").val();
    var sortByDate = $("input[name='sortByDateOption']:checked").val();

    var filter = {
        SearchByName: searchByName,
        SearchBySurname: searchBySurname,
        LowerDateOfBirth: lowerDateOfBirth,
        UpperDateOfBirth: upperDateOfBirth,
        SortByName: sortByName,
        SortByDate: sortByDate
    };

    $.ajax({
        url: "/api/GetAllUsers",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(filter),
        success: function (data) {
            displayUsers(data);
        },
        error: function (error) {
            console.log("Error filtering users:", error);
        }
    });
}

function displayUsers(users) {
    let table = '<table class="table table-striped table-hover table-bordered fs-5"><thead><tr><th scope="col">#</th><th scope="col">Name</th><th scope="col">Surname</th><th scope="col">Username</th><th scope="col">Email</th><th scope="col">Date Of Birth</th><th scope="col">Role</th><th scope="col">Gender</th></tr></thead><tbody>';

    let counter = 0;
    users.forEach(function (user) {
        counter++;
        let row = '<td>' + counter.toString() + '</td>';
        row += '<td>' + user.Name + '</td>';
        row += '<td>' + user.Surname + '</td>';
        row += '<td>' + user.Username + '</td>';
        row += '<td>' + user.Email + '</td>';
        row += '<td>' + user.DateOfBirth + '</td>';
        row += '<td>' + user.Role + '</td>';
        var genderValue = user.Gender == 0 ? "Male" : "Female";
        row += '<td>' + genderValue + '</td>';

        table += '<tr>' + row + '</tr>';
    });

    table += '</tbody></table>';
    $('#userTable').html(table);
}

function Reset() {
    $("#searchByName").val("");
    $("#searchBySurname").val("");
    $("#lowerDateOfBirth").val("");
    $("#upperDateOfBirth").val("");
    $("input[name='sortByNameOption']").prop("checked", false);
    $("input[name='sortByDateOption']").prop("checked", false);

    LoadUsers();
}


function EditProfile(event){
    event.preventDefault(); 
    let form = $("#profileForm");
    let data = convertFormToJSON(form)
    data = JSON.stringify(data)
    let oldUsername = $("#oldUsername").val()


    $.ajax({
        url: '/api/EditProfile?oldUsername='+oldUsername,
        type:'PUT',
        data:data,
        contentType:"application/json",
        success:function(){
            LoadProfile();
            LoadAllUsers();
            $('#ProfileToast .toast-body').text('Profile edited successfully.');
            $('#ProfileToast').removeClass('text-bg-danger').addClass('text-bg-success');
            var toastEl = new bootstrap.Toast($('#ProfileToast'));
            toastEl.show();
        },
        error:function(xhr){
            var errorMessage = xhr.responseJSON ? xhr.responseJSON.Message : "An error occurred";
            $('#ProfileToast .toast-body').text(errorMessage);
            $('#ProfileToast').removeClass('text-bg-success').addClass('text-bg-danger');
            var toastEl = new bootstrap.Toast($('#ProfileToast'));
            toastEl.show();
        }

    })

    
}

function Discard(){
    LoadProfile()
}

function convertFormToJSON(form) {
    const array = $(form).serializeArray(); // Encodes the set of form elements as an array of names and values.
    const json = {};
    $.each(array, function () {
        json[this.name] = this.value || "";
    });
    return json;
}

