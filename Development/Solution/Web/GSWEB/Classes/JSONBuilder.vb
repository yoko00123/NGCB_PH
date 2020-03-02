Imports Newtonsoft.Json

Public Class JSONBuilder
    Implements IDisposable

    Private dict As New Dictionary(Of String, Object)

    Public Sub Add(name As String, value As Object)
        dict.Add(name, value)
    End Sub

    Public Function GetString() As String
        Return JsonConvert.SerializeObject(dict, Formatting.None)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        dict = Nothing
        GC.Collect()
        GC.SuppressFinalize(Me)
    End Sub

    Public Shared Function TrimString(data As String) As String
        Return data.Replace(vbTab, " ").Replace(vbCrLf, " ")
    End Function

End Class
