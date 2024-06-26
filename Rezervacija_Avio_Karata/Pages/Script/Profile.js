
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

function LoadAllUsers(){
    $.get("/api/GetAllUsers",function(data){
        let table = '<table class="table table-striped table-hover table-bordered fs-5"><thead><tr><th scope="col">#</th><th scope="col">Name</th><th scope="col">Surname</th><th scope="col">Username</th><th scope="col">Email</th><th scope="col">Date Of Birth</th><th scope="col">Role</th><th scope="col">Gender</th></tr></thead><tbody>';

        let counter = 0;
        for(user in data){
            counter++;
            let row = '<td>' + counter.toString() + '</td>'; 
            row += '<td>' + data[user].Name + '</td>';
            row += '<td>' + data[user].Surname + '</td>'; 
            row += '<td>' + data[user].Username + '</td>'; 
            row += '<td>' + data[user].Email + '</td>'; 
            row += '<td>' + data[user].DateOfBirth + '</td>'; 
            row += '<td>' + data[user].Role + '</td>'; 
            var genderValue = data[user].Gender == 0 ? "Male" : "Female";
            row += '<td>' + genderValue + '</td>'; 

            table += '<tr>' + row + '<tr/>';
        }

        table += '</tbody></table>';
        $('#userTable').html(table);
    })
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

