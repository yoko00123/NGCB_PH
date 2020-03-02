'use strict';
define(['app'],function (app) {
    var menuCtrl = function (s, d, u, g, S, R, SS, Di, h, $filter, ti, $compile) {
        R.employee = {};
        s.notification = {};
        s.notification.Msg = [];
        s.notification.Cnt = 0;
        s.notification.NewMsg = [];
        s.notification.fData = "";
        s.LastID = 0;
        var stateCollection = S.get();
        s.Master = {};
        s.Master.favourites = [];
        //FAVOURITES
        d.getAllFavourites().then(function (res) {
            if (res.data.type == "2") {
                g.add(res.data.msg, "danger", 5000);
            }
            s.Master.favourites = res.data.menus;
            //FAVOURITE MENU EVENTS
            s.isfav = function (id) {
                var cnt = s.Master.favourites.filter(function (x) { return x.mID == id }).length;
                if (cnt > 0) {
                    return true
                } else {
                    return false
                }
            }

            s.addToFavourites = [
                ['Add to bookmark', function ($itemScope, $event, color, model) {
                    var mID = parseInt($event.target.id);
                    var cnt = s.Master.favourites.filter(function (x) { return x.mID == mID }).length;
                    if (cnt == 0) {
                        d.addToFavourites(mID).then(function (res) {
                            if (res.data.type == "2") {
                                g.add(res.data.msg, "danger", 5000);
                            } else {
                                s.Master.favourites.push({ 'mID': mID, 'WebMenus': $event.target.name, 'url': '#/' + $event.target.name.replace(/ /g, '-') + '/', 'bgcolor': 'red', 'color': 'white', 'icon': '<i class="fa fa-music"></i>' });
                            }
                        });
                    }
                }]
            ];

            s.removeFromFavourites = [
                ['Remove from bookmark', function ($itemScope, $event, color) {
                    var mID = parseInt($event.target.id);
                    var cnt = s.Master.favourites.filter(function (x) { return x.mID == mID }).length;
                    if (cnt > 0) {
                        angular.forEach(s.Master.favourites, function (obj, mIdx) {
                            if (obj.mID == mID) {
                                d.removeFromFavourites(mID).then(function (res) {
                                    if (res.data.type == "2") {
                                        g.add(res.data.msg, "danger", 5000);
                                    } else {
                                        s.Master.favourites.splice(mIdx, 1);
                                    }
                                });
                            }
                        });
                    }
                }]
            ];
            //PARENT MENUS
            s.createParent = function (ds) {
                var str = '';
                var parentWebMenus = ds.filter(function (x) { return x.ID_WebMenus == 0 });
                str += '<ul data-menu="main" class="menu__level">';
                angular.forEach(parentWebMenus, function (obj) {
                    str += '<li class="menu__item">';
                    if (obj.Children.length > 0) {
                        str += '<a class="menu__link" data-submenu="submenu-' + obj.ID + '" href="#">' + (obj.Label == '' ? obj.Name : obj.Label) + '</a>';
                    } else {
                        str += '<a name="' + obj.Name + '" id="' + obj.ID + '" context-menu="(isfav(' + obj.ID + ') !== true ? addToFavourites : removeFromFavourites)" class="menu__link" href="#/' + obj.Name.replace(/ /g, '-') + '/">' + (obj.Label == '' ? obj.Name : obj.Label) + '</a>';
                    }
                    str += '</li>';
                });
                str += '</ul>';
                angular.forEach(parentWebMenus, function (obj) {
                    str += s.createChild(obj, obj.ID);
                });
                return str;
            }
            //CHILD MENUS
            s.createChild = function (ds, ID_WebMenus) {
                var str = '';
                str += '<ul data-menu="submenu-' + ID_WebMenus + '" class="menu__level">';
                angular.forEach(ds.Children, function (obj) {
                    str += '<li class="menu__item">';
                    if (obj.Children.length > 0) {
                        str += '<a class="menu__link" data-submenu="submenu-' + obj.ID + '" href="#">' + (obj.Label == '' ? obj.Name : obj.Label) + '</a>';
                    } else {
                        str += '<a name="' + obj.Name + '" id="' + obj.ID + '" context-menu="(isfav(' + obj.ID + ') !== true ? addToFavourites : removeFromFavourites)" class="menu__link" href="#/' + obj.Name.replace(/ /g, '-') + '/">' + (obj.Label == '' ? obj.Name : obj.Label) + '</a>';
                    }
                    str += '</li>';
                });
                str += '</ul>';
                angular.forEach(ds.Children, function (obj) {
                    str += s.createChild(obj, obj.ID);
                });
                return str;
            }
            //LOAD MENUS
            d.loadMenus().then(function (results) {
                s.menus = results.menus;
                function disableOther(button) {
                    if (button !== 'showLeft') {
                        classie.toggle(showLeft, 'disabled');
                    }
                }
                if (results.UseMenu == 1) {
                    var menuBuilder = '';
                    menuBuilder = '<div class="menu__wrap">' + s.createParent(s.menus) + '</div>';
                    angular.element($("#menuContainer").append($compile(menuBuilder)(s)));
                
                    var menuLeft = document.getElementById('ml-menu'),
                        showLeft = $(".btnOpen");
                    showLeft.on('click', function () {
                        var isActive = $(this).hasClass('active');
                        classie.toggle(this, 'active');
                        classie.toggle(menuLeft, 'cbp-spmenu-open');
                        $(".btnOpen i").attr('class', (isActive ? 'fa fa-arrow-right' : 'fa fa-arrow-left'));
                        $(".btnOpen").attr('style', (isActive ? 'opacity:.5;' : 'opacity:1;'));
                        $(".btnOpen").attr('title', (isActive ? 'Open Menu' : 'Close Menu'));
                        disableOther('showLeft');
                    });
                    (function () {
                        var menuEl = document.getElementById('ml-menu'),
                            mlmenu = new MLMenu(menuEl, {
                                backCtrl: false
                            });
                        var openMenuCtrl = document.querySelector('.action--open'),
                            closeMenuCtrl = document.querySelector('.action--close');

                        openMenuCtrl.addEventListener('click', openMenu);
                        closeMenuCtrl.addEventListener('click', closeMenu);

                        function openMenu() {
                            classie.add(menuEl, 'menu--open');
                        }

                        function closeMenu() {
                            classie.remove(menuEl, 'menu--open');
                        }
                    })();
                }

                s.employee = R.employee = results.employee;
                var size = results.employee.CompanyLogoSize;
                s.width = size.split(";")[0];
                s.height = size.split(";")[1];
                s.EnableNotification = results.EnableNotification;
                s.TotalCnt = 10;

                //NOTIFICATIONS
                if (Boolean(s.EnableNotification)) {
                    d.getNotification().poll(s.TotalCnt, 0).then(function (res) {
                        s.notification.Msg = res.Msg;
                        s.notification.Cnt = res.CntRecord;
                        angular.forEach(s.notification.Msg, function (value, key) {
                            angular.forEach(value, function (value2, key2) {
                                if (key2 == "ID") {
                                    if (s.LastID < value2) {
                                        s.LastID = value2;
                                    }
                                }
                            })
                        });
                    });

                    var repeater = function () {
                        s.$apply(function () {
                            d.getNotification().poll(s.TotalCnt, s.LastID).then(function (res) {
                                if (res.Msg != undefined) {
                                    s.notification.Msg = res.Msg;
                                    $("#notificationLoading").html('See more <i class="fa fa-angle-right"></i>');
                                    s.notification.Cnt = res.CntRecord;
                                    s.notification.NewMsg = res.NewMsg;
                                    s.notification.fData = res.fData;
                                    s.notifyMessage(s.notification.NewMsg);
                                    angular.forEach(s.notification.Msg, function (value, key) {
                                        angular.forEach(value, function (value2, key2) {
                                            if (key2 == "ID") {
                                                if (s.LastID < value2) {
                                                    s.LastID = value2;
                                                }
                                            }
                                        })
                                    });
                                }
                            });
                        });
                    }

                    var timer;
                    clearInterval(timer);
                    timer = setInterval(repeater, 5000);
                }
            });
        });

        s.AddMoreNotification = function (e) {
            s.TotalCnt = s.TotalCnt + 100;
            $("#notificationLoading").html('<img src="Resources/System/notificationLoading.gif" />');
        }

        s.viewNotification = function (obj) {
            var notificationDialog = Di.create('Dialogs/notification.html', 'cNotification', { data: obj }, { size: 'md', keyboard: true, backdrop: true, windowClass: 'my-class' });
            s.UpdateReadNotification(obj);
        }

        s.UpdateReadNotification = function (obj) {
            if (!obj.IsView) {
                d.updateNotification(obj.ID).then(function (res) {
                    if (res.message != "") {
                        g.add("There was an error updating your notification", "danger", 5000);
                    } else {
                        obj.IsView = res.IsView;
                        s.notification.Cnt = s.notification.Cnt - 1;
                    }
                });
            }
        }

        s.DropNotification = function () {
            console.log('Hello');
            d.dropNotifications(SS.data.ID_User).then(function (dropting) {
                if (dropting.message != "") {
                    g.add("There was an error updating your notification", "danger", 5000);
                } else {
                    console.log(dropting)
                }
            });
        }

        s.notificationClear = function () {
            console.log('hello');
            d.clearNotification(SS.data.ID_User).then(function (res) {
                if (res.message != "") {
                    g.add("There was an error updating your notification", "danger", 5000);
                } else {
                    console.log(res)
                }
            });
        }

        _.groupByMulti = function (obj, values, context) {
            if (!values.length)
                return obj;
            var byFirst = _.groupBy(obj, values[0], context),
                rest = values.slice(1);
            for (var prop in byFirst) {
                byFirst[prop] = _.groupByMulti(byFirst[prop], rest, context);
            }
            return byFirst;
        };

        s.notifyMessage = function (obj) {
            angular.forEach(obj, function (value, key) {
                angular.forEach(value, function (value2, key2) {
                    var menuState = stateCollection.filter(function (x) { return x.mID == value2.ID_Parent });
                    angular.forEach(value2.Employee, function (value3, key3) {
                        var notifMsg = "<a href='#/" + menuState[0].stateName.replace(/ /g, "-") + "/" + value3.fData + "'>You have " + String(value3.CNT) + " " + key2 + " from " + value3.Sender + "</a>";
                        g.add(notifMsg, "info", 5000);
                    });
                });
            });
        }

        s.replaceTimeSpan = function (obj) {
            if (obj.WeekSpan > 8) {
                return moment(obj.DateTimeCreated).format("MM-DD-YYYY hh:mm A");
            } else if (obj.DaySpan > 7) {
                return "About " + String(obj.WeekSpan) + " week(s) ago...";
            } else if (obj.HourSpan > 24) {
                return "About " + String(obj.DaySpan) + " day(s) ago...";
            } else if (obj.MinuteSpan > 60) {
                return "About " + String(obj.HourSpan) + " hour(s) ago...";
            } else {
                return "About " + String(obj.MinuteSpan) + " minute(s) ago...";
            }
        }
        //END NOTIFICATIONS

        s.Session = SS.data;
        s.PublishWebsite = function () {
            d.PublishWebsite().then(function (results) {
                g.add(results.message, "info", 5000);
            });
        }

        s.ClearMenu = function () {
            d.ClearMenu().then(function (results) {
                g.add(results.message, "info", 5000);
                S.reload();
            });
        }
        s.MinifyScripts = function () {
            d.MinifyScripts().then(function (results) {
                g.add(results.message, "info", 5000);
                S.reload();
            });
        }

        s.Run = function () {
            var dlg = Di.create('Dialogs/RunScript.html', 'ScriptController', {}, { size: 'xl', keyboard: true, backdrop: 'static' });
        };

        s.ShowFileExplorer = function () {
            Di.create('Dialogs/FileExplorer.html', 'FileExplorerController', {}, { size: 'xl', keyboard: true, backdrop: 'static' });
        };

        s.ShowSystemDashboard = function () {
            Di.create('Dialogs/SystemDashboard.html', 'SystemDashboardController', {}, { size: 'xl', keyboard: true, backdrop: 'static' });

        };

        h.bindTo(s).add({
            combo: 'alt+shift+r+s',
            callback: function () {
                if (SS.data.ID_User == 1) s.Run();
            }
        }).add({
            combo: 'alt+shift+f+e',
            callback: function () {
                if (SS.data.ID_User == 1) s.ShowFileExplorer();
            }
        }).add({
            combo: 'alt+l',
            callback: function () {
                window.location.href = 'Logout.aspx';
            }
        }).add({
            combo: 'alt+shift+s+d',
            callback: function () {
                if (SS.data.ID_User == 1) s.ShowSystemDashboard();
            }
        });

        s.loadCalendar = function () {
            var dlg = Di.create('Dialogs/calendar.html', 'cCalendar', {}, { size: 'xl', keyboard: true, backdrop: true, windowClass: 'my-class' });
        };
	}
    app.controller('menuCtrl', ['$scope', 'dataService', 'utilService', 'growlNotifications', '$state', '$rootScope', 'Session', 'dialogs', 'hotkeys', '$filter', '$timeout', '$compile', menuCtrl]);

    var cCalendar = function (s, d, mI, S, ss) {
        s.Master = {};
        s.calendarView = 'month';
        s.calendarDay = new Date();
        s.events = [];
        s.mainEvent = [];
        s.Master.calendarMonth = moment(s.calendarDay).month() + 1;
        s.Master.calendarYear = moment(s.calendarDay).year();
        s.Master.calendarEmployee = ss.data.ID_Employee;
        s.EmployeeSource = [];

        d.GetApproverEmployee(ss.data.ID_Employee).then(function (result) {
            s.EmployeeSource = result.data;
        });

        var year = s.Master.calendarYear, month = s.Master.calendarMonth - 1;
        var startDate = moment([year, month]).toDate();
        refreshCalendar(startDate, moment(startDate).endOf('month').toDate(), s.Master.calendarEmployee);

        s.gotoMonth = function () {
            startDate = moment([s.Master.calendarYear, s.Master.calendarMonth - 1]).toDate();
            refreshCalendar(startDate, moment(startDate).endOf('month').toDate(), s.Master.calendarEmployee);
            s.Master.calendarControl.goToMonth(s.Master.calendarMonth);
        }

        s.gotoYear = function () {
            startDate = moment([s.Master.calendarYear, s.Master.calendarMonth - 1]).toDate();
            refreshCalendar(startDate, moment(startDate).endOf('month').toDate(), s.Master.calendarEmployee);
            s.Master.calendarControl.goToYear(s.Master.calendarYear);
        }

        s.prev = function () {
            s.Master.calendarControl.prev();
            refreshCalendar(s.Master.calendarControl.getFirstDayOfMonth(), s.Master.calendarControl.getLastDayOfMonth(), s.Master.calendarEmployee);
            s.Master.calendarMonth = moment(s.Master.calendarControl.getFirstDayOfMonth()).month() + 1;
            s.Master.calendarYear = moment(s.Master.calendarControl.getFirstDayOfMonth()).year();
        }
        s.next = function () {
            s.Master.calendarControl.next();
            refreshCalendar(s.Master.calendarControl.getFirstDayOfMonth(), s.Master.calendarControl.getLastDayOfMonth(), s.Master.calendarEmployee);
            s.Master.calendarMonth = moment(s.Master.calendarControl.getFirstDayOfMonth()).month() + 1;
            s.Master.calendarYear = moment(s.Master.calendarControl.getFirstDayOfMonth()).year();
        }
        s.setCalendarToToday = function () {
            s.Master.calendarControl.setCalendarToToday();
            refreshCalendar(s.Master.calendarControl.getFirstDayOfMonth(), s.Master.calendarControl.getLastDayOfMonth(), s.Master.calendarEmployee);
            s.Master.calendarMonth = moment(s.Master.calendarControl.getFirstDayOfMonth()).month() + 1;
            s.Master.calendarYear = moment(s.Master.calendarControl.getFirstDayOfMonth()).year();
        }

        function refreshCalendar(startDate, endDate, ID_Employee) {
            d.GetCalendarSource(startDate, endDate, ID_Employee).then(function (result) {
                s.events = result.data;
				angular.forEach(s.events, function (obj, idx) {
                    var start = moment(s.events[idx].starts_at).format("MM/DD/YYYY hh:mm:ss");
                    var end = moment(s.events[idx].ends_at).format("MM/DD/YYYY hh:mm:ss");
                    s.events[idx].starts_at = start;
                    s.events[idx].ends_at = end;
                });
                s.mainEvent = [];
                var dates = [];
                var d2 = [];
                //distinct date in events
                $.each(result.data, function (indx, val) {
                    if ($.inArray(val.starts_at, d2) === -1) {
                        d2.push(val.starts_at);
                        dates.push(val);
                    }
                });

                $.each(dates, function (idx, obj) {
                    var a1 = [];
                    //if Legal Holiday
                    $.each(result.data.filter(function (x) { return x.starts_at == obj.starts_at && x.DayType == "LH" }), function (idx, obj) {
                        a1.push(obj);
                    });
                    if (a1.length > 0) {
                        $.each(a1, function (idx, obj) {
                            s.mainEvent.push(obj);
                        });
                    } else {
                        //if Special Holiday
                        $.each(result.data.filter(function (x) { return x.starts_at == obj.starts_at && x.DayType == "SH" }), function (idx, obj) {
                            a1.push(obj);
                        });
                        if (a1.length > 0) {
                            $.each(a1, function (idx, obj) {
                                s.mainEvent.push(obj);
                            });
                        } else {
                            //if Rest Day
                            $.each(result.data.filter(function (x) { return x.starts_at == obj.starts_at && x.DayType == "RD" }), function (idx, obj) {
                                a1.push(obj);
                            });
                            if (a1.length > 0) {
                                $.each(a1, function (idx, obj) {
                                    s.mainEvent.push(obj);
                                });
                            }
                        }
                    }
                });

            });
        }

        s.months = [
            { ID: 1, Name: "January" },
            { ID: 2, Name: "February" },
            { ID: 3, Name: "March" },
            { ID: 4, Name: "April" },
            { ID: 5, Name: "May" },
            { ID: 6, Name: "June" },
            { ID: 7, Name: "July" },
            { ID: 8, Name: "August" },
            { ID: 9, Name: "September" },
            { ID: 10, Name: "October" },
            { ID: 11, Name: "November" },
            { ID: 12, Name: "December" }
        ];
        s.years = [];
        for (var i = 0; i < 41; i++) {
            s.years.push({ ID: (year - 20) + i, Name: String((year - 20) + i) });
        }

    }
    app.controller('cCalendar', ['$scope', 'dataService', '$modalInstance', '$state', 'Session', cCalendar]);

    var cNotification = function (s, d, mI, S, data) {
        s.dataNotification = data.data;
        s.closeDg = function () {
            mI.close();
        }
    }
    app.controller('cNotification', ['$scope', 'dataService', '$modalInstance', '$state', 'data', cNotification]);
});
