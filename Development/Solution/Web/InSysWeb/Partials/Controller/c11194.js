'use strict';define(['app'],function(app){var c11194=function($c,s,r,d,u,S,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=11194;s.rID=S.params.ID_11194;s.goPrevious=function(){S.go('11193',{},{reload:true,inherit:false,notify:true});};s.gridOptions={11195:{data:'Detail[11195]',enableSorting:true,columnDefs:[{field:'ID',width:'*',displayName:'ID'},{field:'Criteria',width:'*',displayName:'Criteria',cellTemplate:'<div class=\'ngCellText\'><span class=\'control-label\' ng-bind-html=\'row.entity.Criteria | trustedHTML\'></span></div>'},{field:'Score',width:'*',displayName:'Score',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.Score.$invalid && appScope.mainform.$submitted }" ><input type=\'text\' name=\'Score\' placeholder=\'Score\'  required  class=\'form-control\' ng-model=\'row.entity.Score\'/></div>'},{field:'Percentage',width:'*',displayName:'Percentage'},{field:'Rating',width:'*',displayName:'Rating'},],},};};app.register.controller('c11194',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session','ckFormPristine',c11194]);});