"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=7094;j.rID=k.params.ID_7094;j.goPrevious=function(){k.go("7092",{},{reload:true,inherit:false,notify:true})};j.gridOptions={7095:{data:"Detail[7095]",enableSorting:true,columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ng-if='row.entity.ID > 0'><span><a ui-sref='7094.7095({ ID_7095:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"Code",width:"*",displayName:"Code",cellTemplate:"<div ng-form name='x' ><input type='text' name='Code' placeholder='Code'  class='form-control' ng-model='row.entity.Code'/></div>"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.Name.$invalid && appScope.mainform.$submitted }\" ><input type='text' name='Name' placeholder='Name'  required  class='form-control' ng-model='row.entity.Name'/></div>"},{field:"ID_Month",width:"*",displayName:"Month",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.ID_Month.$invalid && appScope.mainform.$submitted }\" ><select name='ID_Month' ng-options='item.ID as item.Name for item in appScope.dropdown_source[8237]' required  class='form-control' ng-model='row.entity.ID_Month'><option value>- Select -</option></select>"},{field:"ID_Date",width:"*",displayName:"Date",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.ID_Date.$invalid && appScope.mainform.$submitted }\" ><select name='ID_Date' ng-options='item.ID as item.Name for item in appScope.dropdown_source[8238]' required  class='form-control' ng-model='row.entity.ID_Date'><option value>- Select -</option></select>"},{field:"ID_HolidayType",width:"*",displayName:"Holiday Type",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.ID_HolidayType.$invalid && appScope.mainform.$submitted }\" ><select name='ID_HolidayType' ng-options='item.ID as item.Name for item in appScope.dropdown_source[8239]' required  class='form-control' ng-model='row.entity.ID_HolidayType'><option value>- Select -</option></select>"},{field:"ID_SuspensionType",width:"*",displayName:"Suspension Type",cellTemplate:"<div ng-form name='x' ><select name='ID_SuspensionType' ng-options='item.ID as item.Name for item in appScope.dropdown_source[8240]' class='form-control' ng-model='row.entity.ID_SuspensionType'><option value>- Select -</option></select>"},{field:"Date",width:"*",displayName:"Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"ID_Area",width:"*",displayName:"Area",cellTemplate:"<div ng-form name='x' ><select name='ID_Area' ng-options='item.ID as item.Name for item in appScope.dropdown_source[8242]' class='form-control' ng-model='row.entity.ID_Area'><option value>- Select -</option></select>"},{field:"IsWorking",width:"*",displayName:"Working",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='IsWorking' class='form-checkbox' ng-model='row.entity.IsWorking'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"Comment",width:"*",displayName:"Comment",cellTemplate:"<div ng-form name='x' ><input type='text' name='Comment' placeholder='Comment'  class='form-control' ng-model='row.entity.Comment'/></div>"},{field:"$delete",width:20,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='ngCellText'><span><a class='row-delete' ng-click='appScope.removeRow(7095,row,\"Are you sure?\")'  ng-show=''  ><i class='fa fa-times'></i></a></span></div>"},],},}};a.register.controller("c7094",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});