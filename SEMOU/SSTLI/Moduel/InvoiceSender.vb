Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks

Public Class InvoiceSender
    Private client As HttpClient

    ' Constructor
    Public Sub New()
        client = New HttpClient()
    End Sub

    ' دالة لإرسال الفاتورة
    Private Async Function SendInvoiceAsync(jsonContent As String) As Task(Of String)
        Dim client As New HttpClient()
        Dim request As New HttpRequestMessage(HttpMethod.Post, "http://localhost:2020/SendInvoice")

        request.Headers.Add("accept", "*/*")
        request.Headers.Add("Authorization", OtpSender.Token__)

        request.Content = New StringContent(jsonContent)
        request.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")

        Dim response As HttpResponseMessage = Await client.SendAsync(request)
        response.EnsureSuccessStatusCode()
        Return Await response.Content.ReadAsStringAsync()
    End Function
End Class
