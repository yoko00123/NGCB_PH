"use strict";
define(["app"], function (a) {
    var d = function (n, i, t, k, o, m, p, j, l, f, r, e) {
        m.employee = {};
        n.notification = {};
        n.notification.Msg = [];
        n.notification.Cnt = 0;
        n.notification.NewMsg = [];
        n.notification.fData = "";
        n.LastID = 0;
        var q = o.get();
        n.Master = {};
        n.Master.favourites = [];
        i.getAllFavourites().then(function (g) {
            if (g.data.type == "2") {
                k.add(g.data.msg, "danger", 5000)
            }
            n.Master.favourites = g.data.menus;
            n.isfav = function (s) {
                var h = n.Master.favourites.filter(function (u) {
                    return u.mID == s
                }).length;
                if (h > 0) {
                    return true
                } else {
                    return false
                }
            };
            n.addToFavourites = [
                ["Add to bookmark", function (s, h, v, x) {
                    var w = parseInt(h.target.id);
                    var u = n.Master.favourites.filter(function (y) {
                        return y.mID == w
                    }).length;
                    if (u == 0) {
                        i.addToFavourites(w).then(function (y) {
                            if (y.data.type == "2") {
                                k.add(y.data.msg, "danger", 5000)
                            } else {
                                n.Master.favourites.push({
                                    mID: w,
                                    WebMenus: h.target.name,
                                    url: "#/" + h.target.name.replace(/ /g, "-") + "/",
                                    bgcolor: "red",
                                    color: "white",
                                    icon: '<i class="fa fa-music"></i>'
                                })
                            }
                        })
                    }
                }]
            ];
            n.removeFromFavourites = [
                ["Remove from bookmark", function (s, h, v) {
                    var w = parseInt(h.target.id);
                    var u = n.Master.favourites.filter(function (y) {
                        return y.mID == w
                    }).length;
                    if (u > 0) {
                        angular.forEach(n.Master.favourites, function (y, x) {
                            if (y.mID == w) {
                                i.removeFromFavourites(w).then(function (z) {
                                    if (z.data.type == "2") {
                                        k.add(z.data.msg, "danger", 5000)
                                    } else {
                                        n.Master.favourites.splice(x, 1)
                                    }
                                })
                            }
                        })
                    }
                }]
            ];
            n.createParent = function (h) {
                var u = "";
                var s = h.filter(function (v) {
                    return v.ID_WebMenus == 0
                });
                u += '<ul data-menu="main" class="menu__level">';
                angular.forEach(s, function (v) {
                    u += '<li class="menu__item">';
                    if (v.Children.length > 0) {
                        u += '<a class="menu__link" data-submenu="submenu-' + v.ID + '" href="#">' + (v.Label == "" ? v.Name : v.Label) + "</a>"
                    } else {
                        u += '<a name="' + v.Name + '" id="' + v.ID + '" context-menu="(isfav(' + v.ID + ') !== true ? addToFavourites : removeFromFavourites)" class="menu__link" href="#/' + v.Name.replace(/ /g, "-") + '/">' + (v.Label == "" ? v.Name : v.Label) + "</a>"
                    }
                    u += "</li>"
                });
                u += "</ul>";
                angular.forEach(s, function (v) {
                    u += n.createChild(v, v.ID)
                });
                return u
            };
            n.createChild = function (h, s) {
                var u = "";
                u += '<ul data-menu="submenu-' + s + '" class="menu__level">';
                angular.forEach(h.Children, function (v) {
                    u += '<li class="menu__item">';
                    if (v.Children.length > 0) {
                        u += '<a class="menu__link" data-submenu="submenu-' + v.ID + '" href="#">' + (v.Label == "" ? v.Name : v.Label) + "</a>"
                    } else {
                        u += '<a name="' + v.Name + '" id="' + v.ID + '" context-menu="(isfav(' + v.ID + ') !== true ? addToFavourites : removeFromFavourites)" class="menu__link" href="#/' + v.Name.replace(/ /g, "-") + '/">' + (v.Label == "" ? v.Name : v.Label) + "</a>"
                    }
                    u += "</li>"
                });
                u += "</ul>";
                angular.forEach(h.Children, function (v) {
                    u += n.createChild(v, v.ID)
                });
                return u
            };
            i.loadMenus().then(function (w) {
                n.menus = w.menus;

                function h(A) {
                    if (A !== "showLeft") {
                        classie.toggle(x, "disabled")
                    }
                }
                if (w.UseMenu == 1) {
                    var s = "";
                    s = '<div class="menu__wrap">' + n.createParent(n.menus) + "</div>";
                    angular.element($("#menuContainer").append(e(s)(n)));
                    var u = document.getElementById("ml-menu"),
                        x = $(".btnOpen");
                    x.on("click", function () {
                        var A = $(this).hasClass("active");
                        classie.toggle(this, "active");
                        classie.toggle(u, "cbp-spmenu-open");
                        $(".btnOpen i").attr("class", (A ? "fa fa-arrow-right" : "fa fa-arrow-left"));
                        $(".btnOpen").attr("style", (A ? "opacity:.5;" : "opacity:1;"));
                        $(".btnOpen").attr("title", (A ? "Open Menu" : "Close Menu"));
                        h("showLeft")
                    });
                    (function () {
                        var C = document.getElementById("ml-menu"),
                            D = new MLMenu(C, {
                                backCtrl: false
                            });
                        var F = document.querySelector(".action--open"),
                            B = document.querySelector(".action--close");
                        F.addEventListener("click", E);
                        B.addEventListener("click", A);

                        function E() {
                            classie.add(C, "menu--open")
                        }

                        function A() {
                            classie.remove(C, "menu--open")
                        }
                    })()
                }
                n.employee = m.employee = w.employee;
                var y = w.employee.CompanyLogoSize;
                n.width = y.split(";")[0];
                n.height = y.split(";")[1];
                n.EnableNotification = w.EnableNotification;
                n.TotalCnt = 10;
                if (Boolean(n.EnableNotification)) {
                    i.getNotification().poll(n.TotalCnt, 0).then(function (A) {
                        n.notification.Msg = A.Msg;
                        n.notification.Cnt = A.CntRecord;
                        angular.forEach(n.notification.Msg, function (C, B) {
                            angular.forEach(C, function (E, D) {
                                if (D == "ID") {
                                    if (n.LastID < E) {
                                        n.LastID = E
                                    }
                                }
                            })
                        })
                    });
                    var v = function () {
                        n.$apply(function () {
                            i.getNotification().poll(n.TotalCnt, n.LastID).then(function (A) {
                                if (A.Msg != undefined) {
                                    n.notification.Msg = A.Msg;
                                    $("#notificationLoading").html('See more notification <i class="fa fa-angle-right"></i>');
                                    n.notification.Cnt = A.CntRecord;
                                    n.notification.NewMsg = A.NewMsg;
                                    n.notification.fData = A.fData;
                                    n.notifyMessage(n.notification.NewMsg);
                                    angular.forEach(n.notification.Msg, function (C, B) {
                                        angular.forEach(C, function (E, D) {
                                            if (D == "ID") {
                                                if (n.LastID < E) {
                                                    n.LastID = E
                                                }
                                            }
                                        })
                                    })
                                }
                            })
                        })
                    };
                    var z;
                    clearInterval(z);
                    z = setInterval(v, 5000)
                }
            })
        });
        n.AddMoreNotification = function (g) {
            n.TotalCnt = n.TotalCnt + 10;
            $("#notificationLoading").html('<img src="Resources/System/notificationLoading.gif" />')
        };
        n.viewNotification = function (h) {
            var g = j.create("Dialogs/notification.html", "cNotification", {
                data: h
            }, {
                size: "md",
                keyboard: true,
                backdrop: true,
                windowClass: "my-class"
            });
            n.UpdateReadNotification(h)
        };
        n.UpdateReadNotification = function (g) {
            if (!g.IsView) {
                i.updateNotification(g.ID).then(function (h) {
                    if (h.message != "") {
                        k.add("There was an error updating your notification", "danger", 5000)
                    } else {
                        g.IsView = h.IsView;
                        n.notification.Cnt = n.notification.Cnt - 1
                    }
                })
            }
        };

        
        _.groupByMulti = function (s, w, h) {
            if (!w.length) {
                return s
            }
            var g = _.groupBy(s, w[0], h),
                v = w.slice(1);
            for (var u in g) {
                g[u] = _.groupByMulti(g[u], v, h)
            }
            return g
        };
        n.notifyMessage = function (g) {
            angular.forEach(g, function (s, h) {
                angular.forEach(s, function (w, u) {
                    var v = q.filter(function (y) {
                        return y.mID == w.ID_Parent
                    });
                    angular.forEach(w.Employee, function (z, x) {
                        var y = "<a href='#/" + v[0].stateName.replace(/ /g, "-") + "/" + z.fData + "'>You have " + String(z.CNT) + " " + u + " from " + z.Sender + "</a>";
                        k.add(y, "info", 5000)
                    })
                })
            })
        };
       
        n.replaceTimeSpan = function (g) {
            if (g.WeekSpan > 8) {
                return moment(g.DateTimeCreated).format("MM/DD/YYYY hh:mm A")
            } else {
                if (g.DaySpan > 7) {
                    return "About " + String(g.WeekSpan) + " week(s) ago..."
                } else {
                    if (g.HourSpan > 24) {
                        return "About " + String(g.DaySpan) + " day(s) ago..."
                    } else {
                        if (g.MinuteSpan > 60) {
                            return "About " + String(g.HourSpan) + " hour(s) ago..."
                        } else {
                            return "About " + String(g.MinuteSpan) + " minute(s) ago..."
                        }
                    }
                }
            }
        };
        n.Session = p.data;
        n.PublishWebsite = function () {
            i.PublishWebsite().then(function (g) {
                k.add(g.message, "info", 5000)
            })
        };
        n.ClearMenu = function () {
            i.ClearMenu().then(function (g) {
                k.add(g.message, "info", 5000);
                o.reload()
            })
        };
        n.MinifyScripts = function () {
            i.MinifyScripts().then(function (g) {
                k.add(g.message, "info", 5000);
                o.reload()
            })
        };
        n.Run = function () {
            var g = j.create("Dialogs/RunScript.html", "ScriptController", {}, {
                size: "xl",
                keyboard: true,
                backdrop: "static"
            })
        };
        n.ShowFileExplorer = function () {
            j.create("Dialogs/FileExplorer.html", "FileExplorerController", {}, {
                size: "xl",
                keyboard: true,
                backdrop: "static"
            })
        };
        n.ShowSystemDashboard = function () {
            j.create("Dialogs/SystemDashboard.html", "SystemDashboardController", {}, {
                size: "xl",
                keyboard: true,
                backdrop: "static"
            })
        };
        l.bindTo(n).add({
            combo: "alt+shift+r+s",
            callback: function () {
                if (p.data.ID_User == 1) {
                    n.Run()
                }
            }
        }).add({
            combo: "alt+shift+f+e",
            callback: function () {
                if (p.data.ID_User == 1) {
                    n.ShowFileExplorer()
                }
            }
        }).add({
            combo: "alt+l",
            callback: function () {
                window.location.href = "Logout.aspx"
            }
        }).add({
            combo: "alt+shift+s+d",
            callback: function () {
                if (p.data.ID_User == 1) {
                    n.ShowSystemDashboard()
                }
            }
        });
        n.loadCalendar = function () {
            var g = j.create("Dialogs/calendar.html", "cCalendar", {}, {
                size: "xl",
                keyboard: true,
                backdrop: true,
                windowClass: "my-class"
            })
        }
    };
    a.controller("menuCtrl", ["$scope", "dataService", "utilService", "growlNotifications", "$state", "$rootScope", "Session", "dialogs", "hotkeys", "$filter", "$timeout", "$compile", d]);
    var b = function (k, e, g, l, m) {
        k.Master = {};
        k.calendarView = "month";
        k.calendarDay = new Date();
        k.events = [];
        k.mainEvent = [];
        k.Master.calendarMonth = moment(k.calendarDay).month() + 1;
        k.Master.calendarYear = moment(k.calendarDay).year();
        k.Master.calendarEmployee = m.data.ID_Employee;
        k.EmployeeSource = [];
        e.GetApproverEmployee(m.data.ID_Employee).then(function (i) {
            k.EmployeeSource = i.data
        });
        var o = k.Master.calendarYear,
            h = k.Master.calendarMonth - 1;
        var n = moment([o, h]).toDate();
        j(n, moment(n).endOf("month").toDate(), k.Master.calendarEmployee);
        k.gotoMonth = function () {
            n = moment([k.Master.calendarYear, k.Master.calendarMonth - 1]).toDate();
            j(n, moment(n).endOf("month").toDate(), k.Master.calendarEmployee);
            k.Master.calendarControl.goToMonth(k.Master.calendarMonth)
        };
        k.gotoYear = function () {
            n = moment([k.Master.calendarYear, k.Master.calendarMonth - 1]).toDate();
            j(n, moment(n).endOf("month").toDate(), k.Master.calendarEmployee);
            k.Master.calendarControl.goToYear(k.Master.calendarYear)
        };
        k.prev = function () {
            k.Master.calendarControl.prev();
            j(k.Master.calendarControl.getFirstDayOfMonth(), k.Master.calendarControl.getLastDayOfMonth(), k.Master.calendarEmployee);
            k.Master.calendarMonth = moment(k.Master.calendarControl.getFirstDayOfMonth()).month() + 1;
            k.Master.calendarYear = moment(k.Master.calendarControl.getFirstDayOfMonth()).year()
        };
        k.next = function () {
            k.Master.calendarControl.next();
            j(k.Master.calendarControl.getFirstDayOfMonth(), k.Master.calendarControl.getLastDayOfMonth(), k.Master.calendarEmployee);
            k.Master.calendarMonth = moment(k.Master.calendarControl.getFirstDayOfMonth()).month() + 1;
            k.Master.calendarYear = moment(k.Master.calendarControl.getFirstDayOfMonth()).year()
        };
        k.setCalendarToToday = function () {
            k.Master.calendarControl.setCalendarToToday();
            j(k.Master.calendarControl.getFirstDayOfMonth(), k.Master.calendarControl.getLastDayOfMonth(), k.Master.calendarEmployee);
            k.Master.calendarMonth = moment(k.Master.calendarControl.getFirstDayOfMonth()).month() + 1;
            k.Master.calendarYear = moment(k.Master.calendarControl.getFirstDayOfMonth()).year()
        };

        function j(q, i, p) {
            e.GetCalendarSource(q, i, p).then(function (t) {
                k.events = t.data;
                angular.forEach(k.events, function (w, v) {
                    var x = moment(k.events[v].starts_at).format("MM/DD/YYYY hh:mm:ss");
                    var u = moment(k.events[v].ends_at).format("MM/DD/YYYY hh:mm:ss");
                    k.events[v].starts_at = x;
                    k.events[v].ends_at = u
                });
                k.mainEvent = [];
                var s = [];
                var r = [];
                $.each(t.data, function (u, v) {
                    if ($.inArray(v.starts_at, r) === -1) {
                        r.push(v.starts_at);
                        s.push(v)
                    }
                });
                $.each(s, function (v, w) {
                    var u = [];
                    $.each(t.data.filter(function (y) {
                        return y.starts_at == w.starts_at && y.DayType == "LH"
                    }), function (x, y) {
                        u.push(y)
                    });
                    if (u.length > 0) {
                        $.each(u, function (x, y) {
                            k.mainEvent.push(y)
                        })
                    } else {
                        $.each(t.data.filter(function (y) {
                            return y.starts_at == w.starts_at && y.DayType == "SH"
                        }), function (x, y) {
                            u.push(y)
                        });
                        if (u.length > 0) {
                            $.each(u, function (x, y) {
                                k.mainEvent.push(y)
                            })
                        } else {
                            $.each(t.data.filter(function (y) {
                                return y.starts_at == w.starts_at && y.DayType == "RD"
                            }), function (x, y) {
                                u.push(y)
                            });
                            if (u.length > 0) {
                                $.each(u, function (x, y) {
                                    k.mainEvent.push(y)
                                })
                            }
                        }
                    }
                })
            })
        }
        k.months = [{
            ID: 1,
            Name: "January"
        }, {
            ID: 2,
            Name: "February"
        }, {
            ID: 3,
            Name: "March"
        }, {
            ID: 4,
            Name: "April"
        }, {
            ID: 5,
            Name: "May"
        }, {
            ID: 6,
            Name: "June"
        }, {
            ID: 7,
            Name: "July"
        }, {
            ID: 8,
            Name: "August"
        }, {
            ID: 9,
            Name: "September"
        }, {
            ID: 10,
            Name: "October"
        }, {
            ID: 11,
            Name: "November"
        }, {
            ID: 12,
            Name: "December"
        }];
        k.years = [];
        for (var f = 0; f < 41; f++) {
            k.years.push({
                ID: (o - 20) + f,
                Name: String((o - 20) + f)
            })
        }
    };
    a.controller("cCalendar", ["$scope", "dataService", "$modalInstance", "$state", "Session", b]);
    var c = function (h, e, g, i, f) {
        h.dataNotification = f.data;
        h.closeDg = function () {
            g.close()
        }
    };
    a.controller("cNotification", ["$scope", "dataService", "$modalInstance", "$state", "data", c])
});