"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={6067:{data:"gridData[6067]",columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='6067({ ID_6067:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"Reference ID"},{field:"Code",width:"*",displayName:"Code",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Code | trustedHTML'></span></div>"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[6067],enableColumnResizing:false,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[6067]=d;k.gridEvents[6067].on.sortChange(k,function(g){k.refreshGrid(6067,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[6067])});k.gridEvents[6067].on.pageChange(k,function(g){k.refreshGrid(6067,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[6067])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={6067:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c5058",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});