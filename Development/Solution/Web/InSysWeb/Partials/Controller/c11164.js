'use strict';define(['app'],function(app){var c11164=function($c,s,r,d,u,S,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=11164;s.rID=S.params.ID_11164;s.goPrevious=function(){S.go('11163',{},{reload:true,inherit:false,notify:true});};s.gridOptions={11166:{data:'Detail[11166]',enableSorting:true,columnDefs:[{field:'ID',width:'*',displayName:'ID'},{field:'LowerBound',width:'*',displayName:'Lower Bound',cellTemplate:'<div ng-form name=\'x\' ><input type=\'text\' name=\'LowerBound\' placeholder=\'Lower Bound\'  class=\'form-control\' ng-model=\'row.entity.LowerBound\'/></div>'},{field:'UpperBound',width:'*',displayName:'Upper Bound',cellTemplate:'<div ng-form name=\'x\' ><input type=\'text\' name=\'UpperBound\' placeholder=\'Upper Bound\'  class=\'form-control\' ng-model=\'row.entity.UpperBound\'/></div>'},{field:'RecommendedIncrease',width:'*',displayName:'Recommended Increase(%)',cellTemplate:'<div ng-form name=\'x\' ><input type=\'text\' name=\'RecommendedIncrease\' placeholder=\'Recommended Increase(%)\'  class=\'form-control\' ng-model=\'row.entity.RecommendedIncrease\'/></div>'},{field:'$delete',width:20,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'ngCellText\'><span><a class=\'row-delete\' ng-click=\'appScope.removeRow(11166,row,"Are you sure")\'  ><i class=\'fa fa-times\'></i></a></span></div>'},],},};};app.register.controller('c11164',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session','ckFormPristine',c11164]);});