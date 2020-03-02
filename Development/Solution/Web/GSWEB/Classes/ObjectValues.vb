Imports Microsoft.VisualBasic
Imports System.Web
Public Class ObjectValues

    Public ID As String
    Public Name As String
    Public _Value As String
    Public menuID As Integer
    Public refID As Integer
    Public seq_no As Integer
    Public columnName As String


    Public Property Value As String
        Get
            Return HttpUtility.UrlDecode(Me._Value.Replace("+", "%2b"), System.Text.Encoding.Default)
        End Get
        Set(value As String)
            Me._Value = HttpUtility.UrlDecode(value.Replace("+", "%2b"), System.Text.Encoding.Default)
        End Set
    End Property

End Class

Public Class FilterValues

    Public Sub New()

    End Sub
    Public Sub New(ByVal menuID As Integer, ByVal columnName As String, ByVal columnValue As String, filterString As String)
        Me.menuID = menuID
        Me.columnName = columnName
        Me.columnValue = columnValue
        Me.filterString = filterString
    End Sub
    Public Property menuID As Integer
    Public Property columnName As String
    Public Property columnValue As String
    Public Property filterString As String
End Class

Public Class UserLinkValues

    Public Sub New()

    End Sub

    Public Sub New(ByVal ID As Integer, ByVal ID_WebMenuLinksAssignment As Integer, SeqNo As Integer)
        Me.ID = ID
        Me.ID_WebMenuLinksAssignment = ID_WebMenuLinksAssignment
        Me.SeqNo = SeqNo
    End Sub

    Public Property ID As String
    Public Property ID_WebMenuLinksAssignment As String
    Public Property SeqNo As String
End Class

Public Class MenuReferenceMap

    Public Sub New()

    End Sub

    Public Sub New(ByVal menuID As Integer, ByVal GuID As String)
        Me.menuID = menuID
        Me.GuID = GuID
    End Sub

    Public Property menuID As String
    Public Property GuID As String
End Class

Public Class ReferenceMap

    Public Sub New()

    End Sub

    Public Sub New(ByVal menuID As Integer, ByVal refID As Integer, ByVal menuGuID As String, ByVal refGuID As String)
        Me.menuID = menuID
        Me.refID = refID
        Me.menuGuID = menuGuID
        Me.refGuID = refGuID
    End Sub

    Public Property menuID As Integer
    Public Property refID As Integer
    Public Property menuGuID As String
    Public Property refGuID As String
End Class