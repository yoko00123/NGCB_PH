var nLeft = 25;
var divID = 1;
var nTop = 54;
var tableNo = 0;
var imglocation;
var myTable = new Array();
var xTableID;
function Browser() {
  var ua, s, i;

  this.isIE    = false;
  this.isNS    = false;
  this.version = null;

  ua = navigator.userAgent;

  s = "MSIE";
  if ((i = ua.indexOf(s)) >= 0) {
    this.isIE = true;
    this.version = parseFloat(ua.substr(i + s.length));
    return;
  }

  s = "Netscape6/";
  if ((i = ua.indexOf(s)) >= 0) {
    this.isNS = true;
    this.version = parseFloat(ua.substr(i + s.length));
    return;
  }

  // Treat any other "Gecko" browser as NS 6.1.

  s = "Gecko";
  if ((i = ua.indexOf(s)) >= 0) {
    this.isNS = true;
    this.version = 6.1;
    return;
  }
}

var browser = new Browser();
function addDiv(){

//	alert("asdasd");
tableNo = 1 + tableNo;
//nLeft = 150 + nLeft;
//divID = 1 + divID;
//nTop = 54 + nTop;
//var left = nLeft+"px";
//var top = nTop+"px";
document.getElementById("floor").innerHTML += "<div id='x' style='left:400px;top:50px;position:absolute;cursor:crosshair' onmousedown='dragStart(event)'><embed src='_persona.svg'>"+tableNo+"</div>\n";
}
//<img class='' style='left:400px;top:50px;position:absolute;' onmousedown='dragStart(event)' id='y' src='__SerialNumber.png'>
//<div id='x' class='box content' style='left:400px;top:50px;' onmousedown='dragStart(event)'>
function heightControl() {
	var iHeight = document.body.clientHeight;
	iHeight = iHeight - 12;
	myHeight = iHeight + "px";
	var x =	document.getElementById('floor').style.height = "100%";
}
var dragObj = new Object();
dragObj.zIndex = 0;

function dragStart(event, id) {

  var el;
  var x, y;

  if (id)
    dragObj.elNode = document.getElementById(id);
  else {
    if (browser.isIE)
      dragObj.elNode = window.event.srcElement;
    if (browser.isNS)
      dragObj.elNode = event.target;

    if (dragObj.elNode.nodeType == 3)
      dragObj.elNode = dragObj.elNode.parentNode;
  }

  if (browser.isIE) {
    x = window.event.clientX + document.documentElement.scrollLeft
      + document.body.scrollLeft;
    y = window.event.clientY + document.documentElement.scrollTop
      + document.body.scrollTop;
  }
  if (browser.isNS) {
    x = event.clientX + window.scrollX;
    y = event.clientY + window.scrollY;
  }

  dragObj.cursorStartX = x;
  dragObj.cursorStartY = y;
  dragObj.elStartLeft  = parseInt(dragObj.elNode.style.left, 10);
  dragObj.elStartTop   = parseInt(dragObj.elNode.style.top,  10);

  if (isNaN(dragObj.elStartLeft)) dragObj.elStartLeft = 0;
  if (isNaN(dragObj.elStartTop))  dragObj.elStartTop  = 0;

  dragObj.elNode.style.zIndex = ++dragObj.zIndex;

  if (browser.isIE) {
    document.attachEvent("onmousemove", dragGo);
    document.attachEvent("onmouseup",   dragStop);
    window.event.cancelBubble = true;
    window.event.returnValue = false;
  }
  if (browser.isNS) {
    document.addEventListener("mousemove", dragGo,   true);
    document.addEventListener("mouseup",   dragStop, true);

    event.preventDefault();
  }
}

function dragGo(event) {

  var x, y;

  if (browser.isIE) {
    x = window.event.clientX + document.documentElement.scrollLeft
      + document.body.scrollLeft;
    y = window.event.clientY + document.documentElement.scrollTop
      + document.body.scrollTop;
  }
  if (browser.isNS) {
    x = event.clientX + window.scrollX;
    y = event.clientY + window.scrollY;
  }
  dragObj.elNode.style.left = (dragObj.elStartLeft + x - dragObj.cursorStartX) + "px";
  dragObj.elNode.style.top  = (dragObj.elStartTop  + y - dragObj.cursorStartY) + "px";

  if (browser.isIE) {
    window.event.cancelBubble = true;
    window.event.returnValue = false;
  }
  if (browser.isNS)
    event.preventDefault();
}

function dragStop(event) {
  if (browser.isIE) {
    document.detachEvent("onmousemove", dragGo);
    document.detachEvent("onmouseup",   dragStop);
  }
  if (browser.isNS) {
    document.removeEventListener("mousemove", dragGo,   true);
    document.removeEventListener("mouseup",   dragStop, true);
  }
}
function showCoor(){
    var myObjClass = document.getElementsByTagName("div");
    var myObjLength = document.getElementsByTagName("div").length;
    var myObj = document.getElementsByTagName("div");
    var myCoor = new Array();
    var rLen = 0;
    var y = 0;
    for(var x=0;x<myObjLength;x++){
        if(myObj[x].className == "object"){            
            myCoor[y] = myObj[x].getAttribute("data-id")+"|"+myObj[x].style.left+"|"+myObj[x].style.top+"|"+myObj[x].style.zIndex;
        y++;
        }
    }
    return myCoor.toString();
}
function getClickedTable(mID,Name,mysrcbtn){
    var table = document.getElementById(mID);
    var tablediv = document.getElementsByTagName("div");
    //alert(mysrcbtn);
    if(mysrcbtn == 1){
        if(table.getAttribute("data-status") == "vacant"){
            table.setAttribute("data-status","selected");
            for(var x=0;x<tablediv.length;x++){
                if(tablediv[x].getAttribute("data-id") == mID){
                    tablediv[x].style.backgroundColor = "Mistyrose";
                }
            }
        }else if(table.getAttribute("data-status") == "occupied"){
            alert("Table Occupied.");
        }
    }else {
        if(table.getAttribute("data-status") == "occupied"){
            table.setAttribute("data-status","payment");
            for(var x=0;x<tablediv.length;x++){
                if(tablediv[x].getAttribute("data-id") == mID){
                    tablediv[x].style.backgroundColor = "Mistyrose";
                }
            }
        }
    }
    //var iLoc = table[Name].src.substr(0,imglocation+3);
//    if(table[Name].src == iLoc+"Resto/RestoTable.png"){
//        table[Name].src = iLoc+"Resto/RestoTableSelected.png";
//        table[Name].setAttribute("data-status","selected");
//    }else{
//        table[Name].src = iLoc+"Resto/RestoTable.png";
//        table[Name].setAttribute("data-status","vacant");
//    }
    //alert(table[Name].src = iLoc+"Resto\\RestoTableSelected.png");    
       
    //alert(Name);
    //window.external.DineIn(mID,Name);
}
function getPaymentTable(){
         var selectedTable = new Array();
    
    var myTableName = new Array();
    
    var imgsrc = document.getElementsByTagName("img");
    var imgsrcL = document.getElementsByTagName("img").length;
    var y=0;
    for(var x = 0; x < imgsrcL; x++){
        if(imgsrc[x].getAttribute("data-status") == "payment"){
            myTable[y] = imgsrc[x].name+";"+imgsrc[x].getAttribute("data-id");
            y++;
        }
    }
    return myTable.toString();
}

function getSelectedTable(){
    var selectedTable = new Array();
    
    var myTableName = new Array();
    
    var imgsrc = document.getElementsByTagName("img");
    var imgsrcL = document.getElementsByTagName("img").length;
    var y=0;
    for(var x = 0; x < imgsrcL; x++){
        if(imgsrc[x].getAttribute("data-status") == "selected"){
            xTableID = imgsrc[x].getAttribute("data-id");
            myTable[y] = imgsrc[x].getAttribute("data-id")+";"+imgsrc[x].name;
            y++;
        }else {
            myTable[y] = "0;0";
        }
    }
    return myTable.toString();
    
}
function getTableID(){
    return xTableID;
}
//var addtables = document.getElementById("addtable");
//eval("addtables.onclick = function(){addDiv();}");