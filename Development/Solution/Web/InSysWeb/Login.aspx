<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>

<%@ Import Namespace="System.Web.Optimization" %>

<!DOCTYPE html>
<meta http-equiv="X-UA-Compatible" content="IE=EDGE" />
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
    <%--<link rel="stylesheet" type="text/css" href="Styles/System/bootstrap.3.2.min.css" />
    <link rel="stylesheet" type="text/css" href="Styles/System/font-awesome.min.css" />
    <link rel="stylesheet" type="text/css" href="Styles/Default/default.css" />
    <link rel="stylesheet" type="text/css" href="Styles/Default/additional.css" />
    <link rel="stylesheet" type="text/css" href="Styles/System/loading-bar.css" />--%>
    <asp:PlaceHolder ID="PlaceHolder1" runat="server">        
        <%: Styles.Render("~/Styles/System/login-css") %> 
    </asp:PlaceHolder>
    <%--<script src="Scripts/angular.min.1.2.22.js" type='text/javascript'></script>
    <script src="Scripts/ui-bootstrap-tpls.js" type='text/javascript'></script>
    <script src="Scripts/loading-bar.js" type='text/javascript'></script>--%>

</head>
<body id="extr-page" class="animated fadeInDown desktop-detected" ng-app="app" ng-controller="LoginCtrl">
    <header id="header">

	    <div id="logo-group">
            <span id="logo"></span>
               
	    </div>
	    <%--<span class="extr-page-header-space" ng-show="!register"> 
            <a class="btn btn-danger" href="Register.aspx">SIGN UP</a>
	    </span>--%>

    </header>

    <div id="main" role="main">

	    <div id="content" class="container">

		    <div class="row">
			    <div class="col-xs-12 col-sm-12 col-md-7 col-lg-8 hidden-xs hidden-sm">
				    <%--<h1 class="txt-color-red login-header-big">Intellismart</h1>--%>
				    <div class="hero">
                        <div style="height: 100%; width: 100%">
                            <div carousel interval="5000">
                                <div slide ng-repeat="slide in slides" active="slide.active">
                                    <img ng-src="{{slide.image}}" style="margin: auto;">
                                </div>
                            </div>
                        </div>
					  
				    </div>

				  

			    </div>
                <div class="col-xs-12 col-sm-12 col-md-5 col-lg-4">
                    <div class="well no-padding" ng-show="register == 1">
                        <form id="login-form" autocomplete="off" name="login_form" class="smart-form client-form" novalidate="novalidate">
						    <header>
							    Sign In
						    </header>

						    <fieldset>
							    <section>
								    <label class="label">Username</label>
                                    <label class="input" ng-class="{ 'state-error' : login_form.txtusername.$invalid && login_form.submitted  }">
                                        <i class="icon-append fa fa-user"></i>
									    <input type="text" id="txtusername" name="txtusername" ng-model="master.Username" required>
                                    </label>
									   <%-- <b class="tooltip tooltip-top-right"><i class="fa fa-user txt-color-teal"></i> Please enter email address/username</b><em for="email" class="invalid">Please enter your email address</em>--%>
							    </section>

							    <section>
								    <label class="label">Password</label>
                                    <label class="input">
                                        <i class="icon-append fa fa-lock"></i>
									    <input type="password" id="txtpassword" name="txtpassword" ng-model="master.Password">
                                    </label>
									   <%-- <b class="tooltip tooltip-top-right"><i class="fa fa-lock txt-color-teal"></i> Enter your password</b><em for="password" class="invalid">Please enter your password</em>--%>
								    <div class="note">

                                        <p class="error">{{error_message}}</p>
									    <a ng-click="register = 2">Forgot password?</a>
								    </div>
							    </section>

						    </fieldset>
						    <footer>
							    <button type="submit" class="btn btn-primary" ng-click="Login()">
								    Sign in
							    </button>
						    </footer>
					    </form>

				    </div>

                    <div class="well no-padding" ng-show="register == 2">
                        <form id="Form1" name="forgot_password" autocomplete="off" class="smart-form client-form" novalidate="novalidate">
						    <header>
							    Forgot Password
						    </header>

						    <fieldset>
							    <section>
								    <label class="label">Username</label>
                                    <label class="input" ng-class="{ 'state-error' : forgot_password.txtusername.$invalid && forgot_password.submitted  }">
                                        <i class="icon-append fa fa-user"></i>
									    <input type="text" name="txtusername" ng-model="forgot_password.Username" required>
                                    </label>
                                    <label class="label">Security Question</label>
                                    <label class="input" ng-class="{ 'state-error' : forgot_password.txtQuestion.$invalid && forgot_password.submitted  }">
                                        <i class="icon-append fa fa-lock"></i>
                                        <select style="width:96.4%;" class="form-control" name="txtQuestion" ng-model="forgot_password.ID_SecurityQuestion" required>
                                            <option value="" selected>-Select One-</option>
                                            <option ng-repeat="d in QuestionList" value="{{d.ID}}">{{d.Name}}</option>
                                        </select>
                                    </label>
                                    <label class="label">Security Answer</label>
                                    <label class="input" ng-class="{ 'state-error' : forgot_password.txtSecurity.$invalid && forgot_password.submitted  }">
                                        <i class="icon-append fa fa-user"></i>
									    <input type="text" name="txtSecurity" ng-model="forgot_password.SecurityAnswer" required>
                                    </label>
                                     <div class="note">

                                        <p class="error">{{error_message}}</p>
                                        <p class="success" style="font-size:20px;font-weight:bold;color:#ff9933">{{success_message}}</p>
								    </div>
									   <%-- <b class="tooltip tooltip-top-right"><i class="fa fa-user txt-color-teal"></i> Please enter email address/username</b><em for="email" class="invalid">Please enter your email address</em>--%>
							    </section>

						    </fieldset>
						    <footer>
							    <button type="submit" class="btn btn-primary" ng-click="ForgotPassword()">
								    Submit
							    </button>
                                <button type="submit" class="btn btn-primary" ng-click="Back()">
								    Back
							    </button>
						    </footer>
					    </form>

				    </div>

                    <div class="well no-padding" ng-show="register == 3">

							<form id="register_form" autocomplete="off" name="register_form" class="smart-form client-form" novalidate="novalidate">
								<header>
									Registration is FREE*
								</header>

								<fieldset>
									<section>
                                    <label class="input" ng-class="{ 'state-error' : register_form.txtRegUsername.$invalid && register_form.submitted  }">
                                        <i class="icon-append fa fa-user"></i>
											<input type="text" name="txtRegUsername" id="txtRegUsername" placeholder="Username" ng-model="registration.RegUsername" required>
                                        <b class="tooltip tooltip-bottom-right">Needed to enter the website</b>
                                    </label>
									</section>

									<section>
                                    <label class="input" ng-class="{ 'state-error' : register_form.txtRegPassword.$invalid && register_form.submitted  }">
                                        <i class="icon-append fa fa-lock"></i>
											<input type="password" name="txtRegPassword" id="txtRegPassword" placeholder="Password" ng-model="registration.RegPassword" required>
                                        <b class="tooltip tooltip-bottom-right">Don't forget your password</b>
                                    </label>
									</section>

									<section>
                                    <label class="input" ng-class="{ 'state-error' : register_form.txtRegConfirmPassword.$invalid && register_form.submitted  }">
                                        <i class="icon-append fa fa-lock"></i>
											<input type="password" name="txtRegConfirmPassword" id="txtRegConfirmPassword" placeholder="Confirm password" ng-model="registration.RegConfirmPassword" required>
                                        <b class="tooltip tooltip-bottom-right">Don't forget your password</b>
                                    </label>
									</section>
								</fieldset>

                                <fieldset>
                                    <div class="row">
                                        <section class="col col-6">
										    <label class="input" ng-class="{ 'state-error' : register_form.txtRegFirstName.$invalid && register_form.submitted  }">
											    <input type="text" name="txtRegFirstName" id="txtRegFirstName" placeholder="First Name" ng-model="registration.RegFirstName" required>
									    </section>

                                        <section class="col col-6">
                                            <label class="input" ng-class="{ 'state-error' : register_form.txtRegLastName.$invalid && register_form.submitted  }">
											    <input type="text" name="txtRegLastName" id="txtRegLastName" placeholder="Last Name" ng-model="registration.RegLastName" required>

                                            <div class="note">

                                                <p class="error">{{error_reg_message}}</p>
								            </div>
                                        </section>
                                    </div>
                                </fieldset>

								<footer>
									<button type="submit" class="btn btn-primary" ng-click="Register()">
										Register
									</button>
								</footer>

<%--								<div class="message">
									<i class="fa fa-check"></i>
									<p>
										Thank you for your registration!
									</p>
								</div>--%>
							</form>

						</div>
				
				   
				
			    </div>
		    </div>
	    </div>

    </div>
   
</body>
<asp:PlaceHolder ID="PlaceHolder2" runat="server">
    <%: Scripts.Render("~/bundles/login-script") %>
</asp:PlaceHolder>
<script type="text/javascript" src="Scripts/login.js"></script>

</html>
