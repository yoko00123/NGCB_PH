"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={11143:{data:"gridData[11143]",columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='11143({ ID_11143:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"Reference ID"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},{field:"Company",width:"*",displayName:"Company"},{field:"StartDate",width:"*",displayName:"Start Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"FilingStatus",width:"*",displayName:"Filing Status"},{field:"isDefault",width:"*",displayName:"Default Schedule",cellTemplate:"<div class='m-grid-cell-contents'><div style='margin-top:-3px!important;' class='smart-form noselect for_checkbox material-switch'><input disabled id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='isDefault' class='form-checkbox' ng-model='row.entity.isDefault' disabled/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-default'></label></div></div>"},{field:"IsExecuted",width:"*",displayName:"Posted",cellTemplate:"<div class='m-grid-cell-contents'><div style='margin-top:-3px!important;' class='smart-form noselect for_checkbox material-switch'><input disabled id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='IsExecuted' class='form-checkbox' ng-model='row.entity.IsExecuted' disabled/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-default'></label></div></div>"},{field:"DateExecuted",width:"*",displayName:"Date Posted",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"$delete",width:20,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='ngCellText'><span><a class='row-delete' ng-click='appScope.removeRow(11143,row,\"\")'  ><i class='fa fa-times'></i></a></span></div>"},],totalServerItems:j.totalServerItems[11143],enableColumnResizing:false,currentSortColumn:"ID",currentSortDirection:"DESC",registerApi:function(d){k.gridEvents[11143]=d;k.gridEvents[11143].on.sortChange(k,function(g){k.refreshGrid(11143,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[11143])});k.gridEvents[11143].on.pageChange(k,function(g){k.refreshGrid(11143,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[11143])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={11143:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c11105",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});