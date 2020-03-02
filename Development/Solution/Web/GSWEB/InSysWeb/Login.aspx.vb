
Partial Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not Me.Session("ID_User") Is Nothing And Not Me.Session("ID_SelectedCompany") Is Nothing And (Not (Me.Session("IsSecretQuestionReady")) Is Nothing And Me.Session("IsSecretQuestionReady") = 1) Then Me.Response.Redirect("~/Index.aspx")
    End Sub

End Class
