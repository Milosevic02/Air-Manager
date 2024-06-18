
function LoadProfile() {
    $.get("/api/GetCurrentUser", function (data) {

        $("#username").val(data.Username);
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
            let gender = GetGender(data[user].Gender);
            row += '<td>' + gender + '</td>'; 

            table += '<tr>' + row + '<tr/>';
        }

        table += '</tbody></table>';
        $('#userTable').html(table);
    })
}

function GetGender(param){
    if(param == 0){
        return "Male";
    }else if(param == 1){
        return "Female";
    }
}