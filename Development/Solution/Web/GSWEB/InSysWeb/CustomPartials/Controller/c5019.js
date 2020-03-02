'use strict';
define(['app'], function (app) {
    var c5019 = function ($c, s, r, d, u, S, g, SS, ck) {
        $c('BaseFormController', {
            $scope: s,
            resources: r
        });
        s.mID = 5019;
        s.rID = S.params.ID_5019;
        s.goPrevious = function () {
            S.go('1', {}, {
                reload: true,
                inherit: false,
                notify: true
            });
        };
        s.gridOptions = {
            5023: {
                data: 'Detail[5023]',
                enableSorting: true,
                columnDefs: [{
                    field: 'LastName',
                    width: '*',
                    displayName: 'Last Name',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.LastName | trustedHTML\'></span></div>'
                }, {
                    field: 'FirstName',
                    width: '*',
                    displayName: 'First Name',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.FirstName | trustedHTML\'></span></div>'
                }, {
                    field: 'MiddleName',
                    width: '*',
                    displayName: 'Middle Name',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.MiddleName | trustedHTML\'></span></div>'
                }, {
                    field: 'BirthDate',
                    width: '*',
                    displayName: 'Birth Date',
                    headerCellTemplateUrl: 'mgrid/headerCellTemplateDate.html',
                    cellFilter: 'date:\'MM/dd/yyyy\''
                }, ],
            },
            5022: {
                data: 'Detail[5022]',
                enableSorting: true,
                columnDefs: [{
                    field: 'Name',
                    width: '*',
                    displayName: 'Name',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Name | trustedHTML\'></span></div>'
                }, {
                    field: 'Address',
                    width: '*',
                    displayName: 'Address',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Address | trustedHTML\'></span></div>'
                }, {
                    field: 'ContactNo',
                    width: '*',
                    displayName: 'ContactNo',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.ContactNo | trustedHTML\'></span></div>'
                }, {
                    field: 'Relationship',
                    width: '*',
                    displayName: 'Relationship',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Relationship | trustedHTML\'></span></div>'
                }, ],
            },
            5021: {
                data: 'Detail[5021]',
                enableSorting: true,
                columnDefs: [{
                    field: 'SchoolName',
                    width: '*',
                    displayName: 'School Name',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.SchoolName | trustedHTML\'></span></div>'
                }, {
                    field: 'DegreeMajorHonor',
                    width: '*',
                    displayName: 'Degree/Major/Honor',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.DegreeMajorHonor | trustedHTML\'></span></div>'
                }, {
                    field: 'YearsAttended',
                    width: '*',
                    displayName: 'Years From - To',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.YearsAttended | trustedHTML\'></span></div>'
                }, ],
            },
            5020: {
                data: 'Detail[5020]',
                enableSorting: true,
                columnDefs: [{
                    field: 'Designation',
                    width: '*',
                    displayName: 'Designation',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Designation | trustedHTML\'></span></div>'
                }, {
                    field: 'StartDate',
                    width: '*',
                    displayName: 'StartDate',
                    headerCellTemplateUrl: 'mgrid/headerCellTemplateDate.html',
                    cellFilter: 'date:\'MM/dd/yyyy\''
                }, {
                    field: 'EndDate',
                    width: '*',
                    displayName: 'EndDate',
                    headerCellTemplateUrl: 'mgrid/headerCellTemplateDate.html',
                    cellFilter: 'date:\'MM/dd/yyyy\''
                }, {
                    field: 'EmployerName',
                    width: '*',
                    displayName: 'Employer',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.EmployerName | trustedHTML\'></span></div>'
                }, {
                    field: 'CompanyIndustry',
                    width: '*',
                    displayName: 'Industry',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.CompanyIndustry | trustedHTML\'></span></div>'
                }, {
                    field: 'Company',
                    width: '*',
                    displayName: 'Company',
                    cellTemplate: '<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Company | trustedHTML\'></span></div>'
                }, ],
            },
        };
    };
    app.register.controller('c5019', ['$controller', '$scope', 'resources', 'dataService', 'utilService', '$state', 'growlNotifications', 'Session', 'ckFormPristine', c5019]);
});