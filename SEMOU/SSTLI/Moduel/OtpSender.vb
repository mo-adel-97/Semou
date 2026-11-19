Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks

Public Class OtpSender
    Private client As HttpClient
    Public Shared Token__ As String

    ' Constructor
    Public Sub New()
        client = New HttpClient()
        client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", Token__)
    End Sub

    ' دالة لإرسال OTP
    Public Async Function SendOtpAsync(otp As String) As Task(Of String)
        Dim request As New HttpRequestMessage(HttpMethod.Post, "http://localhost:2020/Otp")
        request.Headers.Add("accept", "*/*")

        ' إنشاء محتوى JSON بناءً على المدخلات
        Dim jsonContent As String = $"{{""otp"": ""{otp}""}}"
        request.Content = New StringContent(jsonContent, Encoding.UTF8, "application/json")

        Try
            Dim response As HttpResponseMessage = Await client.SendAsync(request)
            response.EnsureSuccessStatusCode()
            Dim responseBody As String = Await response.Content.ReadAsStringAsync()
            Return responseBody
        Catch ex As Exception
            Return "Error: " & ex.Message
        End Try
    End Function
End Class
