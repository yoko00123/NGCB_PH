"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=9099;j.rID=k.params.ID_9099;j.goPrevious=function(){k.go("9098",{},{reload:true,inherit:false,notify:true})};j.gridOptions={9100:{data:"Detail[9100]",enableSorting:true,columnDefs:[{field:"ID",width:"*",displayName:"ID"},{field:"ID_PayrollItem",width:"*",displayName:"PayrollItem",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.ID_PayrollItem.$invalid && appScope.mainform.$submitted }\" ><select name='ID_PayrollItem' ng-options='item.ID as item.Name for item in appScope.dropdown_source[11247]' required  class='form-control' ng-model='row.entity.ID_PayrollItem'><option value>- Select -</option></select>"},{field:"Rate",width:"*",displayName:"Rate",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.Rate.$invalid && appScope.mainform.$submitted }\" ><input type='text' name='Rate' placeholder='Rate'  required  ng-numeric-only class='form-control' ng-model='row.entity.Rate'/></div>"},{field:"Amt",width:"*",displayName:"Amt",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.Amt.$invalid && appScope.mainform.$submitted }\" ><input type='text' name='Amt' placeholder='Amt'  required  ng-numeric-only class='form-control' ng-model='row.entity.Amt'/></div>"},{field:"$delete",width:20,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='ngCellText'><span><a class='row-delete' ng-click='appScope.removeRow(9100,row,\"\")'  ><i class='fa fa-times'></i></a></span></div>"},],},9102:{data:"Detail[9102]",enableSorting:true,columnDefs:[{field:"ID",width:"*",displayName:"ID"},{field:"ID_PayrollItem",width:"*",displayName:"Payroll Item",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.ID_PayrollItem.$invalid && appScope.mainform.$submitted }\" ><select name='ID_PayrollItem' ng-options='item.ID as item.Name for item in appScope.dropdown_source[11252]' required  class='form-control' ng-model='row.entity.ID_PayrollItem'><option value>- Select -</option></select>"},{field:"Amt",width:"*",displayName:"Amt",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.Amt.$invalid && appScope.mainform.$submitted }\" ><input type='text' name='Amt' placeholder='Amt'  required  ng-numeric-only class='form-control' ng-model='row.entity.Amt'/></div>"},{field:"ID_PayrollItemSetupOption",width:"*",displayName:"PayrollItemSetupOption",cellTemplate:"<div ng-form name='x' ><select name='ID_PayrollItemSetupOption' ng-options='item.ID as item.Name for item in appScope.dropdown_source[11254]' class='form-control' ng-model='row.entity.ID_PayrollItemSetupOption'><option value>- Select -</option></select>"},{field:"Period1",width:"*",displayName:"Period1",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='Period1' class='form-checkbox' ng-model='row.entity.Period1'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"Period2",width:"*",displayName:"Period2",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='Period2' class='form-checkbox' ng-model='row.entity.Period2'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"Period3",width:"*",displayName:"Period3",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='Period3' class='form-checkbox' ng-model='row.entity.Period3'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"Period4",width:"*",displayName:"Period4",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='Period4' class='form-checkbox' ng-model='row.entity.Period4'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"Period5",width:"*",displayName:"Period5",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='Period5' class='form-checkbox' ng-model='row.entity.Period5'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"Period6",width:"*",displayName:"Period6",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='Period6' class='form-checkbox' ng-model='row.entity.Period6'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"Period7",width:"*",displayName:"Period7",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='Period7' class='form-checkbox' ng-model='row.entity.Period7'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"Period8",width:"*",displayName:"Period8",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='Period8' class='form-checkbox' ng-model='row.entity.Period8'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"Period9",width:"*",displayName:"Period9",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='Period9' class='form-checkbox' ng-model='row.entity.Period9'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"$delete",width:20,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='ngCellText'><span><a class='row-delete' ng-click='appScope.removeRow(9102,row,\"\")'  ><i class='fa fa-times'></i></a></span></div>"},],},9103:{data:"Detail[9103]",enableSorting:true,columnDefs:[{field:"ID",width:"*",displayName:"ID"},{field:"ID_PayrollItem",width:"*",displayName:"Payroll Item",cellTemplate:"<div class='smart-form noselect material-switch'><input id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='ID_PayrollItem' class='form-checkbox' ng-model='row.entity.ID_PayrollItem'/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-primary'></label></div>"},{field:"Rate",width:"*",displayName:"Rate",cellTemplate:"<div ng-form name='x' ><input type='text' name='Rate' placeholder='Rate'  ng-numeric-only class='form-control' ng-model='row.entity.Rate'/></div>"},{field:"$delete",width:20,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='ngCellText'><span><a class='row-delete' ng-click='appScope.removeRow(9103,row,\"\")'  ><i class='fa fa-times'></i></a></span></div>"},],},}};a.register.controller("c9099",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});