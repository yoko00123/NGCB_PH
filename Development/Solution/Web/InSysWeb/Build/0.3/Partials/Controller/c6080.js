"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=6080;j.rID=k.params.ID_6080;j.goPrevious=function(){k.go("6077",{},{reload:true,inherit:false,notify:true})};j.gridOptions={}};a.register.controller("c6080",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});