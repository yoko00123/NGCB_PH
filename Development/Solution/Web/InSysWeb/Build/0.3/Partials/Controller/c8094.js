"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={8095:{data:"gridData[8095]",multiSelect:true,columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='8095({ ID_8095:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[8095],enableColumnResizing:true,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[8095]=d;k.gridEvents[8095].on.sortChange(k,function(g){k.refreshGrid(8095,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[8095])});k.gridEvents[8095].on.pageChange(k,function(g){k.refreshGrid(8095,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[8095])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={8095:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c8094",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});