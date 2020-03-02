"use strict";define(["app"],function(a){var b=function(l,j,c,p,m,f,o,n){if(j.error!==undefined){f.add(j.error,"danger",5000)}l.Sum=function(g,d){return p.Sum(g,d)};l.Max=function(g,d){return p.Max(g,d)};l.Min=function(g,d){return p.Min(g,d)};l.Ave=function(g,d){return p.Ave(g,d)};var e="<div ng-if='showFooter' class='ngFooterPanel' ng-style='footerStyle()'><div class='ngTotalSelectContainer' ><div class='ngFooterTotalItems' ng-class=\"{'ngNoMultiSelect':!multiSelect}\"><span class='ngLabel'>View {{(totalServerItems==0?0:pagingOptions.pageSize*(pagingOptions.currentPage-1)+1)}}-{{(totalServerItems<(pagingOptions.pageSize*pagingOptions.currentPage)?totalServerItems:pagingOptions.pageSize*pagingOptions.currentPage)}} of {{totalServerItems}}</span><span ng-show='filterText.length>0' class='ngLabel'>({{i18n.ngShowingItemsLabel}} {{totalFilteredItemsLength()}})</span></div></div><div class='ngPagerContainer' style='float:right;margin-top:6px;' ng-if='enablePaging' ng-class=\"{'ngNoMultiSelect':!multiSelect}\"><div style='float:left;margin-right:10px;' class='ngRowCountPicker'><span style='float:left;margin-top:4.5px;' class='ngLabel'>{{i18n.ngPageSizeLabel}}&nbsp;</span><select style='float:left;height:25px;width:50px' ng-model='pagingOptions.pageSize'><option ng-repeat='size in pagingOptions.pageSizes'>{{size}}</option></select></div><div style='float:left;margin-right:10px;line-height:25px;' class='ngPagerControl' style='float:left;min-width:135px;'><button type='button' class='ngPagerButton' ng-click='pageToFirst()' ng-disabled='cantPageBackward()' title='{{i18n.ngPagerFirstTitle}}'><div class='ngPagerFirstTriangle'><div class='ngPagerFirstBar'></div></div></button><button type='button' class='ngPagerButton' ng-click='pageBackward()' ng-disabled='cantPageBackward()' title='{{i18n.ngPagerPrevTitle}}'><div class='ngPagerFirstTriangle ngPagerPrevTriangle'></div></button><button type='button' class='ngPagerButton' ng-click='pageForward()' ng-disabled='cantPageForward()' title='{{i18n.ngPagerNextTitle}}'><div class='ngPagerLastTriangle ngPagerNextTriangle'></div></button><button type='button' class='ngPagerButton' ng-click='pageToLast()' ng-disabled='cantPageToLast()' title='{{i18n.ngPagerLastTitle}}'><div class='ngPagerLastTriangle'><div class='ngPagerLastBar'></div></div></button></div></div></div>";l.Session=n.data;l.gridData=j.gridData;l.pagingOptions={1052:{pageSizes:[10,20,50,100],pageSize:10,currentPage:1},};l.sortInfo={1052:{fields:["ID"],directions:["DESC"],firstload:true},};angular.forEach(l.pagingOptions,function(g,d){l.$watch("pagingOptions["+d+"].pageSize",function(i,q){if(i!==q){if(l.pagingOptions[d].currentPage==1){k(d,i,1,l.sortInfo[d].fields[0],l.sortInfo[d].directions[0],l.filter[d])}else{l.pagingOptions[d].currentPage=1}}});l.$watch("pagingOptions["+d+"].currentPage",function(i,q){if(i!==q){k(d,l.pagingOptions[d].pageSize,i,l.sortInfo[d].fields[0],l.sortInfo[d].directions[0],l.filter[d])}});l.$watch("sortInfo["+d+"]",function(i,q){if(q.firstload){l.sortInfo[d].firstload=false}if((i.fields[0]!==q.fields[0]||i.directions[0]!==q.directions[0])&&!q.firstload){k(d,l.pagingOptions[d].pageSize,l.pagingOptions[d].currentPage,i.fields[0],i.directions[0],l.filter[d])}},true)});l.gridOptions={1052:{data:"gridData[1052]",virtualizationThreshold:50,columnDefs:[{field:"ID",width:30,displayName:"",cellTemplate:"<div class='ngCellText' ng-if='row.entity.ID > 0'><span><a ui-sref='1052({ ID_1052:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:100,displayName:"ID"},{field:"Code",width:100,displayName:"Code",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Code | trustedHTML'></span></div>"},{field:"Name",width:100,displayName:"Name",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[1052],groupsCollapsedByDefault:true,enableColumnResize:true,enableSorting:true,sortInfo:l.sortInfo[1052],useExternalSorting:true,enablePaging:true,showFooter:true,pagingOptions:l.pagingOptions[1052],footerTemplate:e,footerRowHeight:40,init:function(g,d){setTimeout(function(){d.gridOptions[1052].$gridServices.DomUtilityService.RebuildGrid(d.gridOptions[1052].$gridScope,d.gridOptions[1052].ngGrid)},500)}},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){l.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){l.gridOptions[h].groups=j.groups[h]}}l.filter={1052:{},};l.treeViewOptions={};l.clearFilter=function(d){l.filter[d]={};l.Search(d)};l.filter_dropdown_source=j.filter_dropdown_source;l.filter_rdb_source=j.filter_rdb_source;l.filter_autocomplete_source=j.filter_autocomplete_source;l.filter_text_autocomplete_source=j.filter_text_autocomplete_source;l.filter_lookup_source=j.filter_lookup_source;l.setID=function(g,i,d){g[i]=d.ID};l.clearAutoComplete=function(d,g){d[g]=null};l.Search=function(d){k(d,l.pagingOptions[d].pageSize,l.pagingOptions[d].currentPage,l.sortInfo[d].fields[0],l.sortInfo[d].directions[0],l.filter[d])};function k(r,s,d,i,g,q){c.getPagedData(r,s,d,i,g,q).then(function(t){l.gridData[r]=t.data;l.gridOptions[r].$gridScope.totalServerItems=t.totalServerItems;if(!l.$$phase){l.$apply()}})}};a.register.controller("c1045",["$scope","resources","dataService","utilService","$state","growlNotifications","$templateCache","Session",b])});