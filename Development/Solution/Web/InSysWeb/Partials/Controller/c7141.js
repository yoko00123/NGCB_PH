'use strict';define(['app'],function(app){var c7141=function($c,s,r,d,u,S,g,SS){s.gridOptions={7142:{data:'gridData[7142]',columnDefs:[{field:'$$',width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' ><span><a ui-sref=\'7142({ ID_7142:row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},{field:'ID',width:'*',displayName:'Reference ID'},{field:'Company',width:'*',displayName:'Company'},{field:'LBound',width:'*',displayName:'Lower Bound'},{field:'UBound',width:'*',displayName:'Upper Bound'},{field:'Value',width:'*',displayName:'Value'},],totalServerItems:r.totalServerItems[7142],enableColumnResizing: false,currentSortColumn:'ID',currentSortDirection:'DESC',enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi : function(events){s.gridEvents[7142] = events;s.gridEvents[7142].on.sortChange(s,function(opts){s.refreshGrid(7142, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[7142]);});s.gridEvents[7142].on.pageChange(s, function(opts){s.refreshGrid(7142, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[7142]);});},},};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={7142:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});};app.register.controller('c7141',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c7141]);});