"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={11190:{data:"gridData[11190]",multiSelect:true,columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='11190({ ID_11190:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"DateTimeCreated",width:"*",displayName:"Date Created",cellFilter:"date:'MM/dd/yyyy hh:mm a'"},{field:"User",width:"*",displayName:"Created By"},{field:"FilingStatus",width:"*",displayName:"Filing Status"},{field:"TypeOfSalary",width:"*",displayName:"Type of Salary / Wage Adjustment"},{field:"Amount",width:"*",displayName:"Amount / Percentage"},{field:"EffectivityDate",width:"*",displayName:"Effectivity Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"Reason",width:"*",displayName:"Reason for Adjustment",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Reason | trustedHTML'></span></div>"},{field:"$delete",width:20,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='ngCellText'><span><a class='row-delete' ng-click='appScope.removeRow(11190,row,\"\")'  ><i class='fa fa-times'></i></a></span></div>"},],totalServerItems:j.totalServerItems[11190],enableColumnResizing:false,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[11190]=d;k.gridEvents[11190].on.sortChange(k,function(g){k.refreshGrid(11190,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[11190])});k.gridEvents[11190].on.pageChange(k,function(g){k.refreshGrid(11190,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[11190])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={11190:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c11189",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});