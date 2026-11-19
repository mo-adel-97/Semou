Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class EncodingLevel
    Dim dbcontext As New SemouEntities
    Dim REASON As String = ""
    Private client As HttpClient
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Public Function GET___NO() As Integer
        Dim MaxId = dbcontext.Levels.OrderByDescending(Function(p) p.ID).FirstOrDefault()?.ID
        Dim MaxDay = New Level
        If (IsNothing(MaxId)) Then
            MaxId = 1
            TxtCode.Text = MaxId
        Else
            MaxId += 1
            MaxDay.ID = MaxId
            TxtCode.Text = MaxId
        End If
        Return MaxId
    End Function
    Sub ADDLEVEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddLevel", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Id", GET___NO)
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        comm.Parameters.AddWithValue("@LevelName", TxtLevelName.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub UPDATELEVEL()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("UpdateLevel", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("LevelName", TxtLevelName.Text)
        comm.Parameters.AddWithValue("@Notes", TxtNotes.Text)
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub ADD_USERACTION_()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("Add_UserAction", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@ActionGuid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____)
        comm.Parameters.AddWithValue("@Date_", Convert.ToDateTime(DateTime.Now))
        comm.Parameters.AddWithValue("@Time_", Convert.ToDateTime(DateTime.Now.ToShortTimeString))
        comm.Parameters.AddWithValue("@Notes_", AllFiles.ActionType___)
        comm.Parameters.AddWithValue("@ActionTypeGuid", TxtTypeGuid.Text)
        comm.Parameters.AddWithValue("@ActionReason", REASON)
        comm.Parameters.AddWithValue("@ActionCode", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Sub SHOWLEVELNFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand
        comm = New SqlCommand("ShowLevelInfo", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Code", TxtCode.Text)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowLevelInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtLevelName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtNotes.Text = ds.Tables(0).Rows(i)(1).ToString()
                ChkStaut.Value = ds.Tables(0).Rows(i)(2)
                TxtGuid.Text = ds.Tables(0).Rows(i)(3).ToString()
            Next
        End If
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtLevelName.Text = Nothing Then
            MessageBox.Show("برجاء ادخال اسم المستوي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If
        Return True
    End Function
    Sub FORSAVE()
        If AllFiles.FORSAVE("addlevel") = False Then
            Return
        Else
            AllFiles.ActionType___ = "جديد"
            ADDLEVEL()
            SHOWLEVELNFO()
            ADD_USERACTION_()
            MessageBox.Show("تم حفظ المستوي بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Sub FORUPDATE()
        If AllFiles.FORUPDATE("addlevel") = False Then
            Return
        Else
            AllFiles.ActionType___ = "تعديل"
            UPDATELEVEL()
            ADD_USERACTION_()
            MessageBox.Show("تم تعديل المستوي بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub EncodingLevel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            client = New HttpClient()
            RegDocPosted.WHATFORMTYPEGUID("addlevel")
            TxtTypeGuid.Text = RegDocPosted.TYPEGUID
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtGuid.Text = Nothing Then
                    FORSAVE()
                Else
                    Dim w As New UpdateResaon
                    w.ShowDialog()
                    If w.IS_Yes = True Then
                        REASON = w.TxtNotes.Text
                        FORUPDATE()
                    End If
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub NEW___()
        TxtLevelName.Text = Nothing
        TxtNotes.Text = Nothing
        TxtCode.Text = Nothing
        TxtGuid.Text = Nothing
        ChkStaut.Value = True
    End Sub

    Private Sub CMDNew_Click(sender As Object, e As EventArgs) Handles CMDNew.Click
        Try
            NEW___()
            EncodingLevel_Load(e, e)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            If AllFiles.FORFIND("addlevel") = False Then
                Return
            Else
                Dim w As New LevelList
                w.ShowDialog()
                If w.IS_Yes = True Then
                    Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                    TxtCode.Text = w.BranchGrid.Rows(ii).Cells(0).Value.ToString()
                    TxtGuid.Text = w.BranchGrid.Rows(ii).Cells(3).Value.ToString()
                End If
                If TxtGuid.Text <> Nothing Then
                    SHOWLEVELNFO()
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Private Async Function GetInvoiceDataAsync(invoiceID As String) As Task(Of String)
        Dim invoiceData As New Dictionary(Of String, Object)
        Dim dbcontext As New SemouEntities
        Using connection = dbcontext.Database.Connection
            If connection.State = ConnectionState.Closed Then
                connection.Open()
            End If
            Try
                Using comm = New SqlCommand("ShowBillInfo", CType(connection, SqlConnection))
                    comm.CommandType = CommandType.StoredProcedure
                    comm.Parameters.AddWithValue("@Code", TxtBillCode.Text)

                    Using reader As SqlDataReader = Await comm.ExecuteReaderAsync()
                        If reader.HasRows Then
                            While reader.Read()
                                invoiceData("id") = AllFiles.GetTaxInvoiceNo(reader("Guid").ToString()) ' رقم الفاتورة الضريبية الجديد
                                invoiceData("issueDate") = Convert.ToDateTime(reader("BillDate")).ToString("yyyy-MM-dd")
                                invoiceData("issueTime") = Convert.ToDateTime(reader("BillDate")).ToString("HH:mm:ss")
                                invoiceData("customer") = New Dictionary(Of String, Object) From {
                                              {"registrationName", reader("StudentName").ToString()},
                                              {"taxId", Nothing},
                                              {"address", Nothing}
                                                                   }
                                invoiceData("invoiceTotals") = New Dictionary(Of String, Object) From {
                                             {"lineExtensionAmount", Convert.ToDecimal(reader("BillTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                                             {"taxExclusiveAmount", Convert.ToDecimal(reader("BillTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                                             {"taxInclusiveAmount", Convert.ToDecimal(reader("BillSubTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                                             {"prepaidAmount", 0.00},
                                             {"payableAmount", Convert.ToDecimal(reader("BillSubTotal")).ToString("F2", CultureInfo.InvariantCulture)}
                                                                                                          }
                            End While
                        Else
                            MessageBox.Show("لم يتم العثور على بيانات الفاتورة!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return Nothing
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show(ex.ToString)
            End Try

            Dim invoiceLines As New List(Of Dictionary(Of String, Object))

            Using comm = New SqlCommand("ShowBillDetail", CType(connection, SqlConnection))
                comm.CommandType = CommandType.StoredProcedure
                comm.Parameters.AddWithValue("@BillGuid", TxtBillGuid.Text)
                comm.Parameters.AddWithValue("@Type_", 1)
                Using reader As SqlDataReader = Await comm.ExecuteReaderAsync()
                    While reader.Read()
                        invoiceLines.Add(New Dictionary(Of String, Object) From {
                            {"id", reader("Code").ToString()},
                            {"quantity", Convert.ToDecimal(reader("Qty")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"itemName", reader("DiplomName").ToString()},
                            {"price", Convert.ToDecimal(reader("Cost")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"taxRate", Convert.ToDecimal(reader("TaxValue")).ToString},
                            {"taxCategoryCode", reader("TAXCATOGREY").ToString()},
                            {"lineExtensionAmount", Convert.ToDecimal(reader("SubTotal")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"taxAmount", Convert.ToDecimal(reader("Tax")).ToString("F2", CultureInfo.InvariantCulture)},
                            {"roundingAmount", Convert.ToDecimal(reader("SubTotal")).ToString("F2", CultureInfo.InvariantCulture)}
                        })
                    End While
                End Using
            End Using

            invoiceData("invoiceLines") = invoiceLines
        End Using

        Return JsonConvert.SerializeObject(invoiceData, Formatting.Indented)

    End Function
    Private Async Function SendInvoiceAsync(jsonContent As String) As Task(Of String)
        MessageBox.Show(jsonContent, "JSON Content")
        Dim client As New HttpClient()
        Dim request As New HttpRequestMessage(HttpMethod.Post, "http://localhost:2020/SendInvoice")

        request.Headers.Add("accept", "*/*")
        request.Headers.Authorization = New AuthenticationHeaderValue("Bearer", OtpSender.Token__)

        request.Content = New StringContent(jsonContent)
        request.Content.Headers.ContentType = New MediaTypeHeaderValue("application/json")

        Dim response As HttpResponseMessage = Await client.SendAsync(request)
        response.EnsureSuccessStatusCode()
        Return Await response.Content.ReadAsStringAsync()
    End Function
    Private Async Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        Try
            AllFiles.SHOWSYSTEMINFO()
            Dim userName As String = AllFiles.ZUSERNAME
            Dim password As String = AllFiles.ZPASSWORD

            If String.IsNullOrWhiteSpace(userName) OrElse String.IsNullOrWhiteSpace(password) Then
                MessageBox.Show("الرجاء إدخال اسم المستخدم وكلمة المرور.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
            End If

            Dim result As String = Await SendRequestAsync(userName, password)

            ' عرض التوكن في TextBox
            TxtNotes.Text = result
            OtpSender.Token__ = result

            ''''''''''''''''''''''''''''''''' OTP
            Dim otp As String = AllFiles.ZOTP  ' استلام الـ OTP من TextBox
            Dim sender2 As New OtpSender()
            Dim result2 As String = Await sender2.SendOtpAsync(otp)
            MessageBox.Show(result2, "Response", MessageBoxButtons.OK, MessageBoxIcon.Information)

            '''''''''''''''''''''''''''''''''' Send Invoice
            Try
                Dim invoiceID As String = TxtBillCode.Text ' طلب كود الفاتورة من المستخدم
                If String.IsNullOrEmpty(invoiceID) Then
                    MessageBox.Show("يرجى إدخال كود الفاتورة!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                Dim jsonContent As String = Await GetInvoiceDataAsync(invoiceID) ' جلب بيانات الفاتورة من SQL

                If String.IsNullOrEmpty(jsonContent) Then
                    MessageBox.Show("لم يتم العثور على بيانات الفاتورة!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                Dim response As String = Await SendInvoiceAsync(jsonContent)

                MessageBox.Show("تم إرسال الفاتورة بنجاح!" & vbCrLf & response, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show("حدث خطأ: " & ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
            ''Dim unused = APILOGIN("semou", "123")
            ''Dim smssender = New SmsSender
            ''Await SmsSender.SENDSMSASYNC("sstli", "966532140378", "S S T L I", "c9978d4fea0ee0f8edbfba4e452354df", "hello", "UTF8")
            'Dim client = New HttpClient()
            'Dim request = New HttpRequestMessage()

            'request.RequestUri = New Uri("https://www.msegat.com/gw/sendsms.php")
            'request.Method = New HttpMethod("POST")
            'request.Content = New StringContent("{
            '                                       ""userName"": ""terabyte"",
            '                                       ""numbers"": ""966534135529"",
            '                                       ""userSender"": ""terabyte"",
            '                                       ""apiKey"": ""B69206CEF369F8631308FBD313EF94C1"",
            '                                       ""msg"": ""hello""
            '                                                           }")

            'request.Headers.TryAddWithoutValidation("Content-Type", "application/json")

            'Using response = Await client.SendAsync(request)
            '    Dim responseData = Await response.Content.ReadAsStringAsync()

            '    Console.WriteLine("Status " & CInt(response.StatusCode))
            '    Console.WriteLine("Headers " & response.Headers.ToString())
            '    Console.WriteLine("Data " & responseData)
            'End Using
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try

    End Sub

    '' Constructor to initialize HttpClient
    'Public Sub New()
    '    client = New HttpClient()
    'End Sub

    ' دالة إرسال الطلب إلى API
    Private Async Function SendRequestAsync(userName As String, password As String) As Task(Of String)
        Dim request As New HttpRequestMessage(HttpMethod.Post, "http://localhost:2020/Auth/login")
        request.Headers.Add("accept", "*/*")

        ' إنشاء محتوى JSON بناءً على المدخلات
        Dim jsonContent As String = $"{{""userName"": ""{userName}"", ""password"": ""{password}""}}"
        request.Content = New StringContent(jsonContent, Encoding.UTF8, "application/json")

        Try
            Dim response As HttpResponseMessage = Await client.SendAsync(request)
            response.EnsureSuccessStatusCode()
            Dim responseBody As String = Await response.Content.ReadAsStringAsync()

            ' استخراج التوكن فقط
            Dim json As JObject = JObject.Parse(responseBody)
            Dim token As String = json("token")?.ToString()

            Return If(String.IsNullOrEmpty(token), "Token not found!", token)
        Catch ex As Exception
            Return "Error: " & ex.Message
        End Try
    End Function

    Private Sub GroupPanel3_Click(sender As Object, e As EventArgs) Handles GroupPanel3.Click

    End Sub
End Class