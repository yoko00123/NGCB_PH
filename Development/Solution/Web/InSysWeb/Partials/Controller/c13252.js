'use strict';define(['app'],function(app){var c13252=function($c,s,r,d,u,S,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=13252;s.rID=S.params.ID_13252;s.goPrevious=function(){S.go('13248',{},{reload:true,inherit:false,notify:true});};s.$watch('Master.ID_Designation', function(nv,ov){if(nv !== ov){if (nv == undefined || nv == null) {s.Master.Designation = '';} else {var obj = s.lookup_source[16880].filter(function (x) { return x.ID == nv });s.Master.Designation = obj[0]['Name'];}}});s.gridOptions={13254:{data:'Detail[13254]',enableSorting:true,columnDefs:[{field:'ID',width:'*',displayName:'ID'},{field:'ID_SkillCompetencies',width:'*',displayName:'Skill/Competencies',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.ID_SkillCompetencies.$invalid && appScope.mainform.$submitted }" ><select name=\'ID_SkillCompetencies\' ng-options=\'item.ID as item.Name for item in appScope.dropdown_source[16883]\' required  class=\'form-control\' ng-model=\'row.entity.ID_SkillCompetencies\'><option value>- Select -</option></select>'},{field:'ID_SkillCompetencyType',width:'*',displayName:'Type',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.ID_SkillCompetencyType.$invalid && appScope.mainform.$submitted }" ><select name=\'ID_SkillCompetencyType\' ng-options=\'item.ID as item.Name for item in appScope.dropdown_source[16884]\' required  class=\'form-control\' ng-model=\'row.entity.ID_SkillCompetencyType\'><option value>- Select -</option></select>'},{field:'$delete',width:20,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'ngCellText\'><span><a class=\'row-delete\' ng-click=\'appScope.removeRow(13254,row,"Are you sure?")\'  ><i class=\'fa fa-times\'></i></a></span></div>'},],},};};app.register.controller('c13252',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session','ckFormPristine',c13252]);});