"use strict";define(["app"],function(a){var b=function(c,k,j,f,n,l,i,h,m,e){c("BaseFormController",{$scope:k,resources:j});k.mID=5016;k.rID=l.params.ID_5016;k.close=function(){i.dismiss("close");l.go("^",{reload:true})};k.goPrevious=function(){l.go("^",{reload:true})};k.gridOptions={}};a.register.controller("c5016",["$controller","$scope","resources","dataService","utilService","$state","$modalInstance","growlNotifications","Session","ckFormPristine",b])});