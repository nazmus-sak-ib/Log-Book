Public Class RenewalActivity
    ' A certain type of activity performed on different days
    Public Property ActivityID As String                              ' A1/B1/B2

    Public ReadOnly Property ActivityType As String                   ' A/B/C
        Get
            If Not String.IsNullOrEmpty(ActivityID) Then
                Return ActivityID(0)
            Else
                Return ""
            End If
        End Get
    End Property

    Public Property AcitvityName As String                            ' Performance of NDT Activity 
    Public Property PPD As New List(Of String)                       ' Points per day (3 levels, 3 list items)
    Public Property MaxPointPerYear As New List(Of Integer)           ' Maximum points possible for an activity PER YEAR (3 levels, 3 list items)
    Public Property MaxPointFiveYears As New List(Of Integer)         ' Maximum points possible for an activity PER 5 YEARS (3 levels, 3 list items)
End Class
