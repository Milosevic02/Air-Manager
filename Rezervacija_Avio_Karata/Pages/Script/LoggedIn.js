function SignOut() {
    $.ajax({
        url:"/api/SignOut",
        type:"GET",
        complete:function(result){
            window.location.href = "Index.html"
        }
    })
}