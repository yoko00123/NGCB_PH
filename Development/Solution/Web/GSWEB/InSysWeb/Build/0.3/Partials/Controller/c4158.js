"use strict";define(["app"],function(a){var b=function(c,k,j,f,o,l,i,h,n,m,e){c("BaseFormController",{$scope:k,resources:j});k.mID=4158;k.rID=l.params.ID_4158;k.close=function(){i.dismiss("close");l.go("^",{reload:true})};k.goPrevious=function(){l.go("^",{reload:true})};k.gridOptions={}};a.register.controller("c4158",["$controller","$scope","resources","dataService","utilService","$state","$modalInstance","growlNotifications","$templateCache","Session","ckFormPristine",b])});