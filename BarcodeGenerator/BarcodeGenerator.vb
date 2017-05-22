Public Class BarcodeGenerate
	Dim strlength As Integer
	Private Function even(ByVal decode As String) As String
		Dim encode As String
		encode = Chr(205)
		Dim str1, str2 As String
		Dim ans, ans1, ans2, tem As Integer
		str1 = Mid(decode, 1, 2)

		ans1 = check_ascii(CType(str1, Integer))
		encode &= Chr(ans1)
		ans = CType(str1, Integer) + 105
		Dim i As Integer
		Dim cnt As Integer = 2
		For i = 3 To decode.Length - 1 Step 2
			str2 = Mid(decode, i, 2)
			ans2 = check_ascii(CType(str2, Integer))
			encode &= Chr(ans2)
			tem = (CType(str2, Integer)) * cnt
			cnt = cnt + 1
			ans += tem
		Next
		ans = ans Mod 103
		ans = check_ascii(ans)
		encode &= Chr(ans)
		encode &= Chr(206)
		Return encode
	End Function
	Public Function str(ByVal decode As String) As String
		Dim encode As String
		encode = Chr(204)
		Dim ans, re As Integer
		Dim ascii As Integer
		Dim value, i As Integer
		Dim ch() As Char
		Dim ans1 As Integer
		ans = 0
		ans1 = 0
		ch = decode.ToCharArray()
		value = check_value(Asc(ch(0))) * 1
		ans += value + 104
		For i = 1 To ch.Length - 1
			value = check_value(Asc(ch(i))) * (i + 1)
			ans += value
		Next
		ans = ans Mod 103
		ans1 = check_ascii(ans)
		encode &= decode & Chr(ans1) & Chr(206)
		Return encode
		'    REM: for example 78
		'    REM: get 7 
		'    REM: call check_value for 7=23
		'    REM:now 23 * 1= 23
		'    REM: 23+104=127
		'    REM: now get 8
		'    REM: call chek_value for 8=24
		'    REM: now 24* 2=48
		'    REM: 127+48=175
		'    REM: 175 mod 103=72
		'    REM: call check_ascii for getting caracter of 72
		'    REM: append char of 72 with i78hi
	End Function
	Private Function check_ascii(ByVal val As Integer) As Integer
		Dim ans As Integer
		If val = 0 Then
			ans = 194
		ElseIf val >= 95 And val <= 105 Then
			ans = val + 100
		ElseIf val >= 1 And val <= 94 Then
			ans = val + 32
		End If
		Return ans
	End Function
	Private Function check_value(ByVal ascii As Integer) As Integer
		Dim ans As Integer
		If ascii = 194 Then
			ans = 0
		ElseIf ascii >= 195 And ascii <= 205 Then
			ans = ascii - 100
		ElseIf ascii >= 1 And ascii <= 126 Then
			ans = ascii - 32
		End If
		Return ans
	End Function



	Private Function odd(ByVal decode As String) As String
		Dim encode As String
		encode = Chr(205)
		Dim str1, str2 As String
		Dim ans, ans1, ans2, tem As Integer
		str1 = Mid(decode, 1, 2)

		ans1 = check_ascii(CType(str1, Integer))
		encode &= Chr(ans1)
		ans = CType(str1, Integer) + 105
		Dim i As Integer
		Dim cnt As Integer = 2
		For i = 3 To decode.Length - 1 Step 2
			str2 = Mid(decode, i, 2)
			ans2 = check_ascii(CType(str2, Integer))
			encode &= Chr(ans2)
			tem = (CType(str2, Integer)) * cnt
			cnt = cnt + 1
			ans += tem
		Next
		encode &= Chr(200)
		ans = ans + (cnt * 100)
		str2 = Mid(decode, i)
		encode &= str2
		cnt = cnt + 1
		tem = check_value(Asc(str2)) * cnt
		ans = ans + tem
		ans = ans Mod 103
		ans = check_ascii(ans)
		encode &= Chr(ans)
		encode &= Chr(206)
		Return encode
	End Function


	Public Function Convert(value As String) As String
		Dim retrunvalue As String
		If value <> "" Then

			Dim DECODE As String
			strlength = Len(value)
			If strlength = 1 Then
				DECODE = "000" & value
			ElseIf strlength = 2 Then
				DECODE = "00" & value
			ElseIf strlength = 3 Then
				DECODE = "0" & value
			Else
				DECODE = value
			End If
			strlength = Len(DECODE)
			If strlength Mod 2 = 0 Then
				retrunvalue = even(DECODE)
			Else
				retrunvalue = odd(DECODE)
			End If
			Return retrunvalue
		Else
			Return ""
		End If

	End Function

	Public Function Decodestring(value As String) As String

		If value <> "" Then

			Dim DECODE As String = ""
			strlength = Len(value)
			For i = 2 To strlength - 2
				DECODE = DECODE + check_value(Asc(Mid(value, i, 1))).ToString()
			Next
			Return DECODE
		Else
			Return ""
		End If

	End Function


End Class
