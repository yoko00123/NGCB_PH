<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Index.aspx.vb" Inherits="Index" EnableSessionState="true" %>

<%@ Import Namespace="System.Web.Optimization" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="icon" href="Resources/System/Insys.ico" />
    <title>Insys HRMS</title>
    <meta charset="UTF-8" />
    <meta name="description" content="Integrated Online System(IONS) and Human Resource Management System(HRMS) - Authors:Mark Follero, Rossu Belmonte" />
    <meta name="keywords" content="HRMS,Human Resource Management System,Mark Follero,AngularJs,Rossu Belmonte" />
    <meta name="author" content="Mark Follero,topefollero22@gmail.com" />
    <link rel="author" href="https://facebook.com/topefollero22"/>
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">        
        <%: Styles.Render("~/Styles/System/system-css") %> 
        <%: Styles.Render("~/Styles/System/additional-css") %> 
    </asp:PlaceHolder>
    <%=If(Session("Skins").ToString.Length > 0,"<link rel='stylesheet' type='text/css' href='Styles/Skins/" & Session("Skins") & ".css'/>","")%>
    <%If System.Configuration.ConfigurationManager.AppSettings("UseMenu") = 0 Then%>
    <link rel="stylesheet" type="text/css" href="Styles/Default/DefaultMenu.css" />
    <%End If%>
</head>
<body class="<%=Session("Skins")%>"><!--menu-on-top fixed-header fixed-navigation -->
    <div ng-controller="menuCtrl">
	<header id="header">
		<div id="logo-group">
			<span id="logo">
                <img ng-src="Contents/Company//{{employee.CompanyImage}}" alt="Intellismart" ng-style="{'width' : width, 'height' : height}" count-watch/>
                <span ng-cloak ng-if="employee.ShowCompanyName" id="company-name" style="font-size: 20px;margin-left: 10px;letter-spacing: 2px;">{{employee.Company}}</span>
			</span>
		</div>
		<div class="pull-right">		
			<ul id="mobile-profile-img" class="header-dropdown-list hidden-xs padding-5">
				<li class="">
                    <div id="userbutton" data-toggle-dropdown>
                        <span>{{employee.FirstName}}</span>
					    <a href="javascript:void(0);" class="dropdown-toggle no-margin userdropdown"> 
						    <img id="onlineImage" ng-src="Upload/{{employee.ImageFile_GUID}}" alt="Avatar" class="online">  
					    </a>
                    </div>
					<ul class="dropdown-menu pull-right">
						<li>
							<a href="#/Accounts/{{employee.$$ID_User}}" class="padding-10 padding-top-0 padding-bottom-0"> <i class="fa fa-user"></i> <u>A</u>ccount </a>
						</li>
						<li class="divider"></li>
                        <li ng-if="employee.ID_User == 1">
							<a accesskey="p" href="javascript:void(0);" ng-click="PublishWebsite()" class="padding-10 padding-top-0 padding-bottom-0"> <i class="fa fa-book"></i> <u>P</u>ublish Website</a>
						</li>
						<li ng-if="employee.ID_User == 1" class="divider"></li>
                        <li ng-if="employee.ID_User == 1">
							<a accesskey="c" href="javascript:void(0);" ng-click="ClearMenu()" class="padding-10 padding-top-0 padding-bottom-0"> <i class="fa fa-cogs"></i> <u>C</u>lear Menu</a>
						</li>
						<li ng-if="employee.ID_User == 1" class="divider"></li>
                        <li ng-if="employee.ID_User == 1">
							<a accesskey="m" href="javascript:void(0);" ng-click="MinifyScripts()" class="padding-10 padding-top-0 padding-bottom-0"> <i class="fa fa-file-text"></i> Minify Resources</a>
						</li>
						<li ng-if="employee.ID_User == 1" class="divider"></li>
						<li>
							<a href="Logout.aspx" class="padding-10 padding-top-0 padding-bottom-5" data-action="userLogout"><i class="fa fa-sign-out fa-lg"></i> <u>L</u>ogout</a>
						</li>
					</ul>
				</li>
			</ul> 
			<%--<div id="hide-menu" class="btn-header pull-right">
				<span>
					<a href="javascript:void(0);" data-toggle-hide-nav data-action="toggleMenu" title="Collapse Menu">
						<i class="fa fa-reorder"></i>
					</a>
				</span>
			</div>--%>
            <%If System.Configuration.ConfigurationManager.AppSettings("UseMenu") = 0 Then%>
            <div id="hide-menu" class="btn-header pull-right">
				<span>
					<a href="javascript:void(0);" data-toggle-hide-nav data-action="toggleMenu" title="Collapse Menu">
						<i class="fa fa-reorder"></i>
					</a>
				</span>
			</div>
            <%End If%>

            <%--full screen--%>
		<%--	<div id="fullscreen" class="btn-header transparent pull-right">
				<span> <a href="javascript:void(0);" toggle-full-screen title="Full Screen"><i class="fa fa-arrows-alt"></i></a></span>
			</div>--%>
            <div class="btn-header transparent pull-right">
				<span><a href="javascript:void(0);" ng-click="loadCalendar()" title="Calendar"><i class="fa fa-calendar"></i></a></span>
			</div>
            <%If System.Configuration.ConfigurationManager.AppSettings("UseMenu") = 1 Then%>
            <div class="btn-header transparent pull-right">
                <span>
                    <button title="Open Menu" style="position:relative!important;left:0px!important;color:black!important;" class="action action--open" aria-label="Open Menu"><i class="fa fa-reorder"></i></button>
                </span>
            </div>
            <%End If%>
            <ul ng-if="EnableNotification" class="header-dropdown-list hidden-xs pull-right" style="right:-5px;">
				<li class="">
                    <div class="btn-header transparent pull-right" data-toggle-dropdown >
                        <span>
                            <div ng-if="notification.Cnt > 100" class="notificationCount">100+</div>
                            <div ng-if="notification.Cnt <= 100" class="notificationCount">{{notification.Cnt}}</div>
                            <a href="javascript:void(0);" title="Notification">
                                <i class="glyphicon glyphicon-bell"></i>
                            </a>
                        </span>
                    </div>
					<ul id="notificationDropdown" class="dropdown-menu dropdown-messages pull-right" style="width: 500%;">
                        <li ng-click="$event.stopPropagation();" ng-if="notification.Msg.length == 0">
                            <a>
                                <div style="text-align:center;" class="text-muted"><i>No available notification.</i></div>
                            </a>
                        </li>
						<li ng-click="$event.stopPropagation();" ng-repeat="data in notification.Msg">
                            <a ng-click="viewNotification(data)" ng-if="data.ID_WebMenus == 0 || data.ID_WebMenus == null" ng-class="{'isViewed': !data.IsView}" class="notificationA">
                                <div>
                                    <div style="float:left;">
                                        <img ng-src="Upload/{{data.ImageFile}}" class="notificationImg" />
                                    </div>
                                    <strong>{{data.Sender}}</strong>
                                    <span class="pull-right text-muted">
                                        <em>{{replaceTimeSpan(data)}}</em>
                                    </span>
                                </div>
                                <div>{{data.Title}}</div>
                            </a>
                            <a ng-click="UpdateReadNotification(data)" ng-if="data.ID_WebMenus != 0 && data.ID_WebMenus != null" ui-sref="{{data.ID_WebMenus}}({ID_{{data.ID_WebMenus}}:data.ReferenceID})" ng-class="{'isViewed': !data.IsView}" class="notificationA">
                                <div>
                                    <div style="float:left;">
                                        <img ng-src="Upload/{{data.ImageFile}}" class="notificationImg" />
                                    </div>
                                    <strong>{{data.Sender}}</strong>
                                    <span class="pull-right text-muted">
                                        <em>{{replaceTimeSpan(data)}}</em>
                                    </span>
                                </div>
                                <div>{{data.WebMenus}}</div>
                            </a>
                        </li>
                        <li ng-click="$event.stopPropagation();">
                            <div class="text-center dNotif">
                                <table id="notificationTable" width="100%">
                                    <tr>
                                        <td style="min-width:50%;width:35%;">
                                            <a ng-click="AddMoreNotification()" id="notificationLoading" style="font-weight:bold;">
                                                View all
                                                <i class="fa fa-angle-right"></i>
                                            </a>
                                        </td>
                                        <td>
                                           <%-- <a href="#/Notification/" style="font-weight:bold;">--%>
                                            <a ng-click="notificationClear()"  id="notificationLoading"  style="font-weight:bold;">
                                                View all as read
                                                <i class="glyphicon glyphicon-envelope"></i>
                                            </a>
                                        </td>
                                        <td style="min-width:50%;width:32%;">
                                            <%-- <a href="#/Notification/" style="font-weight:bold;">--%>
                                                <a ng-click="DropNotification()"  id="notificationLoading" style="font-weight:bold;">
                                                Clear Inbox
                                                <i class="glyphicon glyphicon-trash"></i>
                                            </a>
                                        </td>    
                                    </tr>
                                </table>
                            </div>
                        </li>
					</ul>
				</li>
			</ul>
		</div>
	</header>
	<aside id="left-panel">
        <%If System.Configuration.ConfigurationManager.AppSettings("UseMenu") = 0 Then%>
		<nav><div ng-recursive-menu menus="menus" favourites="Master.favourites"></div></nav>
        <%End If%>
	</aside>
        <%If System.Configuration.ConfigurationManager.AppSettings("UseMenu") = 1 Then%>
        <nav id="ml-menu" class="menu cbp-spmenu cbp-spmenu-vertical cbp-spmenu-left">
            <button class="action action--close" aria-label="Close Menu"><span class="icon2 icon--cross"></span></button>
            <div id="menuContainer"></div>
            <div class="btnOpen" title="Open Menu"><i class="fa fa-arrow-right"></i></div>
		</nav>
        <%End If%>
        <div class="kc_fab_wrapper" ><div ng-fab favourites="Master.favourites"></div></div>
    </div>
	<div id="main" role="main" class="slide-animation" ui-view>
	</div>  
	<div growl-notifications  class="notifications"></div>
	<div data-wc-overlay="" data-wc-overlay-delay="300">
		<h1 class="ajax-loading-animation"><i class="fa fa-spinner fa-spin"></i> Loading...</h1>
	</div> 

   <div id="initial-spinner" class="spinner-wrapper">
        <div class="overlay-spinner spinner2 center">
            <div class="loader">
                <!--[if lte IE 9]>	Loading ...<![endif]-->
            </div>
        </div>
    </div>

    <style>
        .spinner-wrapper {
            width: 100%;
            height: 100%;
            background: #f2f2f2;
            position: absolute;
            z-index: 9999;
            top: 0px;
        }

        .overlay-spinner.center {
            width: 80%;
            left: 10%;
        }
        .spinner2 {
            height: 100%;
            line-height: 564px;
            text-align: center;
        }
        .overlay-spinner {
            overflow: hidden;
            background: #fff;
            width: 100%;
            height: 100%;
            max-width: 1800px;
            margin: 0 auto;
            position: absolute;
            z-index: 99;
        }

        .overlay-spinner .loader, .overlay-spinner .loader_ie {
            margin: 20% auto;
        }
        .loader {
            border: 3px solid #eee;
            font-size: 60px;
            width: 1em;
            height: 1em;
            border-radius: .5em;
            -webkit-box-sizing: border-box;
            -moz-box-sizing: border-box;
            -ms-box-sizing: border-box;
            -o-box-sizing: border-box;
            box-sizing: border-box;
            -webkit-animation: spin 1s linear infinite;
            -moz-animation: spin 1s linear infinite;
            animation: spin 1s linear infinite;
            border-top-color: #333;
        }
    </style>
    <div style="display:none;" id="hubNotification">
        <div class="notifications">
            <div class="alert alert-info"></div>
        </div>
    </div>
    
</body>
    
    <asp:PlaceHolder ID="PlaceHolder2" runat="server">        
        <%--<%: Scripts.Render("~/bundles/script") %>--%>
        <%: Scripts.Render("~/bundles/core-scripts") %>
        <%: Scripts.Render("~/bundles/plugin-scripts") %>
        <%: Scripts.Render("~/bundles/custom-scripts") %>
    </asp:PlaceHolder>
    <script src="../signalr/hubs"></script>
    <%If System.Configuration.ConfigurationManager.AppSettings("EnableSignalR") = True Then%>
    <script type="text/javascript">
        var notification = $.connection.SignalRHub;
        var ConnectionID;
        $.connection.hub.qs = "UserID=<%=Session("ID_User")%>&SessionID=<%=Session.SessionID%>";
        $.connection.hub.start().done(function () {
            ConnectionID = $.connection.hub.id;
            console.log("Hub connected");

        }).fail(function () {
            console.log("Error in hub connection.");
        });
        $.connection.hub.connectionSlow(function () {
            console.log('We are currently experiencing difficulties with the connection.');
        });

        try {
            notification.client.NewUserNotification = function (message) {
                $("#hubNotification").attr("style", "");
                $("#hubNotification .notifications .alert").empty();
                $("#hubNotification .notifications .alert").append(message);
            }

            notification.client.logout = function () {
                alert('Your account has been log in to another computer. The system will automatically logout.');
                location.href = 'Logout.aspx';
            }
        } catch (e) {
            console.log(e.message);
        }

    </script>
    <%End If%>
    <script>
         var Web_System_Version = '<%=Session.Item("SystemVersion")%>',
             ForDev = '<%=Session.Item("ForDevelopment")%>',
             ClientCustomFolder = '<%=Session.Item("ClientCustomFolder")%>';
        document.write("<script src='Scripts/require.js' type='text/javascript' data-main='scripts/main.js?v=" + Web_System_Version + "' ><\/script>");
    </script>
</html>