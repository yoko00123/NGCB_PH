"use strict";define(["app"],function(a){var b=function(k,j,f,o,l,i,n,m,e,h){if(j.error!==undefined){i.add(j.error,"danger",5000)}k.Sum=function(g,d){return o.Sum(g,d)};k.Max=function(g,d){return o.Max(g,d)};k.Min=function(g,d){return o.Min(g,d)};k.Ave=function(g,d){return o.Ave(g,d)};k.mID=4065;k.rID=l.params.ID_4065;k.goPrevious=function(){l.go("4064",{},{reload:true,inherit:false,notify:true})};k.Session=m.data;k.Master=j.Master;k.Parent=j.Parent;k.Detail=j.Detail;k.dropdown_source=j.dropdown_source;k.rdb_source=j.rdb_source;k.autocomplete_source=j.autocomplete_source;k.text_autocomplete_source=j.text_autocomplete_source;k.lookup_source=j.lookup_source;k.detailedlookup_source=j.detailedlookup_source;k.detailedValuelookup_source=j.detailedValuelookup_source;f.RequestDropdownSource("ID,Name","vEmployeeDesignation","ID_Designation="+k.Master.ID_Designation+" and ID <> "+k.Master.ID).then(function(d){k.dropdown_source[9243]=d.data});k.gridOptions={4067:{data:"Detail[4067]",multiSelect:false,virtualizationThreshold:50,columnDefs:[{field:"LastName",width:"*",displayName:"Last Name",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.LastName | trustedHTML'></span></div>"},{field:"FirstName",width:"*",displayName:"First Name",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.FirstName | trustedHTML'></span></div>"},{field:"MiddleName",width:"*",displayName:"Middle Name",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.MiddleName | trustedHTML'></span></div>"},{field:"BirthDate",width:"*",displayName:"Birth Date",headerCellTemplate:n.get("nggrid/headerCellTemplateDate.html"),cellFilter:"date:'MM/dd/yyyy'"},],init:function(g,d){setTimeout(function(){d.gridOptions[4067].$gridServices.DomUtilityService.RebuildGrid(d.gridOptions[4067].$gridScope,d.gridOptions[4067].ngGrid)},500)}},4068:{data:"Detail[4068]",multiSelect:false,virtualizationThreshold:50,columnDefs:[{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},{field:"Address",width:"*",displayName:"Address",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Address | trustedHTML'></span></div>"},{field:"ContactNo",width:"*",displayName:"ContactNo",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.ContactNo | trustedHTML'></span></div>"},{field:"Relationship",width:"*",displayName:"Relationship",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Relationship | trustedHTML'></span></div>"},],init:function(g,d){setTimeout(function(){d.gridOptions[4068].$gridServices.DomUtilityService.RebuildGrid(d.gridOptions[4068].$gridScope,d.gridOptions[4068].ngGrid)},500)}},4069:{data:"Detail[4069]",multiSelect:false,virtualizationThreshold:50,columnDefs:[{field:"SchoolName",width:"*",displayName:"School Name",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.SchoolName | trustedHTML'></span></div>"},{field:"DegreeMajorHonor",width:"*",displayName:"Degree/Major/Honor",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.DegreeMajorHonor | trustedHTML'></span></div>"},{field:"YearsAttended",width:"*",displayName:"Years From - To",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.YearsAttended | trustedHTML'></span></div>"},],init:function(g,d){setTimeout(function(){d.gridOptions[4069].$gridServices.DomUtilityService.RebuildGrid(d.gridOptions[4069].$gridScope,d.gridOptions[4069].ngGrid)},500)}},4070:{data:"Detail[4070]",multiSelect:false,virtualizationThreshold:50,columnDefs:[{field:"Designation",width:"*",displayName:"Designation",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Designation | trustedHTML'></span></div>"},{field:"StartDate",width:"*",displayName:"StartDate",headerCellTemplate:n.get("nggrid/headerCellTemplateDate.html"),cellFilter:"date:'MM/dd/yyyy'"},{field:"EndDate",width:"*",displayName:"EndDate",headerCellTemplate:n.get("nggrid/headerCellTemplateDate.html"),cellFilter:"date:'MM/dd/yyyy'"},{field:"EmployerName",width:"*",displayName:"Employer",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.EmployerName | trustedHTML'></span></div>"},{field:"CompanyIndustry",width:"*",displayName:"Industry",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.CompanyIndustry | trustedHTML'></span></div>"},{field:"Company",width:"*",displayName:"Company",cellTemplate:"<div class='ngCellText'><span class='control-label' ng-bind-html='row.entity.Company | trustedHTML'></span></div>"},],init:function(g,d){setTimeout(function(){d.gridOptions[4070].$gridServices.DomUtilityService.RebuildGrid(d.gridOptions[4070].$gridScope,d.gridOptions[4070].ngGrid)},500)}},};k.removeRow=function(g,p){var d=k.Detail[g].indexOf(p.entity);if(p.entity.ID==0){k.Detail[g].splice(d,1)}else{f.GridDelete(g,p.entity.ID).then(function(q){k.Detail[g].splice(d,1)})}};k.setID=function(g,p,d){g[p.substring(3)]=d.Name;g[p]=d.ID};k.getAutoCompleteItems=function(g,d,p){return f.getAutoCompleteItems(g,d,p).then(function(q){return q.items})};k.tabs=[];k.tabs.activeTab=(localStorage.getItem("mID")==k.mID?(localStorage.getItem("tab")==undefined?0:localStorage.getItem("tab")):0);localStorage.removeItem("mID");localStorage.removeItem("tab");k.loadedTab=[];k.loadTab=function(d){if(k.loadedTab.indexOf(d)==-1){f.loadTab(d,k.rID).then(function(g){k.Detail[d]=g.data;k.loadedTab.push(d)})}};k.selectedFiles=[];k.onFileSelect=function(d,q,r,p){for(var g=0;g<k.selectedFiles.length;g++){if(k.selectedFiles[g].mID==q&&k.selectedFiles[g].name==r&&k.selectedFiles[g].idx==p){k.selectedFiles.splice(g,1);break}}k.selectedFiles.push({file:d[0],mID:q,name:r,idx:p})};k.CascadingDropdown=function(p,d,q,g){f.CascadingDropdown(p,d,q).then(function(t){var r=g.split(",");for(var s=0;s<r.length;s++){k.dropdown_source[r[s]]=t.data[r[s]]}})};e.check(k);k.getSupervisor=function(){f.RequestDropdownSource("ID_Designation as ID","vDesignation","ID="+k.Master.ID_Designation).then(function(g){var d=g.data[0].ID;f.RequestDropdownSource("ID,Name","vEmployeeDesignation","ID_Designation="+d+" and ID <> "+k.Master.ID).then(function(p){k.dropdown_source[9243]=p.data})})};k.updateEmployeeStatus=function(){f.RequestDropdownSource("ID,Name","tEmployeeStatus","IsTerminated=1").then(function(g){var d=h.create("Dialogs/updateEmployeeStatus.html","updateEmployeeStatus",{source:g.data,ID_Employee:k.Master.ID,Name:k.Master.Name,Code:k.Master.Code},{size:"md",keyboard:true,backdrop:true,windowClass:"my-class"});d.result.then(function(p){l.go(l.current,l.params,{reload:true,inherit:false,notify:true})},function(){})})}};a.register.controller("c4065",["$scope","resources","dataService","utilService","$state","growlNotifications","$templateCache","Session","ckFormPristine","dialogs",b]);var c=function(j,e,i,f,h){j.Data={ID_Employee:f.ID_Employee,Name:f.Name,Code:f.Code,ID_EmployeeStatus:null,Comment:null};j.source=f.source;j.cancel=function(){i.dismiss("Canceled")};j.save=function(){e.UpdateEmployeeStatus(j.Data).then(function(d){h.add(d.message,"info",5000);i.close()})}};a.register.controller("updateEmployeeStatus",["$scope","dataService","$modalInstance","data","growlNotifications",c])});