function getWaiter(){
    
    var waiter = document.getElementsByTagName("div");
    var myImg;
    var idName = new Array();
    for(var x=0;x<waiter.length;x++){
        if(waiter[x].getAttribute("data-status") == "selected"){
            idName = [waiter[x].id,waiter[x].getAttribute("data-name").toString(), waiter[x].getAttribute("data-img")];
            
        }
                
    }
    
    return idName.toString(); 
}

function selectWaiter(did){
    var sWaiter = document.getElementById(did);
    var waiters = document.getElementsByTagName("div");
    for(var x=0; x < waiters.length; x++){
        waiters[x].style.backgroundColor = "";
        waiters[x].setAttribute("data-status","vacant");
    }
        
    for(var x=0; x < waiters.length; x++){
        if(waiters[x].getAttribute("data-status") == "vacant" && waiters[x].id == did){
            sWaiter.setAttribute("data-status","selected");
            sWaiter.style.backgroundColor = "mistyrose";
        }    
    }
    
}