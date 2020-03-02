Public Class Grid
    Private _data As String = String.Empty
    Private _enableColumnResizing As Boolean = False
    Private _enableGridMenu As Boolean = False
    Private _enableRowHeaderSelection As Boolean = False
    Private _enableRowSelection As Boolean = False
    Private _enableSelectAll As Boolean = False
    Private _multiSelect As Boolean = False

    Private _columnDefs As New List(Of GSWEB.GridColumnDefinition)
#Region "Properties"

    Public Property data() As String
        Get
            Return _data
        End Get
        Set(value As String)
            _data = value
        End Set
    End Property

    Public ReadOnly Property columnDefs() As List(Of GSWEB.GridColumnDefinition)
        Get
            Return _columnDefs
        End Get
    End Property

    Public Property enableColumnResizing() As Boolean
        Get
            Return _enableColumnResizing
        End Get
        Set(value As Boolean)
            _enableColumnResizing = value
        End Set
    End Property

    Public Property enableGridMenu() As Boolean
        Get
            Return _enableGridMenu
        End Get
        Set(value As Boolean)
            _enableGridMenu = value
        End Set
    End Property

    Public Property enableRowHeaderSelection() As Boolean
        Get
            Return _enableRowHeaderSelection
        End Get
        Set(value As Boolean)
            _enableRowHeaderSelection = value
        End Set
    End Property

    Public Property enableSelectAll() As Boolean
        Get
            Return _enableSelectAll
        End Get
        Set(value As Boolean)
            _enableSelectAll = value
        End Set
    End Property

    Public Property multiSelect() As Boolean
        Get
            Return _multiSelect
        End Get
        Set(value As Boolean)
            _multiSelect = value
        End Set
    End Property
#End Region

#Region "Subs"

    Public Sub AddColumns(column As GSWEB.GridColumnDefinition)
        columnDefs.Add(column)
    End Sub

    Public Sub ClearColumns()
        columnDefs.Clear()
    End Sub

    Public Sub Dispose()
        _data = Nothing
        _enableColumnResizing = Nothing
        _enableGridMenu = Nothing
        _enableRowHeaderSelection = Nothing
        _enableRowSelection = Nothing
        _enableSelectAll = Nothing
        _multiSelect = Nothing

        ClearColumns()
        GC.Collect()
        GC.SuppressFinalize(Me)
    End Sub

#End Region
End Class
