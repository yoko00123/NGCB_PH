"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=4027;j.rID=k.params.ID_4027;j.goPrevious=function(){k.go("4015",{},{reload:true,inherit:false,notify:true})};j.gridOptions={4028:{data:"Detail[4028]",enableSorting:true,enablePinning:true,enableColumnResizing:true,columnDefs:[{field:"EmployeeCode",width:"*",displayName:"Employee Code",pinned:true,cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.EmployeeCode | trustedHTML'></span></div>"},{field:"ID",width:"*",displayName:"ID",pinned:true},{field:"Employee",width:"*",displayName:"Employee",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Employee | trustedHTML'></span></div>"},{field:"Schedule1",width:"*",displayName:"Schedule 1",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Schedule1 | trustedHTML'></span></div>"},{field:"Schedule2",width:"*",displayName:"Schedule 2",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Schedule2 | trustedHTML'></span></div>"},{field:"Schedule3",width:"*",displayName:"Schedule 3",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Schedule3 | trustedHTML'></span></div>"},{field:"Schedule4",width:"*",displayName:"Schedule 4",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Schedule4 | trustedHTML'></span></div>"},{field:"Schedule5",width:"*",displayName:"Schedule 5",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Schedule5 | trustedHTML'></span></div>"},{field:"Schedule6",width:"*",displayName:"Schedule 6",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Schedule6 | trustedHTML'></span></div>"},{field:"Schedule7",width:"*",displayName:"Schedule 7",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Schedule7 | trustedHTML'></span></div>"},],},5038:{data:"Detail[5038]",enableSorting:true,columnDefs:[{field:"ID",width:"*",displayName:"ID"},{field:"Name",width:"*",displayName:"File",cellTemplate:"<div class='m-grid-cell-contents'><span download-file='{{row.entity.Name_GUID}}' filename='{{row.entity.Name}}' style='cursor:pointer;text-decoration:underline' class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],},}};a.register.controller("c4027",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});