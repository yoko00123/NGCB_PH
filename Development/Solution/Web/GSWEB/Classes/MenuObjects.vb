Imports GSWEB.Common
Imports System.Data.DataTableExtensions

Namespace MenuCollection

    Public Class MenuObjects
        Implements ICollection(Of DataRow)

#Region "Declarations"

        Private innerObject As List(Of DataRow)
        Private isRO As Boolean = False

#End Region

#Region "Properties"

        Public ReadOnly Property Count() As Integer _
        Implements ICollection(Of DataRow).Count
            Get
                Return innerObject.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean _
            Implements ICollection(Of DataRow).IsReadOnly

            Get
                Return isRO
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As DataRow
            Get
                Return CType(innerObject(index), DataRow)
            End Get
            Set(ByVal Value As DataRow)
                innerObject(index) = Value
            End Set
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
            innerObject = New List(Of DataRow)
        End Sub

#End Region

#Region "Sub"

        Public Function GetObject(ByVal ID As Integer) As System.Collections.Generic.IEnumerable(Of DataRow)
            Return From objects In Me _
                   Where objects.Item("ID") = ID
        End Function

        Public Function GetObjectByName(ByVal Name As String) As System.Collections.Generic.IEnumerable(Of DataRow)
            Return From objects In Me _
                   Where objects.Item("Name") = Name
        End Function

        Public Sub Add(ByVal item As DataRow) _
            Implements ICollection(Of DataRow).Add

            If Not Me.Contains(item) Then
                innerObject.Add(item)
            Else
                Console.WriteLine("A Object with ID {0} was already added to the collection.",
                    item.Item("ID").ToString())
            End If
        End Sub

        Public Sub CopyTo(ByVal array() As DataRow, ByVal arrayIndex As Integer) _
            Implements ICollection(Of DataRow).CopyTo
            Throw New NotImplementedException()
        End Sub

        Public Sub Clear() Implements ICollection(Of DataRow).Clear
            innerObject.Clear()
        End Sub

#End Region

#Region "Functions"

        Public Function Remove(ByVal item As DataRow) As Boolean _
                Implements ICollection(Of DataRow).Remove
            Dim result As Boolean = False

            Dim i As Integer
            For i = 0 To innerObject.Count - 1

                Dim curBox As DataRow = CType(innerObject(i), DataRow)

                If curBox.Item("ID") = item.Item("ID").ToString Then
                    innerObject.RemoveAt(i)
                    result = True
                    Exit For
                End If
            Next
            Return result
        End Function

        Public Function Contains(ByVal item As DataRow) As Boolean _
                Implements ICollection(Of DataRow).Contains
            Dim query = From mObject In Me _
                   Where mObject.Item("ID") = item.Item("ID")
            If query.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetEnumerator() As IEnumerator(Of DataRow) _
            Implements IEnumerable(Of DataRow).GetEnumerator

            Return New ObjectEnumerator(Me)
        End Function

        Private Function GetEnumerator1() As IEnumerator _
            Implements IEnumerable.GetEnumerator

            Return Me.GetEnumerator()
        End Function

#End Region

    End Class

    Public Class ObjectEnumerator
        Implements IEnumerator(Of DataRow)

#Region "Declarations"

        Private _collection As MenuObjects
        Private curIndex As Integer
        Private curObject As DataRow

#End Region

#Region "Properties"

        Public ReadOnly Property Current() As DataRow _
    Implements IEnumerator(Of DataRow).Current
            Get
                If curObject Is Nothing Then
                    Throw New InvalidOperationException()
                End If

                Return curObject
            End Get
        End Property

        Private ReadOnly Property Current1() As Object _
            Implements IEnumerator.Current
            Get
                Return Me.Current
            End Get
        End Property

#End Region

#Region "Constructors"

        Public Sub New(ByVal collection As MenuObjects)
            MyBase.New()
            _collection = collection
            curIndex = -1
            curObject = Nothing
        End Sub

#End Region

#Region "Functions"

        Private Property mObject As DataRow
        Public Function MoveNext() As Boolean _
            Implements IEnumerator(Of DataRow).MoveNext
            curIndex = curIndex + 1
            If curIndex = _collection.Count Then
                Return False
            Else
                curObject = _collection(curIndex)
            End If
            Return True
        End Function

        Public Sub Reset() _
            Implements IEnumerator(Of DataRow).Reset
            curIndex = -1
        End Sub

        Public Sub Dispose() _
            Implements IEnumerator(Of DataRow).Dispose
        End Sub

#End Region

    End Class

End Namespace