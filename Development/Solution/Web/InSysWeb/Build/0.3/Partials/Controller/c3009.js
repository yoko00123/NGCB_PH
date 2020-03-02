"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={3010:{data:"gridData[3010]",multiSelect:true,multiSelectIf:"row.entity.IsPosted == false",columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ><span><a ui-sref='3010({ ID_3010:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"FileDate",width:"*",displayName:"Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"WorkDate",width:"*",displayName:"Work Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"StartTime",width:"*",displayName:"Start Time",headerCellTemplateUrl:"mgrid/headerCellTemplateTime.html",cellFilter:"date:'hh:mm a'"},{field:"EndTime",width:"*",displayName:"End Time",headerCellTemplateUrl:"mgrid/headerCellTemplateTime.html",cellFilter:"date:'hh:mm a'"},{field:"ComputedHours",width:"*",displayName:"Computed Hours"},{field:"Reason",width:"*",displayName:"Reason",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Reason | trustedHTML'></span></div>"},{field:"IsPosted",width:"*",displayName:"Posted",cellTemplate:"<div class='m-grid-cell-contents'><div style='margin-top:-3px!important;' class='smart-form noselect for_checkbox material-switch'><input disabled id='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' type='checkbox' name='IsPosted' class='form-checkbox' ng-model='row.entity.IsPosted' disabled/><label for='someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}' class='label-default'></label></div></div>"},{field:"FilingStatus",width:"*",displayName:"Filing Status"},{field:"ApproverStatus",width:"*",displayName:"Approver Status"},{field:"ApprovalHistory",width:"*",displayName:"Approval History",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.ApprovalHistory | trustedHTML'></span></div>"},{field:"PreviousApproverComment",width:"*",displayName:"Previous Comment",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.PreviousApproverComment | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[3010],enableColumnResizing:true,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[3010]=d;k.gridEvents[3010].on.sortChange(k,function(g){k.refreshGrid(3010,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[3010])});k.gridEvents[3010].on.pageChange(k,function(g){k.refreshGrid(3010,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[3010])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={3010:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c3009",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});