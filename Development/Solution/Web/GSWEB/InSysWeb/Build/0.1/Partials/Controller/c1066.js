"use strict";define(["app"],function(a){var b=function(l,j,c,p,m,f,o,n){if(j.error!==undefined){f.add(j.error,"danger",5000)}l.Sum=function(g,d){return p.Sum(g,d)};l.Max=function(g,d){return p.Max(g,d)};l.Min=function(g,d){return p.Min(g,d)};l.Ave=function(g,d){return p.Ave(g,d)};var e="<div ng-if='showFooter' class='ngFooterPanel' ng-style='footerStyle()'><div class='ngTotalSelectContainer' ><div class='ngFooterTotalItems' ng-class=\"{'ngNoMultiSelect':!multiSelect}\"><span class='ngLabel'>View {{(totalServerItems==0?0:pagingOptions.pageSize*(pagingOptions.currentPage-1)+1)}}-{{(totalServerItems<(pagingOptions.pageSize*pagingOptions.currentPage)?totalServerItems:pagingOptions.pageSize*pagingOptions.currentPage)}} of {{totalServerItems}}</span><span ng-show='filterText.length>0' class='ngLabel'>({{i18n.ngShowingItemsLabel}} {{totalFilteredItemsLength()}})</span></div></div><div class='ngPagerContainer' style='float:right;margin-top:6px;' ng-if='enablePaging' ng-class=\"{'ngNoMultiSelect':!multiSelect}\"><div style='float:left;margin-right:10px;' class='ngRowCountPicker'><span style='float:left;margin-top:4.5px;' class='ngLabel'>{{i18n.ngPageSizeLabel}}&nbsp;</span><select style='float:left;height:25px;width:50px' ng-model='pagingOptions.pageSize'><option ng-repeat='size in pagingOptions.pageSizes'>{{size}}</option></select></div><div style='float:left;margin-right:10px;line-height:25px;' class='ngPagerControl' style='float:left;min-width:135px;'><button type='button' class='ngPagerButton' ng-click='pageToFirst()' ng-disabled='cantPageBackward()' title='{{i18n.ngPagerFirstTitle}}'><div class='ngPagerFirstTriangle'><div class='ngPagerFirstBar'></div></div></button><button type='button' class='ngPagerButton' ng-click='pageBackward()' ng-disabled='cantPageBackward()' title='{{i18n.ngPagerPrevTitle}}'><div class='ngPagerFirstTriangle ngPagerPrevTriangle'></div></button><button type='button' class='ngPagerButton' ng-click='pageForward()' ng-disabled='cantPageForward()' title='{{i18n.ngPagerNextTitle}}'><div class='ngPagerLastTriangle ngPagerNextTriangle'></div></button><button type='button' class='ngPagerButton' ng-click='pageToLast()' ng-disabled='cantPageToLast()' title='{{i18n.ngPagerLastTitle}}'><div class='ngPagerLastTriangle'><div class='ngPagerLastBar'></div></div></button></div></div></div>";l.Session=n.data;l.gridData=j.gridData;l.pagingOptions={1067:{pageSizes:[10,20,50,100],pageSize:10,currentPage:1},1069:{pageSizes:[10,20,50,100],pageSize:10,currentPage:1},};l.sortInfo={1067:{fields:["ID"],directions:["DESC"],firstload:true},1069:{fields:["ID"],directions:["DESC"],firstload:true},};angular.forEach(l.pagingOptions,function(g,d){l.$watch("pagingOptions["+d+"].pageSize",function(i,q){if(i!==q){if(l.pagingOptions[d].currentPage==1){k(d,i,1,l.sortInfo[d].fields[0],l.sortInfo[d].directions[0],l.filter[d])}else{l.pagingOptions[d].currentPage=1}}});l.$watch("pagingOptions["+d+"].currentPage",function(i,q){if(i!==q){k(d,l.pagingOptions[d].pageSize,i,l.sortInfo[d].fields[0],l.sortInfo[d].directions[0],l.filter[d])}});l.$watch("sortInfo["+d+"]",function(i,q){if(q.firstload){l.sortInfo[d].firstload=false}if((i.fields[0]!==q.fields[0]||i.directions[0]!==q.directions[0])&&!q.firstload){k(d,l.pagingOptions[d].pageSize,l.pagingOptions[d].currentPage,i.fields[0],i.directions[0],l.filter[d])}},true)});l.gridOptions={1067:{data:"gridData[1067]",virtualizationThreshold:50,columnDefs:[{field:"ID",width:30,displayName:"",cellTemplate:"<div class='ngCellText' ng-if='row.entity.ID > 0'><span><a ui-sref='1067({ ID_1067:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:100,displayName:"ID"},{field:"Name",width:100,displayName:"Name",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[1067],groupsCollapsedByDefault:true,enableColumnResize:true,init:function(g,d){setTimeout(function(){d.gridOptions[1067].$gridServices.DomUtilityService.RebuildGrid(d.gridOptions[1067].$gridScope,d.gridOptions[1067].ngGrid)},500)}},1069:{data:"gridData[1069]",virtualizationThreshold:50,columnDefs:[{field:"ID",width:30,displayName:"",cellTemplate:"<div class='ngCellText' ng-if='row.entity.ID > 0'><span><a ui-sref='1069({ ID_1069:row.entity.$$rID})'><i class='fa fa-lg fa-fw fa-edit'></i></a></span></div>"},{field:"ID",width:100,displayName:"ID"},{field:"Name",width:100,displayName:"Name",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},],totalServerItems:j.totalServerItems[1069],groupsCollapsedByDefault:true,enableColumnResize:true,init:function(g,d){setTimeout(function(){d.gridOptions[1069].$gridServices.DomUtilityService.RebuildGrid(d.gridOptions[1069].$gridScope,d.gridOptions[1069].ngGrid)},500)}},};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){l.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){l.gridOptions[h].groups=j.groups[h]}}l.filter={1067:{},1069:{},};l.treeViewOptions={};l.clearFilter=function(d){l.filter[d]={};l.Search(d)};l.filter_dropdown_source=j.filter_dropdown_source;l.filter_rdb_source=j.filter_rdb_source;l.filter_autocomplete_source=j.filter_autocomplete_source;l.filter_text_autocomplete_source=j.filter_text_autocomplete_source;l.filter_lookup_source=j.filter_lookup_source;l.setID=function(g,i,d){g[i]=d.ID};l.clearAutoComplete=function(d,g){d[g]=null};l.Search=function(d){k(d,l.pagingOptions[d].pageSize,l.pagingOptions[d].currentPage,l.sortInfo[d].fields[0],l.sortInfo[d].directions[0],l.filter[d])};function k(r,s,d,i,g,q){c.getPagedData(r,s,d,i,g,q).then(function(t){l.gridData[r]=t.data;l.gridOptions[r].$gridScope.totalServerItems=t.totalServerItems;if(!l.$$phase){l.$apply()}})}};a.register.controller("c1066",["$scope","resources","dataService","utilService","$state","growlNotifications","$templateCache","Session",b])});