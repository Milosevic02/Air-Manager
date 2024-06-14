function GetUserRole(pageRole) {
    //event.preventDefault();

    let role = ""
    $.ajax({
        url:"/api/GetUserRole",
        type:"GET",
        complete: function(result){
            if(result.status == 200){
                role = result.responseJSON
                if(role != pageRole){
                    if(role == "Admin"){
                        window.location.href = "Admin.html"
                    }
                    else if(role == "Passenger"){
                        window.location.href = "Passenger.html"
                    }else{
                        window.location.href = "Index.html"
                    }
                }
            }
        }
    })
}