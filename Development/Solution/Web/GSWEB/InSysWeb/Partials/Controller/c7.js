'use strict';define(['app'],function(app){var c7=function($c,s,r,d,u,S,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=7;s.rID=S.params.ID_7;s.goPrevious=function(){S.go('6',{},{reload:true,inherit:false,notify:true});};s.gridOptions={9:{data:'Detail[9]',enableSorting:true,columnDefs:[{field:'ID',width:'*',displayName:'ID'},{field:'LogDate',width:'*',displayName:'Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellTemplate:'<div ng-form name=\'x\' ><div class=\'input-group\'><input type=\'text\' name=\'LogDate\' date-format=\'MM/dd/yyyy\' placeholder=\'Date\'  disabled  bs-datepicker data-container=\'body\' date-to-iso class=\'form-control\' ng-model=\'row.entity.LogDate\'/><span class=\'input-group-addon\'><i class=\'fa fa-calendar\'></i></span></div></div>'},{field:'LogTime',width:'*',displayName:'Time',headerCellTemplateUrl:'mgrid/headerCellTemplateTime.html',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.LogTime.$invalid && appScope.mainform.$submitted }" ><div class=\'input-group\'><input type=\'text\' name=\'LogTime\' placeholder=\'Time\'  required  bs-timepicker time-to-iso data-container=\'body\' class=\'form-control\' ng-model=\'row.entity.LogTime\'/><span class=\'input-group-addon\'><i class=\'fa fa-clock-o\'></i></span></div></div>'},{field:'ID_AttendanceLogType',width:'*',displayName:'Log Type',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.ID_AttendanceLogType.$invalid && appScope.mainform.$submitted }" ><select name=\'ID_AttendanceLogType\' ng-options=\'item.ID as item.Name for item in appScope.dropdown_source[32]\' required  class=\'form-control\' ng-model=\'row.entity.ID_AttendanceLogType\'><option value>- Select -</option></select>'},{field:'ID_EmployeeMissedLog',width:'*',displayName:'ID_EmployeeMissedLog',visible:false,cellTemplate:'<input type=\'hidden\' name=\'ID_EmployeeMissedLog\' class=\'form-control\' ng-model=\'row.entity.ID_EmployeeMissedLog\'/>'},{field:'$delete',width:20,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'ngCellText\'><span><a class=\'row-delete\' ng-click=\'appScope.removeRow(9,row,"Are you sure?")\'  ><i class=\'fa fa-times\'></i></a></span></div>'},],},};};app.register.controller('c7',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session','ckFormPristine',c7]);});