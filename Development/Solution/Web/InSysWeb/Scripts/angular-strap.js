/**
 * angular-strap
 * @version v2.0.5 - 2014-08-07
 * @link http://mgcrea.github.io/angular-strap
 * @author Olivier Louvignes (olivier@mg-crea.com)
 * @license MIT License, http://www.opensource.org/licenses/MIT
 */
(function (window, document, undefined) {
    'use strict';
    // Source: module.js
    angular.module('mgcrea.ngStrap', [
      'mgcrea.ngStrap.datepicker',
      'mgcrea.ngStrap.popover',
      'mgcrea.ngStrap.select',
      'mgcrea.ngStrap.tab',
      'mgcrea.ngStrap.timepicker',
      'mgcrea.ngStrap.typeahead'
    ]);

    // Source: datepicker.js
    angular.module('mgcrea.ngStrap.datepicker', [
      'mgcrea.ngStrap.helpers.dateParser',
      'mgcrea.ngStrap.tooltip'
    ]).provider('$datepicker', function () {
        var defaults = this.defaults = {
            animation: 'am-fade',
            prefixClass: 'datepicker',
            placement: 'bottom-left',
            template: 'datepicker.tpl.html',
            trigger: 'focus',
            container: false,
            keyboard: true,
            html: false,
            delay: 0,
            useNative: false,
            dateType: 'date',
            dateFormat: 'shortDate',
            modelDateFormat: null,
            dayFormat: 'dd',
            strictFormat: false,
            autoclose: false,
            minDate: -Infinity,
            maxDate: +Infinity,
            startView: 0,
            minView: 0,
            startWeek: 0,
            daysOfWeekDisabled: '',
            iconLeft: 'glyphicon glyphicon-chevron-left',
            iconRight: 'glyphicon glyphicon-chevron-right'
        };
        this.$get = [
          '$window',
          '$document',
          '$rootScope',
          '$sce',
          '$locale',
          'dateFilter',
          'datepickerViews',
          '$tooltip',
          function ($window, $document, $rootScope, $sce, $locale, dateFilter, datepickerViews, $tooltip) {
              var bodyEl = angular.element($window.document.body);
              var isNative = /(ip(a|o)d|iphone|android)/gi.test($window.navigator.userAgent);
              var isTouch = 'createTouch' in $window.document && isNative;
              if (!defaults.lang)
                  defaults.lang = $locale.id;
              function DatepickerFactory(element, controller, config) {
                  var $datepicker = $tooltip(element, angular.extend({}, defaults, config));
                  var parentScope = config.scope;
                  var options = $datepicker.$options;
                  var scope = $datepicker.$scope;
                  if (options.startView)
                      options.startView -= options.minView;
                  // View vars
                  var pickerViews = datepickerViews($datepicker);
                  $datepicker.$views = pickerViews.views;
                  var viewDate = pickerViews.viewDate;
                  scope.$mode = options.startView;
                  scope.$iconLeft = options.iconLeft;
                  scope.$iconRight = options.iconRight;
                  var $picker = $datepicker.$views[scope.$mode];
                  // Scope methods
                  scope.$select = function (date) {
                      $datepicker.select(date);
                  };
                  scope.$selectPane = function (value) {
                      $datepicker.$selectPane(value);
                  };
                  scope.$toggleMode = function () {
                      $datepicker.setMode((scope.$mode + 1) % $datepicker.$views.length);
                  };
                  // Public methods
                  $datepicker.update = function (date) {
                      // console.warn('$datepicker.update() newValue=%o', date);
                      if (angular.isDate(date) && !isNaN(date.getTime())) {
                          $datepicker.$date = date;
                          $picker.update.call($picker, date);
                      }
                      // Build only if pristine
                      $datepicker.$build(true);
                  };
                  $datepicker.updateDisabledDates = function (dateRanges) {
                      options.disabledDateRanges = dateRanges;
                      for (var i = 0, l = scope.rows.length; i < l; i++) {
                          angular.forEach(scope.rows[i], $datepicker.$setDisabledEl);
                      }
                  };
                  $datepicker.select = function (date, keep) {
                      // console.warn('$datepicker.select', date, scope.$mode);
                      if (!angular.isDate(controller.$dateValue))
                          controller.$dateValue = new Date(date);
                      if (!scope.$mode || keep) {
                          controller.$setViewValue(angular.copy(date));
                          controller.$render();
                          if (options.autoclose && !keep) {
                              $datepicker.hide(true);
                          }
                      } else {
                          angular.extend(viewDate, {
                              year: date.getFullYear(),
                              month: date.getMonth(),
                              date: date.getDate()
                          });
                          $datepicker.setMode(scope.$mode - 1);
                          $datepicker.$build();
                      }
                  };
                  $datepicker.setMode = function (mode) {
                      // console.warn('$datepicker.setMode', mode);
                      scope.$mode = mode;
                      $picker = $datepicker.$views[scope.$mode];
                      $datepicker.$build();
                  };
                  // Protected methods
                  $datepicker.$build = function (pristine) {
                      // console.warn('$datepicker.$build() viewDate=%o', viewDate);
                      if (pristine === true && $picker.built)
                          return;
                      if (pristine === false && !$picker.built)
                          return;
                      $picker.build.call($picker);
                  };
                  $datepicker.$updateSelected = function () {
                      for (var i = 0, l = scope.rows.length; i < l; i++) {
                          angular.forEach(scope.rows[i], updateSelected);
                      }
                  };
                  $datepicker.$isSelected = function (date) {
                      return $picker.isSelected(date);
                  };
                  $datepicker.$setDisabledEl = function (el) {
                      el.disabled = $picker.isDisabled(el.date);
                  };
                  $datepicker.$selectPane = function (value) {
                      var steps = $picker.steps;
                      var targetDate = new Date(Date.UTC(viewDate.year + (steps.year || 0) * value, viewDate.month + (steps.month || 0) * value, viewDate.date + (steps.day || 0) * value));
                      angular.extend(viewDate, {
                          year: targetDate.getUTCFullYear(),
                          month: targetDate.getUTCMonth(),
                          date: targetDate.getUTCDate()
                      });
                      $datepicker.$build();
                  };
                  $datepicker.$onMouseDown = function (evt) {
                      // Prevent blur on mousedown on .dropdown-menu
                      evt.preventDefault();
                      evt.stopPropagation();
                      // Emulate click for mobile devices
                      if (isTouch) {
                          var targetEl = angular.element(evt.target);
                          if (targetEl[0].nodeName.toLowerCase() !== 'button') {
                              targetEl = targetEl.parent();
                          }
                          targetEl.triggerHandler('click');
                      }
                  };
                  $datepicker.$onKeyDown = function (evt) {
                      if (!/(38|37|39|40|13)/.test(evt.keyCode) || evt.shiftKey || evt.altKey)
                          return;
                      evt.preventDefault();
                      evt.stopPropagation();
                      if (evt.keyCode === 13) {
                          if (!scope.$mode) {
                              return $datepicker.hide(true);
                          } else {
                              return scope.$apply(function () {
                                  $datepicker.setMode(scope.$mode - 1);
                              });
                          }
                      }
                      // Navigate with keyboard
                      $picker.onKeyDown(evt);
                      parentScope.$digest();
                  };
                  // Private
                  function updateSelected(el) {
                      el.selected = $datepicker.$isSelected(el.date);
                  }
                  function focusElement() {
                      element[0].focus();
                  }
                  // Overrides
                  var _init = $datepicker.init;
                  $datepicker.init = function () {
                      if (isNative && options.useNative) {
                          element.prop('type', 'date');
                          element.css('-webkit-appearance', 'textfield');
                          return;
                      } else if (isTouch) {
                          element.prop('type', 'text');
                          element.attr('readonly', 'true');
                          element.on('click', focusElement);
                      }
                      _init();
                  };
                  var _destroy = $datepicker.destroy;
                  $datepicker.destroy = function () {
                      if (isNative && options.useNative) {
                          element.off('click', focusElement);
                      }
                      _destroy();
                  };
                  var _show = $datepicker.show;
                  $datepicker.show = function () {
                      _show();
                      setTimeout(function () {
                          $datepicker.$element.on(isTouch ? 'touchstart' : 'mousedown', $datepicker.$onMouseDown);
                          if (options.keyboard) {
                              element.on('keydown', $datepicker.$onKeyDown);
                          }
                      });
                  };
                  var _hide = $datepicker.hide;
                  $datepicker.hide = function (blur) {
                      $datepicker.$element.off(isTouch ? 'touchstart' : 'mousedown', $datepicker.$onMouseDown);
                      if (options.keyboard) {
                          element.off('keydown', $datepicker.$onKeyDown);
                      }
                      _hide(blur);
                  };
                  return $datepicker;
              }
              DatepickerFactory.defaults = defaults;
              return DatepickerFactory;
          }
        ];
    }).directive('bsDatepicker', [
      '$window',
      '$parse',
      '$q',
      '$locale',
      'dateFilter',
      '$datepicker',
      '$dateParser',
      '$timeout',
      function ($window, $parse, $q, $locale, dateFilter, $datepicker, $dateParser, $timeout) {
          var defaults = $datepicker.defaults;
          var isNative = /(ip(a|o)d|iphone|android)/gi.test($window.navigator.userAgent);
          var isNumeric = function (n) {
              return !isNaN(parseFloat(n)) && isFinite(n);
          };
          return {
              restrict: 'EAC',
              require: 'ngModel',
              link: function postLink(scope, element, attr, controller) {
                  // Directive options
                  var options = {
                      scope: scope,
                      controller: controller
                  };
                  angular.forEach([
                    'placement',
                    'container',
                    'delay',
                    'trigger',
                    'keyboard',
                    'html',
                    'animation',
                    'template',
                    'autoclose',
                    'dateType',
                    'dateFormat',
                    'modelDateFormat',
                    'dayFormat',
                    'strictFormat',
                    'startWeek',
                    'startDate',
                    'useNative',
                    'lang',
                    'startView',
                    'minView',
                    'iconLeft',
                    'iconRight',
                    'daysOfWeekDisabled'
                  ], function (key) {
                      if (angular.isDefined(attr[key]))
                          options[key] = attr[key];
                  });
                  // Visibility binding support
                  attr.bsShow && scope.$watch(attr.bsShow, function (newValue, oldValue) {
                      if (!datepicker || !angular.isDefined(newValue))
                          return;
                      if (angular.isString(newValue))
                          newValue = newValue.match(',?(datepicker),?');
                      newValue === true ? datepicker.show() : datepicker.hide();
                  });
                  // Set expected iOS format
                  if (isNative && options.useNative)
                      options.dateFormat = 'yyyy-MM-dd';
                  // Initialize datepicker
                  var datepicker = $datepicker(element, controller, options);
                  options = datepicker.$options;
                  // Observe attributes for changes
                  angular.forEach([
                    'minDate',
                    'maxDate'
                  ], function (key) {
                      // console.warn('attr.$observe(%s)', key, attr[key]);
                      angular.isDefined(attr[key]) && attr.$observe(key, function (newValue) {
                          // console.warn('attr.$observe(%s)=%o', key, newValue);
                          if (newValue === 'today') {
                              var today = new Date();
                              datepicker.$options[key] = +new Date(today.getFullYear(), today.getMonth(), today.getDate() + (key === 'maxDate' ? 1 : 0), 0, 0, 0, key === 'minDate' ? 0 : -1);
                          } else if (angular.isString(newValue) && newValue.match(/^".+"$/)) {
                              // Support {{ dateObj }}
                              datepicker.$options[key] = +new Date(newValue.substr(1, newValue.length - 2));
                          } else if (isNumeric(newValue)) {
                              datepicker.$options[key] = +new Date(parseInt(newValue, 10));
                          } else if (angular.isString(newValue) && 0 === newValue.length) {
                              // Reset date
                              datepicker.$options[key] = key === 'maxDate' ? +Infinity : -Infinity;
                          } else {
                              datepicker.$options[key] = +new Date(newValue);
                          }
                          // Build only if dirty
                          !isNaN(datepicker.$options[key]) && datepicker.$build(false);
                      });
                  });
                  // Watch model for changes
                  scope.$watch(attr.ngModel, function (newValue, oldValue) {
                      datepicker.update(controller.$dateValue);
                  }, true);
                  // Normalize undefined/null/empty array,
                  // so that we don't treat changing from undefined->null as a change.
                  function normalizeDateRanges(ranges) {
                      if (!ranges || !ranges.length)
                          return null;
                      return ranges;
                  }
                  if (angular.isDefined(attr.disabledDates)) {
                      scope.$watch(attr.disabledDates, function (disabledRanges, previousValue) {
                          disabledRanges = normalizeDateRanges(disabledRanges);
                          previousValue = normalizeDateRanges(previousValue);
                          if (disabledRanges !== previousValue) {
                              datepicker.updateDisabledDates(disabledRanges);
                          }
                      });
                  }
                  var dateParser = $dateParser({
                      format: options.dateFormat,
                      lang: options.lang,
                      strict: options.strictFormat
                  });
                  // viewValue -> $parsers -> modelValue
                  controller.$parsers.unshift(function (viewValue) {
                      // console.warn('$parser("%s"): viewValue=%o', element.attr('ng-model'), viewValue);
                      // Null values should correctly reset the model value & validity
                      if (!viewValue) {
                          controller.$setValidity('date', true);
                          return;
                      }
                      var parsedDate = dateParser.parse(viewValue, controller.$dateValue);
                      if (!parsedDate || isNaN(parsedDate.getTime())) {
                          controller.$setValidity('date', false);
                          return;
                      } else {
                          var isMinValid = isNaN(datepicker.$options.minDate) || parsedDate.getTime() >= datepicker.$options.minDate;
                          var isMaxValid = isNaN(datepicker.$options.maxDate) || parsedDate.getTime() <= datepicker.$options.maxDate;
                          var isValid = isMinValid && isMaxValid;
                          controller.$setValidity('date', isValid);
                          controller.$setValidity('min', isMinValid);
                          controller.$setValidity('max', isMaxValid);
                          // Only update the model when we have a valid date
                          if (isValid)
                              controller.$dateValue = parsedDate;
                      }
                      if (options.dateType === 'string') {
                          return dateFilter(parsedDate, options.modelDateFormat || options.dateFormat);
                      } else if (options.dateType === 'number') {
                          return controller.$dateValue.getTime();
                      } else if (options.dateType === 'iso') {
                          return controller.$dateValue.toISOString();
                      } else {
                          return new Date(controller.$dateValue);
                      }
                  });
                  // modelValue -> $formatters -> viewValue
                  controller.$formatters.push(function (modelValue) {
                      // console.warn('$formatter("%s"): modelValue=%o (%o)', element.attr('ng-model'), modelValue, typeof modelValue);
                      var date;
                      if (angular.isUndefined(modelValue) || modelValue === null) {
                          date = NaN;
                      } else if (angular.isDate(modelValue)) {
                          date = modelValue;
                      } else if (options.dateType === 'string') {
                          date = dateParser.parse(modelValue, null, options.modelDateFormat);
                      } else {
                          date = new Date(modelValue);
                      }
                      // Setup default value?
                      // if(isNaN(date.getTime())) {
                      //   var today = new Date();
                      //   date = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 0, 0, 0, 0);
                      // }
                      controller.$dateValue = date;
                      return controller.$dateValue;
                  });
                  // viewValue -> element
                  controller.$render = function () {
                      // console.warn('$render("%s"): viewValue=%o', element.attr('ng-model'), controller.$viewValue);
                      element.val(!controller.$dateValue || isNaN(controller.$dateValue.getTime()) ? '' : dateFilter(controller.$dateValue, options.dateFormat));
                  };
                  // Garbage collection
                  scope.$on('$destroy', function () {
                      if (datepicker)
                          datepicker.destroy();
                      options = null;
                      datepicker = null;
                  });
              }
          };
      }
    ]).provider('datepickerViews', function () {
        var defaults = this.defaults = {
            dayFormat: 'dd',
            daySplit: 7
        };
        // Split array into smaller arrays
        function split(arr, size) {
            var arrays = [];
            while (arr.length > 0) {
                arrays.push(arr.splice(0, size));
            }
            return arrays;
        }
        // Modulus operator
        function mod(n, m) {
            return (n % m + m) % m;
        }
        this.$get = [
          '$locale',
          '$sce',
          'dateFilter',
          function ($locale, $sce, dateFilter) {
              return function (picker) {
                  var scope = picker.$scope;
                  var options = picker.$options;
                  var weekDaysMin = $locale.DATETIME_FORMATS.SHORTDAY;
                  var weekDaysLabels = weekDaysMin.slice(options.startWeek).concat(weekDaysMin.slice(0, options.startWeek));
                  var weekDaysLabelsHtml = $sce.trustAsHtml('<th class="dow text-center">' + weekDaysLabels.join('</th><th class="dow text-center">') + '</th>');
                  var startDate = picker.$date || (options.startDate ? new Date(options.startDate) : new Date());
                  var viewDate = {
                      year: startDate.getFullYear(),
                      month: startDate.getMonth(),
                      date: startDate.getDate()
                  };
                  var timezoneOffset = startDate.getTimezoneOffset() * 60000;
                  var views = [
                      {
                          format: options.dayFormat,
                          split: 7,
                          steps: { month: 1 },
                          update: function (date, force) {
                              if (!this.built || force || date.getFullYear() !== viewDate.year || date.getMonth() !== viewDate.month) {
                                  angular.extend(viewDate, {
                                      year: picker.$date.getFullYear(),
                                      month: picker.$date.getMonth(),
                                      date: picker.$date.getDate()
                                  });
                                  picker.$build();
                              } else if (date.getDate() !== viewDate.date) {
                                  viewDate.date = picker.$date.getDate();
                                  picker.$updateSelected();
                              }
                          },
                          build: function () {
                              var firstDayOfMonth = new Date(viewDate.year, viewDate.month, 1), firstDayOfMonthOffset = firstDayOfMonth.getTimezoneOffset();
                              var firstDate = new Date(+firstDayOfMonth - mod(firstDayOfMonth.getDay() - options.startWeek, 7) * 86400000), firstDateOffset = firstDate.getTimezoneOffset();
                              // Handle daylight time switch
                              if (firstDateOffset !== firstDayOfMonthOffset)
                                  firstDate = new Date(+firstDate + (firstDateOffset - firstDayOfMonthOffset) * 60000);
                              var days = [], day;
                              for (var i = 0; i < 42; i++) {
                                  // < 7 * 6
                                  day = new Date(firstDate.getFullYear(), firstDate.getMonth(), firstDate.getDate() + i);
                                  days.push({
                                      date: day,
                                      label: dateFilter(day, this.format),
                                      selected: picker.$date && this.isSelected(day),
                                      muted: day.getMonth() !== viewDate.month,
                                      disabled: this.isDisabled(day)
                                  });
                              }
                              scope.title = dateFilter(firstDayOfMonth, 'MMMM yyyy');
                              scope.showLabels = true;
                              scope.labels = weekDaysLabelsHtml;
                              scope.rows = split(days, this.split);
                              this.built = true;
                          },
                          isSelected: function (date) {
                              return picker.$date && date.getFullYear() === picker.$date.getFullYear() && date.getMonth() === picker.$date.getMonth() && date.getDate() === picker.$date.getDate();
                          },
                          isDisabled: function (date) {
                              var time = date.getTime();
                              // Disabled because of min/max date.
                              if (time < options.minDate || time > options.maxDate)
                                  return true;
                              // Disabled due to being a disabled day of the week
                              if (options.daysOfWeekDisabled.indexOf(date.getDay()) !== -1)
                                  return true;
                              // Disabled because of disabled date range.
                              if (options.disabledDateRanges) {
                                  for (var i = 0; i < options.disabledDateRanges.length; i++) {
                                      if (time >= options.disabledDateRanges[i].start) {
                                          if (time <= options.disabledDateRanges[i].end)
                                              return true;
                                          // The disabledDateRanges is expected to be sorted, so if time >= start,
                                          // we know it's not disabled.
                                          return false;
                                      }
                                  }
                              }
                              return false;
                          },
                          onKeyDown: function (evt) {
                              var actualTime = picker.$date.getTime();
                              var newDate;
                              if (evt.keyCode === 37)
                                  newDate = new Date(actualTime - 1 * 86400000);
                              else if (evt.keyCode === 38)
                                  newDate = new Date(actualTime - 7 * 86400000);
                              else if (evt.keyCode === 39)
                                  newDate = new Date(actualTime + 1 * 86400000);
                              else if (evt.keyCode === 40)
                                  newDate = new Date(actualTime + 7 * 86400000);
                              if (!this.isDisabled(newDate))
                                  picker.select(newDate, true);
                          }
                      },
                      {
                          name: 'month',
                          format: 'MMM',
                          split: 4,
                          steps: { year: 1 },
                          update: function (date, force) {
                              if (!this.built || date.getFullYear() !== viewDate.year) {
                                  angular.extend(viewDate, {
                                      year: picker.$date.getFullYear(),
                                      month: picker.$date.getMonth(),
                                      date: picker.$date.getDate()
                                  });
                                  picker.$build();
                              } else if (date.getMonth() !== viewDate.month) {
                                  angular.extend(viewDate, {
                                      month: picker.$date.getMonth(),
                                      date: picker.$date.getDate()
                                  });
                                  picker.$updateSelected();
                              }
                          },
                          build: function () {
                              var firstMonth = new Date(viewDate.year, 0, 1);
                              var months = [], month;
                              for (var i = 0; i < 12; i++) {
                                  month = new Date(viewDate.year, i, 1);
                                  months.push({
                                      date: month,
                                      label: dateFilter(month, this.format),
                                      selected: picker.$isSelected(month),
                                      disabled: this.isDisabled(month)
                                  });
                              }
                              scope.title = dateFilter(month, 'yyyy');
                              scope.showLabels = false;
                              scope.rows = split(months, this.split);
                              this.built = true;
                          },
                          isSelected: function (date) {
                              return picker.$date && date.getFullYear() === picker.$date.getFullYear() && date.getMonth() === picker.$date.getMonth();
                          },
                          isDisabled: function (date) {
                              var lastDate = +new Date(date.getFullYear(), date.getMonth() + 1, 0);
                              return lastDate < options.minDate || date.getTime() > options.maxDate;
                          },
                          onKeyDown: function (evt) {
                              var actualMonth = picker.$date.getMonth();
                              var newDate = new Date(picker.$date);
                              if (evt.keyCode === 37)
                                  newDate.setMonth(actualMonth - 1);
                              else if (evt.keyCode === 38)
                                  newDate.setMonth(actualMonth - 4);
                              else if (evt.keyCode === 39)
                                  newDate.setMonth(actualMonth + 1);
                              else if (evt.keyCode === 40)
                                  newDate.setMonth(actualMonth + 4);
                              if (!this.isDisabled(newDate))
                                  picker.select(newDate, true);
                          }
                      },
                      {
                          name: 'year',
                          format: 'yyyy',
                          split: 4,
                          steps: { year: 12 },
                          update: function (date, force) {
                              if (!this.built || force || parseInt(date.getFullYear() / 20, 10) !== parseInt(viewDate.year / 20, 10)) {
                                  angular.extend(viewDate, {
                                      year: picker.$date.getFullYear(),
                                      month: picker.$date.getMonth(),
                                      date: picker.$date.getDate()
                                  });
                                  picker.$build();
                              } else if (date.getFullYear() !== viewDate.year) {
                                  angular.extend(viewDate, {
                                      year: picker.$date.getFullYear(),
                                      month: picker.$date.getMonth(),
                                      date: picker.$date.getDate()
                                  });
                                  picker.$updateSelected();
                              }
                          },
                          build: function () {
                              var firstYear = viewDate.year - viewDate.year % (this.split * 3);
                              var years = [], year;
                              for (var i = 0; i < 12; i++) {
                                  year = new Date(firstYear + i, 0, 1);
                                  years.push({
                                      date: year,
                                      label: dateFilter(year, this.format),
                                      selected: picker.$isSelected(year),
                                      disabled: this.isDisabled(year)
                                  });
                              }
                              scope.title = years[0].label + '-' + years[years.length - 1].label;
                              scope.showLabels = false;
                              scope.rows = split(years, this.split);
                              this.built = true;
                          },
                          isSelected: function (date) {
                              return picker.$date && date.getFullYear() === picker.$date.getFullYear();
                          },
                          isDisabled: function (date) {
                              var lastDate = +new Date(date.getFullYear() + 1, 0, 0);
                              return lastDate < options.minDate || date.getTime() > options.maxDate;
                          },
                          onKeyDown: function (evt) {
                              var actualYear = picker.$date.getFullYear(), newDate = new Date(picker.$date);
                              if (evt.keyCode === 37)
                                  newDate.setYear(actualYear - 1);
                              else if (evt.keyCode === 38)
                                  newDate.setYear(actualYear - 4);
                              else if (evt.keyCode === 39)
                                  newDate.setYear(actualYear + 1);
                              else if (evt.keyCode === 40)
                                  newDate.setYear(actualYear + 4);
                              if (!this.isDisabled(newDate))
                                  picker.select(newDate, true);
                          }
                      }
                  ];
                  return {
                      views: options.minView ? Array.prototype.slice.call(views, options.minView) : views,
                      viewDate: viewDate
                  };
              };
          }
        ];
    });

    // Source: date-parser.js
    angular.module('mgcrea.ngStrap.helpers.dateParser', []).provider('$dateParser', [
      '$localeProvider',
      function ($localeProvider) {
          var proto = Date.prototype;
          function isNumeric(n) {
              return !isNaN(parseFloat(n)) && isFinite(n);
          }
          var defaults = this.defaults = {
              format: 'shortDate',
              strict: false
          };
          this.$get = [
            '$locale',
            'dateFilter',
            function ($locale, dateFilter) {
                var DateParserFactory = function (config) {
                    var options = angular.extend({}, defaults, config);
                    var $dateParser = {};
                    var regExpMap = {
                        'sss': '[0-9]{3}',
                        'ss': '[0-5][0-9]',
                        's': options.strict ? '[1-5]?[0-9]' : '[0-9]|[0-5][0-9]',
                        'mm': '[0-5][0-9]',
                        'm': options.strict ? '[1-5]?[0-9]' : '[0-9]|[0-5][0-9]',
                        'HH': '[01][0-9]|2[0-3]',
                        'H': options.strict ? '1?[0-9]|2[0-3]' : '[01]?[0-9]|2[0-3]',
                        'hh': '[0][1-9]|[1][012]',
                        'h': options.strict ? '[1-9]|1[012]' : '0?[1-9]|1[012]',
                        'a': 'AM|PM',
                        'EEEE': $locale.DATETIME_FORMATS.DAY.join('|'),
                        'EEE': $locale.DATETIME_FORMATS.SHORTDAY.join('|'),
                        'dd': '0[1-9]|[12][0-9]|3[01]',
                        'd': options.strict ? '[1-9]|[1-2][0-9]|3[01]' : '0?[1-9]|[1-2][0-9]|3[01]',
                        'MMMM': $locale.DATETIME_FORMATS.MONTH.join('|'),
                        'MMM': $locale.DATETIME_FORMATS.SHORTMONTH.join('|'),
                        'MM': '0[1-9]|1[012]',
                        'M': options.strict ? '[1-9]|1[012]' : '0?[1-9]|1[012]',
                        'yyyy': '[1]{1}[0-9]{3}|[2]{1}[0-9]{3}',
                        'yy': '[0-9]{2}',
                        'y': options.strict ? '-?(0|[1-9][0-9]{0,3})' : '-?0*[0-9]{1,4}'
                    };
                    var setFnMap = {
                        'sss': proto.setMilliseconds,
                        'ss': proto.setSeconds,
                        's': proto.setSeconds,
                        'mm': proto.setMinutes,
                        'm': proto.setMinutes,
                        'HH': proto.setHours,
                        'H': proto.setHours,
                        'hh': proto.setHours,
                        'h': proto.setHours,
                        'dd': proto.setDate,
                        'd': proto.setDate,
                        'a': function (value) {
                            var hours = this.getHours();
                            return this.setHours(value.match(/pm/i) ? hours + 12 : hours);
                        },
                        'MMMM': function (value) {
                            return this.setMonth($locale.DATETIME_FORMATS.MONTH.indexOf(value));
                        },
                        'MMM': function (value) {
                            return this.setMonth($locale.DATETIME_FORMATS.SHORTMONTH.indexOf(value));
                        },
                        'MM': function (value) {
                            return this.setMonth(1 * value - 1);
                        },
                        'M': function (value) {
                            return this.setMonth(1 * value - 1);
                        },
                        'yyyy': proto.setFullYear,
                        'yy': function (value) {
                            return this.setFullYear(2000 + 1 * value);
                        },
                        'y': proto.setFullYear
                    };
                    var regex, setMap;
                    $dateParser.init = function () {
                        $dateParser.$format = $locale.DATETIME_FORMATS[options.format] || options.format;
                        regex = regExpForFormat($dateParser.$format);
                        setMap = setMapForFormat($dateParser.$format);
                    };
                    $dateParser.isValid = function (date) {
                        if (angular.isDate(date))
                            return !isNaN(date.getTime());
                        return regex.test(date);
                    };
                    $dateParser.parse = function (value, baseDate, format) {
                        if (angular.isDate(value))
                            value = dateFilter(value, format || $dateParser.$format);
                        var formatRegex = format ? regExpForFormat(format) : regex;
                        var formatSetMap = format ? setMapForFormat(format) : setMap;
                        var matches = formatRegex.exec(value);
                        if (!matches)
                            return false;
                        var date = baseDate || new Date(0, 0, 1);
                        for (var i = 0; i < matches.length - 1; i++) {
                            formatSetMap[i] && formatSetMap[i].call(date, matches[i + 1]);
                        }
                        return date;
                    };
                    // Private functions
                    function setMapForFormat(format) {
                        var keys = Object.keys(setFnMap), i;
                        var map = [], sortedMap = [];
                        // Map to setFn
                        var clonedFormat = format;
                        for (i = 0; i < keys.length; i++) {
                            if (format.split(keys[i]).length > 1) {
                                var index = clonedFormat.search(keys[i]);
                                format = format.split(keys[i]).join('');
                                if (setFnMap[keys[i]])
                                    map[index] = setFnMap[keys[i]];
                            }
                        }
                        // Sort result map
                        angular.forEach(map, function (v) {
                            if (v)
                                sortedMap.push(v);
                        });
                        return sortedMap;
                    }
                    function escapeReservedSymbols(text) {
                        return text.replace(/\//g, '[\\/]').replace('/-/g', '[-]').replace(/\./g, '[.]').replace(/\\s/g, '[\\s]');
                    }
                    function regExpForFormat(format) {
                        var keys = Object.keys(regExpMap), i;
                        var re = format;
                        // Abstract replaces to avoid collisions
                        for (i = 0; i < keys.length; i++) {
                            re = re.split(keys[i]).join('${' + i + '}');
                        }
                        // Replace abstracted values
                        for (i = 0; i < keys.length; i++) {
                            re = re.split('${' + i + '}').join('(' + regExpMap[keys[i]] + ')');
                        }
                        format = escapeReservedSymbols(format);
                        return new RegExp('^' + re + '$', ['i']);
                    }
                    $dateParser.init();
                    return $dateParser;
                };
                return DateParserFactory;
            }
          ];
      }
    ]);

    // Source: dimensions.js
    angular.module('mgcrea.ngStrap.helpers.dimensions', []).factory('dimensions', [
      '$document',
      '$window',
      function ($document, $window) {
          var jqLite = angular.element;
          var fn = {};
          /**
           * Test the element nodeName
           * @param element
           * @param name
           */
          var nodeName = fn.nodeName = function (element, name) {
              return element.nodeName && element.nodeName.toLowerCase() === name.toLowerCase();
          };
          /**
           * Returns the element computed style
           * @param element
           * @param prop
           * @param extra
           */
          fn.css = function (element, prop, extra) {
              var value;
              if (element.currentStyle) {
                  //IE
                  value = element.currentStyle[prop];
              } else if (window.getComputedStyle) {
                  value = window.getComputedStyle(element)[prop];
              } else {
                  value = element.style[prop];
              }
              return extra === true ? parseFloat(value) || 0 : value;
          };
          /**
           * Provides read-only equivalent of jQuery's offset function:
           * @required-by bootstrap-tooltip, bootstrap-affix
           * @url http://api.jquery.com/offset/
           * @param element
           */
          fn.offset = function (element) {
              var boxRect = element.getBoundingClientRect();
              var docElement = element.ownerDocument;
              return {
                  width: boxRect.width || element.offsetWidth,
                  height: boxRect.height || element.offsetHeight,
                  top: boxRect.top + (window.pageYOffset || docElement.documentElement.scrollTop) - (docElement.documentElement.clientTop || 0),
                  left: boxRect.left + (window.pageXOffset || docElement.documentElement.scrollLeft) - (docElement.documentElement.clientLeft || 0)
              };
          };
          /**
           * Provides read-only equivalent of jQuery's position function
           * @required-by bootstrap-tooltip, bootstrap-affix
           * @url http://api.jquery.com/offset/
           * @param element
           */
          fn.position = function (element) {
              var offsetParentRect = {
                  top: 0,
                  left: 0
              }, offsetParentElement, offset;
              // Fixed elements are offset from window (parentOffset = {top:0, left: 0}, because it is it's only offset parent
              if (fn.css(element, 'position') === 'fixed') {
                  // We assume that getBoundingClientRect is available when computed position is fixed
                  offset = element.getBoundingClientRect();
              } else {
                  // Get *real* offsetParentElement
                  offsetParentElement = offsetParent(element);
                  offset = fn.offset(element);
                  // Get correct offsets
                  offset = fn.offset(element);
                  if (!nodeName(offsetParentElement, 'html')) {
                      offsetParentRect = fn.offset(offsetParentElement);
                  }
                  // Add offsetParent borders
                  offsetParentRect.top += fn.css(offsetParentElement, 'borderTopWidth', true);
                  offsetParentRect.left += fn.css(offsetParentElement, 'borderLeftWidth', true);
              }
              // Subtract parent offsets and element margins
              return {
                  width: element.offsetWidth,
                  height: element.offsetHeight,
                  top: offset.top - offsetParentRect.top - fn.css(element, 'marginTop', true),
                  left: offset.left - offsetParentRect.left - fn.css(element, 'marginLeft', true)
              };
          };
          /**
           * Returns the closest, non-statically positioned offsetParent of a given element
           * @required-by fn.position
           * @param element
           */
          var offsetParent = function offsetParentElement(element) {
              var docElement = element.ownerDocument;
              var offsetParent = element.offsetParent || docElement;
              if (nodeName(offsetParent, '#document'))
                  return docElement.documentElement;
              while (offsetParent && !nodeName(offsetParent, 'html') && fn.css(offsetParent, 'position') === 'static') {
                  offsetParent = offsetParent.offsetParent;
              }
              return offsetParent || docElement.documentElement;
          };
          /**
           * Provides equivalent of jQuery's height function
           * @required-by bootstrap-affix
           * @url http://api.jquery.com/height/
           * @param element
           * @param outer
           */
          fn.height = function (element, outer) {
              var value = element.offsetHeight;
              if (outer) {
                  value += fn.css(element, 'marginTop', true) + fn.css(element, 'marginBottom', true);
              } else {
                  value -= fn.css(element, 'paddingTop', true) + fn.css(element, 'paddingBottom', true) + fn.css(element, 'borderTopWidth', true) + fn.css(element, 'borderBottomWidth', true);
              }
              return value;
          };
          /**
           * Provides equivalent of jQuery's width function
           * @required-by bootstrap-affix
           * @url http://api.jquery.com/width/
           * @param element
           * @param outer
           */
          fn.width = function (element, outer) {
              var value = element.offsetWidth;
              if (outer) {
                  value += fn.css(element, 'marginLeft', true) + fn.css(element, 'marginRight', true);
              } else {
                  value -= fn.css(element, 'paddingLeft', true) + fn.css(element, 'paddingRight', true) + fn.css(element, 'borderLeftWidth', true) + fn.css(element, 'borderRightWidth', true);
              }
              return value;
          };
          return fn;
      }
    ]);


    // Source: select.js
    angular.module('mgcrea.ngStrap.select', [
      'mgcrea.ngStrap.tooltip',
      'mgcrea.ngStrap.helpers.parseOptions'
    ]).provider('$select', function () {
        var defaults = this.defaults = {
            animation: 'am-fade',
            prefixClass: 'select',
            prefixEvent: '$select',
            placement: 'bottom-left',
            template: 'select/select.tpl.html',
            trigger: 'focus',
            container: false,
            keyboard: true,
            html: false,
            delay: 0,
            multiple: false,
            allNoneButtons: false,
            sort: true,
            caretHtml: '&nbsp;<span class="caret"></span>',
            placeholder: '- SELECT -',
            maxLength: 3,
            maxLengthHtml: 'selected',
            iconCheckmark: 'glyphicon glyphicon-ok'
        };
        this.$get = [
          '$window',
          '$document',
          '$rootScope',
          '$tooltip',
          function ($window, $document, $rootScope, $tooltip) {
              var bodyEl = angular.element($window.document.body);
              var isNative = /(ip(a|o)d|iphone|android)/gi.test($window.navigator.userAgent);
              var isTouch = 'createTouch' in $window.document && isNative;
              function SelectFactory(element, controller, config) {
                  var $select = {};
                  // Common vars
                  var options = angular.extend({}, defaults, config);
                  options.width = element.prop('offsetWidth');
                  $select = $tooltip(element, options);
                  var scope = $select.$scope;
                  scope.$matches = [];
                  scope.$activeIndex = 0;
                  scope.$isMultiple = options.multiple;
                  scope.$showAllNoneButtons = options.allNoneButtons && options.multiple;
                  scope.$iconCheckmark = options.iconCheckmark;
                  scope.$activate = function (index) {
                      scope.$$postDigest(function () {
                          $select.activate(index);
                      });
                  };
                  scope.$select = function (match, evt) {

                      scope.$$postDigest(function () {
                          $select.select(scope.$matches.indexOf(match));
                      });
                  };
                  scope.$isVisible = function () {
                      return $select.$isVisible();
                  };
                  scope.$isActive = function (index) {
                      return $select.$isActive(index);
                  };
                  scope.$selectAll = function () {
                      for (var i = 0; i < scope.$matches.length; i++) {
                          if (!scope.$isActive(i)) {
                              scope.$select(i);
                          }
                      }
                  };
                  scope.$selectNone = function () {
                      for (var i = 0; i < scope.$matches.length; i++) {
                          if (scope.$isActive(i)) {
                              scope.$select(i);
                          }
                      }
                  };
                  // Public methods
                  $select.update = function (matches) {
                      scope.$matches = matches;
                      scope.$matches.unshift({ "label": "- Select -", "value": null });
                      $select.$updateActiveIndex();
                  };
                  $select.activate = function (index) {
                      if (options.multiple) {
                          scope.$activeIndex.sort();
                          $select.$isActive(index) ? scope.$activeIndex.splice(scope.$activeIndex.indexOf(index), 1) : scope.$activeIndex.push(index);
                          if (options.sort)
                              scope.$activeIndex.sort();
                      } else {
                          scope.$activeIndex = index;
                      }
                      return scope.$activeIndex;
                  };
                  $select.select = function (index) {
                      var value = scope.$matches[index].value;
                      scope.$apply(function () {
                          $select.activate(index);
                          if (options.multiple) {
                              controller.$setViewValue(scope.$activeIndex.map(function (index) {
                                  return scope.$matches[index].value;
                              }));
                          } else {
                              controller.$setViewValue(value);
                              // Hide if single select
                              $select.hide();
                          }
                      });
                      // Emit event
                      scope.$emit(options.prefixEvent + '.select', value, index);
                  };
                  // Protected methods
                  $select.$updateActiveIndex = function () {
                      if (controller.$modelValue && scope.$matches.length) {
                          if (options.multiple && angular.isArray(controller.$modelValue)) {
                              scope.$activeIndex = controller.$modelValue.map(function (value) {
                                  return $select.$getIndex(value);
                              });
                          } else {
                              scope.$activeIndex = $select.$getIndex(controller.$modelValue);
                          }
                      } else if (scope.$activeIndex >= scope.$matches.length) {
                          scope.$activeIndex = options.multiple ? [] : 0;
                      }
                  };
                  $select.$isVisible = function () {
                      if (!options.minLength || !controller) {
                          return scope.$matches.length;
                      }
                      // minLength support
                      return scope.$matches.length && controller.$viewValue.length >= options.minLength;
                  };
                  $select.$isActive = function (index) {

                      if (options.multiple) {
                          return scope.$activeIndex.indexOf(index) !== -1;
                      } else {
                          return scope.$activeIndex === index;
                      }
                  };
                  $select.$getIndex = function (value) {
                      var l = scope.$matches.length, i = l;
                      if (!l)
                          return;
                      for (i = l; i--;) {
                          if (scope.$matches[i].value === value)
                              break;
                      }
                      if (i < 0)
                          return;
                      return i;
                  };
                  $select.$onMouseDown = function (evt) {
                      // Prevent blur on mousedown on .dropdown-menu
                      evt.preventDefault();
                      evt.stopPropagation();
                      // Emulate click for mobile devices
                      if (isTouch) {
                          var targetEl = angular.element(evt.target);
                          targetEl.triggerHandler('click');
                      }
                  };
                  $select.$onKeyDown = function (evt) {
                      if (!/(9|13|38|40)/.test(evt.keyCode))
                          return;
                      evt.preventDefault();
                      evt.stopPropagation();
                      // Select with enter
                      if (!options.multiple && (evt.keyCode === 13 || evt.keyCode === 9)) {
                          return $select.select(scope.$activeIndex);
                      }
                      // Navigate with keyboard
                      if (evt.keyCode === 38 && scope.$activeIndex > 0)
                          scope.$activeIndex--;
                      else if (evt.keyCode === 40 && scope.$activeIndex < scope.$matches.length - 1)
                          scope.$activeIndex++;
                      else if (angular.isUndefined(scope.$activeIndex))
                          scope.$activeIndex = 0;
                      scope.$digest();
                  };
                  // Overrides
                  var _show = $select.show;
                  $select.show = function () {
                      _show();
                      if (options.multiple) {
                          $select.$element.addClass('select-multiple');
                      }
                      setTimeout(function () {
                          $select.$element.on(isTouch ? 'touchstart' : 'mousedown', $select.$onMouseDown);
                          if (options.keyboard) {
                              element.on('keydown', $select.$onKeyDown);
                          }
                      });
                  };
                  var _hide = $select.hide;
                  $select.hide = function () {
                      $select.$element.off(isTouch ? 'touchstart' : 'mousedown', $select.$onMouseDown);
                      if (options.keyboard) {
                          element.off('keydown', $select.$onKeyDown);
                      }
                      _hide(true);
                  };
                  return $select;
              }
              SelectFactory.defaults = defaults;
              return SelectFactory;
          }
        ];
    }).directive('bsSelect', [
      '$window',
      '$parse',
      '$q',
      '$select',
      '$parseOptions',
      function ($window, $parse, $q, $select, $parseOptions) {
          var defaults = $select.defaults;
          return {
              restrict: 'EAC',
              require: 'ngModel',
              link: function postLink(scope, element, attr, controller) {
                  // Directive options
                  var options = { scope: scope };
                  angular.forEach([
                    'placement',
                    'container',
                    'delay',
                    'trigger',
                    'keyboard',
                    'html',
                    'animation',
                    'template',
                    'placeholder',
                    'multiple',
                    'allNoneButtons',
                    'maxLength',
                    'maxLengthHtml'
                  ], function (key) {
                      if (angular.isDefined(attr[key]))
                          options[key] = attr[key];
                  });
                  // Add support for select markup
                  if (element[0].nodeName.toLowerCase() === 'select') {
                      var inputEl = element;
                      inputEl.css('display', 'none');
                      element = angular.element('<button type="button" class="btn btn-default"></button>');
                      inputEl.after(element);
                  }
                  // Build proper ngOptions
                  var parsedOptions = $parseOptions(attr.ngOptions);
                  // Initialize select
                  var select = $select(element, controller, options);
                  // Watch ngOptions values before filtering for changes
                  var watchedOptions = parsedOptions.$match[7].replace(/\|.+/, '').trim();
                  scope.$watch(watchedOptions, function (newValue, oldValue) {
                      // console.warn('scope.$watch(%s)', watchedOptions, newValue, oldValue);
                      parsedOptions.valuesFn(scope, controller).then(function (values) {
                          select.update(values);
                          controller.$render();
                      });
                  }, true);
                  // Watch model for changes
                  scope.$watch(attr.ngModel, function (newValue, oldValue) {
                      // console.warn('scope.$watch(%s)', attr.ngModel, newValue, oldValue);
                      select.$updateActiveIndex();
                      controller.$render();
                  }, true);
                  // Model rendering in view
                  controller.$render = function () {
                      // console.warn('$render', element.attr('ng-model'), 'controller.$modelValue', typeof controller.$modelValue, controller.$modelValue, 'controller.$viewValue', typeof controller.$viewValue, controller.$viewValue);
                      var selected, index;
                      if (options.multiple && angular.isArray(controller.$modelValue)) {
                          selected = controller.$modelValue.map(function (value) {
                              index = select.$getIndex(value);
                              return angular.isDefined(index) ? select.$scope.$matches[index].label : false;
                          }).filter(angular.isDefined);
                          if (selected.length > (options.maxLength || defaults.maxLength)) {
                              selected = selected.length + ' ' + (options.maxLengthHtml || defaults.maxLengthHtml);
                          } else {
                              selected = selected.join(', ');
                          }
                      } else {
                          index = select.$getIndex(controller.$modelValue);
                          selected = angular.isDefined(index) ? select.$scope.$matches[index].label : false;
                      }
                      element.html((selected ? selected : attr.placeholder || defaults.placeholder) + defaults.caretHtml);
                  };
                  // Garbage collection
                  scope.$on('$destroy', function () {
                      select.destroy();
                      options = null;
                      select = null;
                  });
              }
          };
      }
    ]);



    // Source: tab.js
    angular.module('mgcrea.ngStrap.tab', [])

    .provider('$tab', function () {

        var defaults = this.defaults = {
            animation: 'am-fade',
            template: 'tab.tpl.html',
            navClass: 'nav-tabs',
            activeClass: 'active'
        };

        var controller = this.controller = function ($scope, $element, $attrs) {
            var self = this;

            // Attributes options
            self.$options = angular.copy(defaults);
            angular.forEach(['animation', 'navClass', 'activeClass'], function (key) {
                if (angular.isDefined($attrs[key])) self.$options[key] = $attrs[key];
            });

            // Publish options on scope
            $scope.$navClass = self.$options.navClass;
            $scope.$activeClass = self.$options.activeClass;

            self.$panes = $scope.$panes = [];

            self.$viewChangeListeners = [];

            self.$push = function (pane) {
                self.$panes.push(pane);
            };
            //#EDITED
            self.select = function (i) {
                self.$panes[i].onSelect();
            }
            self.$panes.$active = 0;
            self.$setActive = $scope.$setActive = function (value) {
                self.$panes.$active = value;
                self.select(value);
                self.$viewChangeListeners.forEach(function (fn) {
                    fn();
                });
                try {
                    $scope.mainform.$setPristine();
                } catch (e) { }

            };

        };

        this.$get = function () {
            var $tab = {};
            $tab.defaults = defaults;
            $tab.controller = controller;
            return $tab;
        };

    })

    .directive('bsTabs', ["$window", "$animate", "$tab", function ($window, $animate, $tab) {

        var defaults = $tab.defaults;

        return {
            require: ['?ngModel', 'bsTabs'],
            transclude: true,
            scope: true,
            controller: ['$scope', '$element', '$attrs', $tab.controller],
            templateUrl: function (element, attr) {
                return attr.template || defaults.template;
            },
            link: function postLink(scope, element, attrs, controllers) {

                var ngModelCtrl = controllers[0];
                var bsTabsCtrl = controllers[1];

                if (ngModelCtrl) {

                    // Update the modelValue following
                    bsTabsCtrl.$viewChangeListeners.push(function () {
                        ngModelCtrl.$setViewValue(bsTabsCtrl.$panes.$active);
                    });

                    // modelValue -> $formatters -> viewValue
                    ngModelCtrl.$formatters.push(function (modelValue) {
                        // console.warn('$formatter("%s"): modelValue=%o (%o)', element.attr('ng-model'), modelValue, typeof modelValue);
                        bsTabsCtrl.$setActive(modelValue * 1);
                        return modelValue;
                    });

                }

            }
        };

    }])

    .directive('bsPane', ["$window", "$animate", "$sce", "$parse", function ($window, $animate, $sce, $parse) {

        return {
            require: ['^?ngModel', '^bsTabs'],
            //scope: true,
            scope: { //#EDITED
                title: '@',
                ngShow: '@',
                onSelect: '&select',
            },
            link: function postLink(scope, element, attrs, controllers) {

                var ngModelCtrl = controllers[0];
                var bsTabsCtrl = controllers[1];

                // Add base class
                element.addClass('tab-pane in');

                // Observe title attribute for change
                // attrs.$observe('title', function(newValue, oldValue) {
                // scope.title = $sce.trustAsHtml(newValue);
                // });
                if (attrs.ngShow === undefined) {
                    scope.show = true;
                } else {
                    scope.show = attrs.ngShow;
                }
                //attrs.$observe('ngShow', function (newValue, oldValue) {
                //    console.log(attrs.ngShow, newValue);
                //  if(newValue === undefined){
                //	  scope.show = true;
                //  } else {
                //      scope.show = newValue;
                //  }

                //});

                // Add animation class
                if (bsTabsCtrl.$options.animation) {
                    element.addClass(bsTabsCtrl.$options.animation);
                }

                // Push pane to parent bsTabs controller
                bsTabsCtrl.$push(scope);

                function render() {
                    var index = bsTabsCtrl.$panes.indexOf(scope);
                    var active = bsTabsCtrl.$panes.$active;
                    $animate[index === active ? 'addClass' : 'removeClass'](element, bsTabsCtrl.$options.activeClass);
                }


                bsTabsCtrl.$viewChangeListeners.push(function () {
                    render();
                });
                render();

            }
        };

    }]);


    // Source: timepicker.js
    angular.module('mgcrea.ngStrap.timepicker', [
      'mgcrea.ngStrap.helpers.dateParser',
      'mgcrea.ngStrap.tooltip'
    ]).provider('$timepicker', function () {
        var defaults = this.defaults = {
            animation: 'am-fade',
            prefixClass: 'timepicker',
            placement: 'bottom-left',
            template: 'timepicker.tpl.html',
            trigger: 'focus',
            container: false,
            keyboard: true,
            html: false,
            delay: 0,
            useNative: true,
            timeType: 'date',
            timeFormat: 'shortTime',
            modelTimeFormat: null,
            autoclose: false,
            minTime: -Infinity,
            maxTime: +Infinity,
            length: 3,
            hourStep: 1,
            minuteStep: 5,
            iconUp: 'glyphicon glyphicon-chevron-up',
            iconDown: 'glyphicon glyphicon-chevron-down',
            arrowBehavior: 'pager'
        };
        this.$get = [
          '$window',
          '$document',
          '$rootScope',
          '$sce',
          '$locale',
          'dateFilter',
          '$tooltip',
          function ($window, $document, $rootScope, $sce, $locale, dateFilter, $tooltip) {
              var bodyEl = angular.element($window.document.body);
              var isNative = /(ip(a|o)d|iphone|android)/gi.test($window.navigator.userAgent);
              var isTouch = 'createTouch' in $window.document && isNative;
              if (!defaults.lang)
                  defaults.lang = $locale.id;
              function timepickerFactory(element, controller, config) {
                  var $timepicker = $tooltip(element, angular.extend({}, defaults, config));
                  var parentScope = config.scope;
                  var options = $timepicker.$options;
                  var scope = $timepicker.$scope;
                  // View vars
                  var selectedIndex = 0;
                  var startDate = controller.$dateValue || new Date();
                  var viewDate = {
                      hour: startDate.getHours(),
                      meridian: startDate.getHours() < 12,
                      minute: startDate.getMinutes(),
                      second: startDate.getSeconds(),
                      millisecond: startDate.getMilliseconds()
                  };
                  var format = $locale.DATETIME_FORMATS[options.timeFormat] || options.timeFormat;
                  var formats = /(h+)([:\.])?(m+)[ ]?(a?)/i.exec(format).slice(1);
                  scope.$iconUp = options.iconUp;
                  scope.$iconDown = options.iconDown;
                  // Scope methods
                  scope.$select = function (date, index) {
                      $timepicker.select(date, index);
                  };
                  scope.$moveIndex = function (value, index) {
                      $timepicker.$moveIndex(value, index);
                  };
                  scope.$switchMeridian = function (date) {
                      $timepicker.switchMeridian(date);
                  };
                  // Public methods
                  $timepicker.update = function (date) {
                      // console.warn('$timepicker.update() newValue=%o', date);
                      if (angular.isDate(date) && !isNaN(date.getTime())) {
                          $timepicker.$date = date;
                          angular.extend(viewDate, {
                              hour: date.getHours(),
                              minute: date.getMinutes(),
                              second: date.getSeconds(),
                              millisecond: date.getMilliseconds()
                          });
                          $timepicker.$build();
                      } else if (!$timepicker.$isBuilt) {
                          $timepicker.$build();
                      }
                  };
                  $timepicker.select = function (date, index, keep) {
                      // console.warn('$timepicker.select', date, scope.$mode);
                      if (!controller.$dateValue || isNaN(controller.$dateValue.getTime()))
                          controller.$dateValue = new Date(1970, 0, 1);
                      if (!angular.isDate(date))
                          date = new Date(date);
                      if (index === 0)
                          controller.$dateValue.setHours(date.getHours());
                      else if (index === 1)
                          controller.$dateValue.setMinutes(date.getMinutes());
                      controller.$setViewValue(controller.$dateValue);
                      controller.$render();
                      if (options.autoclose && !keep) {
                          $timepicker.hide(true);
                      }
                  };
                  $timepicker.switchMeridian = function (date) {
                      var hours = (date || controller.$dateValue).getHours();
                      controller.$dateValue.setHours(hours < 12 ? hours + 12 : hours - 12);
                      controller.$setViewValue(controller.$dateValue);
                      controller.$render();
                  };
                  // Protected methods
                  $timepicker.$build = function () {
                      // console.warn('$timepicker.$build() viewDate=%o', viewDate);
                      var i, midIndex = scope.midIndex = parseInt(options.length / 2, 10);
                      var hours = [], hour;
                      for (i = 0; i < options.length; i++) {
                          hour = new Date(1970, 0, 1, viewDate.hour - (midIndex - i) * options.hourStep);
                          hours.push({
                              date: hour,
                              label: dateFilter(hour, formats[0]),
                              selected: $timepicker.$date && $timepicker.$isSelected(hour, 0),
                              disabled: $timepicker.$isDisabled(hour, 0)
                          });
                      }
                      var minutes = [], minute;
                      for (i = 0; i < options.length; i++) {
                          minute = new Date(1970, 0, 1, 0, viewDate.minute - (midIndex - i) * options.minuteStep);
                          minutes.push({
                              date: minute,
                              label: dateFilter(minute, formats[2]),
                              selected: $timepicker.$date && $timepicker.$isSelected(minute, 1),
                              disabled: $timepicker.$isDisabled(minute, 1)
                          });
                      }
                      var rows = [];
                      for (i = 0; i < options.length; i++) {
                          rows.push([
                            hours[i],
                            minutes[i]
                          ]);
                      }
                      scope.rows = rows;
                      scope.showAM = !!formats[3];
                      scope.isAM = ($timepicker.$date || hours[midIndex].date).getHours() < 12;
                      scope.timeSeparator = formats[1];
                      $timepicker.$isBuilt = true;
                  };
                  $timepicker.$isSelected = function (date, index) {
                      if (!$timepicker.$date)
                          return false;
                      else if (index === 0) {
                          return date.getHours() === $timepicker.$date.getHours();
                      } else if (index === 1) {
                          return date.getMinutes() === $timepicker.$date.getMinutes();
                      }
                  };
                  $timepicker.$isDisabled = function (date, index) {
                      var selectedTime;
                      if (index === 0) {
                          selectedTime = date.getTime() + viewDate.minute * 60000;
                      } else if (index === 1) {
                          selectedTime = date.getTime() + viewDate.hour * 3600000;
                      }
                      return selectedTime < options.minTime * 1 || selectedTime > options.maxTime * 1;
                  };
                  scope.$arrowAction = function (value, index) {
                      if (options.arrowBehavior === 'picker') {
                          $timepicker.$setTimeByStep(value, index);
                      } else {
                          $timepicker.$moveIndex(value, index);
                      }
                  };
                  $timepicker.$setTimeByStep = function (value, index) {
                      var newDate = new Date($timepicker.$date);
                      var hours = newDate.getHours(), hoursLength = dateFilter(newDate, 'h').length;
                      var minutes = newDate.getMinutes(), minutesLength = dateFilter(newDate, 'mm').length;
                      if (index === 0) {
                          newDate.setHours(hours - parseInt(options.hourStep, 10) * value);
                      } else {
                          newDate.setMinutes(minutes - parseInt(options.minuteStep, 10) * value);
                      }
                      $timepicker.select(newDate, index, true);
                      parentScope.$digest();
                  };
                  $timepicker.$moveIndex = function (value, index) {
                      var targetDate;
                      if (index === 0) {
                          targetDate = new Date(1970, 0, 1, viewDate.hour + value * options.length, viewDate.minute);
                          angular.extend(viewDate, { hour: targetDate.getHours() });
                      } else if (index === 1) {
                          targetDate = new Date(1970, 0, 1, viewDate.hour, viewDate.minute + value * options.length * options.minuteStep);
                          angular.extend(viewDate, { minute: targetDate.getMinutes() });
                      }
                      $timepicker.$build();
                  };
                  $timepicker.$onMouseDown = function (evt) {
                      // Prevent blur on mousedown on .dropdown-menu
                      if (evt.target.nodeName.toLowerCase() !== 'input')
                          evt.preventDefault();
                      evt.stopPropagation();
                      // Emulate click for mobile devices
                      if (isTouch) {
                          var targetEl = angular.element(evt.target);
                          if (targetEl[0].nodeName.toLowerCase() !== 'button') {
                              targetEl = targetEl.parent();
                          }
                          targetEl.triggerHandler('click');
                      }
                  };
                  $timepicker.$onKeyDown = function (evt) {
                      if (!/(38|37|39|40|13)/.test(evt.keyCode) || evt.shiftKey || evt.altKey)
                          return;
                      evt.preventDefault();
                      evt.stopPropagation();
                      // Close on enter
                      if (evt.keyCode === 13)
                          return $timepicker.hide(true);
                      // Navigate with keyboard
                      var newDate = new Date($timepicker.$date);
                      var hours = newDate.getHours(), hoursLength = dateFilter(newDate, 'h').length;
                      var minutes = newDate.getMinutes(), minutesLength = dateFilter(newDate, 'mm').length;
                      var lateralMove = /(37|39)/.test(evt.keyCode);
                      var count = 2 + !!formats[3] * 1;
                      // Navigate indexes (left, right)
                      if (lateralMove) {
                          if (evt.keyCode === 37)
                              selectedIndex = selectedIndex < 1 ? count - 1 : selectedIndex - 1;
                          else if (evt.keyCode === 39)
                              selectedIndex = selectedIndex < count - 1 ? selectedIndex + 1 : 0;
                      }
                      // Update values (up, down)
                      var selectRange = [
                          0,
                          hoursLength
                      ];
                      if (selectedIndex === 0) {
                          if (evt.keyCode === 38)
                              newDate.setHours(hours - parseInt(options.hourStep, 10));
                          else if (evt.keyCode === 40)
                              newDate.setHours(hours + parseInt(options.hourStep, 10));
                          selectRange = [
                            0,
                            hoursLength
                          ];
                      } else if (selectedIndex === 1) {
                          if (evt.keyCode === 38)
                              newDate.setMinutes(minutes - parseInt(options.minuteStep, 10));
                          else if (evt.keyCode === 40)
                              newDate.setMinutes(minutes + parseInt(options.minuteStep, 10));
                          selectRange = [
                            hoursLength + 1,
                            hoursLength + 1 + minutesLength
                          ];
                      } else if (selectedIndex === 2) {
                          if (!lateralMove)
                              $timepicker.switchMeridian();
                          selectRange = [
                            hoursLength + 1 + minutesLength + 1,
                            hoursLength + 1 + minutesLength + 3
                          ];
                      }
                      $timepicker.select(newDate, selectedIndex, true);
                      createSelection(selectRange[0], selectRange[1]);
                      parentScope.$digest();
                  };
                  // Private
                  function createSelection(start, end) {
                      if (element[0].createTextRange) {
                          var selRange = element[0].createTextRange();
                          selRange.collapse(true);
                          selRange.moveStart('character', start);
                          selRange.moveEnd('character', end);
                          selRange.select();
                      } else if (element[0].setSelectionRange) {
                          element[0].setSelectionRange(start, end);
                      } else if (angular.isUndefined(element[0].selectionStart)) {
                          element[0].selectionStart = start;
                          element[0].selectionEnd = end;
                      }
                  }
                  function focusElement() {
                      element[0].focus();
                  }
                  // Overrides
                  var _init = $timepicker.init;
                  $timepicker.init = function () {
                      if (isNative && options.useNative) {
                          element.prop('type', 'time');
                          element.css('-webkit-appearance', 'textfield');
                          return;
                      } else if (isTouch) {
                          element.prop('type', 'text');
                          element.attr('readonly', 'true');
                          element.on('click', focusElement);
                      }
                      _init();
                  };
                  var _destroy = $timepicker.destroy;
                  $timepicker.destroy = function () {
                      if (isNative && options.useNative) {
                          element.off('click', focusElement);
                      }
                      _destroy();
                  };
                  var _show = $timepicker.show;
                  $timepicker.show = function () {
                      _show();
                      setTimeout(function () {
                          $timepicker.$element.on(isTouch ? 'touchstart' : 'mousedown', $timepicker.$onMouseDown);
                          if (options.keyboard) {
                              element.on('keydown', $timepicker.$onKeyDown);
                          }
                      });
                  };
                  var _hide = $timepicker.hide;
                  $timepicker.hide = function (blur) {
                      $timepicker.$element.off(isTouch ? 'touchstart' : 'mousedown', $timepicker.$onMouseDown);
                      if (options.keyboard) {
                          element.off('keydown', $timepicker.$onKeyDown);
                      }
                      _hide(blur);
                  };
                  return $timepicker;
              }
              timepickerFactory.defaults = defaults;
              return timepickerFactory;
          }
        ];
    }).directive('bsTimepicker', [
      '$window',
      '$parse',
      '$q',
      '$locale',
      'dateFilter',
      '$timepicker',
      '$dateParser',
      '$timeout',
      function ($window, $parse, $q, $locale, dateFilter, $timepicker, $dateParser, $timeout) {
          var defaults = $timepicker.defaults;
          var isNative = /(ip(a|o)d|iphone|android)/gi.test($window.navigator.userAgent);
          var requestAnimationFrame = $window.requestAnimationFrame || $window.setTimeout;
          return {
              restrict: 'EAC',
              require: 'ngModel',
              link: function postLink(scope, element, attr, controller) {
                  // Directive options
                  var options = {
                      scope: scope,
                      controller: controller
                  };
                  angular.forEach([
                    'placement',
                    'container',
                    'delay',
                    'trigger',
                    'keyboard',
                    'html',
                    'animation',
                    'template',
                    'autoclose',
                    'timeType',
                    'timeFormat',
                    'modelTimeFormat',
                    'useNative',
                    'hourStep',
                    'minuteStep',
                    'length',
                    'arrowBehavior'
                  ], function (key) {
                      if (angular.isDefined(attr[key]))
                          options[key] = attr[key];
                  });
                  // Visibility binding support
                  attr.bsShow && scope.$watch(attr.bsShow, function (newValue, oldValue) {
                      if (!timepicker || !angular.isDefined(newValue))
                          return;
                      if (angular.isString(newValue))
                          newValue = newValue.match(',?(timepicker),?');
                      newValue === true ? timepicker.show() : timepicker.hide();
                  });
                  // Initialize timepicker
                  if (isNative && (options.useNative || defaults.useNative))
                      options.timeFormat = 'HH:mm';
                  var timepicker = $timepicker(element, controller, options);
                  options = timepicker.$options;
                  // Initialize parser
                  var dateParser = $dateParser({
                      format: options.timeFormat,
                      lang: options.lang
                  });
                  // Observe attributes for changes
                  angular.forEach([
                    'minTime',
                    'maxTime'
                  ], function (key) {
                      // console.warn('attr.$observe(%s)', key, attr[key]);
                      angular.isDefined(attr[key]) && attr.$observe(key, function (newValue) {
                          if (newValue === 'now') {
                              timepicker.$options[key] = new Date().setFullYear(1970, 0, 1);
                          } else if (angular.isString(newValue) && newValue.match(/^".+"$/)) {
                              timepicker.$options[key] = +new Date(newValue.substr(1, newValue.length - 2));
                          } else {
                              timepicker.$options[key] = dateParser.parse(newValue, new Date(1970, 0, 1, 0));
                          }
                          !isNaN(timepicker.$options[key]) && timepicker.$build();
                      });
                  });
                  // Watch model for changes
                  scope.$watch(attr.ngModel, function (newValue, oldValue) {
                      // console.warn('scope.$watch(%s)', attr.ngModel, newValue, oldValue, controller.$dateValue);
                      timepicker.update(controller.$dateValue);
                  }, true);
                  // viewValue -> $parsers -> modelValue
                  controller.$parsers.unshift(function (viewValue) {
                      // console.warn('$parser("%s"): viewValue=%o', element.attr('ng-model'), viewValue);
                      // Null values should correctly reset the model value & validity
                      if (!viewValue) {
                          controller.$setValidity('date', true);
                          return;
                      }
                      var parsedTime = dateParser.parse(viewValue, controller.$dateValue);
                      if (!parsedTime || isNaN(parsedTime.getTime())) {
                          controller.$setValidity('date', false);
                      } else {
                          var isValid = parsedTime.getTime() >= options.minTime && parsedTime.getTime() <= options.maxTime;
                          controller.$setValidity('date', isValid);
                          // Only update the model when we have a valid date
                          if (isValid)
                              controller.$dateValue = parsedTime;
                      }
                      if (options.timeType === 'string') {
                          return dateFilter(parsedTime, options.modelTimeFormat || options.timeFormat);
                      } else if (options.timeType === 'number') {
                          return controller.$dateValue.getTime();
                      } else if (options.timeType === 'iso') {
                          return controller.$dateValue.toISOString();
                      } else {
                          return new Date(controller.$dateValue);
                      }
                  });
                  // modelValue -> $formatters -> viewValue
                  controller.$formatters.push(function (modelValue) {
                      // console.warn('$formatter("%s"): modelValue=%o (%o)', element.attr('ng-model'), modelValue, typeof modelValue);
                      var date;
                      if (angular.isUndefined(modelValue) || modelValue === null) {
                          date = NaN;
                      } else if (angular.isDate(modelValue)) {
                          date = modelValue;
                      } else if (options.timeType === 'string') {
                          date = dateParser.parse(modelValue, null, options.modelTimeFormat);
                      } else {
                          date = new Date(modelValue);
                      }

                      // Setup default value?
                      // if(isNaN(date.getTime())) date = new Date(new Date().setMinutes(0) + 36e5);

                      //ROSSU 10/5/2015 fix to wrong selected value when timepicker popsup
                      date = new Date(dateFilter(moment(date).utc()).format("YYYY-MM-DD hh:mm:ss A"));
                      //END FIX

                      controller.$dateValue = date;
                      return controller.$dateValue;
                  });
                  // viewValue -> element
                  controller.$render = function () {
                      // console.warn('$render("%s"): viewValue=%o', element.attr('ng-model'), controller.$viewValue);
                      element.val(!controller.$dateValue || isNaN(controller.$dateValue.getTime()) ? null : dateFilter(moment(controller.$dateValue).format("hh:mm A"), options.timeFormat));
                  };
                  // Garbage collection
                  scope.$on('$destroy', function () {
                      timepicker.destroy();
                      options = null;
                      timepicker = null;
                  });
              }
          };
      }
    ]);

    // Popover
    angular.module('mgcrea.ngStrap.popover', ['mgcrea.ngStrap.tooltip'])

      .provider('$popover', function () {

          var defaults = this.defaults = {
              animation: 'am-fade',
              customClass: '',
              container: false,
              target: false,
              placement: 'right',
              template: 'popover.tpl.html',
              contentTemplate: false,
              trigger: 'click',
              keyboard: true,
              html: false,
              title: '',
              content: '',
              delay: 0,
              autoClose: false
          };

          this.$get = ["$tooltip", function ($tooltip) {

              function PopoverFactory(element, config) {

                  // Common vars
                  var options = angular.extend({}, defaults, config);

                  var $popover = $tooltip(element, options);

                  // Support scope as string options [/*title, */content]
                  if (options.content) {
                      $popover.$scope.content = options.content;
                  }

                  return $popover;

              }

              return PopoverFactory;

          }];

      })

      .directive('bsPopover', ["$window", "$sce", "$popover", function ($window, $sce, $popover) {

          var requestAnimationFrame = $window.requestAnimationFrame || $window.setTimeout;

          return {
              restrict: 'EAC',
              scope: true,
              link: function postLink(scope, element, attr) {

                  // Directive options
                  var options = { scope: scope };
                  angular.forEach(['template', 'contentTemplate', 'placement', 'container', 'target', 'delay', 'trigger', 'keyboard', 'html', 'animation', 'customClass', 'autoClose', 'id'], function (key) {
                      if (angular.isDefined(attr[key])) options[key] = attr[key];
                  });

                  // Support scope as data-attrs
                  angular.forEach(['title', 'content'], function (key) {
                      attr[key] && attr.$observe(key, function (newValue, oldValue) {
                          scope[key] = $sce.trustAsHtml(newValue);
                          angular.isDefined(oldValue) && requestAnimationFrame(function () {
                              popover && popover.$applyPlacement();
                          });
                      });
                  });

                  // Support scope as an object
                  attr.bsPopover && scope.$watch(attr.bsPopover, function (newValue, oldValue) {
                      if (angular.isObject(newValue)) {
                          angular.extend(scope, newValue);
                      } else {
                          scope.content = newValue;
                      }
                      angular.isDefined(oldValue) && requestAnimationFrame(function () {
                          popover && popover.$applyPlacement();
                      });
                  }, true);

                  // Visibility binding support
                  attr.bsShow && scope.$watch(attr.bsShow, function (newValue, oldValue) {
                      if (!popover || !angular.isDefined(newValue)) return;
                      if (angular.isString(newValue)) newValue = !!newValue.match(/true|,?(popover),?/i);
                      newValue === true ? popover.show() : popover.hide();
                  });

                  // Initialize popover
                  var popover = $popover(element, options);

                  // Garbage collection
                  scope.$on('$destroy', function () {
                      if (popover) popover.destroy();
                      options = null;
                      popover = null;
                  });

              }
          };

      }]);


    // Source: tooltip.js
    angular.module('mgcrea.ngStrap.tooltip', ['mgcrea.ngStrap.helpers.dimensions']).provider('$tooltip', function () {
        var defaults = this.defaults = {
            animation: 'am-fade',
            customClass: '',
            prefixClass: 'tooltip',
            prefixEvent: 'tooltip',
            container: false,
            target: false,
            placement: 'top',
            template: 'tooltip.tpl.html',
            contentTemplate: false,
            trigger: 'hover focus',
            keyboard: false,
            html: false,
            show: false,
            title: '',
            type: '',
            delay: 0
        };
        this.$get = [
          '$window',
          '$rootScope',
          '$compile',
          '$q',
          '$templateCache',
          '$http',
          '$animate',
          'dimensions',
          '$$rAF',
          function ($window, $rootScope, $compile, $q, $templateCache, $http, $animate, dimensions, $$rAF) {
              var trim = String.prototype.trim;
              var isTouch = 'createTouch' in $window.document;
              var htmlReplaceRegExp = /ng-bind="/gi;
              function TooltipFactory(element, config) {
                  var $tooltip = {};
                  // Common vars
                  var nodeName = element[0].nodeName.toLowerCase();
                  var options = $tooltip.$options = angular.extend({}, defaults, config);
                  $tooltip.$promise = fetchTemplate(options.template);
                  var scope = $tooltip.$scope = options.scope && options.scope.$new() || $rootScope.$new();
                  if (options.delay && angular.isString(options.delay)) {
                      options.delay = parseFloat(options.delay);
                  }
                  // Support scope as string options
                  if (options.title) {
                      $tooltip.$scope.title = options.title;
                  }
                  // Provide scope helpers
                  scope.$hide = function () {
                      scope.$$postDigest(function () {
                          $tooltip.hide();
                      });
                  };
                  scope.$show = function () {
                      //#EDITED $$postDigest inconsistent
                      setTimeout(function () {
                          $tooltip.show();
                      }, 0);
                      // scope.$$postDigest(function () {
                      // $tooltip.show();
                      // });
                  };
                  scope.$toggle = function () {
                      scope.$$postDigest(function () {
                          $tooltip.toggle();
                      });
                  };
                  $tooltip.$isShown = scope.$isShown = false;
                  // Private vars
                  var timeout, hoverState;
                  // Support contentTemplate option
                  if (options.contentTemplate) {
                      $tooltip.$promise = $tooltip.$promise.then(function (template) {
                          var templateEl = angular.element(template);
                          return fetchTemplate(options.contentTemplate).then(function (contentTemplate) {
                              var contentEl = findElement('[ng-bind="content"]', templateEl[0]);
                              if (!contentEl.length)
                                  contentEl = findElement('[ng-bind="title"]', templateEl[0]);
                              contentEl.removeAttr('ng-bind').html(contentTemplate);
                              return templateEl[0].outerHTML;
                          });
                      });
                  }
                  // Fetch, compile then initialize tooltip
                  var tipLinker, tipElement, tipTemplate, tipContainer;
                  $tooltip.$promise.then(function (template) {
                      if (angular.isObject(template))
                          template = template.data;
                      if (options.html)
                          template = template.replace(htmlReplaceRegExp, 'ng-bind-html="');
                      template = trim.apply(template);
                      tipTemplate = template;
                      tipLinker = $compile(template);
                      $tooltip.init();
                  });
                  $tooltip.init = function () {
                      // Options: delay
                      if (options.delay && angular.isNumber(options.delay)) {
                          options.delay = {
                              show: options.delay,
                              hide: options.delay
                          };
                      }
                      // Replace trigger on touch devices ?
                      // if(isTouch && options.trigger === defaults.trigger) {
                      //   options.trigger.replace(/hover/g, 'click');
                      // }
                      // Options : container
                      if (options.container === 'self') {
                          tipContainer = element;
                      } else if (angular.isElement(options.container)) {
                          tipContainer = options.container;
                      } else if (options.container) {
                          tipContainer = findElement(options.container);
                      }
                      // Options: trigger
                      var triggers = options.trigger.split(' ');
                      angular.forEach(triggers, function (trigger) {
                          if (trigger === 'click') {
                              element.on('click', $tooltip.toggle);
                              //angular.element('body').on('click',$tooltip.leave);
                          } else if (trigger !== 'manual') {
                              element.on(trigger === 'hover' ? 'mouseenter' : 'focus', $tooltip.enter);
                              element.on(trigger === 'hover' ? 'mouseleave' : 'blur', $tooltip.leave);
                              nodeName === 'button' && trigger !== 'hover' && element.on(isTouch ? 'touchstart' : 'mousedown', $tooltip.$onFocusElementMouseDown);
                          }
                      });
                      // Options: target
                      if (options.target) {
                          options.target = angular.isElement(options.target) ? options.target : findElement(options.target)[0];
                      }
                      // Options: show
                      if (options.show) {
                          scope.$$postDigest(function () {
                              options.trigger === 'focus' ? element[0].focus() : $tooltip.show();
                          });
                      }
                  };
                  $tooltip.destroy = function () {
                      // Unbind events
                      var triggers = options.trigger.split(' ');
                      for (var i = triggers.length; i--;) {
                          var trigger = triggers[i];
                          if (trigger === 'click') {
                              element.off('click', $tooltip.toggle);
                          } else if (trigger !== 'manual') {
                              element.off(trigger === 'hover' ? 'mouseenter' : 'focus', $tooltip.enter);
                              element.off(trigger === 'hover' ? 'mouseleave' : 'blur', $tooltip.leave);
                              nodeName === 'button' && trigger !== 'hover' && element.off(isTouch ? 'touchstart' : 'mousedown', $tooltip.$onFocusElementMouseDown);
                          }
                      }
                      // Remove element
                      if (tipElement) {
                          tipElement.remove();
                          tipElement = null;
                      }
                      // Cancel pending callbacks
                      clearTimeout(timeout);
                      // Destroy scope
                      scope.$destroy();
                  };
                  $tooltip.enter = function () {
                      clearTimeout(timeout);
                      hoverState = 'in';
                      if (!options.delay || !options.delay.show) {
                          return $tooltip.show();
                      }
                      timeout = setTimeout(function () {
                          if (hoverState === 'in')
                              $tooltip.show();
                      }, options.delay.show);
                  };
                  $tooltip.show = function () {
                      scope.$emit(options.prefixEvent + '.show.before', $tooltip);
                      var parent = options.container ? tipContainer : null;
                      var after = options.container ? null : element;
                      // Hide any existing tipElement
                      if (tipElement)
                          tipElement.remove();
                      // Fetch a cloned element linked from template
                      tipElement = $tooltip.$element = tipLinker(scope, function (clonedElement, scope) {
                      });
                      // Set the initial positioning.  Make the tooltip invisible
                      // so IE doesn't try to focus on it off screen.
                      tipElement.css({
                          top: '-9999px',
                          left: '-9999px',
                          display: 'block',
                          visibility: 'hidden'
                      }).addClass(options.placement);
                      // Options: animation
                      if (options.animation)
                          tipElement.addClass(options.animation);
                      // Options: type
                      if (options.type)
                          tipElement.addClass(options.prefixClass + '-' + options.type);
                      // Options: custom classes
                      if (options.customClass)
                          tipElement.addClass(options.customClass);
                      $animate.enter(tipElement, parent, after, function () {
                          scope.$emit(options.prefixEvent + '.show', $tooltip);
                      });
                      $tooltip.$isShown = scope.$isShown = true;
                      scope.$$phase || scope.$root && scope.$root.$$phase || scope.$digest();
                      $$rAF(function () {
                          $tooltip.$applyPlacement();
                          // Once placed, make the tooltip visible
                          tipElement.css({ visibility: 'visible' });
                      });
                      // var a = bodyEl.offsetWidth + 1; ?
                      // Bind events
                      if (options.keyboard) {
                          if (options.trigger !== 'focus') {
                              $tooltip.focus();
                              tipElement.on('keyup', $tooltip.$onKeyUp);
                          } else {
                              element.on('keyup', $tooltip.$onFocusKeyUp);
                          }
                      }
                  };
                  $tooltip.leave = function () {
                      clearTimeout(timeout);
                      hoverState = 'out';
                      if (!options.delay || !options.delay.hide) {
                          return $tooltip.hide();
                      }
                      timeout = setTimeout(function () {
                          if (hoverState === 'out') {
                              $tooltip.hide();
                          }
                      }, options.delay.hide);
                  };
                  $tooltip.hide = function (blur) {
                      if (!$tooltip.$isShown)
                          return;
                      scope.$emit(options.prefixEvent + '.hide.before', $tooltip);
                      $animate.leave(tipElement, function () {
                          scope.$emit(options.prefixEvent + '.hide', $tooltip);
                          // Allow to blur the input when hidden, like when pressing enter key
                          if (blur && options.trigger === 'focus') {
                              return element[0].blur();
                          }
                      });
                      $tooltip.$isShown = scope.$isShown = false;
                      scope.$$phase || scope.$root && scope.$root.$$phase || scope.$digest();
                      // Unbind events
                      if (options.keyboard && tipElement !== null) {
                          tipElement.off('keyup', $tooltip.$onKeyUp);
                      }
                  };
                  $tooltip.toggle = function () {
                      $tooltip.$isShown ? $tooltip.leave() : $tooltip.enter();
                  };
                  $tooltip.focus = function () {
                      tipElement[0].focus();
                  };
                  // Protected methods
                  $tooltip.$applyPlacement = function () {
                      if (!tipElement)
                          return;
                      // Get the position of the tooltip element.
                      var elementPosition = getPosition();
                      // Get the height and width of the tooltip so we can center it.
                      var tipWidth = tipElement.prop('offsetWidth'), tipHeight = tipElement.prop('offsetHeight');
                      // Get the tooltip's top and left coordinates to center it with this directive.
                      var tipPosition = getCalculatedOffset(options.placement, elementPosition, tipWidth, tipHeight);
                      // Now set the calculated positioning.
                      tipPosition.top += 'px';
                      tipPosition.left += 'px';
                      if (options.width) { tipPosition.width = options.width + 'px'; }
                      tipElement.css(tipPosition);
                  };
                  $tooltip.$onKeyUp = function (evt) {
                      evt.which === 27 && $tooltip.hide();
                  };
                  $tooltip.$onFocusKeyUp = function (evt) {
                      evt.which === 27 && element[0].blur();
                  };
                  $tooltip.$onFocusElementMouseDown = function (evt) {
                      evt.preventDefault();
                      evt.stopPropagation();
                      // Some browsers do not auto-focus buttons (eg. Safari)
                      $tooltip.$isShown ? element[0].blur() : element[0].focus();
                  };
                  // Private methods
                  function getPosition() {
                      if (options.container === 'body') {
                          return dimensions.offset(options.target[0] || element[0]);
                      } else {
                          return dimensions.position(options.target[0] || element[0]);
                      }
                  }
                  function getCalculatedOffset(placement, position, actualWidth, actualHeight) {
                      var offset;
                      var split = placement.split('-');
                      switch (split[0]) {
                          case 'right':
                              offset = {
                                  top: position.top + position.height / 2 - actualHeight / 2,
                                  left: position.left + position.width
                              };
                              break;
                          case 'bottom':
                              offset = {
                                  top: position.top + position.height,
                                  left: position.left + position.width / 2 - actualWidth / 2
                              };
                              break;
                          case 'left':
                              offset = {
                                  top: position.top + position.height / 2 - actualHeight / 2,
                                  left: position.left - actualWidth
                              };
                              break;
                          default:
                              offset = {
                                  top: position.top - actualHeight,
                                  left: position.left + position.width / 2 - actualWidth / 2
                              };
                              break;
                      }
                      if (!split[1]) {
                          return offset;
                      }
                      // Add support for corners @todo css
                      if (split[0] === 'top' || split[0] === 'bottom') {
                          switch (split[1]) {
                              case 'left':
                                  offset.left = position.left;
                                  break;
                              case 'right':
                                  offset.left = position.left + position.width - actualWidth;
                          }
                      } else if (split[0] === 'left' || split[0] === 'right') {
                          switch (split[1]) {
                              case 'top':
                                  offset.top = position.top - actualHeight;
                                  break;
                              case 'bottom':
                                  offset.top = position.top + position.height;
                          }
                      }
                      return offset;
                  }
                  return $tooltip;
              }
              // Helper functions
              function findElement(query, element) {
                  return angular.element((element || document).querySelectorAll(query));
              }
              function fetchTemplate(template) {
                  return $q.when($templateCache.get(template) || $http.get(template)).then(function (res) {
                      if (angular.isObject(res)) {
                          $templateCache.put(template, res.data);
                          return res.data;
                      }
                      return res;
                  });
              }
              return TooltipFactory;
          }
        ];
    }).directive('bsTooltip', [
      '$window',
      '$location',
      '$sce',
      '$tooltip',
      '$$rAF',
      function ($window, $location, $sce, $tooltip, $$rAF) {
          return {
              restrict: 'EAC',
              scope: true,
              link: function postLink(scope, element, attr, transclusion) {
                  // Directive options
                  var options = { scope: scope };
                  angular.forEach([
                    'template',
                    'contentTemplate',
                    'placement',
                    'container',
                    'target',
                    'delay',
                    'trigger',
                    'keyboard',
                    'html',
                    'animation',
                    'type',
                    'customClass'
                  ], function (key) {
                      if (angular.isDefined(attr[key]))
                          options[key] = attr[key];
                  });
                  // Observe scope attributes for change
                  angular.forEach(['title'], function (key) {
                      attr.$observe(key, function (newValue, oldValue) {
                          scope[key] = $sce.trustAsHtml(newValue);
                          angular.isDefined(oldValue) && $$rAF(function () {
                              tooltip && tooltip.$applyPlacement();
                          });
                      });
                  });
                  // Support scope as an object
                  attr.bsTooltip && scope.$watch(attr.bsTooltip, function (newValue, oldValue) {
                      if (angular.isObject(newValue)) {
                          angular.extend(scope, newValue);
                      } else {
                          scope.title = newValue;
                      }
                      angular.isDefined(oldValue) && $$rAF(function () {
                          tooltip && tooltip.$applyPlacement();
                      });
                  }, true);
                  // Visibility binding support
                  attr.bsShow && scope.$watch(attr.bsShow, function (newValue, oldValue) {
                      if (!tooltip || !angular.isDefined(newValue))
                          return;
                      if (angular.isString(newValue))
                          newValue = newValue.match(',?(tooltip),?');
                      newValue === true ? tooltip.show() : tooltip.hide();
                  });
                  // Initialize popover
                  var tooltip = $tooltip(element, options);
                  // Garbage collection
                  scope.$on('$destroy', function () {
                      if (tooltip)
                          tooltip.destroy();
                      options = null;
                      tooltip = null;
                  });
              }
          };
      }
    ]);

    //Source typeahead.js
    angular.module('mgcrea.ngStrap.typeahead', [
      'mgcrea.ngStrap.tooltip',
      'mgcrea.ngStrap.helpers.parseOptions'
    ]).provider('$typeahead', function () {
        var defaults = this.defaults = {
            animation: 'am-fade',
            prefixClass: 'typeahead',
            prefixEvent: '$typeahead',
            placement: 'bottom-left',
            template: 'typeahead.tpl.html',
            trigger: 'focus',
            container: false,
            keyboard: true,
            html: false,
            delay: 0,
            minLength: 1,
            filter: 'filter',
            limit: 6
        };
        this.$get = [
          '$window',
          '$rootScope',
          '$tooltip',
          function ($window, $rootScope, $tooltip) {
              var bodyEl = angular.element($window.document.body);
              function TypeaheadFactory(element, controller, config) {
                  var $typeahead = {};
                  // Common vars
                  var options = angular.extend({}, defaults, config);
                  $typeahead = $tooltip(element, options);
                  var parentScope = config.scope;
                  var scope = $typeahead.$scope;
                  scope.$resetMatches = function () {
                      scope.$matches = [];
                      scope.$activeIndex = 0;
                  };
                  scope.$resetMatches();
                  scope.$activate = function (index) {
                      scope.$$postDigest(function () {
                          $typeahead.activate(index);
                      });
                  };
                  scope.$select = function (index, evt) {
                      scope.$$postDigest(function () {
                          $typeahead.select(index);
                      });
                  };
                  scope.$isVisible = function () {
                      return $typeahead.$isVisible();
                  };
                  // Public methods
                  $typeahead.update = function (matches) {
                      scope.$matches = matches;
                      if (scope.$activeIndex >= matches.length) {
                          scope.$activeIndex = 0;
                      }
                  };
                  $typeahead.activate = function (index) {
                      scope.$activeIndex = index;
                  };
                  $typeahead.select = function (index) {
                      var value = scope.$matches[index].value;
                      controller.$setViewValue(value);
                      controller.$render();
                      scope.$resetMatches();
                      if (parentScope)
                          parentScope.$digest();
                      // Emit event
                      scope.$emit(options.prefixEvent + '.select', value, index);
                  };
                  // Protected methods
                  $typeahead.$isVisible = function () {
                      if (!options.minLength || !controller) {
                          return !!scope.$matches.length;
                      }
                      // minLength support
                      return scope.$matches.length && angular.isString(controller.$viewValue) && controller.$viewValue.length >= options.minLength;
                  };
                  $typeahead.$getIndex = function (value) {
                      var l = scope.$matches.length, i = l;
                      if (!l)
                          return;
                      for (i = l; i--;) {
                          if (scope.$matches[i].value === value)
                              break;
                      }
                      if (i < 0)
                          return;
                      return i;
                  };
                  $typeahead.$onMouseDown = function (evt) {
                      // Prevent blur on mousedown
                      evt.preventDefault();
                      evt.stopPropagation();
                  };
                  $typeahead.$onKeyDown = function (evt) {
                      if (!/(38|40|13)/.test(evt.keyCode))
                          return;
                      // Let ngSubmit pass if the typeahead tip is hidden
                      if ($typeahead.$isVisible()) {
                          evt.preventDefault();
                          evt.stopPropagation();
                      }
                      // Select with enter
                      if (evt.keyCode === 13 && scope.$matches.length) {
                          $typeahead.select(scope.$activeIndex);
                      }  // Navigate with keyboard
                      else if (evt.keyCode === 38 && scope.$activeIndex > 0)
                          scope.$activeIndex--;
                      else if (evt.keyCode === 40 && scope.$activeIndex < scope.$matches.length - 1)
                          scope.$activeIndex++;
                      else if (angular.isUndefined(scope.$activeIndex))
                          scope.$activeIndex = 0;
                      scope.$digest();
                  };
                  // Overrides
                  var show = $typeahead.show;
                  $typeahead.show = function () {
                      show();
                      setTimeout(function () {
                          $typeahead.$element.on('mousedown', $typeahead.$onMouseDown);
                          if (options.keyboard) {
                              element.on('keydown', $typeahead.$onKeyDown);
                          }
                      });
                  };
                  var hide = $typeahead.hide;
                  $typeahead.hide = function () {
                      $typeahead.$element.off('mousedown', $typeahead.$onMouseDown);
                      if (options.keyboard) {
                          element.off('keydown', $typeahead.$onKeyDown);
                      }
                      hide();
                  };
                  return $typeahead;
              }
              TypeaheadFactory.defaults = defaults;
              return TypeaheadFactory;
          }
        ];
    }).directive('bsTypeahead', [
      '$window',
      '$parse',
      '$q',
      '$typeahead',
      '$parseOptions',
      function ($window, $parse, $q, $typeahead, $parseOptions) {
          var defaults = $typeahead.defaults;
          return {
              restrict: 'EAC',
              require: 'ngModel',
              link: function postLink(scope, element, attr, controller) {
                  // Directive options
                  var options = { scope: scope };
                  angular.forEach([
                    'placement',
                    'container',
                    'delay',
                    'trigger',
                    'keyboard',
                    'html',
                    'animation',
                    'template',
                    'filter',
                    'limit',
                    'minLength',
                    'watchOptions',
                    'selectMode'
                  ], function (key) {
                      if (angular.isDefined(attr[key]))
                          options[key] = attr[key];
                  });
                  // Build proper ngOptions
                  var filter = options.filter || defaults.filter;
                  var limit = options.limit || defaults.limit;
                  var ngOptions = attr.ngOptions;
                  if (filter)
                      ngOptions += ' | ' + filter + ':$viewValue';
                  if (limit)
                      ngOptions += ' | limitTo:' + limit;
                  var parsedOptions = $parseOptions(ngOptions);
                  // Initialize typeahead
                  var typeahead = $typeahead(element, controller, options);
                  // Watch options on demand
                  if (options.watchOptions) {
                      // Watch ngOptions values before filtering for changes, drop function calls
                      var watchedOptions = parsedOptions.$match[7].replace(/\|.+/, '').replace(/\(.*\)/g, '').trim();
                      scope.$watch(watchedOptions, function (newValue, oldValue) {
                          // console.warn('scope.$watch(%s)', watchedOptions, newValue, oldValue);
                          parsedOptions.valuesFn(scope, controller).then(function (values) {
                              typeahead.update(values);
                              controller.$render();
                          });
                      }, true);
                  }
                  // Watch model for changes
                  scope.$watch(attr.ngModel, function (newValue, oldValue) {
                      // console.warn('$watch', element.attr('ng-model'), newValue);
                      scope.$modelValue = newValue;
                      // Publish modelValue on scope for custom templates
                      parsedOptions.valuesFn(scope, controller).then(function (values) {
                          // Prevent input with no future prospect if selectMode is truthy
                          // @TODO test selectMode
                          if (options.selectMode && !values.length && newValue.length > 0) {
                              controller.$setViewValue(controller.$viewValue.substring(0, controller.$viewValue.length - 1));
                              return;
                          }
                          if (values.length > limit)
                              values = values.slice(0, limit);
                          var isVisible = typeahead.$isVisible();
                          isVisible && typeahead.update(values);
                          // Do not re-queue an update if a correct value has been selected
                          if (values.length === 1 && values[0].value === newValue)
                              return;
                          !isVisible && typeahead.update(values);
                          // Queue a new rendering that will leverage collection loading
                          controller.$render();
                      });
                  });
                  // Model rendering in view
                  controller.$render = function () {
                      // console.warn('$render', element.attr('ng-model'), 'controller.$modelValue', typeof controller.$modelValue, controller.$modelValue, 'controller.$viewValue', typeof controller.$viewValue, controller.$viewValue);
                      if (controller.$isEmpty(controller.$viewValue))
                          return element.val('');
                      var index = typeahead.$getIndex(controller.$modelValue);
                      var selected = angular.isDefined(index) ? typeahead.$scope.$matches[index].label : controller.$viewValue;
                      selected = angular.isObject(selected) ? selected.label : selected;
                      element.val(selected.replace(/<(?:.|\n)*?>/gm, '').trim());
                  };
                  // Garbage collection
                  scope.$on('$destroy', function () {
                      typeahead.destroy();
                      options = null;
                      typeahead = null;
                  });
              }
          };
      }
    ]);

    //HELPER
    angular.module('mgcrea.ngStrap.helpers.parseOptions', []).provider('$parseOptions', function () {
        var defaults = this.defaults = { regexp: /^\s*(.*?)(?:\s+as\s+(.*?))?(?:\s+group\s+by\s+(.*))?\s+for\s+(?:([\$\w][\$\w]*)|(?:\(\s*([\$\w][\$\w]*)\s*,\s*([\$\w][\$\w]*)\s*\)))\s+in\s+(.*?)(?:\s+track\s+by\s+(.*?))?$/ };
        this.$get = [
          '$parse',
          '$q',
          function ($parse, $q) {
              function ParseOptionsFactory(attr, config) {
                  var $parseOptions = {};
                  // Common vars
                  var options = angular.extend({}, defaults, config);
                  $parseOptions.$values = [];
                  // Private vars
                  var match, displayFn, valueName, keyName, groupByFn, valueFn, valuesFn;
                  $parseOptions.init = function () {
                      $parseOptions.$match = match = attr.match(options.regexp);
                      displayFn = $parse(match[2] || match[1]), valueName = match[4] || match[6], keyName = match[5], groupByFn = $parse(match[3] || ''), valueFn = $parse(match[2] ? match[1] : valueName), valuesFn = $parse(match[7]);
                  };
                  $parseOptions.valuesFn = function (scope, controller) {
                      return $q.when(valuesFn(scope, controller)).then(function (values) {
                          $parseOptions.$values = values ? parseValues(values, scope) : {};
                          return $parseOptions.$values;
                      });
                  };
                  // Private functions
                  function parseValues(values, scope) {
                      return values.map(function (match, index) {
                          var locals = {}, label, value;
                          locals[valueName] = match;
                          label = displayFn(scope, locals);
                          value = valueFn(scope, locals) || index;
                          return {
                              label: label,
                              value: value
                          };
                      });
                  }
                  $parseOptions.init();
                  return $parseOptions;
              }
              return ParseOptionsFactory;
          }
        ];
    });

    angular.module('mgcrea.ngStrap').run(['$templateCache', function ($templateCache) {
        'use strict';

        $templateCache.put('datepicker.tpl.html',
        '<div class="dropdown-menu datepicker" ng-class="\'datepicker-mode-\' + $mode" style="max-width: 320px;">' +
            '<table style="table-layout: fixed; height: 100%; width: 100%;">' +
                '<thead>' +
                    '<tr class="text-center">' +
                        '<th>' +
                            '<button tabindex="-1" type="button" class="btn btn-default pull-left" ng-click="$selectPane(-1)">' +
                                '<i class="{{$iconLeft}}"></i>' +
                            '</button>' +
                        '</th>' +
                        '<th colspan="{{ rows[0].length - 2 }}">' +
                            '<button tabindex="-1" type="button" class="btn btn-default btn-block text-strong"  ng-click="$toggleMode()">' +
                                '<strong style="text-transform: capitalize;" ng-bind="title"></strong>' +
                            '</button>' +
                        '</th>' +
                        '<th>' +
                            '<button tabindex="-1" type="button" class="btn btn-default pull-right" ng-click="$selectPane(+1)">' +
                                '<i class="{{$iconRight}}"></i>' +
                            '</button>' +
                        '</th>' +
                    '</tr>' +
                    '<tr ng-show="showLabels" ng-bind-html="labels"></tr>' +
                '</thead>' +
                '<tbody>' +
                    '<tr ng-repeat="(i, row) in rows" height="{{ 100 / rows.length }}%">' +
                        '<td class="text-center" ng-repeat="(j, el) in row">' +
                            '<button tabindex="-1" type="button" class="btn btn-default" style="width: 100%" ng-class="{\'btn-primary\': el.selected}" ng-click="$select(el.date)" ng-disabled="el.disabled">' +
                                '<span ng-class="{\'text-muted\': el.muted}" ng-bind="el.label"></span>' +
                            '</button>' +
                        '</td>' +
                    '</tr>' +
                '</tbody>' +
            '</table>' +
        '</div>'
        );

        $templateCache.put('popover.tpl.html',
        '<div class="popover">' +
            '<div class="arrow"></div>' +
            '<h3 class="popover-title" ng-bind="title" ng-show="title"></h3>' +
            '<div class="popover-content" ng-bind="content"></div>' +
        '</div>'
        );


        $templateCache.put('select/select.tpl.html',
        '<ul vs-repeat="30" tabindex="-1" style="position:absolute;min-height:20px;max-height:250px;overflow-y:scroll;" class="select dropdown-menu" role="select">' +  //ng-show="$isVisible()"
            // '<li ng-if="$showAllNoneButtons">' +
                // '<div class="btn-group" style="margin-bottom: 5px; margin-left: 5px">' + 
                    // '<button class="btn btn-default btn-xs" ng-click="$selectAll()">All</button>' + 
                    // '<button class="btn btn-default btn-xs" ng-click="$selectNone()">None</button>' + 
                // '</div>' + 
            // '</li>' + 
            '<li ng-repeat="match in $matches track by $index">' +  // role="presentation" // ng-class="{active: $isActive($index)}"
                '<a role="menuitem" tabindex="-1" ng-click="$select(match, $event)">' +
                    '<span>{{match.label}}</span>' + //'<span ng-bind="match.label"></span>' + 
                '</a>' +
                //'<i style="cursor: default" class="{{$iconCheckmark}} pull-right" ng-if="$isMultiple && $isActive($index)" ng-click="$select(match, $event)"></i>' + 
            '</li>' +
        '</ul>'
        );


        $templateCache.put('tab.tpl.html',
        '<ul class="nav" ng-class="$navClass" role="tablist">' +
            '<li ng-repeat="$pane in $panes" ng-show="$pane.show" ng-class="$index == $panes.$active ? $activeClass : \'\'">' +
                '<a role="tab" data-toggle="tab" ng-click="$setActive($index)" data-index="{{ $index }}" ng-bind-html="$pane.title"></a>' +
            '</li>' +
        '</ul>' +
        '<div ng-transclude class="tab-content"></div>'
        );

        $templateCache.put('timepicker.tpl.html',
        '<div class="dropdown-menu timepicker" style="min-width: 0px;width: auto;">' +
            '<table height="100%">' +
              '<thead>' +
                '<tr class="text-center">' +
                  '<th>' +
                    '<button tabindex="-1" type="button" class="btn btn-default pull-left" ng-click="$arrowAction(-1, 0)">' +
                      '<i class="{{ $iconUp }}"></i>' +
                    '</button>' +
                  '</th>' +
                  '<th>&nbsp;</th>' +
                  '<th>' +
                    '<button tabindex="-1" type="button" class="btn btn-default pull-left" ng-click="$arrowAction(-1, 1)">' +
                      '<i class="{{ $iconUp }}"></i>' +
                    '</button>' +
                  '</th>' +
                '</tr>' +
              '</thead>' +
              '<tbody>' +
                '<tr ng-repeat="(i, row) in rows">' +
                  '<td class="text-center">' +
                    '<button tabindex="-1" style="width: 100%" type="button" class="btn btn-default" ng-class="{\'btn-primary\': row[0].selected}" ng-click="$select(row[0].date, 0)" ng-disabled="row[0].disabled">' +
                      '<span ng-class="{\'text-muted\': row[0].muted}" ng-bind="row[0].label"></span>' +
                    '</button>' +
                  '</td>' +
                  '<td>' +
                    '<span ng-bind="i == midIndex ? timeSeparator : \' \'"></span>' +
                  '</td>' +
                  '<td class="text-center">' +
                    '<button tabindex="-1" ng-if="row[1].date" style="width: 100%" type="button" class="btn btn-default" ng-class="{\'btn-primary\': row[1].selected}" ng-click="$select(row[1].date, 1)" ng-disabled="row[1].disabled">' +
                      '<span ng-class="{\'text-muted\': row[1].muted}" ng-bind="row[1].label"></span>' +
                    '</button>' +
                  '</td>' +
                  '<td ng-if="showAM">&nbsp;</td>' +
                  '<td ng-if="showAM">' +
                    '<button tabindex="-1" ng-show="i == midIndex - !isAM * 1" style="width: 100%" type="button" ng-class="{\'btn-primary\': !!isAM}" class="btn btn-default" ng-click="$switchMeridian()" ng-disabled="el.disabled">AM</button>' +
                    '<button tabindex="-1" ng-show="i == midIndex + 1 - !isAM * 1" style="width: 100%" type="button" ng-class="{\'btn-primary\': !isAM}" class="btn btn-default" ng-click="$switchMeridian()" ng-disabled="el.disabled">PM</button>' +
                  '</td>' +
                '</tr>' +
              '</tbody>' +
              '<tfoot>' +
                '<tr class="text-center">' +
                  '<th>' +
                    '<button tabindex="-1" type="button" class="btn btn-default pull-left" ng-click="$arrowAction(1, 0)">' +
                      '<i class="{{ $iconDown }}"></i>' +
                    '</button>' +
                  '</th>' +
                  '<th>&nbsp;</th>' +
                  '<th>' +
                    '<button tabindex="-1" type="button" class="btn btn-default pull-left" ng-click="$arrowAction(1, 1)">' +
                      '<i class="{{ $iconDown }}"></i>' +
                    '</button>' +
                  '</th>' +
                '</tr>' +
              '</tfoot>' +
            '</table>' +
        '</div>'
        );

        $templateCache.put('tooltip.tpl.html',
        '<div class="tooltip in" ng-show="title">' +
            '<div class="tooltip-arrow"></div>' +
            '<div class="tooltip-inner" ng-bind="title"></div>' +
        '</div>'
        );

        $templateCache.put('typeahead.tpl.html',
        '<ul tabindex="-1" class="typeahead dropdown-menu" ng-show="$isVisible()" role="select">' +
            '<li role="presentation" ng-repeat="match in $matches" ng-class="{active: $index == $activeIndex}">' +
                '<a role="menuitem" tabindex="-1" ng-click="$select($index, $event)" ng-bind="match.label"></a>' +
            '</li>' +
        '</ul>'
        );


    }]);


})(window, document);
