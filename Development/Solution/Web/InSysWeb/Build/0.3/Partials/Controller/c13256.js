"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={13257:{data:"gridData[13257]",multiSelect:true,columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='13257({ ID_13257:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"DateTimeCreated",width:"*",displayName:"Date Created",cellFilter:"date:'MM/dd/yyyy hh:mm a'"},{field:"User",width:"*",displayName:"Created By"},{field:"FilingStatus",width:"*",displayName:"Status"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[13257],enableColumnResizing:true,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[13257]=d;k.gridEvents[13257].on.sortChange(k,function(g){k.refreshGrid(13257,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[13257])});k.gridEvents[13257].on.pageChange(k,function(g){k.refreshGrid(13257,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[13257])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={13257:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c13256",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});