"use strict";define(["app"],function(a){var b=function(c,k,j,f,n,l,i,h,m,e){c("BaseFormController",{$scope:k,resources:j});k.mID=11230;k.rID=l.params.ID_11230;k.close=function(){i.dismiss("close");l.go("^",{reload:true})};k.goPrevious=function(){l.go("^",{reload:true})};k.$watch("Master.ID_Employee",function(d,o){if(d!==o){if(d==undefined||d==null){k.Master.Employee=""}else{var g=k.lookup_source[14818].filter(function(p){return p.ID==d});k.Master.Employee=g[0]["Name"]}}});k.gridOptions={}};a.register.controller("c11230",["$controller","$scope","resources","dataService","utilService","$state","$modalInstance","growlNotifications","Session","ckFormPristine",b])});