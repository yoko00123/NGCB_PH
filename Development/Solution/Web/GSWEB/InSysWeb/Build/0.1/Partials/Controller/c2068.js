"use strict";define(["app"],function(a){var b=function(j,i,e,n,k,h,f,m,l,c){if(i.error!==undefined){f.add(i.error,"danger",5000)}j.Sum=function(g,d){return n.Sum(g,d)};j.Max=function(g,d){return n.Max(g,d)};j.Min=function(g,d){return n.Min(g,d)};j.Ave=function(g,d){return n.Ave(g,d)};j.mID=2068;j.rID=k.params.ID_2068;j.close=function(){h.dismiss("close");k.go("^",{reload:true})};j.goPrevious=function(){k.go("^",{reload:true})};j.Session=l.data;j.Master=i.Master;j.Parent=i.Parent;j.Detail=i.Detail;j.dropdown_source=i.dropdown_source;j.rdb_source=i.rdb_source;j.autocomplete_source=i.autocomplete_source;j.text_autocomplete_source=i.text_autocomplete_source;j.lookup_source=i.lookup_source;j.detailedlookup_source=i.detailedlookup_source;j.detailedValuelookup_source=i.detailedValuelookup_source;j.gridOptions={};j.removeRow=function(g,o){var d=j.Detail[g].indexOf(o.entity);if(o.entity.ID==0){j.Detail[g].splice(d,1)}else{e.GridDelete(g,o.entity.ID).then(function(p){j.Detail[g].splice(d,1)})}};j.setID=function(g,o,d){g[o.substring(3)]=d.Name;g[o]=d.ID};j.getAutoCompleteItems=function(g,d,o){return e.getAutoCompleteItems(g,d,o).then(function(p){return p.items})};j.tabs=[];j.tabs.activeTab=(localStorage.getItem("mID")==j.mID?(localStorage.getItem("tab")==undefined?0:localStorage.getItem("tab")):0);localStorage.removeItem("mID");localStorage.removeItem("tab");j.loadedTab=[];j.loadTab=function(d){if(j.loadedTab.indexOf(d)==-1){e.loadTab(d,j.rID).then(function(g){j.Detail[d]=g.data;j.loadedTab.push(d)})}};j.selectedFiles=[];j.onFileSelect=function(d,p,q,o){for(var g=0;g<j.selectedFiles.length;g++){if(j.selectedFiles[g].mID==p&&j.selectedFiles[g].name==q&&j.selectedFiles[g].idx==o){j.selectedFiles.splice(g,1);break}}j.selectedFiles.push({file:d[0],mID:p,name:q,idx:o})};j.CascadingDropdown=function(o,d,p,g){e.CascadingDropdown(o,d,p).then(function(s){var q=g.split(",");for(var r=0;r<q.length;r++){j.dropdown_source[q[r]]=s.data[q[r]]}})};c.check(j)};a.register.controller("c2068",["$scope","resources","dataService","utilService","$state","$modalInstance","growlNotifications","$templateCache","Session","ckFormPristine",b])});