"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={8119:{data:"gridData[8119]",columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='8119({ ID_8119:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"Code",width:"*",displayName:"Code",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Code | trustedHTML'></span></div>"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},{field:"Company",width:"*",displayName:"Company"},],totalServerItems:j.totalServerItems[8119],enableColumnResizing:true,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[8119]=d;k.gridEvents[8119].on.sortChange(k,function(g){k.refreshGrid(8119,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[8119])});k.gridEvents[8119].on.pageChange(k,function(g){k.refreshGrid(8119,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[8119])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={8119:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c8110",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});