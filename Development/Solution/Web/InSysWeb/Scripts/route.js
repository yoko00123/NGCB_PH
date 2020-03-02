'use strict';define(['app'],function(A){A.forDev=1;A.ClientCustomFolder='';A.SystemVersion='0.3';A.config(['$stateProvider','$stickyStateProvider','$urlRouterProvider','$controllerProvider','$compileProvider','$filterProvider','$provide','$httpProvider',function($stateProvider,$stickyStateProvider,$urlRouterProvider,$controllerProvider,$compileProvider,$filterProvider,$provide,$httpProvider) {var c=(A.forDev==1?'':'Build/'+A.SystemVersion+'/')+'Partials/Controller/',v=(A.forDev==1?'':'Build/'+A.SystemVersion+'/')+'Partials/View/';function rD(q,R,dependencies){var d=q.defer();require(dependencies,function(){d.resolve();R.$apply();});return d.promise;};A.register={controller:$controllerProvider.register,directive:$compileProvider.directive,filter:$filterProvider.register,factory:$provide.factory,service:$provide.service};var S=[];S.push({ name:'4029',stateName:'Announcements',url:'/Announcements/{ID_4029}',controller:'c4029',templateUrl:v+'c4029.html',mID:4029,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4029.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4029,S.ID_4029,0);}]}});S.push({ name:'4033',stateName:'Leave Credits',url:'/Leave-Credits/{ID_4033}',controller:'c4033',templateUrl:v+'c4033.html',mID:4033,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4033.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4033,S.ID_4033,0);}]}});S.push({ name:'4',stateName:'Official Business',url:'/Official-Business/{Filter_4}',controller:'c4',templateUrl:v+'c4.html',mID:4,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4,0,0,S.Filter_4);}]}});S.push({ name:'5',stateName:'Official Business Application',url:'/Official-Business-Application/{ID_5}',controller:'c5',templateUrl:v+'c5.html',mID:5,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c5.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(5,S.ID_5,0);}]}});S.push({ name:'6',stateName:'Missed Log',url:'/Missed-Log/{Filter_6}',controller:'c6',templateUrl:v+'c6.html',mID:6,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c6.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(6,0,0,S.Filter_6);}]}});S.push({ name:'7',stateName:'Missed Log Application',url:'/Missed-Log-Application/{ID_7}',controller:'c7',templateUrl:v+'c7.html',mID:7,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c7.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(7,S.ID_7,0);}]}});S.push({ name:'10',stateName:'Leave',url:'/Leave/{Filter_10}',controller:'c10',templateUrl:v+'c10.html',mID:10,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c10.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(10,0,0,S.Filter_10);}]}});S.push({ name:'11',stateName:'Leave Application',url:'/Leave-Application/{ID_11}',controller:'c11',templateUrl:v+'c11.html',mID:11,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c11.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(11,S.ID_11,0);}]}});S.push({ name:'1008',stateName:'Overtime',url:'/Overtime/{Filter_1008}',controller:'c1008',templateUrl:v+'c1008.html',mID:1008,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c1008.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(1008,0,0,S.Filter_1008);}]}});S.push({ name:'1009',stateName:'Overtime Application',url:'/Overtime-Application/{ID_1009}',controller:'c1009',templateUrl:v+'c1009.html',mID:1009,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c1009.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(1009,S.ID_1009,0);}]}});S.push({ name:'2009',stateName:'Change of Schedule',url:'/Change-of-Schedule/{Filter_2009}',controller:'c2009',templateUrl:v+'c2009.html',mID:2009,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c2009.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(2009,0,0,S.Filter_2009);}]}});S.push({ name:'2010',stateName:'Change of Schedule Application',url:'/Change-of-Schedule-Application/{ID_2010}',controller:'c2010',templateUrl:v+'c2010.html',mID:2010,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c2010.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(2010,S.ID_2010,0);}]}});S.push({ name:'3011',stateName:'Schedule File',url:'/Schedule-File/{Filter_3011}',controller:'c3011',templateUrl:v+'c3011.html',mID:3011,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c3011.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(3011,0,0,S.Filter_3011);}]}});S.push({ name:'3012',stateName:'Schedule File Application',url:'/Schedule-File-Application/{ID_3012}',controller:'c3012',templateUrl:v+'c3012.html',mID:3012,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c3012.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(3012,S.ID_3012,0);}]}});S.push({ name:'4009',stateName:'Official Business Approval',url:'/Official-Business-Approval/{Filter_4009}',controller:'c4009',templateUrl:v+'c4009.html',mID:4009,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4009.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4009,0,0,S.Filter_4009);}]}});S.push({ name:'4016',stateName:'Official Business Approvals',url:'/Official-Business-Approvals/{ID_4016}',controller:'c4016',templateUrl:v+'c4016.html',mID:4016,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4016.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4016,S.ID_4016,0);}]}});S.push({ name:'4010',stateName:'Missed Log Approval',url:'/Missed-Log-Approval/{Filter_4010}',controller:'c4010',templateUrl:v+'c4010.html',mID:4010,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4010.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4010,0,0,S.Filter_4010);}]}});S.push({ name:'4019',stateName:'Missed Log Approvals',url:'/Missed-Log-Approvals/{ID_4019}',controller:'c4019',templateUrl:v+'c4019.html',mID:4019,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4019.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4019,S.ID_4019,0);}]}});S.push({ name:'4011',stateName:'Leave Approval',url:'/Leave-Approval/{Filter_4011}',controller:'c4011',templateUrl:v+'c4011.html',mID:4011,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4011.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4011,0,0,S.Filter_4011);}]}});S.push({ name:'4021',stateName:'Leave Approvals',url:'/Leave-Approvals/{ID_4021}',controller:'c4021',templateUrl:v+'c4021.html',mID:4021,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4021.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4021,S.ID_4021,0);}]}});S.push({ name:'4012',stateName:'Overtime Approval',url:'/Overtime-Approval/{Filter_4012}',controller:'c4012',templateUrl:v+'c4012.html',mID:4012,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4012.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4012,0,0,S.Filter_4012);}]}});S.push({ name:'4023',stateName:'Overtime Approvals',url:'/Overtime-Approvals/{ID_4023}',controller:'c4023',templateUrl:v+'c4023.html',mID:4023,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4023.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4023,S.ID_4023,0);}]}});S.push({ name:'4013',stateName:'Change of Schedule Approval',url:'/Change-of-Schedule-Approval/{Filter_4013}',controller:'c4013',templateUrl:v+'c4013.html',mID:4013,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4013.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4013,0,0,S.Filter_4013);}]}});S.push({ name:'4024',stateName:'Change of Schedule Approvals',url:'/Change-of-Schedule-Approvals/{ID_4024}',controller:'c4024',templateUrl:v+'c4024.html',mID:4024,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4024.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4024,S.ID_4024,0);}]}});S.push({ name:'4014',stateName:'Undertime Approval',url:'/Undertime-Approval/{Filter_4014}',controller:'c4014',templateUrl:v+'c4014.html',mID:4014,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4014.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4014,0,0,S.Filter_4014);}]}});S.push({ name:'4026',stateName:'Undertime Approvals',url:'/Undertime-Approvals/{ID_4026}',controller:'c4026',templateUrl:v+'c4026.html',mID:4026,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4026.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4026,S.ID_4026,0);}]}});S.push({ name:'4015',stateName:'Schedule File Approval',url:'/Schedule-File-Approval/{Filter_4015}',controller:'c4015',templateUrl:v+'c4015.html',mID:4015,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4015.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4015,0,0,S.Filter_4015);}]}});S.push({ name:'4027',stateName:'Schedule File Approvals',url:'/Schedule-File-Approvals/{ID_4027}',controller:'c4027',templateUrl:v+'c4027.html',mID:4027,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4027.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4027,S.ID_4027,0);}]}});S.push({ name:'4032',stateName:'Announcement',url:'/Announcement/{Filter_4032}',controller:'c4032',templateUrl:v+'c4032.html',mID:4032,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4032.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4032,0,0,S.Filter_4032);}]}});S.push({ name:'4031',stateName:'Announcement Maintenance',url:'/Announcement-Maintenance/{ID_4031}',controller:'c4031',templateUrl:v+'c4031.html',mID:4031,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c4031.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(4031,S.ID_4031,0);}]}});S.push({ name:'5017',stateName:'Skin',url:'/Skin/{Filter_5017}',controller:'c5017',templateUrl:v+'c5017.html',mID:5017,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c5017.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(5017,0,0,S.Filter_5017);}]}});S.push({ name:'5008',stateName:'Account',url:'/Account/{Filter_5008}',controller:'c5008',templateUrl:v+'c5008.html',mID:5008,resolve:{load:['$q','$rootScope',function (q,R) {var d=[c+'c5008.js'];return rD(q,R,d); }],resources:['dataService',function(d){return d.getResources(5008,0,0,S.Filter_5008);}]}});S.push({ name:'5024',stateName:'Profile',url:'/Profile/{Filter_5024}',controller:'c5024',templateUrl:v+'c5024.html',mID:5024,resolve:{load:['$q','$rootScope',function (q,R) {var d=[c+'c5024.js'];return rD(q,R,d); }],resources:['dataService',function(d){return d.getResources(5024,0,0,S.Filter_5024);}]}});S.push({ name:'5019',stateName:'My Profile',url:'/My-Profile/{ID_5019}',controller:'c5019',templateUrl:v+'c5019.html',mID:5019,resolve:{load:['$q','$rootScope',function (q,R) {return rD(q,R,[c+'c5019.js']); }],resources:['dataService','$stateParams',function(d,S){return d.getResources(5019,S.ID_5019,0);}]}});angular.forEach(S,function(state){$stateProvider.state(state);});}]);});