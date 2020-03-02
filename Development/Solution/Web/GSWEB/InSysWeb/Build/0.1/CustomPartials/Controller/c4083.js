"use strict";define(["app"],function(a){var c=function(m,l,i,q,n,k,p,o,h,j){if(l.error!==undefined){k.add(l.error,"danger",5000)}m.Sum=function(r,g){return q.Sum(r,g)};m.Max=function(r,g){return q.Max(r,g)};m.Min=function(r,g){return q.Min(r,g)};m.Ave=function(r,g){return q.Ave(r,g)};m.mID=4083;m.rID=n.params.ID_4083;m.goPrevious=function(){n.go("4076",{},{reload:true,inherit:false,notify:true})};m.Session=o.data;m.Master=l.Master;m.Detail=l.Detail;m.dropdown_source=l.dropdown_source;m.rdb_source=l.rdb_source;m.autocomplete_source=l.autocomplete_source;m.text_autocomplete_source=l.text_autocomplete_source;m.gridOptions={};m.removeRow=function(g,r){if(r.entity.ID==0){m.Detail[g].splice(r.rowIndex,1)}else{i.GridDelete(g,r.entity.ID).then(function(s){m.Detail[g].splice(r.rowIndex,1)})}};m.setID=function(r,s,g){r[s.substring(3)]=g.Name;r[s]=g.ID};m.getAutoCompleteItems=function(r,g,s){return i.getAutoCompleteItems(r,g,s).then(function(t){return t.items})};m.tabs=[];m.tabs.activeTab=(localStorage.getItem("mID")==m.mID?(localStorage.getItem("tab")==undefined?0:localStorage.getItem("tab")):0);localStorage.removeItem("mID");localStorage.removeItem("tab");m.loadedTab=[];m.loadTab=function(g){if(m.loadedTab.indexOf(g)==-1){i.loadTab(g,m.rID).then(function(r){m.Detail[g]=r.data;m.loadedTab.push(g)})}};m.refreshTab=function(g){i.loadTab(g,m.rID).then(function(r){m.Detail[g]=r.data})};m.selectedFiles=[];m.onFileSelect=function(g,t,u,s){for(var r=0;r<m.selectedFiles.length;r++){if(m.selectedFiles[r].mID==t&&m.selectedFiles[r].name==u&&m.selectedFiles[r].idx==s){m.selectedFiles.splice(r,1);break}}m.selectedFiles.push({file:g[0],mID:t,name:u,idx:s})};m.CascadingDropdown=function(s,g,t,r){i.CascadingDropdown(s,g,t).then(function(w){var u=r.split(",");for(var v=0;v<u.length;v++){m.dropdown_source[u[v]]=w.data[u[v]]}})};h.check(m);var f={};f=m.Detail[4133];m.toggleUpload=function(){document.getElementById("uploadFile").click()};m.AttachFiles=function(g){var r=g.length;var s=0;for(s=0;s<r;s++){i.UploadAssessmentFile(g[s],m.Master.ID).then(function(t){k.add(t.message,"info",5000);m.refreshTab(4131);m.mainform.$setPristine()})}};m.DownloadAssessmentFile=function(r,g){i.downloadFile("Contents/Attachments/Assessments/"+r.Name_GUID,r.Name)};m.loadModalViewAttachments=function(s,r){var g=j.create("Dialogs/c1061DialogViewAttachments.html","c1061DialogViewAttachments",{row:s,dropdown_source:m.dropdown_source,source:f},{size:"lg",keyboard:true,backdrop:true,windowClass:"my-class"})};m.GetAttachments=function(g){var r;r=jQuery.grep(m.Detail[4132],function(s,t){return s.ID_RecruitmentApplicationProcess==g});return r};m.DownloadAttachment=function(r,g){i.downloadFile("Contents/Attachments/Interview/"+r,g)};m.DownloadProposal=function(r,g){i.downloadFile("Contents/Attachments/ForHiring/"+r,g)};m.removeAttachment=function(r,g,s){i.removeAttachment(r,g).then(function(t){k.add(t.message,"info",5000);m.refreshTab(4132)})};m.toggleUploadNotes=function(g){document.getElementById("upFile_"+g).click()};m.AddInterviewNotes=function(g,t){var r=g.length;var s=0;for(s=0;s<r;s++){i.UploadInterviewFile(g[s],t.ID).then(function(u){if(u.message=="ok"){m.refreshTab(4132);m.mainform.$setPristine()}else{k.add(u.message,"error",5000)}})}};m.SetStatusAndComment=function(t,s,g){if(g==1){var r=j.create("Dialogs/c4083AddIntComment.html","c4083AddIntComment",{row:t,Action:g,source:null,ID_RecruitmentApplications:m.Master.ID,ID_RecruitmentStage:m.Master.ID_RecruitmentStage,isCanSave:m.Session.ID_Employee==t.ID_Employee&&t.IsCompleted==0},{size:"md",keyboard:false,backdrop:"static",windowClass:"my-class"});r.result.then(function(u){m.refreshTab(4126);m.refreshTab(4135);m.mainform.$setPristine()},function(){})}else{i.RequestDropdownSource("ID,Name","tRecruitmentProcessStatus","ID_RecruitmentStage="+m.Master.ID_RecruitmentStage).then(function(v){var u=j.create("Dialogs/c4083AddIntComment.html","c4083AddIntComment",{row:t,Action:g,source:v.data,ID_RecruitmentApplications:m.Master.ID,ID_RecruitmentStage:m.Master.ID_RecruitmentStage,isCanSave:m.Session.ID_Employee==t.ID_Employee},{size:"md",keyboard:false,backdrop:"static",windowClass:"my-class"});u.result.then(function(w){m.refreshTab(4126)},function(){})})}};m.GetProcessName=function(g){if(g>0){var r;r=jQuery.grep(m.Detail[4125],function(s,t){return s.ID==g});if(r.length>0){return r[0].Name}else{return""}}else{return""}};m.loadTemplateList=function(){i.ListTemplates(4083).then(function(r){var g=j.create("Dialogs/TemplateList.html","TemplateList",{source:r.data},{size:"lg",keyboard:true,backdrop:true,windowClass:"my-class"})})};m.SaveStatus=function(){if(m.Master.ID_RecruitmentProcessStatus==null){k.add("Select status for candidate first.","danger",5000)}else{var g=j.confirm(undefined,"Candidate will be endorsed for next step. Do you want to continue?",{size:"sm"});g.result.then(function(r){m.Status={ID:m.Master.CurrentID,Comment:null,Action:2,ID_RecruitmentProcessStatus:m.Master.ID_RecruitmentProcessStatus,ID_RecruitmentApplications:m.Master.ID,ID_RecruitmentStage:m.Master.ID_RecruitmentStage};i.SetStatusAndComment(m.Status).then(function(s){if(s.message==""){m.refreshTab(4126);m.refreshTab(4135);m.mainform.$setPristine();console.log(m.Master.rID)}else{k.add(s.message,"danger",5000)}})},function(r){})}};if(m.Master.BirthDate!=undefined){m.Master.BirthDate=moment(m.Master.BirthDate).format("MM/DD/YYYY")}};a.register.controller("c4083",["$scope","resources","dataService","utilService","$state","growlNotifications","$templateCache","Session","ckFormPristine","dialogs",c]);var b=function(k,f,j,h,i,l){k.Data=h.source;k.dropdown_source=h.dropdown_source;k.cancel=function(){j.dismiss("Canceled")};k.DownloadAttachment=function(g){DownloadFile("../Upload/"+g)}};a.register.controller("c1061DialogViewAttachments",["$scope","dataService","$modalInstance","data","growlNotifications","$state",b]);var d=function(k,f,j,h,i,l){k.Title=(h.Action==1?"Notes / Remarks":"Set Status");k.btnTitle=(h.Action==1?"Save":"Set Status");k.Data={ID:h.row.ID,Comment:h.row.Comment,Action:h.Action,ID_RecruitmentProcessStatus:null,ID_RecruitmentApplications:h.ID_RecruitmentApplications,ID_RecruitmentStage:h.ID_RecruitmentStage};k.isCanSave=h.isCanSave;k.source=h.source;k.save=function(){f.SetStatusAndComment(k.Data).then(function(g){if(g.message==""){j.close({Comment:k.Data.Comment})}else{i.add(g.message,"danger",5000)}})};k.cancel=function(){j.dismiss("Canceled")}};a.register.controller("c4083AddIntComment",["$scope","dataService","$modalInstance","data","growlNotifications","$state",d]);var e=function(k,f,j,h,i,l){k.Title="Interview Templates";k.Count=h.source.length;k.source=h.source;k.cancel=function(){j.dismiss("Canceled")};k.DownloadAttachment=function(m,g){f.downloadFile("Upload/"+m,g)}};a.register.controller("TemplateList",["$scope","dataService","$modalInstance","data","growlNotifications","$state",e])});