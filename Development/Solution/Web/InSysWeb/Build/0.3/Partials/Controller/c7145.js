"use strict";define(["app"],function(a){var b=function(c,k,j,e,n,l,f,m){k.gridOptions={};for(var h in j.columnDefinitions){if(j.columnDefinitions[h].length>0){k.gridOptions[h].columnDefs=j.columnDefinitions[h]}}for(var h in j.groups){if(j.groups[h].length>0){k.gridOptions[h].groups=j.groups[h]}}k.filter={};k.treeViewOptions={};c("BaseListController",{$scope:k,resources:j})};a.register.controller("c7145",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session",b])});