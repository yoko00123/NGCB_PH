'use strict';define(['app'],function(app){var c13228=function($c,s,r,d,u,S,g,SS){s.gridOptions={};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={13229:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});var iframe = $('<iframe />');iframe.attr('style', 'width:100%;height:850px;');iframe.attr('src', '../ModulePage/Report.aspx?menuID=' + 13229 + '&refID=' + 0 + '&params=' + JSON.stringify(s.rawData));iframe.attr('id', 'frame_13229');$('.grid').append(iframe);};app.register.controller('c13228',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c13228]);});