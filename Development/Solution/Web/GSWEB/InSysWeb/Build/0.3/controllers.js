"use strict";define(["app"],function(a){var c=function(i,h,e,m,j,f,l,k){if(h.error!==undefined){f.add(h.error,"danger",5000)}i.Sum=function(n,g){return m.Sum(n,g)};i.Max=function(n,g){return m.Max(n,g)};i.Min=function(n,g){return m.Min(n,g)};i.Ave=function(n,g){return m.Ave(n,g)};i.Session=k.data;i.gridData=h.gridData;i.clearFilter=function(g){i.filter[g]={};i.Search(g)};i.filter_dropdown_source=h.filter_dropdown_source;i.filter_rdb_source=h.filter_rdb_source;i.filter_autocomplete_source=h.filter_autocomplete_source;i.filter_text_autocomplete_source=h.filter_text_autocomplete_source;i.filter_lookup_source=h.filter_lookup_source;i.setID=function(n,o,g){n[o]=g.ID};i.clearAutoComplete=function(g,n){g[n]=null};i.Search=function(g){if(i.pagingOptions[g].currentPage==1){i.refreshGrid(g,i.pagingOptions[g].pageSize,i.pagingOptions[g].currentPage,i.sortInfo[g].fields[0],i.sortInfo[g].directions[0],i.filter[g])}else{i.pagingOptions[g].currentPage=1}};i.refreshGrid=function(q,r,g,o,n,p){e.getPagedData(q,r,g,o,n,p).then(function(s){i.gridData[q]=s.data;i.gridOptions[q].$gridScope.totalServerItems=s.totalServerItems;if(!i.$$phase){i.$apply()}})};i.Refresh=function(){j.go(j.current,j.params,{reload:true,inherit:false,notify:true})}};a.controller("BaseListController",["$scope","resources","dataService","utilService","$state","growlNotifications","$templateCache","Session",c]);var b=function(j,i,f,n,k,h,m,l,e){if(i.error!==undefined){h.add(i.error,"danger",5000)}j.Sum=function(o,g){return n.Sum(o,g)};j.Max=function(o,g){return n.Max(o,g)};j.Min=function(o,g){return n.Min(o,g)};j.Ave=function(o,g){return n.Ave(o,g)};j.Session=l.data;j.Master=i.Master;j.Parent=i.Parent;j.Detail=i.Detail;j.dropdown_source=i.dropdown_source;j.rdb_source=i.rdb_source;j.autocomplete_source=i.autocomplete_source;j.text_autocomplete_source=i.text_autocomplete_source;j.lookup_source=i.lookup_source;j.detailedlookup_source=i.detailedlookup_source;j.detailedValuelookup_source=i.detailedValuelookup_source;j.removeRow=function(o,p){var g=j.Detail[o].indexOf(p.entity);if(p.entity.ID==0){j.Detail[o].splice(g,1)}else{f.GridDelete(o,p.entity.ID).then(function(q){j.Detail[o].splice(g,1)})}};j.setID=function(o,p,g){o[p.substring(3)]=g.Name;o[p]=g.ID};j.getAutoCompleteItems=function(o,g,p){return f.getAutoCompleteItems(o,g,p).then(function(q){return q.items})};j.tabs=[];j.tabs.activeTab=(localStorage.getItem("mID")==j.mID?(localStorage.getItem("tab")==undefined?0:localStorage.getItem("tab")):0);localStorage.removeItem("mID");localStorage.removeItem("tab");j.loadedTab=[];j.loadTab=function(g){if(j.loadedTab.indexOf(g)==-1){f.loadTab(g,j.rID).then(function(o){j.Detail[g]=o.data;j.loadedTab.push(g)})}};j.selectedFiles=[];j.onFileSelect=function(g,q,r,p){for(var o=0;o<j.selectedFiles.length;o++){if(j.selectedFiles[o].mID==q&&j.selectedFiles[o].name==r&&j.selectedFiles[o].idx==p){j.selectedFiles.splice(o,1);break}}j.selectedFiles.push({file:g[0],mID:q,name:r,idx:p})};j.CascadingDropdown=function(p,g,q,o){f.CascadingDropdown(p,g,q).then(function(t){var r=o.split(",");for(var s=0;s<r.length;s++){j.dropdown_source[r[s]]=t.data[r[s]]}})};j.Refresh=function(){k.go(k.current,k.params,{reload:true,inherit:false,notify:true})};e.check(j)};a.controller("BaseFormController",["$scope","resources","dataService","utilService","$state","growlNotifications","$templateCache","Session","ckFormPristine",b]);var d=function(g,f,e){g.run=function(){console.log(g.script);e.RunScript(g.script).then(function(k){var h=k.data;if(h.length>0){var j=h[0];for(var i in j){g.columnDefs.push({field:i})}}else{g.columnDefs=[]}g.data=h;g.gridOptions.$gridScope.firstAdjustmentLeft=false;g.gridOptions.ngGrid.lateBoundColumns=false;g.gridOptions.$gridScope.columns.length=0;g.gridOptions.ngGrid.config.columnDefs=g.columnDefs;g.gridOptions.ngGrid.buildColumns();g.gridOptions.ngGrid.eventProvider.assignEvents();g.gridOptions.$gridServices.DomUtilityService.RebuildGrid(g.gridOptions.$gridScope,g.gridOptions.ngGrid)})};g.data=[];g.columnDefs=[];g.gridOptions={data:"data",columnDefs:g.columnDefs,enableColumnResize:true};g.cancel=function(){f.dismiss()}};a.controller("ScriptController",["$scope","$modalInstance","dataService",d])});