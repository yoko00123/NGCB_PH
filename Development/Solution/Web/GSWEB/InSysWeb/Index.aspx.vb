Imports GSWEB.Common
Imports Newtonsoft.Json.Linq

Partial Class Index
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Session("ID_User") Is Nothing Or Me.Session("ID_SelectedCompany") Is Nothing Or (Me.Session("IsSecretQuestionReady") Is Nothing Or Me.Session("IsSecretQuestionReady") = 0) Then Me.Response.Redirect("~/Login.aspx")
        Dim ret As String = String.Empty
        Dim dictionary As Dictionary(Of String, Object) = SessionToDictionary(HttpContext.Current.Session)
        dictionary.Add("$$ID_User", """" & toAnyBase(dictionary("ID_User")) & """") 'GetIndirectReference(dictionary("ID_User"), HttpContext.Current.Session)
        dictionary.Add("$$ID_Employee", """" & toAnyBase(dictionary("ID_Employee")) & """") 'GetIndirectReference(dictionary("ID_Employee"), HttpContext.Current.Session)
        dictionary.Add("$$ID_Persona", """" & toAnyBase(dictionary("ID_Persona")) & """") ' GetIndirectReference(dictionary("ID_Persona"), HttpContext.Current.Session) 
        Dim a = dictionary.[Select](Function(d) String.Format("""{0}"":{1}", d.Key, IsNull(d.Value, 0)))

        ret = "{" & String.Join(",", a) + "}"
        Dim SessToken As HttpCookie = New HttpCookie("SESS-TOKEN", ret)
        HttpContext.Current.Response.Cookies.Add(SessToken)
        SetCsrfCookie()
        Session.Item("SystemVersion") = ExecScalarNoParams("SELECT dbo.fGetWebParameter('SystemVersion')")
        Session.Item("ForDevelopment") = ExecScalarNoParams("SELECT dbo.fGetWebParameter('ForDevelopment')")
        Session.Item("ClientCustomFolder") = ExecScalarNoParams("SELECT dbo.fGetWebParameter('ClientCustomFolder')")
        
    End Sub

    Private Sub SetCsrfCookie()
        Dim csrfCookie As HttpCookie = New HttpCookie("XSRF-TOKEN", New GSWEB.Utility.CsrfTokenHelper().GenerateCsrfTokenFromAuthToken(CStr(Me.Session("ID_User")) & "_" & CStr(Me.Session("CSRF-TOKEN"))))
        csrfCookie.HttpOnly = False
        HttpContext.Current.Response.Cookies.Add(csrfCookie)
    End Sub
End Class
