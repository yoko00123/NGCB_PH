"use strict";define(["app"],function(a){var b=function(c,j,i,f,m,k,h,l,e){c("BaseFormController",{$scope:j,resources:i});j.mID=1009;j.rID=k.params.ID_1009;j.goPrevious=function(){k.go("1008",{},{reload:true,inherit:false,notify:true})};j.gridOptions={5028:{data:"Detail[5028]",enableSorting:true,columnDefs:[{field:"ID",width:"*",displayName:"ID"},{field:"Name",width:"*",displayName:"File",cellTemplate:"<div class='smart-form input-group' style='width:100%' ng-form name='x' ng-class=\"{ 'has-error' : x.Name.$invalid && appScope.mainform.$submitted }\" ><label for='file' class='input input-file'><div class='button'><input type='file' name='Name'  required file-input ng-file-select='appScope.onFileSelect($files,5028,\"Name\",row.$$rowIndex)'ng-model='row.entity.Name'/>Browse</div><input type='text' ng-model='row.entity.Name' placeholder='Select files...' readonly></label><span class='input-group-addon' ng-if='row.entity.ID > 0 && row.entity.Name !== null' download-file='{{row.entity.Name_GUID}}' filename='{{row.entity.Name}}'><i class='fa fa-download'></i></span></div>"},{field:"$delete",width:20,sortable:false,resizable:false,displayName:" ",cellTemplate:"<div class='ngCellText'><span><a class='row-delete' ng-click='appScope.removeRow(5028,row,\"Are you sure?\")'  ng-show='appScope.Master.IsPosted == false'  ><i class='fa fa-times'></i></a></span></div>"},],},}};a.register.controller("c1009",["$controller","$scope","resources","dataService","utilService","$state","growlNotifications","Session","ckFormPristine",b])});