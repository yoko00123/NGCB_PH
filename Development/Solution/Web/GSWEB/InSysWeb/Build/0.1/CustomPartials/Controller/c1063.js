"use strict";define(["app"],function(a){var b=function(l,k,f,o,m,j,n,h){if(k.error!==undefined){j.add(k.error,"danger",5000)}l.Sum=function(p,g){return o.Sum(p,g)};l.Max=function(p,g){return o.Max(p,g)};l.Min=function(p,g){return o.Min(p,g)};l.Ave=function(p,g){return o.Ave(p,g)};var i="<div if-show='showFooter' class='ngFooterPanel' ng-style='footerStyle()'><div class='ngTotalSelectContainer' ><div class='ngFooterTotalItems' ng-class=\"{'ngNoMultiSelect':!multiSelect}\"><span class='ngLabel'>View {{(totalServerItems==0?0:pagingOptions.pageSize*(pagingOptions.currentPage-1)+1)}}-{{(totalServerItems<(pagingOptions.pageSize*pagingOptions.currentPage)?totalServerItems:pagingOptions.pageSize*pagingOptions.currentPage)}} of {{totalServerItems}}</span><span ng-show='filterText.length>0' class='ngLabel'>({{i18n.ngShowingItemsLabel}} {{totalFilteredItemsLength()}})</span></div></div><div class='ngPagerContainer' style='float:right;margin-top:6px;' ng-show='enablePaging' ng-class=\"{'ngNoMultiSelect':!multiSelect}\"><div style='float:left;margin-right:10px;' class='ngRowCountPicker'><span style='float:left;margin-top:4.5px;' class='ngLabel'>{{i18n.ngPageSizeLabel}}&nbsp;</span><select style='float:left;height:25px;width:50px' ng-model='pagingOptions.pageSize'><option ng-repeat='size in pagingOptions.pageSizes'>{{size}}</option></select></div><div style='float:left;margin-right:10px;line-height:25px;' class='ngPagerControl' style='float:left;min-width:135px;'><button type='button' class='ngPagerButton' ng-click='pageToFirst()' ng-disabled='cantPageBackward()' title='{{i18n.ngPagerFirstTitle}}'><div class='ngPagerFirstTriangle'><div class='ngPagerFirstBar'></div></div></button><button type='button' class='ngPagerButton' ng-click='pageBackward()' ng-disabled='cantPageBackward()' title='{{i18n.ngPagerPrevTitle}}'><div class='ngPagerFirstTriangle ngPagerPrevTriangle'></div></button><button type='button' class='ngPagerButton' ng-click='pageForward()' ng-disabled='cantPageForward()' title='{{i18n.ngPagerNextTitle}}'><div class='ngPagerLastTriangle ngPagerNextTriangle'></div></button><button type='button' class='ngPagerButton' ng-click='pageToLast()' ng-disabled='cantPageToLast()' title='{{i18n.ngPagerLastTitle}}'><div class='ngPagerLastTriangle'><div class='ngPagerLastBar'></div></div></button></div></div></div>";l.Session=n.data;l.manpowerplan_source=0;l.rID=0;l.selectedPlan={ID:0,Name:"",Year:0,Version:0,Date:"",CreatedBy:""};l.Detail={1064:{},2072:{},2073:{},2071:{},};l.loadData=function(g){f.loadData(g).then(function(p){l.Detail[2071]=p.data[2071]})};l.getManpowerPlan=function(){f.getManpowerPlan({ID:l.selectedPlan.ID,menuID:1064}).then(function(g){if(l.selectedPlan.ID==0){l.rID=g.data.ID}l.selectedPlan.ID=g.data.ID;l.selectedPlan.Name=g.data.Name;l.selectedPlan.Year=g.data.Year;l.selectedPlan.Version=g.data.VersionNumber;l.selectedPlan.Date=g.data.DateExecuted;l.selectedPlan.CreatedBy=g.data.CreatedByEmployee;l.loadData("ID_ManpowerPlanning = "+l.selectedPlan.ID)})};l.getManpowerPlan();l.loadHistory=function(){f.getComboBoxItems(1064,"Name","ID").then(function(p){var g=h.create("Dialogs/c1063ManpowerHistory.html","c1063ManpowerHistory",p.source,{size:"md",keyboard:true,backdrop:true,windowClass:"my-class"});g.result.then(function(q){},function(q){if(q!=undefined){l.selectedPlan.ID=q;l.getManpowerPlan()}})})};l.loadActivePlan=function(){l.selectedPlan.ID=l.rID;l.getManpowerPlan()};setTimeout(function(){l.gridOptions[2071].selectedItems=l.Detail[2071]},500);l.reloadData=function(){setTimeout(function(){l.gridOptions[2071].selectedItems=l.Detail[2071]},500);l.loadData("ID_ManpowerPlanning = "+l.selectedPlan.ID)};l.loadModal=function(q,p){var g=h.create("Dialogs/c1063Dialog.html","c1063Dialog",{row:q},{size:"md",keyboard:true,backdrop:true,windowClass:"my-class"});g.result.then(function(r){l.Detail[2071][p].Comment=r.Comment},function(){})};l.viewEmployees=function(p,g){f.GetEmployeeList(p.ID_Designation,p.ID_Company,g,l.selectedPlan.ID).then(function(r){var q=h.create("Dialogs/c1063Employees.html","c1063Employees",{row:p,emplist:r.data},{size:"md",keyboard:true,backdrop:true,windowClass:"my-class"});q.result.then(function(s){},function(){})})};l.gridOptions={2071:{data:"Detail[2071]",virtualizationThreshold:50,columnDefs:[{field:"Position",width:"500",displayName:"Position"},{field:"Plantilla",width:"110",displayName:"Plantilla"},{field:"Actual",width:"110",displayName:"Head Count",cellTemplate:'<div class="ngCellText"><span><a ng-click="viewEmployees(row.entity,3)"> {{row.entity.Actual}}</a></span></div>'},{field:"Vacancy",width:"110",displayName:"Vacancy",cellTemplate:'<div class="ngCellText"><span><a data-container="body" data-html="true" data-content="Budgeted: {{row.entity.Vacancy - (row.entity.VacancyReallocate + row.entity.VacancyNonBudgeted)}}<br />Reallocated: {{row.entity.VacancyReallocate}}<br />Non-Budgeted: {{row.entity.VacancyNonBudgeted}}" data-animation="am-flip-x" data-auto-close="1" data-trigger="hover" bs-popover>{{row.entity.Vacancy}}</a></span></div>'},{field:"OnHold",width:"110",displayName:"On Hold"},{field:"Reallocate",width:"110",displayName:"Reallocate"},{field:"ActiveVacancy",width:"110",displayName:"Active Vacancies"},{field:"Department",width:"110",displayName:"Department",visible:false,},{field:"ID",width:"150",displayName:"Notes / Remarks",cellTemplate:'<div class="ngCellText"><a ng-click="loadModal(row.entity,row.rowIndex)" style="font-size:12px;">View/Create</a></div>'},{field:"CompanyGroup",visible:false,},{field:"Division",visible:false,},{field:"JobClassType",visible:false,},{field:"Company",visible:false,},],totalServerItems:100,enableSorting:true,showFooter:true,footerTemplate:'<div if-show=\'showFooter\' class=\'ngFooterPanel\' ng-style=\'footerStyle()\'> <span class="ngAggregateText">Summary <span class="absolute bold" style="left:505px;">{{FooterSum(Detail[2071],\'Plantilla\')}}</span><span class="absolute bold" style="left:615px;">{{FooterSum(Detail[2071],\'Actual\')}}</span><span class="absolute bold" style="left:725px;">{{FooterSum(Detail[2071],\'Vacancy\')}}</span><span class="absolute bold" style="left:835px;">{{FooterSum(Detail[2071],\'OnHold\')}}</span><span class="absolute bold" style="left:945px;">{{FooterSum(Detail[2071],\'Reallocate\')}}</span><span class="absolute bold" style="left:1055px;">{{FooterSum(Detail[2071],\'ActiveVacancy\')}}</span></span></div>',groups:["CompanyGroup","JobClassType"],groupsCollapsedByDefault:false,init:function(p,g){}},};l.FooterSum=function(q,g){var p=0;var r;angular.forEach(q,function(s){r=s[g];if(r){p+=r}});return p};l.ExportToExcel=function(){f.ExportToExcel(2071,1,1,"ID","ASC",{},0)}};a.register.controller("c1063",["$scope","resources","dataService","utilService","$state","growlNotifications","Session","dialogs",b]);var c=function(k,f,j,h,i){k.Data={Comment:null,ID:0};k.Comments;k.GetManpowerPlanComments=function(){f.GetManpowerPlanComments(h.row.ID).then(function(g){k.Comments=g.data})};k.GetManpowerPlanComments();k.cancel=function(){j.dismiss("Canceled")};k.save=function(){f.SaveComment(h.row.ID,k.Data.ID,k.Data.Comment,2071).then(function(g){if(g.message.length>0){i.add(g.message,"danger",5000)}k.Comments.push(g.data)});k.Data.Comment=null;k.Data.ID=0}};a.register.controller("c1063Dialog",["$scope","dataService","$modalInstance","data","growlNotifications",c]);var d=function(k,f,j,h,i){k.Title=h.row.Position;k.EmployeeList=h.emplist;k.Count=k.EmployeeList.length;k.cancel=function(){j.dismiss("Canceled")}};a.register.controller("c1063Employees",["$scope","dataService","$modalInstance","data","growlNotifications",d]);var e=function(k,f,j,h,i){k.source=h;k.cancel=function(){j.dismiss()};k.View=function(g){j.dismiss(g)}};a.register.controller("c1063ManpowerHistory",["$scope","dataService","$modalInstance","data","growlNotifications",e])});