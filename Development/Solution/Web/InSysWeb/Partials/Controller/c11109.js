'use strict';define(['app'],function(app){var c11109=function($c,s,r,d,u,S,g,SS){s.gridOptions={13237:{data:'gridData[13237]',multiSelect:true,columnDefs:[{field:'$$',width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' ><span><a ui-sref=\'13237({ ID_13237:row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},{field:'ID',width:'*',displayName:'Reference ID'},{field:'Code',width:'*',displayName:'Code',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Code | trustedHTML\'></span></div>'},{field:'Name',width:'*',displayName:'Name',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Name | trustedHTML\'></span></div>'},{field:'FilingStatus',width:'*',displayName:'Filing Status'},{field:'IsExecuted',width:'*',displayName:'Executed',cellTemplate:'<div class=\'m-grid-cell-contents\'><div style=\'margin-top:-3px!important;\' class=\'smart-form noselect for_checkbox material-switch\'><input disabled id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'IsExecuted\' class=\'form-checkbox\' ng-model=\'row.entity.IsExecuted\' disabled/><label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'label-default\'></label></div></div>'},{field:'DateExecuted',width:'*',displayName:'Date Executed',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},],totalServerItems:r.totalServerItems[13237],enableColumnResizing: true,currentSortColumn:'ID',currentSortDirection:'ASC',enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi : function(events){s.gridEvents[13237] = events;s.gridEvents[13237].on.sortChange(s,function(opts){s.refreshGrid(13237, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[13237]);});s.gridEvents[13237].on.pageChange(s, function(opts){s.refreshGrid(13237, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[13237]);});},},};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={13237:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});};app.register.controller('c11109',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c11109]);});