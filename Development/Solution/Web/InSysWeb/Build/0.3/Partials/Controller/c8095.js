"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=8095;j.rID=k.params.ID_8095;j.goPrevious=function(){k.go("8094",{},{reload:true,inherit:false,notify:true})};j.gridOptions={}};a.register.controller("c8095",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});