'use strict';define(['app'],function(app){var c11181=function($c,s,r,d,u,S,mI,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=11181;s.rID=S.params.ID_11181;s.close=function(){mI.dismiss('close');S.go('^',{reload:true});};s.goPrevious=function(){S.go('^',{reload:true});};s.$watch('Master.ID_JobClass', function(nv,ov){if(nv !== ov){if (nv == undefined || nv == null) {s.Master.JobClass = '';} else {var obj = s.lookup_source[14378].filter(function (x) { return x.ID == nv });s.Master.JobClass = obj[0]['Name'];}}});s.gridOptions={};};app.register.controller('c11181',['$controller','$scope','resources','dataService','utilService','$state','$modalInstance','growlNotifications','Session','ckFormPristine',c11181]);});