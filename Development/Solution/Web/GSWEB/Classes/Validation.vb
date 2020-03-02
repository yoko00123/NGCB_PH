Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Text.RegularExpressions

Namespace Common

    Public Class Validation

        Shared Function Required2(ByVal s As String, ByVal controlType As Integer, ByVal ColumnName As String) As Boolean
            Select Case controlType
                Case 23
                    If ColumnName.Contains("ID_") Then
                        If CInt(s) > 0 Then
                            Return False
                        End If
                    Else
                        If Len(Trim(s)) > 0 Then
                            Return False
                        End If
                    End If
                Case 2, 17, 26
                    If CInt(s) > 0 Then
                        Return False
                    End If
                Case 3
                    If CBool(s) = True Then
                        Return False
                    End If
                Case Else
                    If Len(Trim(s)) > 0 Then
                        Return False
                    End If
            End Select
            Return True
        End Function

        Shared Function min_len(ByVal s As String, ByVal l As Integer) As Boolean
            If Len(s) < l Then
                Return False
            End If
            Return True
        End Function

        Shared Function max_len(ByVal s As String, ByVal l As Integer) As Boolean
            If Len(s) > l Then
                Return False
            End If
            Return True
        End Function

        Shared Function is_natural(ByVal s As String) As Boolean
            Dim pattern = New Regex("^[0-9]*$")
            Return Not pattern.IsMatch(s)
        End Function

        Shared Function is_natural_no_zero(ByVal s As String) As Boolean
            Dim pattern = New Regex("^[1-9]*$")
            If CInt(s) > 0 Then Return False
            If pattern.IsMatch(s) Then
                Return False
            End If

            If s = "0" Then
                Return True
            End If

            Return True
        End Function

        Shared Function is_alphaNumeric(ByVal s As String) As Boolean
            Dim pattern = New Regex("^[0-9a-zA-Z*]+$")
            Return Not pattern.IsMatch(s)
        End Function

        Shared Function regex_match(ByVal s As String, ByVal regex As String) As Boolean
            Dim pattern = New Regex(regex)
            If pattern.IsMatch(s) Then
                Return False
            End If
            Return True
        End Function

        Shared Function valid_email(ByVal s As String) As Boolean
            Dim pattern = New Regex("/^([a-z0-9\+_\-]+)(\.[a-z0-9\+_\-]+)*@([a-z0-9\-]+\.)+[a-z]{2,6}$/ix")
            Return Not pattern.IsMatch(s)
        End Function

        Shared Function valid_date(ByVal s As String) As Boolean
            ' Dim pattern = New Regex("/^([a-z0-9\+_\-]+)(\.[a-z0-9\+_\-]+)*@([a-z0-9\-]+\.)+[a-z]{2,6}$/ix")
            ' Return Not pattern.IsMatch(s)
            Dim d As Date
            Try
                d = CDate(s)
                Return False
            Catch ex As Exception
                Return True
            End Try
        End Function

        Shared Function valid_time(ByVal s As String) As Boolean
            'Dim pattern = New Regex("/^([a-z0-9\+_\-]+)(\.[a-z0-9\+_\-]+)*@([a-z0-9\-]+\.)+[a-z]{2,6}$/ix")
            Dim d As Date
            Try
                d = CDate(s)
                Return False
            Catch ex As Exception
                Return True
            End Try
        End Function

        Shared Function valid_datetime(ByVal s As String) As Boolean
            Dim pattern = New Regex("/^([a-z0-9\+_\-]+)(\.[a-z0-9\+_\-]+)*@([a-z0-9\-]+\.)+[a-z]{2,6}$/ix")
            Return Not pattern.IsMatch(s)
        End Function

        Shared Function dateCompare(ByVal s As String, ByVal fieldCompare As String, ByVal rawData As List(Of ObjectValues), ByVal menuID As String, ByVal refID As String) As Boolean
            Dim ok As Boolean = False
            Dim tmp As String = rawData.Where(Function(c) c.menuID = menuID And c.refID = refID And c.columnName = fieldCompare).First.Value
            'For Each a As Object In rawData
            '    If a("name").ToString.Contains("obj_" & menuID & "_" & refID & "_" & fieldCompare) Then
            Try
                If CDate(s) > CDate(tmp) Then
                    ok = True
                    ' Exit For
                End If
            Catch ex As Exception
                ok = True
            End Try
            '    End If
            'Next

            Return ok
        End Function

        Shared Function textCompare(ByVal s As String, ByVal fieldCompare As String, ByVal rawData As List(Of ObjectValues), ByVal menuID As String, ByVal refID As String) As Boolean
            Dim ok As Boolean = False
            Dim tmp As String = rawData.Where(Function(c) c.menuID = menuID And c.refID = refID And c.columnName = fieldCompare).First.Value
            'For Each a As Object In rawData
            'If a("name").ToString.Contains("obj_" & menuID & "_" & refID & "_" & fieldCompare) Then
            Try
                If s = tmp Then
                    ok = True
                    'Exit For
                End If
            Catch ex As Exception
                ok = True
            End Try
            'End If
            'Next

            Return ok
        End Function

        Shared Function timeCompare(ByVal s As String, ByVal fieldCompare As String, ByVal rawData As List(Of ObjectValues), ByVal menuID As String, ByVal refID As String) As Boolean
            Dim ok As Boolean = False
            Dim tmp As String = rawData.Where(Function(c) c.menuID = menuID And c.refID = refID And c.columnName = fieldCompare).First.Value
            ' For Each a As Object In rawData
            'If a("name").ToString.Contains("obj_" & menuID & "_" & refID & "_" & fieldCompare) Then
            Try
                If CDate(s) > CDate(tmp) Then
                    ok = True
                    'Exit For
                End If
            Catch ex As Exception
                ok = True
            End Try
            ' End If
            ' Next

            Return ok
        End Function

        Shared Function is_numeric(ByVal s As String) As Boolean
            If IsNumeric(s) Then
                Return False
            Else
                Return True
            End If
        End Function

    End Class

End Namespace