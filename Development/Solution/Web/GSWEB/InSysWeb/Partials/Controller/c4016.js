'use strict';define(['app'],function(app){var c4016=function($c,s,r,d,u,S,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=4016;s.rID=S.params.ID_4016;s.goPrevious=function(){S.go('4009',{},{reload:true,inherit:false,notify:true});};s.gridOptions={4018:{data:'Detail[4018]',enableSorting:true,columnDefs:[{field:'ID',width:'*',displayName:'ID'},{field:'Date',width:'*',displayName:'Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'StartTime',width:'*',displayName:'Start Time',headerCellTemplateUrl:'mgrid/headerCellTemplateTime.html',cellFilter:'date:\'hh:mm a\''},{field:'EndTime',width:'*',displayName:'End Time',headerCellTemplateUrl:'mgrid/headerCellTemplateTime.html',cellFilter:'date:\'hh:mm a\''},{field:'Comment',width:'*',displayName:'Comment',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Comment | trustedHTML\'></span></div>'},],},5032:{data:'Detail[5032]',enableSorting:true,columnDefs:[{field:'ID',width:'*',displayName:'ID'},{field:'Name',width:'*',displayName:'File',cellTemplate:'<div class=\'m-grid-cell-contents\'><span download-file=\'{{row.entity.Name_GUID}}\' filename=\'{{row.entity.Name}}\' style=\'cursor:pointer;text-decoration:underline\' class=\'control-label\' ng-bind-html=\'row.entity.Name | trustedHTML\'></span></div>'},],},};};app.register.controller('c4016',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session','ckFormPristine',c4016]);});