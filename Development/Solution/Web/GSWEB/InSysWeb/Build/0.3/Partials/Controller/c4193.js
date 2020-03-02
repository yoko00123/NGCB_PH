"use strict";define(["app"],function(a){var b=function(c,l,k,e,p,m,h,o,n){var f="<div ng-if='showFooter' class='ngFooterPanel' ng-style='footerStyle()'><div class='ngTotalSelectContainer' ><div class='ngFooterTotalItems' ng-class=\"{'ngNoMultiSelect':!multiSelect}\"><span class='ngLabel'>View {{(totalServerItems==0?0:pagingOptions.pageSize=='All'?1:pagingOptions.pageSize*(pagingOptions.currentPage-1)+1)}}-{{(totalServerItems<(pagingOptions.pageSize*pagingOptions.currentPage)||pagingOptions.pageSize=='All'?totalServerItems:pagingOptions.pageSize*pagingOptions.currentPage)}} of {{totalServerItems}}</span><span ng-show='filterText.length>0' class='ngLabel'>({{i18n.ngShowingItemsLabel}} {{totalFilteredItemsLength()}})</span></div></div><div class='ngPagerContainer' style='float:right;margin-top:6px;' ng-if='enablePaging' ng-class=\"{'ngNoMultiSelect':!multiSelect}\"><div style='float:left;margin-right:10px;' class='ngRowCountPicker'><span style='float:left;margin-top:4.5px;' class='ngLabel'>{{i18n.ngPageSizeLabel}}&nbsp;</span><select style='float:left;height:25px;width:50px' ng-model='pagingOptions.pageSize'><option ng-repeat='size in pagingOptions.pageSizes'>{{size}}</option></select></div><div style='float:left;margin-right:10px;line-height:25px;' class='ngPagerControl' style='float:left;min-width:135px;'><button type='button' class='ngPagerButton' ng-click='pageToFirst()' ng-disabled='cantPageBackward()' title='{{i18n.ngPagerFirstTitle}}'><div class='ngPagerFirstTriangle'><div class='ngPagerFirstBar'></div></div></button><button type='button' class='ngPagerButton' ng-click='pageBackward()' ng-disabled='cantPageBackward()' title='{{i18n.ngPagerPrevTitle}}'><div class='ngPagerFirstTriangle ngPagerPrevTriangle'></div></button><button type='button' class='ngPagerButton' ng-click='pageForward()' ng-disabled='cantPageForward()' title='{{i18n.ngPagerNextTitle}}'><div class='ngPagerLastTriangle ngPagerNextTriangle'></div></button><button type='button' class='ngPagerButton' ng-click='pageToLast()' ng-disabled='cantPageToLast()' title='{{i18n.ngPagerLastTitle}}'><div class='ngPagerLastTriangle'><div class='ngPagerLastBar'></div></div></button></div></div></div>";l.pagingOptions={4194:{pageSizes:[10,20,50,100,"All"],pageSize:10,currentPage:1},};l.sortInfo={4194:{fields:["ID"],directions:["DESC"],firstload:true},};angular.forEach(l.pagingOptions,function(g,d){l.$watch("pagingOptions["+d+"].pageSize",function(i,q){if(i!==q){if(l.pagingOptions[d].currentPage==1||i!="All"){l.refreshGrid(d,i,1,l.sortInfo[d].fields[0],l.sortInfo[d].directions[0],l.filter[d])}else{l.pagingOptions[d].currentPage=1}}});l.$watch("pagingOptions["+d+"].currentPage",function(i,q){if(i!==q){l.refreshGrid(d,l.pagingOptions[d].pageSize,i,l.sortInfo[d].fields[0],l.sortInfo[d].directions[0],l.filter[d])}});l.$watch("sortInfo["+d+"]",function(i,q){if(q.firstload){l.sortInfo[d].firstload=false}if((i.fields[0]!==q.fields[0]||i.directions[0]!==q.directions[0])&&!q.firstload){l.refreshGrid(d,l.pagingOptions[d].pageSize,l.pagingOptions[d].currentPage,i.fields[0],i.directions[0],l.filter[d])}},true)});l.gridOptions={4194:{data:"gridData[4194]",virtualizationThreshold:50,selectedItems:[],checkboxCellTemplate:"<div class='ngSelectionCell'><input tabindex='-1' class='ngSelectionCheckbox' type='checkbox' ng-checked='row.selected ' /></div>",showSelectionCheckbox:true,checkboxHeaderTemplate:"<input class='ngSelectionHeader' type='checkbox' ng-show='multiSelect' ng-model='allSelected' ng-change=\"toggleSelectAll(allSelected,true,'')\"/>",columnDefs:[{field:"ID",width:30,displayName:"",cellTemplate:"<div class='ngCellText' ng-if='row.entity.ID > 0'><span><a ui-sref='4194({ ID_4194:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:"*",displayName:"ID"},{field:"Code",width:"*",displayName:"Code",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Code | trustedHTML'></span></div>"},{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},{field:"CanViewEmployeeSalary",width:"*",displayName:"Can View Employee Salary",cellTemplate:"<div class='ngCellText' ng-class='col.colIndex()'><div class='smart-form noselect for_checkbox'><label class='toggle state-disabled'><input type='checkbox' name='CanViewEmployeeSalary' class='form-checkbox' ng-model='row.entity.CanViewEmployeeSalary' disabled/><i data-swchon-text='Yes' data-swchoff-text='No'></i></label></div></div>"},{field:"IsActive",width:"*",displayName:"Active",cellTemplate:"<div class='ngCellText' ng-class='col.colIndex()'><div class='smart-form noselect for_checkbox'><label class='toggle state-disabled'><input type='checkbox' name='IsActive' class='form-checkbox' ng-model='row.entity.IsActive' disabled/><i data-swchon-text='Yes' data-swchoff-text='No'></i></label></div></div>"},],totalServerItems:k.totalServerItems[4194],groupsCollapsedByDefault:true,enableColumnResize:true,init:function(g,d){setTimeout(function(){d.gridOptions[4194].$gridServices.DomUtilityService.RebuildGrid(d.gridOptions[4194].$gridScope,d.gridOptions[4194].ngGrid)},500)}},};for(var j in k.columnDefinitions){if(k.columnDefinitions[j].length>0){l.gridOptions[j].columnDefs=k.columnDefinitions[j]}}for(var j in k.groups){if(k.groups[j].length>0){l.gridOptions[j].groups=k.groups[j]}}l.filter={4194:{},};l.treeViewOptions={};c("BaseListController",{$scope:l,resources:k})};a.register.controller("c4193",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","$templateCache","Session",b])});