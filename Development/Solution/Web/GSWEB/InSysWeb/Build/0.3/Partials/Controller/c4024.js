"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=4024;j.rID=k.params.ID_4024;j.goPrevious=function(){k.go("4013",{},{reload:true,inherit:false,notify:true})};j.gridOptions={4025:{data:"Detail[4025]",enableSorting:true,columnDefs:[{field:"ID",width:"*",displayName:"ID"},{field:"SchedDate",width:"*",displayName:"Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"OldSched",width:"*",displayName:"Old Schedule",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.OldSched | trustedHTML'></span></div>"},{field:"NewSched",width:"*",displayName:"New Schedule"},{field:"ForRDSD",width:"*",displayName:"RD/SD"},{field:"Comment",width:"*",displayName:"Comment",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Comment | trustedHTML'></span></div>"},{field:"$delete",width:20,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='ngCellText'><span><a class='row-delete' ng-click='appScope.removeRow(4025,row)'  ng-show='Master.IsPosted == false'  ><i class='fa fa-times'></i></a></span></div>"},],},5036:{data:"Detail[5036]",enableSorting:true,columnDefs:[{field:"ID",width:"*",displayName:"ID"},{field:"Name",width:"*",displayName:"File",cellTemplate:"<div class='m-grid-cell-contents'><span download-file='{{row.entity.Name_GUID}}' filename='{{row.entity.Name}}' style='cursor:pointer;text-decoration:underline' class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],},}};a.register.controller("c4024",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});