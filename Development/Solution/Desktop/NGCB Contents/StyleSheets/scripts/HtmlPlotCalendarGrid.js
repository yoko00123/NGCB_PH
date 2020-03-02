var dragState = false;
var dragRow;
var startCell;
var startCellx;
var endCell;
var HIGHLIGHTCOLOR = "#ede7e1";
var selectedcell = new Array();
var StartDate;
var EndDate;
var HotelRoom;
var HotelRoomType;
var myTable = document.getElementById('plotcalendar'); 
var rows = myTable.getElementsByTagName('tr'); 
var numRows = rows.length;     
var numRowSpan=1; 
var beforehighlight;
var tdID;

function dragStart(e) {
//var pink = document.getElementsByTagName().item()
	clearHighlight();
	var curElement = getEventTarget(e);
		dragState = true;
	startCell = curElement.cellIndex ;
	dragRow = curElement.parentNode.rowIndex;
	highlight(startCell, startCell);
}

function mOver(e) {
	var curElement = getEventTarget(e);	
	if (dragState) {
		highlight(startCell, curElement.cellIndex);
		cancelEvent(e);
	}
}

function dragEnd(e) {
    
			var x;
			var r=0;
			if (dragState) {
			dragState = false;
		}

	var row = document.getElementById("plotcalendar").getElementsByTagName("td").length;
			for (x=0; x < row; x++) {
				if(document.getElementsByTagName("td").item(x).style.backgroundColor){
					var myTD = document.getElementsByTagName("td").item(x);
					var rDate = document.getElementsByTagName("td").item(x).innerHTML;
					var room = document.getElementsByTagName("td").item(x).getAttribute("data-HotelRoom");
					selectedcell[r] = rDate.split("-");
					
					r++;
				}
}
if (selectedcell != "") {
    StartDate = selectedcell[0][0].substring(27, 37);
    EndDate = selectedcell[r - 1][0].substring(27, 37);
    HotelRoomType = document.getElementsByTagName("td").item(x).getAttribute("data-hotelroomtype");
    HotelRoom = room;
} 



if (beforehighlight == "#ffffff" || beforehighlight == "#f8f8f8") {
			    if (dragState) {
			        dragState = false;
			    }
			    if (StartDate != EndDate) {
			        fnewinfo2(StartDate, EndDate, HotelRoom, HotelRoomType);
			    } else {
			        window.external.LoadInfo();
			    }
			} else if (beforehighlight || !beforehighlight) {
			    if (dragState) {
			        dragState = false;
			    }
			    if (StartDate == EndDate) {

			        ///////////////////////////
			        if (selectedcell != "") {
			            window.external.LoadInfo();
			        }
			        else {
			            window.external.LoadInfo(17);
			        }
			    } else {
			        fnewinfo2(StartDate, EndDate, HotelRoom, HotelRoomType);
			    }
			}
}


function clearHighlight() {
	var row = document.getElementById("plotcalendar").getElementsByTagName("tr")[dragRow];
	if (row) {
		var cells = row.getElementsByTagName("td");
		for (var x=0; x < cells.length; x++) {
			cells[x].style.backgroundColor = "";
		}
	}
}
	var mhastext = false;
function highlight(start, end) {
	mhastext = false;
	if (dragState) {
		var row = document.getElementById("plotcalendar").getElementsByTagName("tr")[dragRow];
		if (row) {
			clearHighlight();
			var cells = row.getElementsByTagName("td");
			for (var x=Math.min(start,end); x <= Math.max(start,end); x++) {
				beforehighlight = cells[x].bgColor;
				mselectedrow = cells[x];
				cells[x].style.backgroundColor = HIGHLIGHTCOLOR;
				if (cells[x].textContent!=''){
					mhastext = true;
					}
			}
		}
	}
}

function cancelEvent(e) {
	if (window.event) {
		if (window.event.returnValue) { window.event.returnValue = false; }
		if (window.event.cancelBubble) { window.event.cancelBubble = true; }		
	}
	if (e) {
		if (e.preventDefault) { e.preventDefault(); }
		if (e.stopPropagation) { e.stopPropagation(); }
	}
}

function addEvent(elm, evType, fn, useCapture) {
	if (elm.addEventListener) {
		elm.addEventListener(evType, fn, useCapture);
		return true;
	}
	else if (elm.attachEvent) {
		var r = elm.attachEvent('on' + evType, fn);
		return r;
	}
	else {
		elm['on' + evType] = fn;
	}
}

function getEventTarget(e) {
	if (window.event && window.event.srcElement) { return window.event.srcElement; }
	if (e && e.target) { return e.target; }
	return null
}

var theTable = document.getElementById("plotcalendar");
var tds = theTable.getElementsByTagName("td");
for (var x=0; x < tds.length; x++) {
	if (tds[x].id.length > 0) {
		addEvent(tds[x], 'mousedown', dragStart, false);
		addEvent(tds[x], 'mousemove', mOver, false);
		addEvent(tds[x], 'mouseup', dragEnd, false);
	}
}
function fnewinfo2(myStartDate, myEndDate, myHotelRoom, myHotelRoomType) {
    try {
        window.external.NewInfo2(myStartDate, myEndDate, myHotelRoom, myHotelRoomType);
    } catch (ex) {

    }
 
}
//function showinfo(mypID) {
//    window.external.ShowInfo(mypID);
//}

// for (var j = 1; j <(numRows-1); j++) {
//	//alert(myTable);       
// if (numRowSpan<=1) {
// var currentRow = myTable.getElementsByTagName('tr')[j];
// var currentCell= currentRow.getElementsByTagName('td')[0];  // the 1st column           
// var currentCellData = currentCell.childNodes[0].data;
// }
//
// if (j<numRows-1) {
//
// if (myTable.getElementsByTagName('tr')[j+1]) {
//
// var nextRow = myTable.getElementsByTagName('tr')[j+1];
//
// var nextCell = nextRow.getElementsByTagName('td')[0];
//
// var nextCellData = nextCell.childNodes[0].data;
//
////compare the current cell and the next cell             
//
// if (currentCellData == nextCellData) {
//
// numRowSpan += 1;
//
// currentCell.rowSpan = numRowSpan;
//
// nextCell.style.display = 'none';   //disappear the next cell             
//
// } else {
//
// numRowSpan = 1;
//
// }
//
// }
//
// }
//
// }