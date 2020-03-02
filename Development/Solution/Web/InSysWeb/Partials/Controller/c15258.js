'use strict';define(['app'],function(app){var c15258=function($c,s,r,d,u,S,g,SS){s.gridOptions={15265:{data:'gridData[15265]',multiSelect:true,columnDefs:[{field:'$$',width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' ><span><a ui-sref=\'15265({ ID_15265:row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},{field:'ID',width:'*',displayName:'ID'},{field:'FileDate',width:'*',displayName:'Date Filed',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'Employee',width:'*',displayName:'Employee'},{field:'LeavePayrollItem',width:'*',displayName:'Leave Item'},{field:'StartDate',width:'*',displayName:'Start Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'EndDate',width:'*',displayName:'End Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'Days',width:'*',displayName:'Days'},{field:'DaysWithPay',width:'*',displayName:'Days w/ Pay'},{field:'Reason',width:'*',displayName:'Reason',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Reason | trustedHTML\'></span></div>'},{field:'IsPosted',width:'*',displayName:'Posted',cellTemplate:'<div class=\'m-grid-cell-contents\'><div style=\'margin-top:-3px!important;\' class=\'smart-form noselect for_checkbox material-switch\'><input disabled id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'IsPosted\' class=\'form-checkbox\' ng-model=\'row.entity.IsPosted\' disabled/><label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'label-default\'></label></div></div>'},{field:'FilingStatus',width:'*',displayName:'Filing Status'},{field:'ApproverStatus',width:'*',displayName:'Approver Status'},{field:'ApprovalHistory',width:'*',displayName:'Approval History',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.ApprovalHistory | trustedHTML\'></span></div>'},{field:'PreviousApproverComment',width:'*',displayName:'Previous Comment',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.PreviousApproverComment | trustedHTML\'></span></div>'},{field:'ApproverComment',width:'*',displayName:'Approver Comment',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.ApproverComment | trustedHTML\'></span></div>'},],totalServerItems:r.totalServerItems[15265],enableColumnResizing: true,currentSortColumn:'ID',currentSortDirection:'DESC',enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi : function(events){s.gridEvents[15265] = events;s.gridEvents[15265].on.sortChange(s,function(opts){s.refreshGrid(15265, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[15265]);});s.gridEvents[15265].on.pageChange(s, function(opts){s.refreshGrid(15265, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[15265]);});},},};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={15265:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});};app.register.controller('c15258',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c15258]);});