"use strict";define(["app"],function(a){var b=function(c,k,j,f,n,l,i,h,m,e){c("BaseFormController",{$scope:k,resources:j});k.mID=11180;k.rID=l.params.ID_11180;k.close=function(){i.dismiss("close");l.go("^",{reload:true})};k.goPrevious=function(){l.go("^",{reload:true})};k.gridOptions={}};a.register.controller("c11180",["$controller","$scope","resources","dataService","utilService","$state","$modalInstance","growlNotifications","Session","ckFormPristine",b])});