"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=11151;j.rID=k.params.ID_11151;j.goPrevious=function(){k.go("11113",{},{reload:true,inherit:false,notify:true})};j.$watch("Master.ID_Employee",function(d,n){if(d!==n){if(d==undefined||d==null){j.Master.Employee=""}else{var g=j.lookup_source[13299].filter(function(o){return o.ID==d});j.Master.Employee=g[0]["Name"]}}});j.gridOptions={}};a.register.controller("c11151",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});