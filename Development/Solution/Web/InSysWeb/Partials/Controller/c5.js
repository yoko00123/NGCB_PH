'use strict';define(['app'],function(app){var c5=function($c,s,r,d,u,S,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=5;s.rID=S.params.ID_5;s.goPrevious=function(){S.go('4',{},{reload:true,inherit:false,notify:true});};s.gridOptions={8:{data:'Detail[8]',enableSorting:true,columnDefs:[{field:'ID',width:'*',displayName:'ID'},{field:'Date',width:'*',displayName:'Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellTemplate:'<div ng-form name=\'x\' ><div class=\'input-group\'><input type=\'text\' name=\'Date\' date-format=\'MM/dd/yyyy\' placeholder=\'Date\'  disabled  bs-datepicker data-container=\'body\' date-to-iso class=\'form-control\' ng-model=\'row.entity.Date\'/><span class=\'input-group-addon\'><i class=\'fa fa-calendar\'></i></span></div></div>'},{field:'StartTime',width:'*',displayName:'Start Time',headerCellTemplateUrl:'mgrid/headerCellTemplateTime.html',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.StartTime.$invalid && appScope.mainform.$submitted }" ><div class=\'input-group\'><input type=\'text\' name=\'StartTime\' placeholder=\'Start Time\'  required  bs-timepicker time-to-iso data-container=\'body\' class=\'form-control\' ng-model=\'row.entity.StartTime\'/><span class=\'input-group-addon\'><i class=\'fa fa-clock-o\'></i></span></div></div>'},{field:'EndTime',width:'*',displayName:'End Time',headerCellTemplateUrl:'mgrid/headerCellTemplateTime.html',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.EndTime.$invalid && appScope.mainform.$submitted }" ><div class=\'input-group\'><input type=\'text\' name=\'EndTime\' placeholder=\'End Time\'  required  bs-timepicker time-to-iso data-container=\'body\' class=\'form-control\' ng-model=\'row.entity.EndTime\'/><span class=\'input-group-addon\'><i class=\'fa fa-clock-o\'></i></span></div></div>'},{field:'ID_OB',width:'*',displayName:'ID_OB',visible:false,cellTemplate:'<input type=\'hidden\' name=\'ID_OB\' class=\'form-control\' ng-model=\'row.entity.ID_OB\'/>'},{field:'Comment',width:'*',displayName:'Comment',cellTemplate:'<div ng-form name=\'x\' ><input type=\'text\' name=\'Comment\' placeholder=\'Comment\'  class=\'form-control\' ng-model=\'row.entity.Comment\'/></div>'},{field:'$delete',width:20,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'ngCellText\'><span><a class=\'row-delete\' ng-click=\'appScope.removeRow(8,row,"Are you sure?")\'  ng-show=\'Master.IsPosted == false\'  ><i class=\'fa fa-times\'></i></a></span></div>'},],},};};app.register.controller('c5',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session','ckFormPristine',c5]);});