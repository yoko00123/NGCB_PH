'use strict';define(['app'],function(app){var c11169=function($c,s,r,d,u,S,g,SS){s.gridOptions={11171:{data:'gridData[11171]',multiSelect:true,columnDefs:[{field:'$$',width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' ><span><a ui-sref=\'11171({ ID_11171:row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},{field:'ID',width:'*',displayName:'ID'},{field:'DateTimeCreated',width:'*',displayName:'Date Created',cellFilter:'date:\'MM/dd/yyyy hh:mm a\''},{field:'User',width:'*',displayName:'Created By'},{field:'ReasonForIncentives',width:'*',displayName:'Purpose',grouping:{seqNo:0,}},{field:'FilingStatus',width:'*',displayName:'Status'},{field:'EffectivityDate',width:'*',displayName:'Effectivity Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\'',grouping:{seqNo:0,}},],totalServerItems:r.totalServerItems[11171],enableColumnResizing: false,currentSortColumn:'ID',currentSortDirection:'ASC',enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,enableGrouping:true,hideGroupedValues:true,registerApi : function(events){s.gridEvents[11171] = events;s.gridEvents[11171].on.sortChange(s,function(opts){s.refreshGrid(11171, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[11171]);});s.gridEvents[11171].on.pageChange(s, function(opts){s.refreshGrid(11171, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[11171]);});},},};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={11171:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});};app.register.controller('c11169',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c11169]);});