'use strict';define(['app'],function(app){var c12226=function($c,s,r,d,u,S,mI,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=12226;s.rID=S.params.ID_12226;s.close=function(){mI.dismiss('close');S.go('^',{reload:true});};s.goPrevious=function(){S.go('^',{reload:true});};s.gridOptions={};};app.register.controller('c12226',['$controller','$scope','resources','dataService','utilService','$state','$modalInstance','growlNotifications','Session','ckFormPristine',c12226]);});