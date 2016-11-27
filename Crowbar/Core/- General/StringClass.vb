Public Class StringClass

	Public Shared Function ConvertFromNullTerminatedString(ByVal input As String) As String
		Dim output As String
		Dim positionOfFirstNullChar As Integer
		positionOfFirstNullChar = input.IndexOf(Chr(0))
		output = input.Substring(0, positionOfFirstNullChar)
		Return output
	End Function

	Public Shared Function ConvertFromNullTerminatedOrFullLengthString(ByVal input As String) As String
		Dim output As String
		Dim positionOfFirstNullChar As Integer
		positionOfFirstNullChar = input.IndexOf(Chr(0))
		If positionOfFirstNullChar = -1 Then
			output = input
		Else
			output = input.Substring(0, positionOfFirstNullChar)
		End If
		Return output
	End Function

End Class
