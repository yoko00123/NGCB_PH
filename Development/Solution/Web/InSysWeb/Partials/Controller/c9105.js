'use strict';define(['app'],function(app){var c9105=function($c,s,r,d,u,S,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=9105;s.rID=S.params.ID_9105;s.goPrevious=function(){S.go('9104',{},{reload:true,inherit:false,notify:true});};s.gridOptions={9106:{data:'Detail[9106]',enableSorting:true,columnDefs:[{field:'ID',width:'*',displayName:'ID'},{field:'LBound',width:'*',displayName:'Lower Bound',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.LBound.$invalid && appScope.mainform.$submitted }" ><input type=\'text\' name=\'LBound\' placeholder=\'Lower Bound\'  required  ng-numeric-only class=\'form-control\' ng-model=\'row.entity.LBound\'/></div>'},{field:'UBound',width:'*',displayName:'Upper Bound',cellTemplate:'<div ng-form name=\'x\' ><input type=\'text\' name=\'UBound\' placeholder=\'Upper Bound\'  ng-numeric-only class=\'form-control\' ng-model=\'row.entity.UBound\'/></div>'},{field:'MonthlySalaryCredit',width:'*',displayName:'Monthly Salary Credit',cellTemplate:'<div ng-form name=\'x\' ><input type=\'text\' name=\'MonthlySalaryCredit\' placeholder=\'Monthly Salary Credit\'  ng-numeric-only class=\'form-control\' ng-model=\'row.entity.MonthlySalaryCredit\'/></div>'},{field:'ER',width:'*',displayName:'ER',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.ER.$invalid && appScope.mainform.$submitted }" ><input type=\'text\' name=\'ER\' placeholder=\'ER\'  required  ng-numeric-only class=\'form-control\' ng-model=\'row.entity.ER\'/></div>'},{field:'EC',width:'*',displayName:'EC',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.EC.$invalid && appScope.mainform.$submitted }" ><input type=\'text\' name=\'EC\' placeholder=\'EC\'  required  ng-numeric-only class=\'form-control\' ng-model=\'row.entity.EC\'/></div>'},{field:'EE',width:'*',displayName:'EE',cellTemplate:'<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.EE.$invalid && appScope.mainform.$submitted }" ><input type=\'text\' name=\'EE\' placeholder=\'EE\'  required  ng-numeric-only class=\'form-control\' ng-model=\'row.entity.EE\'/></div>'},{field:'Total',width:'*',displayName:'Total',cellTemplate:'<div ng-form name=\'x\' ><input type=\'text\' name=\'Total\' placeholder=\'Total\'  ng-numeric-only class=\'form-control\' ng-model=\'row.entity.Total\'/></div>'},{field:'$delete',width:20,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'ngCellText\'><span><a class=\'row-delete\' ng-click=\'appScope.removeRow(9106,row,"")\'  ><i class=\'fa fa-times\'></i></a></span></div>'},],},};};app.register.controller('c9105',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session','ckFormPristine',c9105]);});