"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=13262;j.rID=k.params.ID_13262;j.goPrevious=function(){k.go("13259",{},{reload:true,inherit:false,notify:true})};j.$watch("Master.ID_Department",function(d,n){if(d!==n){if(d==undefined||d==null){j.Master.Department=""}else{var g=j.lookup_source[16899].filter(function(o){return o.ID==d});j.Master.Department=g[0]["Name"]}}});j.gridOptions={14240:{data:"Detail[14240]",enableSorting:true,columnDefs:[{field:"ID_ApprovalLevel",width:"*",displayName:"Approval Level",cellTemplate:"<div ng-form name='x' ><select name='ID_ApprovalLevel' ng-options='item.ID as item.Name for item in appScope.dropdown_source[17836]' class='form-control' ng-model='row.entity.ID_ApprovalLevel'><option value>- Select -</option></select>"},{field:"ID_EmployeeApprover1",width:"*",displayName:"Approver 1",cellTemplate:"<div ng-form name='x' ><select name='ID_EmployeeApprover1' ng-options='item.ID as item.Name for item in appScope.dropdown_source[17837]' class='form-control' ng-model='row.entity.ID_EmployeeApprover1'><option value>- Select -</option></select>"},{field:"ID_EmployeeApprover2",width:"*",displayName:"Approver 2",cellTemplate:"<div ng-form name='x' ><select name='ID_EmployeeApprover2' ng-options='item.ID as item.Name for item in appScope.dropdown_source[17838]' class='form-control' ng-model='row.entity.ID_EmployeeApprover2'><option value>- Select -</option></select>"},{field:"ID_EmployeeApprover3",width:"*",displayName:"Approver 3",cellTemplate:"<div ng-form name='x' ><select name='ID_EmployeeApprover3' ng-options='item.ID as item.Name for item in appScope.dropdown_source[17839]' class='form-control' ng-model='row.entity.ID_EmployeeApprover3'><option value>- Select -</option></select>"},{field:"IsPowerApprover",width:"*",displayName:"Power Approver",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='IsPowerApprover' class='form-checkbox' ng-model='row.entity.IsPowerApprover'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"$delete",width:20,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='ngCellText'><span><a class='row-delete' ng-click='appScope.removeRow(14240,row,\"\")'  ><i class='fa fa-times'></i></a></span></div>"},],},14241:{data:"Detail[14241]",enableSorting:true,columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ng-if='row.entity.ID > 0'><span><a ui-sref='13262.14241({ ID_14241:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"ID_Employee",width:"*",displayName:"Employee",cellTemplate:"<div ng-form name='x' ><select name='ID_Employee' ng-options='item.ID as item.Name for item in appScope.dropdown_source[17844]' class='form-control' ng-model='row.entity.ID_Employee'><option value>- Select -</option></select>"},],},}};a.register.controller("c13262",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});