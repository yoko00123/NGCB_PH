"use strict";define(["app"],function(a){var b=function(l,j,c,p,m,f,o,n){if(j.error!==undefined){f.add(j.error,"danger",5000)}l.Sum=function(g,d){return p.Sum(g,d)};l.Max=function(g,d){return p.Max(g,d)};l.Min=function(g,d){return p.Min(g,d)};l.Ave=function(g,d){return p.Ave(g,d)};var e="<div ng-if='showFooter' class='ngFooterPanel' ng-style='footerStyle()'><div class='ngTotalSelectContainer' ><div class='ngFooterTotalItems' ng-class=\"{'ngNoMultiSelect':!multiSelect}\"><span class='ngLabel'>View {{(totalServerItems==0?0:pagingOptions.pageSize*(pagingOptions.currentPage-1)+1)}}-{{(totalServerItems<(pagingOptions.pageSize*pagingOptions.currentPage)?totalServerItems:pagingOptions.pageSize*pagingOptions.currentPage)}} of {{totalServerItems}}</span><span ng-show='filterText.length>0' class='ngLabel'>({{i18n.ngShowingItemsLabel}} {{totalFilteredItemsLength()}})</span></div></div><div class='ngPagerContainer' style='float:right;margin-top:6px;' ng-if='enablePaging' ng-class=\"{'ngNoMultiSelect':!multiSelect}\"><div style='float:left;margin-right:10px;' class='ngRowCountPicker'><span style='float:left;margin-top:4.5px;' class='ngLabel'>{{i18n.ngPageSizeLabel}}&nbsp;</span><select style='float:left;height:25px;width:50px' ng-model='pagingOptions.pageSize'><option ng-repeat='size in pagingOptions.pageSizes'>{{size}}</option></select></div><div style='float:left;margin-right:10px;line-height:25px;' class='ngPagerControl' style='float:left;min-width:135px;'><button type='button' class='ngPagerButton' ng-click='pageToFirst()' ng-disabled='cantPageBackward()' title='{{i18n.ngPagerFirstTitle}}'><div class='ngPagerFirstTriangle'><div class='ngPagerFirstBar'></div></div></button><button type='button' class='ngPagerButton' ng-click='pageBackward()' ng-disabled='cantPageBackward()' title='{{i18n.ngPagerPrevTitle}}'><div class='ngPagerFirstTriangle ngPagerPrevTriangle'></div></button><button type='button' class='ngPagerButton' ng-click='pageForward()' ng-disabled='cantPageForward()' title='{{i18n.ngPagerNextTitle}}'><div class='ngPagerLastTriangle ngPagerNextTriangle'></div></button><button type='button' class='ngPagerButton' ng-click='pageToLast()' ng-disabled='cantPageToLast()' title='{{i18n.ngPagerLastTitle}}'><div class='ngPagerLastTriangle'><div class='ngPagerLastBar'></div></div></button></div></div></div>";l.Session=n.data;l.gridData=j.gridData;l.pagingOptions={};l.sortInfo={};l.gridOptions={};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){l.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){l.gridOptions[h].groups=j.groups[h]}}l.filter={};l.treeViewOptions={};l.clearFilter=function(d){l.filter[d]={};l.Search(d)};l.filter_dropdown_source=j.filter_dropdown_source;l.filter_rdb_source=j.filter_rdb_source;l.filter_autocomplete_source=j.filter_autocomplete_source;l.filter_text_autocomplete_source=j.filter_text_autocomplete_source;l.filter_lookup_source=j.filter_lookup_source;l.setID=function(g,i,d){g[i]=d.ID};l.clearAutoComplete=function(d,g){d[g]=null};l.Search=function(d){k(d,l.pagingOptions[d].pageSize,l.pagingOptions[d].currentPage,l.sortInfo[d].fields[0],l.sortInfo[d].directions[0],l.filter[d])};function k(r,s,d,i,g,q){c.getPagedData(r,s,d,i,g,q).then(function(t){l.gridData[r]=t.data;l.gridOptions[r].$gridScope.totalServerItems=t.totalServerItems;if(!l.$$phase){l.$apply()}})}};a.register.controller("c4210",["$scope","resources","dataService","utilService","$state","growlNotifications","$templateCache","Session",b])});