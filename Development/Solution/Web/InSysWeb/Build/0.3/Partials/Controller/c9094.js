"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={9095:{data:"gridData[9095]",columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='9095({ ID_9095:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"Reference ID"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[9095],enableColumnResizing:false,currentSortColumn:"ID",currentSortDirection:"DESC",registerApi:function(d){k.gridEvents[9095]=d;k.gridEvents[9095].on.sortChange(k,function(g){k.refreshGrid(9095,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[9095])});k.gridEvents[9095].on.pageChange(k,function(g){k.refreshGrid(9095,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[9095])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={9095:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c9094",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});