'use strict';define(['app'],function(app){var c4022=function($c,s,r,d,u,S,mI,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=4022;s.rID=S.params.ID_4022;s.close=function(){mI.dismiss('close');S.go('^',{reload:true});};s.goPrevious=function(){S.go('^',{reload:true});};s.gridOptions={};};app.register.controller('c4022',['$controller','$scope','resources','dataService','utilService','$state','$modalInstance','growlNotifications','Session','ckFormPristine',c4022]);});