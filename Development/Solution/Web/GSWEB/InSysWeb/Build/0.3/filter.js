"use strict";define(["app"],function(a){a.filter("trustedHTML",["$sce",function(b){return function(d){var c=/<script>.*(<\/?script>?)?/gi;return b.trustAsHtml((d==null||d.length==0?null:d.replace("<script>","&lt;script&gt;").replace("</script>","&lt;/script&gt;")))}}]).filter("sum",function(){return function(b,d){if(typeof(b)==="undefined"||typeof(d)==="undefined"){return 0}var e=0;for(var c=0;c<b.length;c++){e=e+b[c][d]}return e}}).filter("filteredData",["$parse",function(b){return function(e,h){if(typeof(h)==="undefined"||h==null){return e}var c=[],g=h.replace("row.","");angular.forEach(e,function(d){if(b(g)(d)){c.push(d)}});return c}}]).filter("SortInGrouping",function(){return function(c,b){console.log(c);return c}}).filter("filterColumns",function(){return function(d,c){var b=c.split(",");var e=[];angular.forEach(d,function(f){if(b.indexOf(f.field)==-1){e.push(f)}});return e}}).filter("filterColumnsByVisible",function(){return function(d,c){var b=c.split(",");var e=[];angular.forEach(d,function(f){if(b.indexOf(f.field)==-1&&f.visible===true){e.push(f)}});return e}}).filter("dynamicFilter",["$interpolate",function(b){return function(d,c){if(c===undefined||c===null){return d}return b("{{value | "+arguments[1]+"}}")({value:arguments[0]})}}]).filter("daterange",function(){return function(c,b,f,d){var e=[];var f=(f&&!isNaN(Date.parse(f)))?Date.parse(f):0;var d=(d&&!isNaN(Date.parse(d)))?Date.parse(d):0;if(c.length>0){$.each(c,function(i,h){var k=new Date(h[b]).getMonth()+1;var j=new Date(h[b]).getDate();var l=new Date(h[b]).getFullYear();var g=Date.parse(new Date(k+"/"+j+"/"+l));if((g>=f&&g<=d)||f==0){e.push(h)}});return e}}})});