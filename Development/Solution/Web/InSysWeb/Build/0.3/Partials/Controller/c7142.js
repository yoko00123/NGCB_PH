"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=7142;j.rID=k.params.ID_7142;j.goPrevious=function(){k.go("7141",{},{reload:true,inherit:false,notify:true})};j.$watch("Master.ID_Company",function(d,n){if(d!==n){if(d==undefined||d==null){j.Master.Company=""}else{var g=j.lookup_source[8410].filter(function(o){return o.ID==d});j.Master.Company=g[0]["Name"]}}});j.gridOptions={}};a.register.controller("c7142",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});