"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={6101:{data:"gridData[6101]",columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='6101({ ID_6101:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},{field:"WebMenuApproval",width:"*",displayName:"Approval Menu"},{field:"WebMenuFiling",width:"*",displayName:"Filing Menu"},{field:"WebMenuPending",width:"*",displayName:"Pending App Menu"},],totalServerItems:j.totalServerItems[6101],enableColumnResizing:false,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[6101]=d;k.gridEvents[6101].on.sortChange(k,function(g){k.refreshGrid(6101,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[6101])});k.gridEvents[6101].on.pageChange(k,function(g){k.refreshGrid(6101,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[6101])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={6101:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c6094",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});