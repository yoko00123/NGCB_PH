'use strict';define(['app'],function(app){var c17282=function($c,s,r,d,u,S,g,SS,ck){$c('BaseFormController',{$scope:s,resources:r});s.mID=17282;s.rID=S.params.ID_17282;s.goPrevious=function(){S.go('17281',{},{reload:true,inherit:false,notify:true});};s.gridOptions={17283:{data:'Detail[17283]',enableSorting:true,columnDefs:[{field:'ID',width:'*',displayName:'ID'},{field:'LogDate',width:'*',displayName:'Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'LogTime',width:'*',displayName:'Time',headerCellTemplateUrl:'mgrid/headerCellTemplateTime.html',cellFilter:'date:\'hh:mm a\''},{field:'AttendanceLogType',width:'*',displayName:'Log Type'},],},};};app.register.controller('c17282',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session','ckFormPristine',c17282]);});