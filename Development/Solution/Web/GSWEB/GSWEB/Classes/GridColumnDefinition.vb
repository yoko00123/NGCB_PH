Imports GSWEB.Common

Public Class GridColumnDefinition

#Region "Declaration"

    Private mRow As DataRow
    Private _enableColumnMenu As Boolean = False
    Private _enableHiding As Boolean = False
    Private _enablePinning As Boolean = False
    Private _field As String = String.Empty

#End Region

#Region "Constructors"

    Public Sub New(ByVal mData As DataRow)
        mRow = mData
        'UnComment PAG Access Rights Gustong nsa memory
        'If CInt(Me.ColumnValue("ID_WebMenuTypes")) = 1 Then
        '    LoadAccessRights()
        'End If
    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property displayName() As String
        Get
            Return IsNull(mRow("Label"), mRow("Name")).ToString
        End Get
    End Property

    Public Property field() As String
        Get
            Return _field
        End Get
        Set(value As String)
            _field = value
        End Set
    End Property

    Public ReadOnly Property width() As String
        Get
            Return IsNull(mRow("Width"), "*")
        End Get
    End Property

    Public Property enableColumnMenu() As Boolean
        Get
            Return _enableColumnMenu
        End Get
        Set(value As Boolean)
            _enableColumnMenu = value
        End Set
    End Property

    Public Property enablePinning() As Boolean
        Get
            Return _enablePinning
        End Get
        Set(value As Boolean)
            _enablePinning = value
        End Set
    End Property

    Public ReadOnly Property pinnedLeft() As Boolean
        Get
            Return IsNull(mRow("IsFrozen"), False)
        End Get
    End Property


#End Region
End Class
