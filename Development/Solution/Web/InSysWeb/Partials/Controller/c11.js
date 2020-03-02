'use strict';
define(['app'], function (app) {
    var c11 = function ($c, s, r, d, u, S, g, SS, ck) {
        $c('BaseFormController', {
            $scope: s,
            resources: r
        });
        s.mID = 11;
        s.rID = S.params.ID_11;
        s.goPrevious = function () {
            S.go('10', {}, {
                reload: true,
                inherit: false,
                notify: true
            });
        };
        s.gridOptions = {
            12: {
                data: 'Detail[12]',
                enableSorting: true,
                columnDefs: [{
                    field: 'ID',
                    width: '*',
                    displayName: 'ID'
                }, {
                    field: 'Date',
                    width: '*',
                    displayName: 'Date',
                    headerCellTemplateUrl: 'mgrid/headerCellTemplateDate.html',
                    cellTemplate: '<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.Date.$invalid && appScope.mainform.$submitted }" ><div class=\'input-group\'><input type=\'text\' name=\'Date\' date-format=\'MM/dd/yyyy\' placeholder=\'Date\'  required  disabled  bs-datepicker data-container=\'body\' date-to-iso class=\'form-control\' ng-model=\'row.entity.Date\'/><span class=\'input-group-addon\'><i class=\'fa fa-calendar\'></i></span></div></div>'
                }, {
                    field: 'ID_HalfDay',
                    width: '*',
                    displayName: 'Duration',
                    cellTemplate: '<div ng-form name=\'x\' ng-class="{ \'has-error\' : x.ID_HalfDay.$invalid && appScope.mainform.$submitted }" ><select name=\'ID_HalfDay\' ng-options=\'item.ID as item.Name for item in appScope.dropdown_source[50]\' required  class=\'form-control\' ng-model=\'row.entity.ID_HalfDay\'><option value>- Select -</option></select>'
                }, {
                    field: 'WithPay',
                    width: '*',
                    displayName: 'With Pay',
                    cellTemplate: '<div class=\'smart-form noselect material-switch\'><input id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'WithPay\'  disabled class=\'form-checkbox\' ng-model=\'row.entity.WithPay\'/><label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'label-default\'></label></div>'
                }, {
                    field: 'Days',
                    width: '*',
                    displayName: 'Days'
                }, {
                    field: 'Comment',
                    width: '*',
                    displayName: 'Comment',
                    cellTemplate: '<div ng-form name=\'x\' ><input type=\'text\' name=\'Comment\' placeholder=\'Comment\'  class=\'form-control\' ng-model=\'row.entity.Comment\'/></div>'
                }, {
                    field: 'ID_Leave',
                    width: '*',
                    displayName: 'ID_Leave',
                    visible: false,
                    cellTemplate: '<input type=\'hidden\' name=\'ID_Leave\' class=\'form-control\' ng-model=\'row.entity.ID_Leave\'/>'
                }, {
                    field: '$delete',
                    width: 20,
                    sortable: false,
                    resizable: false,
                    displayName: ' ',
                    cellTemplate: '<div class=\'ngCellText\'><span><a class=\'row-delete\' ng-click=\'appScope.removeRow(12,row,"Are you sure?")\'  ng-show=\'Master.IsPosted == false\'  ><i class=\'fa fa-times\'></i></a></span></div>'
                },],
            },
            5027: {
                data: 'Detail[5027]',
                enableSorting: true,
                columnDefs: [{
                    field: 'ID',
                    width: '*',
                    displayName: 'ID'
                }, {
                    field: 'Name',
                    width: '*',
                    displayName: 'File',
                    cellTemplate: '<div class=\'smart-form input-group\' style=\'width:100%\' ng-form name=\'x\' ng-class="{ \'has-error\' : x.Name.$invalid && appScope.mainform.$submitted }" ><label for=\'file\' class=\'input input-file\'><div class=\'button\'><input type=\'file\' name=\'Name\'  required file-input ng-file-select=\'appScope.onFileSelect($files,5027,"Name",row.$$rowIndex)\'ng-model=\'row.entity.Name\'/>Browse</div><input type=\'text\' ng-model=\'row.entity.Name\' placeholder=\'Select files...\' readonly></label><span class=\'input-group-addon\' ng-if=\'row.entity.ID > 0 && row.entity.Name !== null\' download-file=\'{{row.entity.Name_GUID}}\' filename=\'{{row.entity.Name}}\'><i class=\'fa fa-download\'></i></span></div>'
                }, {
                    field: '$delete',
                    width: 20,
                    sortable: false,
                    resizable: false,
                    displayName: ' ',
                    cellTemplate: '<div class=\'ngCellText\'><span><a class=\'row-delete\' ng-click=\'appScope.removeRow(5027,row,"Are you sure?")\'  ng-show=\'appScope.Master.IsPosted == false\'  ><i class=\'fa fa-times\'></i></a></span></div>'
                },],
            },
        };
    };
    app.register.controller('c11', ['$controller', '$scope', 'resources', 'dataService', 'utilService', '$state', 'growlNotifications', 'Session', 'ckFormPristine', c11]);
});