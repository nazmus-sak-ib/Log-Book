Public Class UserAction
    ' When a user performs a renewal/initial activity (RenewalActivity class) on any day (ActivityDay class)
    ' Public Property ActivityParent As String    ' Renewal or Initial
    Public Property Level As Int16              ' 1 || 2 || 3
    Public Property ActionID As String          ' Unique action ID, for database primary key
    Public Property ActivityID As String        ' A1/B2/C1
    Public Property ActivityDate As Date        ' Date

End Class
