'use strict';define(['app'],function(app){var c11173=function($c,s,r,d,u,S,g,SS){s.gridOptions={11174:{data:'gridData[11174]',multiSelect:true,columnDefs:[{field:'$$',width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' ><span><a ui-sref=\'11174({ ID_11174:row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},{field:'ID',width:'*',displayName:'ID'},{field:'DateTimeCreated',width:'*',displayName:'Date Created',cellFilter:'date:\'MM/dd/yyyy hh:mm a\''},{field:'User',width:'*',displayName:'Created By'},{field:'Name',width:'*',displayName:'Membership Plan',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Name | trustedHTML\'></span></div>'},{field:'AnnualPlanRate',width:'*',displayName:'Annual Plan Rate'},{field:'SemiAnnualPlanRate',width:'*',displayName:'Semi-Annual Plan Rate'},{field:'QuarterlyPlanRate',width:'*',displayName:'Quarterly Plan Rate'},{field:'MonthlyPlanRate',width:'*',displayName:'Monthly Plan Rate'},],totalServerItems:r.totalServerItems[11174],enableColumnResizing: false,currentSortColumn:'ID',currentSortDirection:'ASC',enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi : function(events){s.gridEvents[11174] = events;s.gridEvents[11174].on.sortChange(s,function(opts){s.refreshGrid(11174, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[11174]);});s.gridEvents[11174].on.pageChange(s, function(opts){s.refreshGrid(11174, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[11174]);});},},};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={11174:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});};app.register.controller('c11173',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c11173]);});