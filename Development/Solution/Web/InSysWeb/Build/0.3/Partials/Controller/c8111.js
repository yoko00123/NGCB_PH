"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={8120:{data:"gridData[8120]",columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='8120({ ID_8120:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"Code",width:"*",displayName:"Code",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Code | trustedHTML'></span></div>"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[8120],enableColumnResizing:true,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[8120]=d;k.gridEvents[8120].on.sortChange(k,function(g){k.refreshGrid(8120,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[8120])});k.gridEvents[8120].on.pageChange(k,function(g){k.refreshGrid(8120,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[8120])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={8120:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c8111",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});