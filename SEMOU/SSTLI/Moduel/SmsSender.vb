Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json

Public Class SmsSender
    Public Shared user_name As String
    Public Shared numbers_ As String
    Public Shared user_sender As String
    Public Shared api_key As String
    Public Shared msg_ As String
    Public Shared msg_encoding As String
    Public Shared Async Function SENDSMSASYNC(USER As String, NUMBER As String, UserSender As String, ApiKey As String, Msg As String, MsgEncoding As String) As Task
        Dim res As String = ""
        Dim sensmsapi = New SmsSender
        user_name = USER
        numbers_ = NUMBER
        user_sender = UserSender
        api_key = ApiKey
        msg_ = Msg
        msg_encoding = MsgEncoding
        Dim json As String = JsonConvert.SerializeObject(sensmsapi)
        Dim baseaddress = New Uri("https://www.msegat.com")
        Dim HttpClient = New HttpClient With {.BaseAddress = baseaddress}
        Using content As New StringContent(json, Encoding.Default, "application/json")
            Using response As HttpResponseMessage = Await HttpClient.PostAsync("/gw/sendsms.php", content)
                Dim responseHeaders As String = response.Headers.ToString()
                Dim responseData As String = Await response.Content.ReadAsStringAsync()
                Console.WriteLine("Status " & CInt(response.StatusCode))
                Console.WriteLine("Headers " + responseHeaders)
                Console.WriteLine("Data " + responseData)
                res = responseData.ToString()
            End Using
        End Using
    End Function
End Class
