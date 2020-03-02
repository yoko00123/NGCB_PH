'use strict';define(['app'],function(app){var c10104=function($c,s,r,d,u,S,g,SS){s.gridOptions={10105:{data:'gridData[10105]',columnDefs:[{field:'$$',width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' ><span><a ui-sref=\'10105({ ID_10105:row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},{field:'Father',width:'*',displayName:'Father',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Father | trustedHTML\'></span></div>'},{field:'ShiftType',width:'*',displayName:'Shift Type'},{field:'Birthdate',width:'*',displayName:'Birthdate',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'ID',width:'*',displayName:'ID'},{field:'CostCenter',width:'*',displayName:'Cost Center'},{field:'CompanyBankAcct',width:'*',displayName:'Company Bank Account'},{field:'FacultyType',width:'*',displayName:'Faculty Type'},{field:'Code',width:'*',displayName:'Code',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Code | trustedHTML\'></span></div>'},{field:'WeeklySchedule',width:'*',displayName:'Weekly Schedule'},{field:'PayrollScheme',width:'*',displayName:'Payroll Scheme'},{field:'AccountNumberType',width:'*',displayName:'Bank Number Type'},{field:'FatherBirthDate',width:'*',displayName:'Birthdate',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'FacultyInstitute',width:'*',displayName:'Institute'},{field:'Age',width:'*',displayName:'Age'},{field:'BankAcctNo',width:'*',displayName:'Bank Account',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.BankAcctNo | trustedHTML\'></span></div>'},{field:'BirthPlace',width:'*',displayName:'BirthPlace',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.BirthPlace | trustedHTML\'></span></div>'},{field:'FatherOccupation',width:'*',displayName:'Occupation',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.FatherOccupation | trustedHTML\'></span></div>'},{field:'Persona',width:'*',displayName:'Persona'},{field:'LeaveParameter',width:'*',displayName:'Leave Parameter'},{field:'PayrollFrequency',width:'*',displayName:'Payroll Frequency'},{field:'EmployeeCategory',width:'*',displayName:'Employee Category'},{field:'LastName',width:'*',displayName:'LastName',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.LastName | trustedHTML\'></span></div>'},{field:'AccessNo',width:'*',displayName:'AccessNo',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.AccessNo | trustedHTML\'></span></div>'},{field:'Height',width:'*',displayName:'Height'},{field:'Mother',width:'*',displayName:'Mother',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Mother | trustedHTML\'></span></div>'},{field:'TaxExemption',width:'*',displayName:'Tax Exemption'},{field:'CompanyBankAcct2',width:'*',displayName:'Company Bank Account'},{field:'FirstName',width:'*',displayName:'FirstName',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.FirstName | trustedHTML\'></span></div>'},{field:'BankAcctNo2',width:'*',displayName:'Bank Account',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.BankAcctNo2 | trustedHTML\'></span></div>'},{field:'Weight',width:'*',displayName:'Weight'},{field:'Parameter',width:'*',displayName:'Parameter'},{field:'IsRequiredToLog',width:'*',displayName:'IsRequiredToLog',cellTemplate:'<div class=\'m-grid-cell-contents\'><div style=\'margin-top:-3px!important;\' class=\'smart-form noselect for_checkbox material-switch\'><input disabled id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'IsRequiredToLog\' class=\'form-checkbox\' ng-model=\'row.entity.IsRequiredToLog\' disabled/><label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'label-default\'></label></div></div>'},{field:'MotherBirthDate',width:'*',displayName:'Birthdate',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'Company',width:'*',displayName:'Company Profile'},{field:'MotherOccupation',width:'*',displayName:'Occupation',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.MotherOccupation | trustedHTML\'></span></div>'},{field:'PaymentMode',width:'*',displayName:'Payment Mode'},{field:'BloodType',width:'*',displayName:'Blood Type'},{field:'EmailAddress',width:'*',displayName:'Email Address',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.EmailAddress | trustedHTML\'></span></div>'},{field:'Spouse',width:'*',displayName:'Spouse',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.Spouse | trustedHTML\'></span></div>'},{field:'SCStartDate',width:'*',displayName:'SC Start Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'Branch',width:'*',displayName:'Branch'},{field:'AlternateEmailAddress',width:'*',displayName:'Alternate Email Add.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.AlternateEmailAddress | trustedHTML\'></span></div>'},{field:'SubstitutedFiling',width:'*',displayName:'Substituted Filing',cellTemplate:'<div class=\'m-grid-cell-contents\'><div style=\'margin-top:-3px!important;\' class=\'smart-form noselect for_checkbox material-switch\'><input disabled id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'SubstitutedFiling\' class=\'form-checkbox\' ng-model=\'row.entity.SubstitutedFiling\' disabled/><label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'label-default\'></label></div></div>'},{field:'SpouseBirthDate',width:'*',displayName:'Birthday',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'Department',width:'*',displayName:'Department'},{field:'SpouseEmployer',width:'*',displayName:'Employer/Company',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.SpouseEmployer | trustedHTML\'></span></div>'},{field:'Gender',width:'*',displayName:'Gender'},{field:'Designation',width:'*',displayName:'Position'},{field:'SpouseOccupation',width:'*',displayName:'Occupation',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.SpouseOccupation | trustedHTML\'></span></div>'},{field:'Nationality',width:'*',displayName:'Nationality'},{field:'EmployeeStatus',width:'*',displayName:'Employee Status'},{field:'PayrollClassifi',width:'*',displayName:'Payroll Classification'},{field:'Citizenship',width:'*',displayName:'Citizenship'},{field:'StartDate',width:'*',displayName:'Start Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'Section',width:'*',displayName:'ID_Section'},{field:'Religion',width:'*',displayName:'Religion'},{field:'RegularizationDate',width:'*',displayName:'Reg Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'Level',width:'*',displayName:'ID_Level'},{field:'LockerNo',width:'*',displayName:'Locker No',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.LockerNo | trustedHTML\'></span></div>'},{field:'CivilStatus',width:'*',displayName:'Civil Status'},{field:'EndDate',width:'*',displayName:'End Date',headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html',cellFilter:'date:\'MM/dd/yyyy\''},{field:'SSSNO',width:'*',displayName:'SSS No.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.SSSNO | trustedHTML\'></span></div>'},{field:'KeyNo',width:'*',displayName:'Key No',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.KeyNo | trustedHTML\'></span></div>'},{field:'YearsOfService',width:'*',displayName:'Years Of Service'},{field:'ShirtSize',width:'*',displayName:'Shirt Size',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.ShirtSize | trustedHTML\'></span></div>'},{field:'SSSStatus',width:'*',displayName:'SSS Status'},{field:'CompanyEmail',width:'*',displayName:'Company Email',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.CompanyEmail | trustedHTML\'></span></div>'},{field:'HDMFNO',width:'*',displayName:'HDMF No.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.HDMFNO | trustedHTML\'></span></div>'},{field:'BadgeNo',width:'*',displayName:'Badge No',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.BadgeNo | trustedHTML\'></span></div>'},{field:'PayrollStatus',width:'*',displayName:'Payroll Status'},{field:'PhilHealthNo',width:'*',displayName:'Philhealth No.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.PhilHealthNo | trustedHTML\'></span></div>'},{field:'LengthOfService2',width:'*',displayName:'Length Of Service',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.LengthOfService2 | trustedHTML\'></span></div>'},{field:'TIN',width:'*',displayName:'TIN No.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.TIN | trustedHTML\'></span></div>'},{field:'EmailPassword',width:'*',displayName:'Email Password',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.EmailPassword | trustedHTML\'></span></div>'},{field:'GSISNo',width:'*',displayName:'GSIS No.',cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity.GSISNo | trustedHTML\'></span></div>'},{field:'UnionMember',width:'*',displayName:'Union Member',cellTemplate:'<div class=\'m-grid-cell-contents\'><div style=\'margin-top:-3px!important;\' class=\'smart-form noselect for_checkbox material-switch\'><input disabled id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'UnionMember\' class=\'form-checkbox\' ng-model=\'row.entity.UnionMember\' disabled/><label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'label-default\'></label></div></div>'},],totalServerItems:r.totalServerItems[10105],enableColumnResizing: false,currentSortColumn:'ID',currentSortDirection:'DESC',registerApi : function(events){s.gridEvents[10105] = events;s.gridEvents[10105].on.sortChange(s,function(opts){s.refreshGrid(10105, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[10105]);});s.gridEvents[10105].on.pageChange(s, function(opts){s.refreshGrid(10105, opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[10105]);});},},};for(var i in r.columnDefinitions){if(r.columnDefinitions[i].length>0){s.gridOptions[i].columnDefs=r.columnDefinitions[i];}};for(var i in r.groups){if(r.groups[i].length>0){s.gridOptions[i].groups=r.groups[i];}};s.filter={10105:{},};s.treeViewOptions={};$c('BaseListController',{$scope:s,resources:r});};app.register.controller('c10104',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c10104]);});