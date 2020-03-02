Imports GSWEB.Common

Namespace MenuCollection

    Public Class MenuClass
        Implements ICollection(Of Menu)

#Region "Declarations"

        Private innerMenu As List(Of Menu)
        Private isRO As Boolean = False

#End Region

#Region "Properties"

        Public ReadOnly Property Count() As Integer _
        Implements ICollection(Of Menu).Count
            Get
                Return innerMenu.Count
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean _
            Implements ICollection(Of Menu).IsReadOnly

            Get
                Return isRO
            End Get
        End Property

        Default Public Property Item(ByVal index As Integer) As Menu
            Get
                Return CType(innerMenu(index), Menu)
            End Get
            Set(ByVal Value As Menu)
                innerMenu(index) = Value
            End Set
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
            innerMenu = New List(Of Menu)
        End Sub

#End Region

#Region "Sub"

        Public Overloads Function GetMenu(ByVal mID As Integer) As Menu
            Dim query = From mMenu In innerMenu _
                   Where mMenu.MenuID = mID
            If query Is Nothing OrElse query.Count = 0 Then
                Return Nothing
            Else
                Return query.First
            End If
        End Function

        Public Sub Add(ByVal item As Menu) _
            Implements ICollection(Of Menu).Add

            If Not Me.Contains(item) Then
                innerMenu.Add(item)
            Else
                Console.WriteLine("A menu with ID {0} was already added to the collection.",
                    item.MenuID.ToString())
            End If
        End Sub

        Public Sub CopyTo(ByVal array() As Menu, ByVal arrayIndex As Integer) _
            Implements ICollection(Of Menu).CopyTo
            Throw New NotImplementedException()
        End Sub

        Public Sub Clear() Implements ICollection(Of Menu).Clear
            innerMenu.Clear()
        End Sub

#End Region

#Region "Functions"

        Public Function GetChild(ByVal pID As Integer) As System.Collections.Generic.IEnumerable(Of Menu)
            Return From mMenu In innerMenu _
                   Where mMenu.ParentID.Equals(pID) _
                   And CBool(mMenu.ColumnValue("IsVisible")) = True
                   Order By IsNull(mMenu.ColumnValue("SeqNo"), 0)
        End Function

        Public Function GetChildMenuBar(ByVal pID As Integer) As System.Collections.Generic.IEnumerable(Of Menu)
            Return From mMenu In innerMenu _
                   Where mMenu.ParentID.Equals(pID) _
                   And mMenu.ColumnValue("ID_WebMenuTypes").ToString = "1" And CBool(mMenu.ColumnValue("IsVisible")) = True
                   Order By mMenu.ColumnValue("SeqNo")
        End Function

        Public Function GetDefaultChild(ByVal pID As Integer) As Menu
            Dim DefaultChild As Menu = Nothing
            For Each m As Menu In From mMenu In innerMenu _
                   Where mMenu.ParentID.Equals(pID) And CBool(mMenu.ColumnValue("IsDefault")) = True And CBool(mMenu.ColumnValue("IsVisible")) = True
                   Order By mMenu.ColumnValue("SeqNo")
                DefaultChild = m
                Exit For
            Next
            Return DefaultChild
        End Function

        Public Function GetMenuFromWhere(ByVal ColumnName As String, ByVal ColumnVaue As Object) As System.Collections.Generic.IEnumerable(Of Menu)
            Return From mMenu In innerMenu _
                   Where mMenu.ColumnValue(ColumnName) = ColumnVaue
                   Order By mMenu.ColumnValue("SeqNo")
        End Function

        Public Function GetRootMenu() As System.Collections.Generic.IEnumerable(Of Menu)
            Return From mMenu In innerMenu _
                   Where mMenu.ParentID.Equals(0) And CInt(mMenu.ColumnValue("ID_WebMenuTypes")) = 1
                   Order By mMenu.ColumnValue("SeqNo")
            'And CBool(mMenu.ColumnValue("IsVisible")) = True
        End Function

        Public Function Remove(ByVal item As Menu) As Boolean _
                Implements ICollection(Of Menu).Remove
            Dim result As Boolean = False

            ' Iterate the inner collection to  
            ' find the box to be removed. 

            Dim i As Integer
            For i = 0 To innerMenu.Count - 1

                Dim curBox As Menu = CType(innerMenu(i), Menu)

                If curBox.MenuID = item.MenuID Then
                    innerMenu.RemoveAt(i)
                    result = True
                    Exit For
                End If
            Next
            Return result
        End Function

        Public Function Contains(ByVal item As Menu) As Boolean _
                Implements ICollection(Of Menu).Contains
            Dim query = From mMenu In innerMenu _
                   Where mMenu.MenuID = item.MenuID
            If query.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetEnumerator() As IEnumerator(Of Menu) _
            Implements IEnumerable(Of Menu).GetEnumerator

            Return New MenuEnumerator(Me)
        End Function

        Private Function GetEnumerator1() As IEnumerator _
            Implements IEnumerable.GetEnumerator

            Return Me.GetEnumerator()
        End Function

#End Region

    End Class

    Public Class MenuEnumerator
        Implements IEnumerator(Of Menu)

#Region "Declarations"

        Private _collection As MenuClass
        Private curIndex As Integer
        Private curMenu As Menu

#End Region

#Region "Properties"

        Public ReadOnly Property Current() As Menu _
    Implements IEnumerator(Of Menu).Current
            Get
                If curMenu Is Nothing Then
                    Throw New InvalidOperationException()
                End If

                Return curMenu
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

        Public Sub New(ByVal collection As MenuClass)
            MyBase.New()
            _collection = collection
            curIndex = -1
            curMenu = Nothing
        End Sub

#End Region

#Region "Functions"

        Private Property mMenu As Menu
        Public Function MoveNext() As Boolean _
            Implements IEnumerator(Of Menu).MoveNext
            curIndex = curIndex + 1
            If curIndex = _collection.Count Then
                Return False
            Else
                curMenu = _collection(curIndex)
            End If
            Return True
        End Function

        Public Sub Reset() _
            Implements IEnumerator(Of Menu).Reset
            curIndex = -1
        End Sub

        Public Sub Dispose() _
            Implements IEnumerator(Of Menu).Dispose
        End Sub

#End Region

    End Class

End Namespace