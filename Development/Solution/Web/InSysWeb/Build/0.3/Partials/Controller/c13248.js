"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={13252:{data:"gridData[13252]",multiSelect:true,columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='13252({ ID_13252:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"DateTimeCreated",width:"*",displayName:"Date Created",cellFilter:"date:'MM/dd/yyyy hh:mm a'"},{field:"User",width:"*",displayName:"Created By"},{field:"FilingStatus",width:"*",displayName:"Status"},{field:"Designation",width:"*",displayName:"Position"},{field:"EffectivityDate",width:"*",displayName:"Effectivity Date"},],totalServerItems:j.totalServerItems[13252],enableColumnResizing:true,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[13252]=d;k.gridEvents[13252].on.sortChange(k,function(g){k.refreshGrid(13252,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[13252])});k.gridEvents[13252].on.pageChange(k,function(g){k.refreshGrid(13252,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[13252])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={13252:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c13248",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});