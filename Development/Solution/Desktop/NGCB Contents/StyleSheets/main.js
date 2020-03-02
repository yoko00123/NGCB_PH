function fselect(sExpID,sRow,scolor)
				{
					var divname;
					divname = eval('id' + sExpID + '.style');
					
				//	 var decColor = red + 256 * green + 65536 * blue;
   			//		divname.backgroundColor = decColor.toString(16); 
   					divname.backgroundColor = scolor   ; 
					/*
switch (sType)	
{
case '0' :
	divname.backgroundColor = 'White';
	break;
case '1' :
	divname.backgroundColor = 'LightGreen';
	break;
case '2' :
	divname.backgroundColor = 'orange';
	location.href= '#a';
	break;
default :
}
			*/
				
				}




				function SelectRow(sExpID)
				{
					var divname;
					divname = eval('Col' + sExpID + '.style');
					divname.Color = 'Blue'    
					
				
					
				}
