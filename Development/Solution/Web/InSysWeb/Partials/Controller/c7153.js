'use strict';define(['app'],function(app){var c7153=function($c,s,r,d,u,S,mI,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=7153;s.rID=S.params.ID_7153;s.close=function(){mI.dismiss('close');S.go('^',{reload:true});};s.goPrevious=function(){S.go('^',{reload:true});};s.gridOptions={};};app.register.controller('c7153',['$controller','$scope','resources','dataService','utilService','$state','$modalInstance','growlNotifications','Session','ckFormPristine',c7153]);});