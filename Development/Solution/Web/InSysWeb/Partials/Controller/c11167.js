'use strict';define(['app'],function(app){var c11167=function($c,s,r,d,u,S,g,SS){s.gridOptions={11168:{data:'gridData[11168]',multiSelect:true,columnDefs:[{field:'$$',width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' ><span><a ui-sref=\'11168({ ID_11168:row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},{field:'ID',width:'*',displayName:'ID'},{field:'DateTimeCreated',width:'*',displayName:'Date Created',cellFilter:'date:\'MM/dd/yyyy hh:mm a\''},{field:'User',width:'*',displayName:'Created By'},{field:'Name',width:'*',displayName:'Name',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Name | trustedHTML\'></span></div>'},],totalServerItems:r.totalServerItems[11168],enableColumnResizing: false,currentSortColumn:'ID',currentSortDirection:'DESC',enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi : function(events){s.gridEvents[11168] = events;s.gridEvents[11168].on.sortChange(s,function(opts){s.refreshGrid(11168, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[11168]);});s.gridEvents[11168].on.pageChange(s, function(opts){s.refreshGrid(11168, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[11168]);});},},};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={11168:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});};app.register.controller('c11167',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c11167]);});