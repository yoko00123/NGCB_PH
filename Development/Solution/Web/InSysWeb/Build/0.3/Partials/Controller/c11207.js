"use strict";define(["app"],function(a){var b=function(c,k,j,f,n,l,i,h,m,e){c("BaseFormController",{$scope:k,resources:j});k.mID=11207;k.rID=l.params.ID_11207;k.close=function(){i.dismiss("close");l.go("^",{reload:true})};k.goPrevious=function(){l.go("^",{reload:true})};k.gridOptions={}};a.register.controller("c11207",["$controller","$scope","resources","dataService","utilService","$state","$modalInstance","growlNotifications","Session","ckFormPristine",b])});