"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=7151;j.rID=k.params.ID_7151;j.goPrevious=function(){k.go("7150",{},{reload:true,inherit:false,notify:true})};j.gridOptions={7152:{data:"Detail[7152]",enableSorting:true,columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ng-if='row.entity.ID > 0'><span><a ui-sref='7151.7152({ ID_7152:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"ID_LeavePayrollItem",width:"*",displayName:"Leave Payroll Item",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.ID_LeavePayrollItem.$invalid && appScope.mainform.$submitted }\" ><select name='ID_LeavePayrollItem' ng-options='item.ID as item.Name for item in appScope.dropdown_source[8427]' required  class='form-control' ng-model='row.entity.ID_LeavePayrollItem'><option value>- Select -</option></select>"},{field:"InitialValue",width:"*",displayName:"Initial Value",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.InitialValue.$invalid && appScope.mainform.$submitted }\" ><input type='text' name='InitialValue' placeholder='Initial Value'  required  class='form-control' ng-model='row.entity.InitialValue'/></div>"},{field:"ID_LeaveAccrualType",width:"*",displayName:"Leave Accrual Type",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.ID_LeaveAccrualType.$invalid && appScope.mainform.$submitted }\" ><select name='ID_LeaveAccrualType' ng-options='item.ID as item.Name for item in appScope.dropdown_source[8429]' required  class='form-control' ng-model='row.entity.ID_LeaveAccrualType'><option value>- Select -</option></select>"},{field:"ID_LeaveParameterItemReferenceDate",width:"*",displayName:"Reference Date",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.ID_LeaveParameterItemReferenceDate.$invalid && appScope.mainform.$submitted }\" ><select name='ID_LeaveParameterItemReferenceDate' ng-options='item.ID as item.Name for item in appScope.dropdown_source[8430]' required  class='form-control' ng-model='row.entity.ID_LeaveParameterItemReferenceDate'><option value>- Select -</option></select>"},{field:"ID_AccrualOption",width:"*",displayName:"Accrual Option",cellTemplate:"<div ng-form name='x' ><select name='ID_AccrualOption' ng-options='item.ID as item.Name for item in appScope.dropdown_source[8431]' class='form-control' ng-model='row.entity.ID_AccrualOption'><option value>- Select -</option></select>"},{field:"AccrualDay",width:"*",displayName:"Accrual Day",cellTemplate:"<div ng-form name='x' ><input type='text' name='AccrualDay' placeholder='Accrual Day'  class='form-control' ng-model='row.entity.AccrualDay'/></div>"},{field:"Comment",width:"*",displayName:"Comment",cellTemplate:"<div ng-form name='x' ><input type='text' name='Comment' placeholder='Comment'  class='form-control' ng-model='row.entity.Comment'/></div>"},{field:"$delete",width:20,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='ngCellText'><span><a class='row-delete' ng-click='appScope.removeRow(7152,row,\"Are you sure?\")'  ><i class='fa fa-times'></i></a></span></div>"},],},}};a.register.controller("c7151",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});