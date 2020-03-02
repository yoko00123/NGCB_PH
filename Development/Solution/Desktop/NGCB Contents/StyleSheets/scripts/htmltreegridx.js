var mselectedrow;
var maltbackgroundcolor;
var mrowcount;
var mgroupbackgroundcolor;
var mdarkbackgroundcolor;
var mgroupcount = 0;
var mselectionmode = false;
var mhovercolor = '#ffff00';
var mgraycolor = '#fcfcfc';
mgroupbackgroundcolor = 'LightSteelBlue';

var vtablebody;
var trs;



function finitrows() {
    var vdatarowindex = 0;
    var tmp = '#' + fmediancolor(mdarkbackgroundcolor, maltbackgroundcolor);
    finitheader();
    mgroupbackgroundcolor = '#' + fmediancolor(mdarkbackgroundcolor, maltbackgroundcolor);
    vtablebody = document.getElementsByTagName('tbody')[0];
    trs = vtablebody.getElementsByTagName('tr');
    mrowcount = 0;
 //   mgroupbackgroundcolor = maltbackgroundcolor;
    mgroupbackgroundcolor = '#ffffff';
  //    maltbackgroundcolor = '#ffffff';
    eval("document.body.attachEvent('onkeydown',function() {return tr_keykdown();}   );");
    for (x = 0; x < trs.length; x++) {
        var tr = trs[x]
        var hhh = tr.getElementsByTagName('td');
        eval("tr.attachEvent('onclick',function () { fselectrow('" + tr.id + "');});");
//        eval("tr.attachEvent('onkeydown',function() {return tr_keykdown();}   );");
         fresetrow(tr.id);
         eval("tr.attachEvent('onmouseover',function () { fhoverrow('" + tr.id + "');});");
         eval("tr.attachEvent('onmouseout',function () { fresetrowifnotselected('" + tr.id + "');});");


         tr.setAttribute('data-checked', false);
         var chk = document.getElementById('chk' + tr.id);
         if (chk) {
             eval("chk.attachEvent('onclick',function () {fcheckchildren('" + tr.id + "');});");
             mselectionmode = true;
         }

         switch (tr.className) {
             case 'htmltreegrid_group':
                 eval("tr.attachEvent('ondblclick',function () {fshowchildren('" + tr.id + "');});");
                 tr.setAttribute('data-isexpanded', true);
                 mgroupcount++;
                 break;
             default:
                 if (tr.getAttribute('data-id')) { //make sure it is a datarow
                     eval("tr.attachEvent('ondblclick',function () { floadinfo();});");
                     if (!vdatarowindex) {
                         fselectrow(x);
                     } //if

                     if (mgroupcount) {
                         hhh[2].style.color = tmp;
                     } else {
                         hhh[1].style.color = tmp;
                     }
                     if (mselectionmode) {
                         hhh[3].style.color = tmp;
                     }

                     vdatarowindex++;
                 } //if
                 break;
        } //switch
        mrowcount++;
      //  hhh[0].style.width = '20px'; //consider sortglyph
    } //for
} //finitrows
/////////////////////////////////////////////////////////////////////////////
function fselectrow(prowid) {
    
    if (mselectedrow) {fresetrow(mselectedrow.id)};
    mselectedrow = document.getElementById(prowid);
    mselectedrow.style.backgroundColor = '#ffcc80';
 //   mselectedrow.getElementsByTagName('td')[0].focus;
    mselectedrow.focus;
    return mselectedrow.id;
	
}
//////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
function fselectrow2(prowid) {
	//alert(mselectedrow);
    mselectedrow = document.getElementById(prowid);
	//alert(mselectedrow);
	floadinfo();
}
//////////////////////////////////////////////////////////////
function fhoverrow(prowid) {
    if (prowid == '') { return; }
    var tr = document.getElementById(prowid);
  //  if (tr != mselectedrow) {

    tr.style.backgroundColor = fmediancolor(tr.style.backgroundColor, mhovercolor);
//}
    
}
////////////////////////////////////////////////////////////

function fresetrowifnotselected(prowid) {
    var tr = document.getElementById(prowid);
    if (tr != mselectedrow) {
        fresetrow(prowid);
    } else {
        mselectedrow.style.backgroundColor = '#ffcc80';
    }

}

/////////////////////////////////////////////////////////////////////////////
function floadinfo() {
    //location.href = pID
    if (mselectedrow) {
        if (!isScrolledIntoView(mselectedrow)) { mselectedrow.scrollIntoView(true) };
        // mselectedrow.style.backgroundColor = '#80ff80'
       try{
           window.external.LoadInfo();
           }
       catch(err){
           loadInfo(fselecteddataid());
           }

       
      //  mselectedrow.style.backgroundColor = '#ffcc80';
    };
}
/////////////////////////////////////////////////////////////////////////////
function fselecteddataid() {
    return mselectedrow ? mselectedrow.getAttribute('data-id')? mselectedrow.getAttribute('data-id') : 0: 0;
}
/////////////////////////////////////////////////////////////////////////////
function fsortby(pColumnName) {
    window.external.SortBy(pColumnName)
}
/////////////////////////////////////////////////////////////////////////////
function tr_keykdown() { 
    var e = window.event;
    var vid = 0;
    var a;
    //alert(e.keyCode);
    switch (e.keyCode) {
        case 13:
            (mselectedrow.className == 'htmltreegrid_group') ? fshowchildren(mselectedrow.id,false) : floadinfo(mselectedrow.id);
            return false;
            break;
        case 37:
            if (mselectedrow.className == 'htmltreegrid_group') { fshowchildren(mselectedrow.id, false) };
            
            return false;
            break;
        case 38:
            a = mselectedrow.previousSibling
            if (a) {
                while (a.style.display == 'none' && a.previousSibling) {
                    a = a.previousSibling;
                }
            }
            if (a) {
                fselectrow(a.id);
                if (a.id == 0) {
                    
                    if (document.getElementById('topDiv')) {
                        document.getElementById('topDiv').scrollIntoView(true);
                    } else {
                        document.getElementById('maintable').scrollIntoView(true);
                    }
                    
                } else {
                    if (!isScrolledIntoView(mselectedrow)) { mselectedrow.scrollIntoView(true) };
                }

            }
            return false;
            break;
       case 39:
           if (mselectedrow.className == 'htmltreegrid_group') { fshowchildren(mselectedrow.id, true) };
           return false;
           break;
        case 40:

            a = mselectedrow.nextSibling
            if (a) {
                while (a.style.display == 'none' && (a.nextSibling)) {
                    a = a.nextSibling;
                }
            }
            if (a) {
                if (a.style.display != 'none') { 
                fselectrow(a.id);
                if (a.id == (mrowcount - 1)) {
                    document.getElementById('maintable').scrollIntoView(false);
                   // document.getElementById('maintable').scrollIntoView(false);
                } else {
                    if (!isScrolledIntoView(mselectedrow)) { mselectedrow.scrollIntoView(false) };
                }
            }
        }
            return false;
            break;
    }
}

function finitheader() {
    var tr = document.getElementById('columnheader');
    var tda = tr.getElementsByTagName('th');
    for (x = 0; x < tda.length; x++) {
        if (tda[x].id) {
            eval("tda[x].attachEvent('onclick',function() {fsortby('" + tda[x].id + "')});");
        }
       // tda[x].style.filter = "progid:DXImageTransform.Microsoft.Gradient(startColorstr='#" + maltbackgroundcolor    + "', endColorstr='#" + '000000'  + "', gradientType='0');"
    }
}



//----------------------------------------------------------------------------------------//fresetrow
function fresetrow(prowid) {
  
    var tr = document.getElementById(prowid);
    var hhh = tr.getElementsByTagName('td');
    switch (tr.className) {
        case 'htmltreegrid_group':
            tr.style.backgroundColor = mgroupbackgroundcolor;
            break;
        case 'htmltreegrid_groupfooter':
            tr.style.backgroundColor = fmediancolor(mdarkbackgroundcolor, maltbackgroundcolor);
            break;
        default:
            if (tr.getAttribute('data-id')) { //make sure it is a datarow
                if (tr.id % 2 == 1) {
                    tr.style.backgroundColor = maltbackgroundcolor;
//                    var x = 0;
//                     for (x = 0; x < hhh.length; x++) {
//                        if ((x > 1) && !(x == 2 && mgroupcount) && !(x == 3 && mselectionmode)) {
//                            hhh[x].style.filter = "progid:DXImageTransform.Microsoft.Gradient(endColorstr='#" + maltbackgroundcolor + "', startColorstr='#ffffff', gradientType='0');"
//                        };
//                    };
                } else {
                    tr.style.backgroundColor = mgraycolor;
//                    var x = 0;
//                    for (x = 0; x < hhh.length; x++) {
//                        if ((x > 1) && !((x == 2) && mgroupcount) && !((x == 3) && mselectionmode)) {
//                            hhh[x].style.filter = "progid:DXImageTransform.Microsoft.Gradient(endColorstr='#" + mgraycolor + "', startColorstr='#ffffff', gradientType='0');"
//                        };
//                    };

                }
            }
            hhh[0].style.backgroundColor = mgroupbackgroundcolor;

            hhh[1].style.backgroundColor = mgroupbackgroundcolor;
            if (mgroupcount) {
                hhh[2].style.backgroundColor = mgroupbackgroundcolor;
            };
            if (mselectionmode) {
                hhh[3].style.backgroundColor = mgroupbackgroundcolor;
            };
            break;
    } //switch


}//----------------------------------------------------------------------------------------//fresetrow


///////////////////////////////////////////////////////////////////////////////////////////////



function fshowchildren(prowid,pshow) {
    var tr = document.getElementById(prowid);
    //(pshow) ? tr.setAttribute('data-isexpanded',true) = 'block' : nx.style.display = 'none';
    if (typeof (pshow) == 'undefined') {
        pshow = !tr.getAttribute('data-isexpanded');
    }
    tr.setAttribute('data-isexpanded', pshow) //= 'block' : nx.style.display = 'none';
    for (x = parseInt(prowid) + 1; x < trs.length; x++) {
        var nx = document.getElementById(x);
        if (nx) {
            var lvl
            lvl = nx.getAttribute('data-level');
            if (lvl) {
                if (lvl <= tr.getAttribute('data-level')) {
                    break;
                }
            }
            //----------------------------same-------------------------------\
            var tda 
            var td
            tda = nx.getElementsByTagName('td');
            for (y = 0; y < tda.length; y++) {
                var td = tda[y];
                (pshow) ? td.style.borderStyle = 'solid' : td.style.borderStyle = 'none';
            }
            //----------
            tda = nx.getElementsByTagName('th');
            for (y = 0; y < tda.length; y++) {
                var td = tda[y];
                if (td.className == 'htmltreegrid_function') {
                    (pshow) ? td.style.borderStyle = 'solid' : td.style.borderStyle = 'none'; 
                }
            }
            //----------------------------same-------------------------------/
            (pshow) ? nx.style.display = 'block' : nx.style.display = 'none';
        } //if
    } //for
} //fcollapsechildren

function fcheckchildren(prowid, pshow) {
    var tr = document.getElementById(prowid);
    if (typeof (pshow) == 'undefined') {
        pshow = !tr.getAttribute('data-checked');
    }
    tr.setAttribute('data-checked', pshow) //= 'block' : nx.style.display = 'none';
    var lvl
    lvl = tr.getAttribute('data-level');

    if (lvl) { 
    for (x = parseInt(prowid) + 1; x < trs.length; x++) {
        var nx = document.getElementById(x);
        if (nx) {
            lvl = nx.getAttribute('data-level');
            if (lvl) {
                if (lvl <= tr.getAttribute('data-level')) {
                    break;
                }
            }
            var chk = document.getElementById('chk' + x);
            if (chk) {
                chk.setAttribute('checked', pshow);
            }

            
            nx.setAttribute('data-checked', pshow)
        } //if
    } //for
}
//alert(fcheckeddataid());
} //fcollapsechildren


function fcheckeddataid() {
    var ret = new Array;
    var ctr = 0;
    for (x = 0; x < trs.length; x++) {
        var tr = trs[x];
        var id = tr.getAttribute('data-id');
        if (id) { //make sure it is a datarow
            var chk = document.getElementById('chk' + x);
            if (chk) {
                if (chk.getAttribute('checked')) {
                    ret[ctr] = id;
                    ctr++;
                };
            };
        };
    };
    if (ret.length > 0) {
        return ret + ''; // +'' to convert from array to string! 
    } else {
    return '';
    }

    
}


//***********************************************************************************************************
/* Get the TOP position of a given element. */
function getPositionTop(element) {
    var offset = 0;
    while (element) {
        offset += element["offsetTop"];
        element = element.offsetParent;
    }
    return offset;
}

/* Is a given element is visible or not? */
function isElementVisible(eltId) {
    var elt = document.getElementById(eltId);
    if (!elt) {
        // Element not found. 
        return false;
    }
    // Get the top and bottom position of the given element. 
    var posTop = getPositionTop(elt);
    var posBottom = posTop + elt.offsetHeight;
    // Get the top and bottom position of the *visible* part of the window. 
    var visibleTop = document.body.scrollTop;
    var visibleBottom = visibleTop + document.documentElement.offsetHeight;
    return ((posBottom >= visibleTop) && (posTop <= visibleBottom));
}

//XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXx
function isScrolledIntoView(elem) {
    var docViewTop = document.body.scrollTop;
    var docViewBottom = docViewTop + document.body.clientHeight;

    var elemTop = getPositionTop(elem);
    var elemBottom = elemTop + elem.clientHeight;
//    var ret = ((elemBottom >= docViewTop) && (elemTop <= docViewBottom));
    var ret = ((elemTop >= docViewTop) && (elemBottom <= docViewBottom));

//    alert(docViewTop);
//    alert(docViewBottom);

//    alert(elemTop);
//    alert(elemBottom);
//    alert(ret);
   
    return ret;
}


//////////////////COLOR

function fmediancolor(c1, c2) {
   // alert(f_r(c1));
    //alert(f_r(c2));

    var r = (f_r(c1) + f_r(c2)) / 2;
    var g = (f_g(c1) + f_g(c2)) / 2;
    var b = (f_b(c1) + f_b(c2)) / 2;
    return RGBtoHex(r, g, b);
}

function f_r(h) { return parseInt((cutHex(h)).substring(0, 2), 16) }
function f_g(h) { return parseInt((cutHex(h)).substring(2, 4), 16) }
function f_b(h) { return parseInt((cutHex(h)).substring(4, 6), 16) }
function cutHex(h) { return (h.charAt(0) == "#") ? h.substring(1, 7) : h }

function RGBtoHex(R, G, B) { return toHex(R) + toHex(G) + toHex(B) }
function toHex(N) {
    if (N == null) return "00";
    N = parseInt(N); if (N == 0 || isNaN(N)) return "00";
    N = Math.max(0, N); N = Math.min(N, 255); N = Math.round(N);
    return "0123456789ABCDEF".charAt((N - N % 16) / 16)
      + "0123456789ABCDEF".charAt(N % 16);
}
function heightControl() {
	var iHeight = document.body.clientHeight;
	//alert(document.body.clientHeight);
	iHeight = iHeight - 12;
	myHeight = iHeight + "px";
	//var x =	document.getElementById('maintable').style.height = myHeight;
}
/////////////////// BILLY SCRIPT
function colorChange(thisID) {
	document.getElementById(thisID).style.backgroundColor = '#ffff00';
}
function colorChange2(thisID) {
	document.getElementById(thisID).style.backgroundColor = '';
}
function printDate(lastDay,dayOfWeek,schedDate) {
}
///////////////////////////////////////////////////////////////////////////////////////////////////////////
function fnewinfo(pdate) {
    window.external.NewInfo(pdate);
}
///////////////////////////////////////////////////////////////////////////////////////////////////////////
function finitcalendar(){
    var a = document.all;
    for (x = 0; x < a.length; x++){
        var o = a[x]
        switch (o.className) {
            case 'hcg_date':
                if (o.getAttribute('data-date')) {
                    o.style.backgroundColor = '#ffffff';
                    o.setAttribute('title', 'Click date to create new schedule.');
                    o.style.cursor = 'pointer';
                    eval("o.attachEvent('onclick',function(){fnewinfo('" + o.getAttribute('data-date') + "');});");
                } else {
                    o.style.backgroundColor = '#c0c0c0';
                }
                break;
            case 'hcg_cell':
                eval("o.attachEvent('onmouseover',function(){ colorChange('" + o.id + "');});");
                eval("o.attachEvent('onmouseout',function(){ colorChange2('" + o.id + "');});");
                break;
        }
    }
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////


/////////////////// BILLY SCRIPT


////////////////////COLOR
