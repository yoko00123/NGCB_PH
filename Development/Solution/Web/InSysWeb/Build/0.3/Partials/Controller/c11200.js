"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={11201:{data:"gridData[11201]",multiSelect:true,columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='11201({ ID_11201:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"DateTimeCreated",width:"*",displayName:"Date CreatedDate Created",cellFilter:"date:'MM/dd/yyyy hh:mm a'"},{field:"User",width:"*",displayName:"Created By"},{field:"Employee",width:"*",displayName:"Employee"},{field:"FilingStatus",width:"*",displayName:"Status"},{field:"MedicalLifeInsurancePurpose",width:"*",displayName:"Purpose"},{field:"MembershipPlan",width:"*",displayName:"Principal Membership Plan"},{field:"ModeOfPayment",width:"*",displayName:"Mode Of Payment"},],totalServerItems:j.totalServerItems[11201],enableColumnResizing:false,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[11201]=d;k.gridEvents[11201].on.sortChange(k,function(g){k.refreshGrid(11201,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[11201])});k.gridEvents[11201].on.pageChange(k,function(g){k.refreshGrid(11201,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[11201])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={11201:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c11200",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});