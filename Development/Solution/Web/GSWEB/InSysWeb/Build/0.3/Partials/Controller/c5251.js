"use strict";define(["app"],function(a){var b=function(c,j,i,f,n,k,h,m,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=5251;j.rID=k.params.ID_5251;j.goPrevious=function(){k.go("1002",{},{reload:true,inherit:false,notify:true})};j.gridOptions={}};a.register.controller("c5251",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","$templateCache","Session","ckFormPristine",b])});