"use strict";define(["app"],function(a){var b=function(o,n,f,t,p,l,j){if(n.error!==undefined){l.add(n.error,"danger",5000)}o.Sum=function(i,g){return t.Sum(i,g)};o.Max=function(i,g){return t.Max(i,g)};o.Min=function(i,g){return t.Min(i,g)};o.Ave=function(i,g){return t.Ave(i,g)};o.mID=1061;o.rID=p.params.ID_1061;o.goPrevious=function(){p.go("1043",{},{reload:true,inherit:false,notify:true})};o.Master=n.Master;o.Detail=n.Detail;var q=o.Master.Suffix;o.Master.Suffix=(q==null?"":q.toUpperCase());if(o.rID==0||o.rID!=0&&o.Detail[2066].length==0){o.Detail[2066].push({ID:0,Company:"",Designation:"",EmploymentStatus:"",YearsOfService:"",ReasonForLeaving:"",ImmediateSupervisor:"",ImmediateSupervisorDesignation:"",ContactNo:"",ID_Persona:""});o.Detail[2066].push({ID:0,Company:"",Designation:"",EmploymentStatus:"",YearsOfService:"",ReasonForLeaving:"",ImmediateSupervisor:"",ImmediateSupervisorDesignation:"",ContactNo:"",ID_Persona:""});o.Detail[2066].push({ID:0,Company:"",Designation:"",EmploymentStatus:"",YearsOfService:"",ReasonForLeaving:"",ImmediateSupervisor:"",ImmediateSupervisorDesignation:"",ContactNo:"",ID_Persona:""});o.Detail[2066].push({ID:0,Company:"",Designation:"",EmploymentStatus:"",YearsOfService:"",ReasonForLeaving:"",ImmediateSupervisor:"",ImmediateSupervisorDesignation:"",ContactNo:"",ID_Persona:""});o.Detail[2068].push({ID:0,SchoolName:"",DegreeMajorHonor:"",YearsAttended:"",ID_Persona:"",IsRequired:1});o.Detail[2068].push({ID:0,SchoolName:"",DegreeMajorHonor:"",YearsAttended:"",ID_Persona:"",IsRequired:0});o.Detail[2069].push({ID:0,Name:"",CompanyName:"",Designation:"",ContactNo:"",ID_Persona:"",IsRequired:1});o.Detail[2069].push({ID:0,Name:"",CompanyName:"",Designation:"",ContactNo:"",ID_Persona:"",IsRequired:0});o.Detail[2069].push({ID:0,Name:"",CompanyName:"",Designation:"",ContactNo:"",ID_Persona:"",IsRequired:0})}if(o.rID!=0&&o.Detail[2069].length>0){for(var m=0;m<o.Detail[2069].length;m++){if(m==0){o.Detail[2069][m].IsRequired=1}else{o.Detail[2069][m].IsRequired=0}}}var h=new Date();o.sDate="1/1/"+parseInt(h.getFullYear()-10);o.minDate="12/31/"+parseInt(h.getFullYear()-10);o.dropdown_source=n.dropdown_source;o.rdb_source=n.rdb_source;o.text_autocomplete_source=n.text_autocomplete_source;o.getAge=function(r){var u=new Date();var i=new Date(r);var g=u.getFullYear()-i.getFullYear();var s=u.getMonth()-i.getMonth();if(s<0||(s===0&&u.getDate()<i.getDate())){g--}if(!isNaN(g)&&(o.Master.BirthDate!=""&&o.Master.BirthDate!=undefined&&o.Master.BirthDate!=null)){o.Master.Age=g;return g}};o.gridOptions={2070:{data:"Detail[2070]",multiSelect:false,virtualizationThreshold:50,columnDefs:[{field:"ID",width:"*",displayName:"ID",visible:false,cellTemplate:"<input type='hidden' name='ID' class='form-control' ng-model='row.entity.ID'/>"},{field:"ID_Questionaire_Details",width:"*",displayName:"ID_Questionaire_Details",visible:false,cellTemplate:"<input type='hidden' name='ID_Questionaire_Details' class='form-control' ng-model='row.entity.ID_Questionaire_Details'/>"},{field:"ID_Persona",width:"*",displayName:"ID_Persona",visible:false,cellTemplate:"<input type='hidden' name='ID_Persona' class='form-control' ng-model='row.entity.ID_Persona'/>"},{field:"TextAreaAnswer",width:"*",displayName:"TextAreaAnswer",cellTemplate:"<div ng-form name='x' ><textarea name='TextAreaAnswer' style='height:40px;' class='form-control' ng-model='row.entity.TextAreaAnswer'></textarea></div>"},{field:"IsCheckAnswer",width:"*",displayName:"IsCheckAnswer",cellTemplate:"<div class='ngCellText ng-scope pinned col2 colt2' ng-class='col.colIndex()'><div class='ngCellText ng-scope pinned col2 colt2' ng-class='col.colIndex()'><div ng-class='for_checkbox'><input type='checkbox' name='IsCheckAnswer' class='form-checkbox' ng-model='row.entity.IsCheckAnswer'/></div></div></div>"},{field:"Question",width:"*",displayName:"Question",cellTemplate:"<div class='ngCellText ng-scope pinned col2 colt2' ng-class='col.colIndex()'><span class='control-label' ng-bind-html='row.entity.Question | trustedHTML'></span></div>"},{field:"Choices",width:"*",displayName:"Choices",cellTemplate:"<div class='ngCellText ng-scope pinned col2 colt2' ng-class='col.colIndex()'><span class='control-label' ng-bind-html='row.entity.Choices | trustedHTML'></span></div>"},{field:"ID_QuestionaireType",width:"*",displayName:"ID_QuestionaireType",visible:false,cellTemplate:"<input type='hidden' name='ID_QuestionaireType' class='form-control' ng-model='row.entity.ID_QuestionaireType'/>"},],init:function(i,g){setTimeout(function(){g.gridOptions[2070].$gridServices.DomUtilityService.RebuildGrid(g.gridOptions[2070].$gridScope,g.gridOptions[2070].ngGrid)},500)}},2066:{data:"Detail[2066]",multiSelect:false,virtualizationThreshold:50,columnDefs:[{field:"ID",width:"*",displayName:"ID",visible:false,cellTemplate:"<input type='hidden' name='ID' class='form-control' ng-model='row.entity.ID'/>"},{field:"Company",width:"*",displayName:"Company Name",cellTemplate:"<div ng-form name='x' ><textarea name='Company' style='height:40px;' class='form-control' ng-model='row.entity.Company'></textarea></div>"},{field:"Designation",width:"*",displayName:"Position Title",cellTemplate:"<div ng-form name='x' ><textarea name='Designation' style='height:40px;' class='form-control' ng-model='row.entity.Designation'></textarea></div>"},{field:"EmploymentStatus",width:"*",displayName:"Employment Status",cellTemplate:"<div ng-form name='x' ><textarea name='EmploymentStatus' style='height:40px;' class='form-control' ng-model='row.entity.EmploymentStatus'></textarea></div>"},{field:"YearsOfService",width:"*",displayName:"Tenure/No. of years worked",cellTemplate:"<div ng-form name='x' ><textarea name='YearsOfService' style='height:40px;' class='form-control' ng-model='row.entity.YearsOfService'></textarea></div>"},{field:"ReasonForLeaving",width:"*",displayName:"Reason for Leaving",cellTemplate:"<div ng-form name='x' ><textarea name='ReasonForLeaving' style='height:40px;' class='form-control' ng-model='row.entity.ReasonForLeaving'></textarea></div>"},{field:"ImmediateSupervisor",width:"*",displayName:"Immediate Supervisor's Name",cellTemplate:"<div ng-form name='x' ><textarea name='ImmediateSupervisor' style='height:40px;' class='form-control' ng-model='row.entity.ImmediateSupervisor'></textarea></div>"},{field:"ImmediateSupervisorDesignation",width:"*",displayName:"Designation",cellTemplate:"<div ng-form name='x' ><textarea name='ImmediateSupervisorDesignation' style='height:40px;' class='form-control' ng-model='row.entity.ImmediateSupervisorDesignation'></textarea></div>"},{field:"ContactNo",width:"*",displayName:"Superior's contact no.",cellTemplate:"<div ng-form name='x' ><textarea name='ContactNo' style='height:40px;' class='form-control' ng-model='row.entity.ContactNo'></textarea></div>"},{field:"ID_Persona",width:"*",displayName:"ID_Persona",visible:false,cellTemplate:"<input type='hidden' name='ID_Persona' class='form-control' ng-model='row.entity.ID_Persona'/>"},],init:function(i,g){setTimeout(function(){g.gridOptions[2066].$gridServices.DomUtilityService.RebuildGrid(g.gridOptions[2066].$gridScope,g.gridOptions[2066].ngGrid)},500)}},2068:{data:"Detail[2068]",multiSelect:false,virtualizationThreshold:50,columnDefs:[{field:"ID",width:"*",displayName:"ID",visible:false,cellTemplate:"<input type='hidden' name='ID' class='form-control' ng-model='row.entity.ID'/>"},{field:"SchoolName",width:"*",displayName:"Name of School",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.SchoolName.$invalid && mainform.$submitted }\" ><textarea name='SchoolName'  required style='height:40px;' class='form-control' ng-model='row.entity.SchoolName'></textarea></div>"},{field:"DegreeMajorHonor",width:"*",displayName:"Degree/Major/Honors",cellTemplate:"<div ng-form name='x' ><textarea name='DegreeMajorHonor' style='height:40px;' class='form-control' ng-model='row.entity.DegreeMajorHonor'></textarea></div>"},{field:"YearsAttended",width:"*",displayName:"Yrs Attended (From-To)",cellTemplate:"<div ng-form name='x' ><textarea name='YearsAttended' style='height:40px;' class='form-control' ng-model='row.entity.YearsAttended'></textarea></div>"},{field:"ID_Persona",width:"*",displayName:"ID_Persona",visible:false,cellTemplate:"<input type='hidden' name='ID_Persona' class='form-control' ng-model='row.entity.ID_Persona'/>"},{field:"IsRequired",width:"*",displayName:"IsRequired",visible:false,cellTemplate:"<input type='hidden' name='IsRequired' class='form-control' ng-model='row.entity.IsRequired'/>"},],init:function(i,g){setTimeout(function(){g.gridOptions[2068].$gridServices.DomUtilityService.RebuildGrid(g.gridOptions[2068].$gridScope,g.gridOptions[2068].ngGrid)},500)}},2069:{data:"Detail[2069]",multiSelect:false,virtualizationThreshold:50,columnDefs:[{field:"Name",width:"*",displayName:"Name",cellTemplate:"<div class='ngCellText ng-scope pinned col2 colt2' ng-class='col.colIndex()'><span class='control-label' ng-bind-html='row.entity.Name | trustedHTML'></span></div>"},{field:"CompanyName",width:"*",displayName:"Company",cellTemplate:"<div class='ngCellText ng-scope pinned col2 colt2' ng-class='col.colIndex()'><span class='control-label' ng-bind-html='row.entity.CompanyName | trustedHTML'></span></div>"},{field:"Designation",width:"*",displayName:"Position",cellTemplate:"<div class='ngCellText ng-scope pinned col2 colt2' ng-class='col.colIndex()'><span class='control-label' ng-bind-html='row.entity.Designation | trustedHTML'></span></div>"},{field:"ContactNo",width:"*",displayName:"Contact #",cellTemplate:"<div class='ngCellText ng-scope pinned col2 colt2' ng-class='col.colIndex()'><span class='control-label' ng-bind-html='row.entity.ContactNo | trustedHTML'></span></div>"},{field:"ID_Persona",width:"*",displayName:"ID_Persona",visible:false,cellTemplate:"<input type='hidden' name='ID_Persona' class='form-control' ng-model='row.entity.ID_Persona'/>"},],init:function(i,g){setTimeout(function(){g.gridOptions[2069].$gridServices.DomUtilityService.RebuildGrid(g.gridOptions[2069].$gridScope,g.gridOptions[2069].ngGrid)},500)}},4235:{data:"Detail[4235]",multiSelect:false,virtualizationThreshold:50,columnDefs:[{field:"ID",width:"*",displayName:"ID",visible:false,cellTemplate:"<input type='hidden' name='ID' class='form-control' ng-model='row.entity.ID'/>"},{field:"ID_Essay",width:"*",displayName:"ID_Essay",visible:false,cellTemplate:"<input type='hidden' name='ID_Essay' class='form-control' ng-model='row.entity.ID_Essay'/>"},{field:"ID_Persona",width:"*",displayName:"ID_Persona",visible:false,cellTemplate:"<input type='hidden' name='ID_Persona' class='form-control' ng-model='row.entity.ID_Persona'/>"},{field:"Answer",width:"*",displayName:"Answer",cellTemplate:"<div ng-form name='x' ng-class=\"{ 'has-error' : x.Answer.$invalid && mainform.$submitted }\" ><textarea required name='Answer' style='height:40px;' class='form-control' ng-model='row.entity.Answer'></textarea></div>"},{field:"Essay",width:"*",displayName:"Essay",cellTemplate:"<div class='ngCellText ng-scope pinned col2 colt2' ng-class='col.colIndex()'><span class='control-label' ng-bind-html='row.entity.Essay | trustedHTML'></span></div>"},],init:function(i,g){setTimeout(function(){g.gridOptions[4235].$gridServices.DomUtilityService.RebuildGrid(g.gridOptions[4235].$gridScope,g.gridOptions[4235].ngGrid)},500)}},};o.removeRow=function(g,i){if(i.entity.ID==0){o.Detail[g].splice(i.rowIndex,1)}else{f.GridDelete(g,i.entity.ID).then(function(r){o.Detail[g].splice(i.rowIndex,1)})}};o.setID=function(i,r,g){i[r.substring(3)]=g.Name;i[r]=g.ID};o.getAutoCompleteItems=function(i,g,r){return f.getAutoCompleteItems(i,g,r).then(function(s){return s.items})};o.tabs=[];o.tabs.activeTab=0;o.loadedTab=[];o.loadTab=function(g){if(o.loadedTab.indexOf(g)==-1){f.loadTab(g,o.rID).then(function(i){t.pushArray(o.dropdown_source,i.dropdown_source);t.pushArray(o.rdb_source,i.rdb_source);o.Detail[g]=i.data;o.loadedTab.push(g)})}};o.removeLiRow=function(r,i,g){if(g==0){o.Detail[r].splice(i,1)}else{f.GridDelete(r,g).then(function(s){o.Detail[r].splice(i,1)})}};o.selectedFiles=[];o.onFileSelect=function(g,u,v,s){for(var r=0;r<o.selectedFiles.length;r++){if(o.selectedFiles[r].mID==u&&o.selectedFiles[r].name==v&&o.selectedFiles[r].idx==s){o.selectedFiles.splice(r,1);break}}o.selectedFiles.push({file:g[0],mID:u,name:v,idx:s})};var k=$("#ImageFile");k.bind("change",function(g){var i=new FileReader();if(g.originalEvent.target.files[0].type=="image/jpeg"||g.originalEvent.target.files[0].type=="image/png"){i.onload=function(r){$("#ImagePreview").attr("src",r.target.result)};i.readAsDataURL(g.originalEvent.target.files[0])}else{l.add("Invalid file format.","danger",5000)}});o.toggleUploadFile=function(){setTimeout(function(){$("#ImageFile").click()},0)};var e={};e=o.Detail[4098];o.loadModalViewAttachments=function(r,i){var g=j.create("Dialogs/c1061DialogViewAttachments.html","c1061DialogViewAttachments",{row:r,dropdown_source:o.dropdown_source,source:e,ID_Persona:o.rID},{size:"lg",keyboard:true,backdrop:"static",windowClass:"my-class"});g.result.then(function(u){console.log(u);var s=0;for(s=0;s<u.length;s++){e.splice(u[s],1)}})};o.loadModalAttachments=function(r,i){var g=j.create("Dialogs/c1061DialogUploadAttachments.html","c1061DialogUploadAttachments",{row:r,dropdown_source:o.dropdown_source,ID_Persona:o.Master.ID},{size:"md",keyboard:true,backdrop:true,windowClass:"my-class"})};o.IgnoreNumbers=function(g){if(g.keyCode>=48&&g.keyCode<=57){console.log(g.keyCode,g);g.preventDefault();return false}};if(o.Master.BirthDate!=undefined){o.Master.BirthDate=moment(o.Master.BirthDate).format("MM/DD/YYYY")}o.formatDate=function(){var r=o.Master.BirthDate;var g;r=r.replace(/^([\d]{2})([\d]{2})([\d]{4})$/,"$1/$2/$3");g=r;var i=moment(g);if(i.isValid()){o.Master.BirthDate=g;o.mainform.BirthDate.$error.pattern=false;o.mainform.BirthDate.$invalid=false}else{o.mainform.BirthDate.$error.pattern=true;o.mainform.BirthDate.$invalid=true}}};a.register.controller("c1061",["$scope","resources","dataService","utilService","$state","growlNotifications","dialogs",b]);var d=function(k,e,j,f,i,l){k.Data=f.source;k.dropdown_source=f.dropdown_source;var h=[];k.cancel=function(){j.close(h)};k.RemoveFiles=function(g,m){console.log(g,m);e.RemoveFiles(g,"../../Upload/",f.ID_Persona).then(function(n){if(n.message!=""){i.add(n.error,"danger",5000)}else{h.push(m);k.Data.splice(m,1);i.add("File Removed Successfully.!","info",5000)}})};k.DownloadAttachment=function(g){DownloadFile("../Upload/"+g)}};a.register.controller("c1061DialogViewAttachments",["$scope","dataService","$modalInstance","data","growlNotifications","$state",d]);var c=function(j,e,i,f,h,k){j.Data={ID_Persona:f.ID_Persona,Name:null};j.dropdown_source=f.dropdown_source;j.cancel=function(){i.dismiss("Canceled")};j.files={};j.save=function(){e.UploadAttachments(j.files,j.Data.ID_Persona).then(function(g){if(g.message!=""){h.add(g.message,"danger",5000)}else{k.go(k.current.name,{},{reload:true});h.add("Attachments Uploaded Successully!","info",5000)}})};e.RequestDropdownSource("ID,Name","vPersona","ID="+f.ID_Persona).then(function(g){j.dropdown_source.Persona=g.data});j.replaceTxtBox=function(m){var l=Array();var n=0;for(n=0;n<m.length;n++){l.push(m[0]["name"])}var g=l.join();j.files=m;$("#fileholder").attr("value",g)}};a.register.controller("c1061DialogUploadAttachments",["$scope","dataService","$modalInstance","data","growlNotifications","$state",c])});