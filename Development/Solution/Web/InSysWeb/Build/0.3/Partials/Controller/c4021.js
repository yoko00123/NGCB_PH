"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=4021;j.rID=k.params.ID_4021;j.goPrevious=function(){k.go("4011",{},{reload:true,inherit:false,notify:true})};j.gridOptions={4022:{data:"Detail[4022]",enableSorting:true,columnDefs:[{field:"ID",width:"*",displayName:"ID"},{field:"Date",width:"*",displayName:"Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"HalfDay",width:"*",displayName:"Duration"},{field:"WithPay",width:"*",displayName:"With Pay",cellTemplate:"<div class='m-grid-cell-contents'><div style='margin-top:-3px!important;' class='smart-form noselect for_checkbox material-switch'><input disabled id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='WithPay' class='form-checkbox' ng-model='row.entity.WithPay' disabled/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-default'></label></div></div>"},{field:"ForTardy",width:"*",displayName:"For Tartdy",cellTemplate:"<div class='m-grid-cell-contents'><div style='margin-top:-3px!important;' class='smart-form noselect for_checkbox material-switch'><input disabled id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='ForTardy' class='form-checkbox' ng-model='row.entity.ForTardy' disabled/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-default'></label></div></div>"},{field:"ForUT",width:"*",displayName:"For UT",cellTemplate:"<div class='m-grid-cell-contents'><div style='margin-top:-3px!important;' class='smart-form noselect for_checkbox material-switch'><input disabled id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='ForUT' class='form-checkbox' ng-model='row.entity.ForUT' disabled/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-default'></label></div></div>"},{field:"Days",width:"*",displayName:"Days"},{field:"Comment",width:"*",displayName:"Comment",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Comment | trustedHTML'></span></div>"},],},5034:{data:"Detail[5034]",enableSorting:true,columnDefs:[{field:"ID",width:"*",displayName:"ID"},{field:"Name",width:"*",displayName:"File",cellTemplate:"<div class='m-grid-cell-contents'><span download-file='{{row.entity.Name_GUID}}' filename='{{row.entity.Name}}' style='cursor:pointer;text-decoration:underline' class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],},}};a.register.controller("c4021",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});