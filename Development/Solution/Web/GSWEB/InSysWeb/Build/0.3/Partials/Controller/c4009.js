"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={4016:{data:"gridData[4016]",multiSelect:true,columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ng-if='row.entity.ID > 0'><span><a ui-sref='4016({ ID_4016:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"FileDate",width:"*",displayName:"Date Filed",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"Employee",width:"*",displayName:"Employee"},{field:"OBType",width:"*",displayName:"OB Type"},{field:"StartDate",width:"*",displayName:"Start Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"EndDate",width:"*",displayName:"End Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"Reason",width:"*",displayName:"Reason",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Reason | trustedHTML'></span></div>"},{field:"IsPosted",width:"*",displayName:"Posted",cellTemplate:"<div class='m-grid-cell-contents'><div class='smart-form noselect for_checkbox'><label class='toggle state-disabled'><input type='checkbox' name='IsPosted' class='form-checkbox' ng-model='row.entity.IsPosted' disabled/><i data-swchon-text='Yes' data-swchoff-text='No'></i></label></div></div>"},{field:"FilingStatus",width:"*",displayName:"Filing Status"},{field:"ApproverStatus",width:"*",displayName:"Approver Status"},{field:"ApprovalHistory",width:"*",displayName:"Approval History",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.ApprovalHistory | trustedHTML'></span></div>"},{field:"PreviousApproverComment",width:"*",displayName:"Previous Comment",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.PreviousApproverComment | trustedHTML'></span></div>"},{field:"ApproverComment",width:"*",displayName:"Approver Comment",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.ApproverComment | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[4016],enableColumnResizing:true,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[4016]=d;k.gridEvents[4016].on.sortChange(k,function(g){k.refreshGrid(4016,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[4016])});k.gridEvents[4016].on.pageChange(k,function(g){k.refreshGrid(4016,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[4016])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={4016:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c4009",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});