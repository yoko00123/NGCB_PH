'use strict';define(['app'],function(app){var c11174=function($c,s,r,d,u,S,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=11174;s.rID=S.params.ID_11174;s.goPrevious=function(){S.go('11173',{},{reload:true,inherit:false,notify:true});};s.gridOptions={11175:{data:'Detail[11175]',enableSorting:true,columnDefs:[{field:'ID',width:'*',displayName:'ID'},{field:'Name',width:'*',displayName:'INCLUSIONS',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.Name.$invalid && appScope.mainform.$submitted }" ><input type=\'text\' name=\'Name\' placeholder=\'INCLUSIONS\'  required  class=\'form-control\' ng-model=\'row.entity.Name\'/></div>'},{field:'$delete',width:20,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'ngCellText\'><span><a class=\'row-delete\' ng-click=\'appScope.removeRow(11175,row,"Are you sure?")\'  ><i class=\'fa fa-times\'></i></a></span></div>'},],},};};app.register.controller('c11174',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session','ckFormPristine',c11174]);});