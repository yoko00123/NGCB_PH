'use strict';define(['app'],function(app){var c17280=function($c,s,r,d,u,S,mI,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=17280;s.rID=S.params.ID_17280;s.close=function(){mI.dismiss('close');S.go('^',{reload:true});};s.goPrevious=function(){S.go('^',{reload:true});};s.gridOptions={};};app.register.controller('c17280',['$controller','$scope','resources','dataService','utilService','$state','$modalInstance','growlNotifications','Session','ckFormPristine',c17280]);});