Imports System.Net
Imports System.Net.Mail
Imports System.Data.SqlClient
Imports System.ComponentModel

Public Class MailSender

#Region "Variables"
    Public smtpServer As String
    Public smtpPort As Integer
    Public SSLEnabled As Boolean
    Public AnonymousAuthentication As Boolean
    Public fromEmail As String
    Public fromEmailPassword As String
    Public fromName As String
    Public displayFromAddress As String
    Public connString As String

    Private sc As New SmtpClient
    Private smtpCredentials As NetworkCredential

#End Region

#Region "Init"
    Public Sub Init()
        sc.Port = smtpPort
        sc.Host = smtpServer

        If Not AnonymousAuthentication Then
            smtpCredentials = New NetworkCredential(fromEmail, fromEmailPassword)
            sc.Credentials = smtpCredentials
            sc.EnableSsl = SSLEnabled
        End If

        AddHandler sc.SendCompleted, AddressOf SendCompletedCallback
    End Sub
#End Region

#Region "Send"
    Public Sub Send(ByVal sendTo As String, ByVal subject As String, ByVal body As String, ByVal ID As Integer)
        Try
            If System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() Then
                Dim Message As New MailMessage
                Message.IsBodyHtml = True
                Message.To.Add(New MailAddress(sendTo))
                If displayFromAddress.Length > 0 Then
                    Message.From = New MailAddress(displayFromAddress, fromName)
                Else
                    Message.From = New MailAddress(fromEmail, fromName)
                End If

                Message.Subject = subject
                Message.Body = body

                Dim userState(5) As String
                userState(0) = sendTo
                userState(1) = subject
                userState(2) = body
                userState(3) = ID
                sc.SendAsync(Message, userState)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub SendCompletedCallback(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs)
        Dim token() As String = CType(e.UserState, String())

        If e.Cancelled Then
            'Sending Cancelled
        End If
        If e.Error IsNot Nothing Then
            Using sqlConn As New SqlConnection(connString)
                sqlConn.Open()
                Dim transaction As SqlTransaction
                transaction = sqlConn.BeginTransaction("Trans")
                Using sqlCommand As New SqlCommand()
                    Try
                        With sqlCommand
                            .CommandText = "INSERT INTO tEmails (Name, Comment, Receiver, ID_Reference)" & _
                                                   "VALUES (@Name, @Comment, @Receiver, @ID)"
                            .Connection = sqlConn
                            .Transaction = transaction
                            .Parameters.AddWithValue("@Name", token(1))
                            .Parameters.AddWithValue("@Comment", token(2))
                            .Parameters.AddWithValue("@Receiver", token(0))
                            .Parameters.AddWithValue("@ID", token(3))
                            .ExecuteNonQuery()
                        End With
                        transaction.Commit()
                    Catch ex As Exception
                        transaction.Rollback()
                    Finally
                        sqlCommand.Dispose()
                    End Try
                End Using
                sqlConn.Close()
            End Using
        Else
            'Message Sent
        End If
    End Sub

#End Region
End Class
