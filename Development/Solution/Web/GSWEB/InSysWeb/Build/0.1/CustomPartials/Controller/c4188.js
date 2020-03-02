"use strict";
define(["app"], function (a) {
    var b = function (l, k, f, q, m, i, o, n, e, h, j) {
        if (k.error !== undefined) {
            i.add(k.error, "danger", 5000)
        }
        l.Sum = function (g, d) {
            return q.Sum(g, d)
        };
        l.Max = function (g, d) {
            return q.Max(g, d)
        };
        l.Min = function (g, d) {
            return q.Min(g, d)
        };
        l.Ave = function (g, d) {
            return q.Ave(g, d)
        };
        l.mID = 4188;
        l.rID = m.params.ID_4188;
        l.goPrevious = function () {
            m.go("4187", {}, {
                reload: true,
                inherit: false,
                notify: true
            })
        };
        l.Session = n.data;
        l.Master = k.Master;
        l.Parent = k.Parent;
        l.Detail = k.Detail;
        l.dropdown_source = k.dropdown_source;
        l.rdb_source = k.rdb_source;
        l.autocomplete_source = k.autocomplete_source;
        l.text_autocomplete_source = k.text_autocomplete_source;
        l.gridOptions = {};
        l.removeRow = function (d, g) {
            if (g.entity.ID == 0) {
                l.Detail[d].splice(g.rowIndex, 1)
            } else {
                f.GridDelete(d, g.entity.ID).then(function (p) {
                    l.Detail[d].splice(g.rowIndex, 1)
                })
            }
        };
        l.setID = function (g, p, d) {
            g[p.substring(3)] = d.Name;
            g[p] = d.ID
        };
        l.getAutoCompleteItems = function (g, d, p) {
            return f.getAutoCompleteItems(g, d, p).then(function (r) {
                return r.items
            })
        };
        l.tabs = [];
        l.tabs.activeTab = (localStorage.getItem("mID") == l.mID ? (localStorage.getItem("tab") == undefined ? 0 : localStorage.getItem("tab")) : 0);
        localStorage.removeItem("mID");
        localStorage.removeItem("tab");
        l.loadedTab = [];
        l.loadTab = function (d) {
            if (l.loadedTab.indexOf(d) == -1) {
                f.loadTab(d, l.rID).then(function (g) {
                    l.Detail[d] = g.data;
                    l.loadedTab.push(d)
                })
            }
        };
        l.selectedFiles = [];
        l.onFileSelect = function (d, r, s, p) {
            for (var g = 0; g < l.selectedFiles.length; g++) {
                if (l.selectedFiles[g].mID == r && l.selectedFiles[g].name == s && l.selectedFiles[g].idx == p) {
                    l.selectedFiles.splice(g, 1);
                    break
                }
            }
            l.selectedFiles.push({
                file: d[0],
                mID: r,
                name: s,
                idx: p
            })
        };
        l.loadPasswordDialog = function () {
            var d = h.create("Dialogs/c4188Dialog.html", "c4188Dialog", {
                id: l.rID,
                isFirstLog: l.Master.IsFirstLog,
                id_session: l.Session.ID_User,
                p: j,
                Di: h
            }, {
                size: "md",
                keyboard: true,
                backdrop: true,
                windowClass: "my-class"
            })
        };
        l.CascadingDropdown = function (p, d, r, g) {
            f.CascadingDropdown(p, d, r).then(function (u) {
                var s = g.split(",");
                for (var t = 0; t < s.length; t++) {
                    l.dropdown_source[s[t]] = u.data[s[t]]
                }
            })
        };
        l.notifyExpiredPassword = function () {
            var d = h.notify("Password", "Password is expired, please change your password.", {
                size: "md",
                keyboard: true,
                backdrop: true,
                windowClass: "my-class"
            });
            d.result.then(function (g) { }, function () { })
        };
        if (l.Session.isPasswordExpired) {
            l.notifyExpiredPassword()
        }
        e.check(l)
    };
    a.register.controller("c4188", ["$scope", "resources", "dataService", "utilService", "$state", "growlNotifications", "$templateCache", "Session", "ckFormPristine", "dialogs", "parameters", b]);
    var c = function (k, e, j, f, i, h) {
        k.Title = "Password";
        k.Data = {
            ID: f.id,
            ID_Session: f.id_session,
            Password: null,
            NewPassword: null,
            ConfirmPassword: null,
            IsFirstLog: f.isFirstLog
        };
        k.cancel = function () {
            j.dismiss("Canceled")
        };
        k.savedPassword = function () {
            var d = h.notify("Password", "Successfully changed password.", {
                size: "md",
                keyboard: true,
                backdrop: true,
                windowClass: "my-class"
            });
            d.result.then(function (g) {
                window.location = "LogOut.aspx"
            }, function () { })
        };
        k.save = function () {
            if (k.Data.NewPassword.lenght != 0 && k.Data.ConfirmPassword.length != 0) {
                if (k.Data.NewPassword == k.Data.ConfirmPassword) {
                    if (parseInt(k.Data.NewPassword.length) >= parseInt(f.p.PassWordSize)) {
                        if (k.Data.NewPassword.match(/^[0-9a-z]+$/)) {
                            e.SavePassword(k.Data).then(function (d) {
                                if (d.message.length > 0) {
                                    i.add(d.message, "danger", 5000)
                                } else {
                                    k.cancel();
                                    k.savedPassword()
                                }
                            })
                        } else {
                            i.add("Please use an alphanumeric characters only.", "danger", 5000)
                        }
                    } else {
                        i.add("Password minimum required character length is (" + f.p.PassWordSize + ").", "danger", 5000)
                    }
                } else {
                    i.add("New password and confirm password did not match.", "danger", 5000)
                }
            } else {
                i.add("Please provide a new password.", "", 5000)
            }
        }
    };
    a.register.controller("c4188Dialog", ["$scope", "dataService", "$modalInstance", "data", "growlNotifications", "dialogs", c])
});