"use strict";define(["app"],function(a){var b=function(i,h,e,m,j,f,l,k,c){if(h.error!==undefined){f.add(h.error,"danger",5000)}i.Sum=function(g,d){return m.Sum(g,d)};i.Max=function(g,d){return m.Max(g,d)};i.Min=function(g,d){return m.Min(g,d)};i.Ave=function(g,d){return m.Ave(g,d)};i.mID=4234;i.rID=j.params.ID_4234;i.goPrevious=function(){j.go("4233",{},{reload:true,inherit:false,notify:true})};i.Session=k.data;i.Master=h.Master;i.Parent=h.Parent;i.Detail=h.Detail;i.dropdown_source=h.dropdown_source;i.rdb_source=h.rdb_source;i.autocomplete_source=h.autocomplete_source;i.text_autocomplete_source=h.text_autocomplete_source;i.lookup_source=h.lookup_source;i.detailedlookup_source=h.detailedlookup_source;i.detailedValuelookup_source=h.detailedValuelookup_source;i.gridOptions={};i.removeRow=function(g,n){var d=i.Detail[g].indexOf(n.entity);if(n.entity.ID==0){i.Detail[g].splice(d,1)}else{e.GridDelete(g,n.entity.ID).then(function(o){i.Detail[g].splice(d,1)})}};i.setID=function(g,n,d){g[n.substring(3)]=d.Name;g[n]=d.ID};i.getAutoCompleteItems=function(g,d,n){return e.getAutoCompleteItems(g,d,n).then(function(o){return o.items})};i.tabs=[];i.tabs.activeTab=(localStorage.getItem("mID")==i.mID?(localStorage.getItem("tab")==undefined?0:localStorage.getItem("tab")):0);localStorage.removeItem("mID");localStorage.removeItem("tab");i.loadedTab=[];i.loadTab=function(d){if(i.loadedTab.indexOf(d)==-1){e.loadTab(d,i.rID).then(function(g){i.Detail[d]=g.data;i.loadedTab.push(d)})}};i.selectedFiles=[];i.onFileSelect=function(d,o,p,n){for(var g=0;g<i.selectedFiles.length;g++){if(i.selectedFiles[g].mID==o&&i.selectedFiles[g].name==p&&i.selectedFiles[g].idx==n){i.selectedFiles.splice(g,1);break}}i.selectedFiles.push({file:d[0],mID:o,name:p,idx:n})};i.CascadingDropdown=function(n,d,o,g){e.CascadingDropdown(n,d,o).then(function(r){var p=g.split(",");for(var q=0;q<p.length;q++){i.dropdown_source[p[q]]=r.data[p[q]]}})};c.check(i)};a.register.controller("c4234",["$scope","resources","dataService","utilService","$state","growlNotifications","$templateCache","Session","ckFormPristine",b])});