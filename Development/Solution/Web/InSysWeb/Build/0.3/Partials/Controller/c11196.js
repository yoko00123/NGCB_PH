"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={11197:{data:"gridData[11197]",multiSelect:true,columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='11197({ ID_11197:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"DateTimeCreated",width:"*",displayName:"DateTimeCreated",cellFilter:"date:'MM/dd/yyyy hh:mm a'"},{field:"User",width:"*",displayName:"ID_User"},{field:"Employee",width:"*",displayName:"ID_Employee",grouping:{seqNo:0,}},{field:"FilingStatus",width:"*",displayName:"ID_FilingStatus"},{field:"ReasonForIncentives",width:"*",displayName:"ID_ReasonForIncentives",grouping:{seqNo:0,}},{field:"TotalRating",width:"*",displayName:"TotalRating"},{field:"Incentive",width:"*",displayName:"Incentive",grouping:{seqNo:0,}},],totalServerItems:j.totalServerItems[11197],enableColumnResizing:false,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,enableGrouping:true,hideGroupedValues:true,registerApi:function(d){k.gridEvents[11197]=d;k.gridEvents[11197].on.sortChange(k,function(g){k.refreshGrid(11197,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[11197])});k.gridEvents[11197].on.pageChange(k,function(g){k.refreshGrid(11197,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[11197])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={11197:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c11196",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});