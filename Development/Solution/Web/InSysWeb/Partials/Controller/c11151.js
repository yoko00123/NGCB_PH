'use strict';define(['app'],function(app){var c11151=function($c,s,r,d,u,S,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=11151;s.rID=S.params.ID_11151;s.goPrevious=function(){S.go('11113',{},{reload:true,inherit:false,notify:true});};s.$watch('Master.ID_Employee', function(nv,ov){if(nv !== ov){if (nv == undefined || nv == null) {s.Master.Employee = '';} else {var obj = s.lookup_source[13299].filter(function (x) { return x.ID == nv });s.Master.Employee = obj[0]['Name'];}}});s.gridOptions={};};app.register.controller('c11151',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session','ckFormPristine',c11151]);});