"use strict";define(["app"],function(a){var b=function(c,l,k,e,o,m,f,n){l.gridOptions={};for(var h in k.columnDefinitions){if(k.columnDefinitions[h].length>0){l.gridOptions[h].columnDefs=k.columnDefinitions[h]}}for(var h in k.groups){if(k.groups[h].length>0){l.gridOptions[h].groups=k.groups[h]}}l.filter={12232:{},};l.treeViewOptions={};c("BaseListController",{$scope:l,resources:k});var j=$("<iframe />");j.attr("style","width:100%;height:850px;");j.attr("src","../ModulePage/Report.aspx?menuID="+12232+"&refID="+0+"&params="+JSON.stringify(l.rawData));j.attr("id","frame_12232");$(".grid").append(j)};a.register.controller("c12231",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});