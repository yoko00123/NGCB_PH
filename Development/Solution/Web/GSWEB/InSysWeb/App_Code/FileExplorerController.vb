Imports System.Net
Imports System.Web.Http
Imports System.Net.Http
Imports GSWEB
Imports GSWEB.Common
Imports System.IO
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports GSWEB.Utility
Imports System.Threading.Tasks
Imports System.Net.Http.Headers
Imports System.IO.Compression

Public Class FileExplorerController
    Inherits BaseMenuController

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function GetFiles(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) <> 1 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim fileItems As New List(Of FileItem)
        Dim currentPath As String = If(String.IsNullOrEmpty(obj("currentPath").ToObject(Of String)()), "", "/" + obj("currentPath").ToObject(Of String)())
        Dim dir As DirectoryInfo = New System.IO.DirectoryInfo(HttpContext.Current.Server.MapPath("~" + currentPath))

        Dim dirInfos As DirectoryInfo() = dir.GetDirectories()
        For Each folder As DirectoryInfo In dirInfos
            fileItems.Add(New FileItem(folder.Name, "dir", folder.LastWriteTime, 0))
        Next



        Dim files() As String = Directory.GetFiles(HttpContext.Current.Server.MapPath("~" + currentPath))
        For Each file In files
            Dim fi As FileInfo = Nothing
            Try
                fi = New FileInfo(file)
                fileItems.Add(New FileItem(fi.Name, "file", fi.LastWriteTime, fi.Length))
            Catch ex As Exception

            End Try
        Next

        Dim ret As String, r As New JObject

        Try
            ret = "{"
            ret &= "files : " & JsonConvert.SerializeObject(fileItems) & ","
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            logError(ex)
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
        End Try


        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Async Function UploadFile() As Task(Of HttpResponseMessage) ' ByVal obj As Newtonsoft.Json.Linq.JObject
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        If Not Request.Content.IsMimeMultipartContent() Then
            Me.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType)
        End If
        Dim ret As String, r As New JObject
        Dim root = HttpContext.Current.Server.MapPath("~/Upload/")
        Directory.CreateDirectory(root)
        Dim provider = New MultipartFormDataStreamProvider(root),
            result = Await Request.Content.ReadAsMultipartAsync(provider),
            currentPath = If(result.FormData.Item("currentPath") = "", "", "/" + result.FormData.Item("currentPath") + "/")

        Dim mfData As MultipartFileData = result.FileData.Item(0),
            uploadedFileInfo = New FileInfo(mfData.LocalFileName),
            fileName As String = String.Empty

        fileName = mfData.Headers.ContentDisposition.FileName
        If fileName.StartsWith("""") And fileName.EndsWith("""") Then
            fileName = fileName.Trim("""")
        End If
        If fileName.Contains("/") Or fileName.Contains("\") Then
            fileName = Path.GetFileName(fileName)
        End If

        Try

            If File.Exists(Path.Combine(HttpContext.Current.Server.MapPath("~" + currentPath), fileName)) Then
                File.Replace(mfData.LocalFileName, Path.Combine(HttpContext.Current.Server.MapPath("~" + currentPath), fileName), Nothing)
            Else
                File.Move(mfData.LocalFileName, Path.Combine(HttpContext.Current.Server.MapPath("~" + currentPath), fileName))
            End If


            ret = "{"
            ret &= "message : 'Done'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            logError(ex)
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
        End Try


        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function GetFileContents(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) <> 1 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim fileItems As New List(Of FileItem)
        Dim currentPath As String = obj("path").ToObject(Of String)()

        Dim ret As String, contents As String = String.Empty, r As New JObject
        Dim lines() As String = File.ReadAllLines(HttpContext.Current.Server.MapPath("~" + currentPath))
        For Each line As String In lines
            contents += line.Replace("\", "\\") + "\n"
        Next
        Try
            ret = "{"
            ret &= "contents : '" & addStripSlashes(contents) & "',"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            logError(ex)
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
        End Try


        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function SaveContents(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) <> 1 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim fileItems As New List(Of FileItem)
        Dim currentPath As String = obj("path").ToObject(Of String)(),
            contents As String = obj("contents").ToObject(Of String)()
        Dim ret As String, r As New JObject

        Try
            System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath("~" + currentPath), contents)
            ret = "{"
            ret &= "ok : true,"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            logError(ex)
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
        End Try


        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpGet>
    Public Function DownloadFile(ByVal path As String, ByVal fileName As String) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) <> 1 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim fileItems As New List(Of FileItem)
        Dim result As New HttpResponseMessage
        Dim localFilePath = HttpContext.Current.Server.MapPath("~/" + path)

        If String.IsNullOrEmpty(path) Then
            result = Request.CreateResponse(HttpStatusCode.BadRequest)
        ElseIf Not File.Exists(localFilePath) Then
            result = Request.CreateResponse(HttpStatusCode.Gone)
        Else
            result = Request.CreateResponse(HttpStatusCode.OK)
            result.Content = New StreamContent(New FileStream(localFilePath, FileMode.Open, FileAccess.Read))
            result.Content.Headers.ContentType = New MediaTypeHeaderValue("application/octet-stream")
            result.Content.Headers.ContentDisposition = New System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            result.Content.Headers.ContentDisposition.FileName = fileName
        End If

        Return result
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function CreateBackUp(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) <> 1 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim ret As String = String.Empty, r As New JObject

        Try
            Dim folders() As String = {"Bin", "Build", "CustomPartials", "Dialogs", "ModulePage", "Scripts", "Styles", "WebHandler"}
            For Each folder In folders
                CopyDirectory(New DirectoryInfo(HttpContext.Current.Server.MapPath("~/" + folder)), New DirectoryInfo(HttpContext.Current.Server.MapPath("~/Backup/tmp/" + folder)))
            Next

            Dim files() As String = {"ErrorLog.txt", "Index.aspx", "Login.aspx", "Logout.aspx", "Register.aspx", "Web.config"}
            For Each f In files
                File.Copy(HttpContext.Current.Server.MapPath("~/" + f), HttpContext.Current.Server.MapPath("~/Backup/tmp/" + f))
            Next
            Dim d As DateTime = DateTime.Now
            ZipFile.CreateFromDirectory(HttpContext.Current.Server.MapPath("~/Backup/tmp"), HttpContext.Current.Server.MapPath("~/Backup/web-" + d.Year.ToString + d.Month.ToString + d.Day.ToString + "-" + d.Hour.ToString + d.Minute.ToString + d.Second.ToString + ".zip"), CompressionLevel.Optimal, False)

            

            r = JObject.Parse(ret)
        Catch ex As Exception
            logError(ex)
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
        Finally
            Dim tmp As DirectoryInfo = New DirectoryInfo(HttpContext.Current.Server.MapPath("~/Backup/tmp"))

            For Each file As FileInfo In tmp.GetFiles()
                file.Delete()
            Next

            For Each dir As DirectoryInfo In tmp.GetDirectories()
                dir.Delete(True)
            Next
        End Try


        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    Private Sub CopyDirectory(source As DirectoryInfo, target As DirectoryInfo)
        Directory.CreateDirectory(target.FullName)
        'Copy each file into the new directory
        For Each fi As FileInfo In source.GetFiles()
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), True)
        Next

        ' Copy each subdirectory
        For Each diSourceSubDir As DirectoryInfo In source.GetDirectories()
            Dim nextTargetSubDir As DirectoryInfo = target.CreateSubdirectory(diSourceSubDir.Name)
            CopyDirectory(diSourceSubDir, nextTargetSubDir)
        Next

    End Sub




End Class
