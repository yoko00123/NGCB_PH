'use strict';define(['app'],function(app){var c7107=function($c,s,r,d,u,S,g,SS){s.gridOptions={7108:{data:'gridData[7108]',columnDefs:[{field:'$$',width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' ><span><a ui-sref=\'7108({ ID_7108:row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},{field:'SCComputation',width:'*',displayName:'Service Charge Computation'},{field:'ID',width:'*',displayName:'Reference ID'},{field:'LogFileFormat',width:'*',displayName:'Log File Format'},{field:'Address',width:'*',displayName:'Address',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Address | trustedHTML\'></span></div>'},{field:'Overview',width:'*',displayName:'Overview'},{field:'ProductsAndServices',width:'*',displayName:'ProductsAndServices'},{field:'Code',width:'*',displayName:'Code',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Code | trustedHTML\'></span></div>'},{field:'ZipCode',width:'*',displayName:'Zip Code',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.ZipCode | trustedHTML\'></span></div>'},{field:'Name',width:'*',displayName:'Name',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Name | trustedHTML\'></span></div>'},{field:'TelNo',width:'*',displayName:'Tel No.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.TelNo | trustedHTML\'></span></div>'},{field:'BusinessNature',width:'*',displayName:'Business'},{field:'CompanyGroup',width:'*',displayName:'Company Group'},{field:'CompanyType',width:'*',displayName:'Company Type'},{field:'SSSNo',width:'*',displayName:'SSS No.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.SSSNo | trustedHTML\'></span></div>'},{field:'PhilHealthNo',width:'*',displayName:'PhilHealth No.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.PhilHealthNo | trustedHTML\'></span></div>'},{field:'HDMFNo',width:'*',displayName:'HDMF No.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.HDMFNo | trustedHTML\'></span></div>'},{field:'TIN',width:'*',displayName:'TIN',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.TIN | trustedHTML\'></span></div>'},{field:'BranchCode',width:'*',displayName:'BIR Branch Code',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.BranchCode | trustedHTML\'></span></div>'},{field:'OrgManagement',width:'*',displayName:'Head'},{field:'VatRegNo',width:'*',displayName:'Vat Reg No.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.VatRegNo | trustedHTML\'></span></div>'},{field:'President',width:'*',displayName:'President',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.President | trustedHTML\'></span></div>'},{field:'VicePresident',width:'*',displayName:'Vice President',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.VicePresident | trustedHTML\'></span></div>'},{field:'Owner',width:'*',displayName:'Owner',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Owner | trustedHTML\'></span></div>'},{field:'ImageFile',width:'*',displayName:'Image',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.ImageFile | trustedHTML\'></span></div>'},{field:'Skins',width:'*',displayName:'Web Skin Color'},],totalServerItems:r.totalServerItems[7108],enableColumnResizing: false,currentSortColumn:'ID',currentSortDirection:'DESC',enableSorting:true,useExternalSorting:true,enablePagination:true,useExternalPagination:true,registerApi : function(events){s.gridEvents[7108] = events;s.gridEvents[7108].on.sortChange(s,function(opts){s.refreshGrid(7108, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[7108]);});s.gridEvents[7108].on.pageChange(s, function(opts){s.refreshGrid(7108, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[7108]);});},},};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={7108:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});};app.register.controller('c7107',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c7107]);});