"use strict";define(["app"],function(a){var b=function(c,k,j,f,o,l,i,h,n,m,e){c("BaseFormController",{$scope:k,resources:j});k.mID=1037;k.rID=l.params.ID_1037;k.close=function(){i.dismiss("close");l.go("^",{reload:true})};k.goPrevious=function(){l.go("^",{reload:true})};k.gridOptions={}};a.register.controller("c1037",["$controller","$scope","resources","dataService","utilService","$state","$modalInstance","growlNotifications","$templateCache","Session","ckFormPristine",b])});