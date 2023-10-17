Partial Class DSRenewal
    Partial Public Class ActivityTableDataTable
        Private Sub ActivityTableDataTable_ColumnChanging(sender As Object, e As DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.L1PerFiveYearColumn.ColumnName) Then
                'Add user code here
            End If

        End Sub

    End Class

    Partial Public Class UserActionTableDataTable


    End Class
End Class
