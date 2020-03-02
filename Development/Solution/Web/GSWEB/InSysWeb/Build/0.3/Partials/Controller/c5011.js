"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={5012:{data:"gridData[5012]",columnDefs:[{field:"$$",width:30,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='m-grid-cell-contents' ng-if='row.entity.ID > 0'><span><a ui-sref='5012({ ID_5012:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"Code",width:"*",displayName:"Employee Code",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Code | trustedHTML'></span></div>"},{field:"StartDate",width:"*",displayName:"StartDate",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},{field:"Branch",width:"*",displayName:"Branch",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Branch | trustedHTML'></span></div>"},{field:"Persona",width:"*",displayName:"Persona",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Persona | trustedHTML'></span></div>"},{field:"Company",width:"*",displayName:"Company",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Company | trustedHTML'></span></div>"},{field:"Department",width:"*",displayName:"Department",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Department | trustedHTML'></span></div>"},{field:"Designation",width:"*",displayName:"Designation",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Designation | trustedHTML'></span></div>"},{field:"EmployeeStatus",width:"*",displayName:"Employee Status",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.EmployeeStatus | trustedHTML'></span></div>"},{field:"PayrollScheme",width:"*",displayName:"Payroll Scheme",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.PayrollScheme | trustedHTML'></span></div>"},{field:"PayrollFrequency",width:"*",displayName:"Payroll Frequency",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.PayrollFrequency | trustedHTML'></span></div>"},{field:"TaxExemption",width:"*",displayName:"Tax Exemption",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.TaxExemption | trustedHTML'></span></div>"},{field:"PaymentMode",width:"*",displayName:"Payment Mode",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.PaymentMode | trustedHTML'></span></div>"},{field:"Parameter",width:"*",displayName:"Parameter",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Parameter | trustedHTML'></span></div>"},{field:"MonthlyRate",width:"*",displayName:"Monthly Rate"},{field:"DailyRate",width:"*",displayName:"Daily Rate"},{field:"HourlyRate",width:"*",displayName:"Hourly Rate"},{field:"Supervisor",width:"*",displayName:"Supervisor",cellTemplate:"<div class='m-grid-cell-contents'><span class='control-label' ng-bind-html='row.entity.Supervisor | trustedHTML'></span></div>"},{field:"HiredDate",width:"*",displayName:"Hired Date",headerCellTemplateUrl:"mgrid/headerCellTemplateDate.html",cellFilter:"date:'MM/dd/yyyy'"},],totalServerItems:j.totalServerItems[5012],enableColumnResizing:false,currentSortColumn:"ID",currentSortDirection:"DESC",enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi:function(d){k.gridEvents[5012]=d;k.gridEvents[5012].on.sortChange(k,function(g){k.refreshGrid(5012,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[5012])});k.gridEvents[5012].on.pageChange(k,function(g){k.refreshGrid(5012,g.currentPageSize,g.currentPage,g.currentSortColumn,g.currentSortDirection,k.filter[5012])})},},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={5012:{},};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c5011",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});