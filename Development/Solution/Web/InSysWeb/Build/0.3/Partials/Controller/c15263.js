"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=15263;j.rID=k.params.ID_15263;j.goPrevious=function(){k.go("15256",{},{reload:true,inherit:false,notify:true})};j.gridOptions={15269:{data:"Detail[15269]",enableSorting:true,columnDefs:[{field:"ID",width:"*",displayName:"ID"},{field:"Date",width:"*",displayName:"Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"StartTime",width:"*",displayName:"Start Time",headerCellTemplateUrl:"mgrid/headerCellTemplateTime.html",cellFilter:"date:'hh:mm a'"},{field:"EndTime",width:"*",displayName:"End Time",headerCellTemplateUrl:"mgrid/headerCellTemplateTime.html",cellFilter:"date:'hh:mm a'"},{field:"Comment",width:"*",displayName:"Comment",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Comment | trustedHTML'></span></div>"},],},}};a.register.controller("c15263",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});