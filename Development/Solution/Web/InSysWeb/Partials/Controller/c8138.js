'use strict';define(['app'],function(app){var c8138=function($c,s,r,d,u,S,g,SS){s.gridOptions={8139:{data:'gridData[8139]',columnDefs:[{field:'$$',width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' ><span><a ui-sref=\'8139({ ID_8139:row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},{field:'ID',width:'*',displayName:'ID'},{field:'Code',width:'*',displayName:'Code',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Code | trustedHTML\'></span></div>'},{field:'Name',width:'*',displayName:'Name',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Name | trustedHTML\'></span></div>'},{field:'Company',width:'*',displayName:'Company Profile'},],totalServerItems:r.totalServerItems[8139],enableColumnResizing: true,currentSortColumn:'ID',currentSortDirection:'DESC',enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi : function(events){s.gridEvents[8139] = events;s.gridEvents[8139].on.sortChange(s,function(opts){s.refreshGrid(8139, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[8139]);});s.gridEvents[8139].on.pageChange(s, function(opts){s.refreshGrid(8139, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[8139]);});},},};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={8139:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});};app.register.controller('c8138',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c8138]);});