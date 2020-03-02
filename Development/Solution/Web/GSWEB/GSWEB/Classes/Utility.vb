Imports System.Runtime.Caching
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web

Namespace Utility

    Public Class CaseInsensitiveDictionary
        Inherits Dictionary(Of String, Object)
        Public Sub New()
            MyBase.New(StringComparer.OrdinalIgnoreCase)
        End Sub
    End Class

    Public Class fileSummary
        Public mID As Integer
        Public name As String
        Public idx As Integer
    End Class

    Public Class MemoryCacher
        Public Function GetValue(key As String) As Object
            Dim memoryCache As MemoryCache = memoryCache.Default
            Return memoryCache.Get(key)
        End Function

        Public Function Contains(key As String) As Boolean
            Return MemoryCache.Default.Contains(key)
        End Function

        Public Sub Add(key As String, value As Object, absExpiration As DateTimeOffset)
            Dim memoryCache As MemoryCache = memoryCache.Default
            memoryCache.Add(key, value, absExpiration)
        End Sub

        Public Sub Delete(key As String)
            Dim memoryCache As MemoryCache = memoryCache.Default
            If memoryCache.Contains(key) Then
                memoryCache.Remove(key)
            End If
        End Sub

        Public Sub Clear()
            Dim memoryCache As MemoryCache = memoryCache.Default
            Dim keys As List(Of String) = memoryCache.Default.Select(Function(x) x.Key).ToList()
            For Each key As String In keys
                memoryCache.Remove(key)
            Next
        End Sub
    End Class

    Public Class OrgChart
        Public Property Position As String
        Public Property Name As String
        Public Property Supervises As New List(Of OrgChart)

        Sub New(Position As String, Name As String)
            Me.Position = Position
            Me.Name = Name
        End Sub

        Public Sub Add(a As OrgChart)
            Me.Supervises.Add(a)
        End Sub
    End Class

    Public Class CsrfTokenHelper
        ''TODO GAWIN DYNAMIC PER CLIENT??
        Const ConstantSalt As String = "X-0m$sP3*ca"

        Public Function GenerateCsrfTokenFromAuthToken(authToken As String) As String
            Return GenerateCookieFriendlyHash(authToken)
        End Function

        Public Function DoesCsrfTokenMatchAuthToken(csrfToken As String, authToken As String) As Boolean
            Return csrfToken = GenerateCookieFriendlyHash(authToken)
        End Function

        Private Function GenerateCookieFriendlyHash(authToken As String) As String
            Using sha = SHA256.Create()
                Dim computedHash = sha.ComputeHash(Encoding.Unicode.GetBytes(authToken + ConstantSalt))
                Dim cookieFriendlyHash = HttpServerUtility.UrlTokenEncode(computedHash)
                Return cookieFriendlyHash
            End Using

        End Function
    End Class

    Public Class UserGroupWebMenus
        Public Property ID As Integer
        Public Property Name As String
        Public Property Label As String
        Public Property ID_WebMenus As Integer
        Public Property ShowMenuIf As String
        Public Property SeqNo As Integer
        Sub New(ID As Integer, Name As String, Label As String, ID_WebMenus As Integer, ShowMenuIf As String, SeqNo As Integer)
            Me.ID = ID
            Me.Name = Name
            Me.Label = Label
            Me.ID_WebMenus = ID_WebMenus
            Me.ShowMenuIf = ShowMenuIf
            Me.SeqNo = SeqNo
        End Sub

    End Class

    Public Class Color

        Private Property Hex As String
        Private Property r As String
        Private Property g As String
        Private Property b As String
        Private Property h As Double
        Private Property s As Double
        Private Property l As Double

        Public Sub New(s As String)
            Me.Hex = s
            Me.toRGB()
            Me.toHSL()
        End Sub

        Public ReadOnly Property RGB() As String()
            Get
                Dim out(2) As String
                out(0) = Me.r
                out(1) = Me.g
                out(2) = Me.b
                Return out
            End Get
        End Property

        Public ReadOnly Property HSL() As String()
            Get
                Dim out(2) As String
                out(0) = Me.h
                out(1) = Me.s
                out(2) = Me.l
                Return out
            End Get
        End Property

        Private Sub toRGB()
            Dim s As String = Hex.Replace("#", "")
            Dim r As String = s.Substring(0, 2),
                g As String = s.Substring(2, 2),
                b As String = s.Substring(4, 2)
            Me.r = Convert.ToInt32(r, 16)
            Me.g = Convert.ToInt32(g, 16)
            Me.b = Convert.ToInt32(b, 16)
        End Sub

        Private Sub toHSL()
            Dim r As Double = CDbl(Me.r),
                g As Double = CDbl(Me.g),
                b As Double = CDbl(Me.b)
            r /= 255
            g /= 255
            b /= 255
            Dim max As Double = Math.Max(Math.Max(r, g), b),
                min As Double = Math.Min(Math.Min(r, g), b)
            Dim h As Double = 0, s As Double = 0, l As Double = (max + min) / 2

            If max <> min Then
                Dim d As Double = max - min
                s = If(l > 0.5, d / (2 - max - min), d / (max + min))
                Select Case max
                    Case r
                        h = (g - b) / d + If(g < b, 6, 0)
                    Case g
                        h = (b - r) / d + 2
                    Case b
                        h = (r - g) / d + 4
                End Select
                h /= 6
            End If
            Me.h = h * 360
            Me.s = s
            Me.l = l

        End Sub

        Public Function lighten(amt As Integer) As String

            Dim tmp As Double = Me.l

            tmp += amt / 100
            tmp = clamp(tmp)
            Dim out() As String = hsla(Me.h, Me.s, tmp)
            Return rgbTohex(out(0), out(1), out(2))
        End Function

        Public Function hsla(h, s, l) As String()
            h = CDbl(h Mod 360) / 360
            s = clamp(CDbl(s))
            l = clamp(CDbl(l))
            Dim m2 As Double = If(l <= 0.5, l * (s + 1), l + s - l * s)
            Dim m1 As Double = l * 2 - m2

            Return rgba(hue(h + 1 / 3, m1, m2) * 255, hue(h, m1, m2) * 255, hue(h - 1 / 3, m1, m2) * 255)
        End Function

        Private Function clamp(val As Double) As Double
            Return Math.Min(1, Math.Max(0, val))
        End Function

        Private Function hue(h As Double, m1 As Double, m2 As Double) As Double
            h = If(h < 0, h + 1, If(h > 1, h - 1, h))
            If (h * 6 < 1) Then
                Return m1 + (m2 - m1) * h * 6
            ElseIf (h * 2 < 1) Then
                Return m2
            ElseIf (h * 3 < 2) Then
                Return m1 + (m2 - m1) * (2 / 3 - h) * 6
            Else
                Return m1
            End If
        End Function


        Private Function rgba(r As Double, g As Double, b As Double) As String()
            Dim out(2) As String
            out(0) = Math.Floor(r)
            out(1) = Math.Floor(g)
            out(2) = Math.Floor(b)
            Return out
        End Function

        Private Function rgbTohex(r As Integer, g As Integer, b As Integer) As String
            Return "#" + CStr(Convert.ToString(r, 16)) + CStr(Convert.ToString(g, 16)) + CStr(Convert.ToString(b, 16))
        End Function
    End Class

    Public Class FileItem
        Public name As String
        Public type As String
        Public [date] As Date
        Public size As Integer

        Public Sub New(name As String, type As String, [date] As Date, size As Integer)
            Me.name = name
            Me.type = type
            Me.date = [date]
            Me.size = size
        End Sub
    End Class
End Namespace