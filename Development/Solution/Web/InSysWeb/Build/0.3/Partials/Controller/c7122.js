"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={7134:{data:"gridData[7134]",columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='7134({ ID_7134:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"Code",width:"*",displayName:"Code",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Code | trustedHTML'></span></div>"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[7134],enableColumnResizing:false,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[7134]=d;k.gridEvents[7134].on.sortChange(k,function(g){k.refreshGrid(7134,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[7134])});k.gridEvents[7134].on.pageChange(k,function(g){k.refreshGrid(7134,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[7134])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={7134:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c7122",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});