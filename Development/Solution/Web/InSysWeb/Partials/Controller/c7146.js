'use strict';define(['app'],function(app){var c7146=function($c,s,r,d,u,S,g,SS){s.gridOptions={7147:{data:'gridData[7147]',columnDefs:[{field:'$$',width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' ><span><a ui-sref=\'7147({ ID_7147:row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},{field:'ID',width:'*',displayName:'Reference ID'},{field:'Name',width:'*',displayName:'Name',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Name | trustedHTML\'></span></div>'},{field:'IsForApproval',width:'*',displayName:'For Approval',cellTemplate:'<div class=\'m-grid-cell-contents\'><div style=\'margin-top:-3px!important;\' class=\'smart-form noselect for_checkbox material-switch\'><input disabled id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'IsForApproval\' class=\'form-checkbox\' ng-model=\'row.entity.IsForApproval\' disabled/><label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'label-default\'></label></div></div>'},],totalServerItems:r.totalServerItems[7147],enableColumnResizing: false,currentSortColumn:'ID',currentSortDirection:'DESC',enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi : function(events){s.gridEvents[7147] = events;s.gridEvents[7147].on.sortChange(s,function(opts){s.refreshGrid(7147, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[7147]);});s.gridEvents[7147].on.pageChange(s, function(opts){s.refreshGrid(7147, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[7147]);});},},};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={7147:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});};app.register.controller('c7146',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c7146]);});