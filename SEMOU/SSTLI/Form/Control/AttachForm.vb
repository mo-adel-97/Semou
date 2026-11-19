Imports System.Data.SqlClient
Imports System.IO
Imports System.Security
Imports System.Security.AccessControl
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Security.Cryptography
Imports Newtonsoft.Json

Public Class AttachForm
    Dim dbcontext As New SemouEntities
    Public IS_Yes As Boolean = False

    ' ==========================
    ' API ENDPOINT
    ' ==========================
    Private Const UPLOAD_URL As String = "https://sstli.com/arc/upload_semou.php"

    ' ⚠️ نفس السر اللي في PHP (الحل السريع)
    ' الأفضل أمنياً: تعمل endpoint في السيرفر يولّد token وترجعه للتطبيق بدل ما تحط السر هنا
    Private Const ARC_SECRET As String = "Your_Very_Long_Secret_Key_1234567890!@#$%^&*()"

    Sub LOADATTACH()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowAttachName", connection)
        If TxtType.Text = 1 Then
            comm.Parameters.AddWithValue("@Type_", TxtType.Text)
        End If
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowAttachName")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = ds.Tables(0).Rows(i)(0).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(2).ToString()

                Select Case BranchGrid.Rows(COUNT__).Cells(3).Value
                    Case "Letter"
                        BranchGrid.Rows(COUNT__).Cells(5).Value = 0
                    Case "contract"
                        If ChkContract.Checked = True Then
                            BranchGrid.Rows(COUNT__).Cells(5).Value = 1
                        Else
                            BranchGrid.Rows(COUNT__).Cells(5).Value = 0
                        End If
                    Case "trans"
                        If TxtPassword.Text = "صندوق المكتب" Then
                            BranchGrid.Rows(COUNT__).Cells(5).Value = 0
                        Else
                            BranchGrid.Rows(COUNT__).Cells(5).Value = 1
                        End If
                    Case "Certifcat"
                        BranchGrid.Rows(COUNT__).Cells(5).Value = 1
                    Case "ID"
                        BranchGrid.Rows(COUNT__).Cells(5).Value = 1
                    Case "trans2"
                        If TxtPasseord2.Text = "صندوق المكتب" Or TxtPasseord2.Text = Nothing Then
                            BranchGrid.Rows(COUNT__).Cells(5).Value = 0
                        ElseIf TxtPasseord2.Text <> "صندوق المكتب" And TxtPasseord2.Text <> Nothing Then
                            BranchGrid.Rows(COUNT__).Cells(5).Value = 1
                        End If
                End Select

                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub

    Private Sub AttachForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADATTACH()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub BranchGrid_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellContentClick
    End Sub

    Private Sub BranchGrid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellDoubleClick
    End Sub

    Public Function NOFILE_() As Boolean
        For i As Integer = 0 To BranchGrid.Rows.Count - 1
            If BranchGrid.Rows(i).Cells(5).Value = 1 Then
                If BranchGrid.Rows(i).Cells(2).Value = Nothing Then
                    Return False
                End If
            End If
        Next
        Return True
    End Function

    ' ==========================
    ' ✅ UPDATED: Upload to API instead of File.Copy
    ' ==========================
    Private Async Sub CmdOk_Click(sender As Object, e As EventArgs) Handles CmdOk.Click
        Try
            If NOFILE_() = False Then
                MessageBox.Show("لا يمكن الحفظ بدون اختيار الملفات المرفقة ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' اجمع الملفات المطلوبة (اللي Cells(5)=1)
            Dim filesToUpload As New List(Of UploadFileItem)
            For Each row As DataGridViewRow In BranchGrid.Rows
                If row.IsNewRow Then Continue For

                Dim required As Boolean = False
                Try
                    required = (Convert.ToInt32(If(row.Cells(5).Value, 0)) = 1)
                Catch
                    required = False
                End Try

                If Not required Then Continue For

                Dim localPath As String = If(row.Cells(2).Value, "").ToString().Trim()
                Dim serverName As String = If(row.Cells(1).Value, "").ToString().Trim()

                If String.IsNullOrWhiteSpace(localPath) OrElse Not File.Exists(localPath) Then
                    ' المفروض NOFILE_ يمنع الحالة دي
                    Continue For
                End If

                If String.IsNullOrWhiteSpace(serverName) Then
                    serverName = Path.GetFileName(localPath)
                End If

                filesToUpload.Add(New UploadFileItem With {
                    .RowIndex = row.Index,
                    .LocalPath = localPath,
                    .OriginalNameOnServer = serverName
                })
            Next

            If filesToUpload.Count = 0 Then
                MessageBox.Show("مفيش ملفات للرفع.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' رفع
            Me.Enabled = False
            Dim resp As SemouUploadResponse = Await UploadFilesAsync(TxtNationalid.Text, filesToUpload)

            ' النتيجة
            If resp IsNot Nothing AndAlso resp.success Then

                ' حط URL في Cells(2) لكل صف حسب origName
                ' (نطابق بالاسم اللي بعته السيرفر = origName اللي هو serverName بتاعنا)
                If resp.saved IsNot Nothing AndAlso resp.saved.Count > 0 Then
                    For Each s In resp.saved
                        Dim matched = filesToUpload.FirstOrDefault(Function(x) String.Equals(x.OriginalNameOnServer, s.origName, StringComparison.OrdinalIgnoreCase))
                        If matched IsNot Nothing Then
                            BranchGrid.Rows(matched.RowIndex).Cells(2).Value = s.url ' نخليها URL بدل المسار المحلي
                        End If
                    Next
                End If

                MessageBox.Show(resp.message, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information)
                IS_Yes = True
                Me.Close()

            Else
                Dim msg As String = If(resp?.message, "فشل رفع الملفات.")
                MessageBox.Show(msg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        Finally
            Me.Enabled = True
        End Try
    End Sub

    Private Sub CmdClose_Click(sender As Object, e As EventArgs) Handles CmdClose.Click
        Me.Close()
    End Sub

    Private Sub BranchGrid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles BranchGrid.CellClick
        Try
            Dim i As Integer = BranchGrid.CurrentCell.RowIndex
            Dim j As Integer = BranchGrid.CurrentCell.ColumnIndex

            If j = 1 Then
                Dim openFileDialog1 As New OpenFileDialog()
                If openFileDialog1.ShowDialog() = DialogResult.OK Then
                    Dim filePath As String = openFileDialog1.FileName
                    Dim fileExtension As String = Path.GetExtension(filePath)
                    Dim newFileName As String = BranchGrid.Rows(i).Cells(3).Value & "_" & TxtNationalid.Text & fileExtension

                    BranchGrid.Rows(i).Cells(1).Value = newFileName
                    BranchGrid.Rows(i).Cells(2).Value = filePath ' قبل الرفع: مسار محلي (وبعد الرفع هنبدله بـ URL)
                End If

            ElseIf j = 5 Then
                If BranchGrid.Rows(i).Cells(5).Value = 1 And BranchGrid.Rows(i).Cells(3).Value = "Letter" Then
                    BranchGrid.Rows(i).Cells(5).Value = 0
                ElseIf BranchGrid.Rows(i).Cells(5).Value = 1 And BranchGrid.Rows(i).Cells(3).Value = "trans" Then
                    If TxtPassword.Text = "صندوق المكتب" Then
                        BranchGrid.Rows(i).Cells(5).Value = 0
                    End If
                ElseIf BranchGrid.Rows(i).Cells(5).Value = 0 And BranchGrid.Rows(i).Cells(3).Value = "trans" Then
                    If TxtPassword.Text = "صندوق المكتب" Then
                        BranchGrid.Rows(i).Cells(5).Value = 1
                    End If
                ElseIf BranchGrid.Rows(i).Cells(5).Value = 1 And BranchGrid.Rows(i).Cells(3).Value = "trans2" Then
                    If TxtPasseord2.Text = "صندوق المكتب" Or TxtPasseord2.Text = Nothing Then
                        BranchGrid.Rows(i).Cells(5).Value = 0
                    End If
                ElseIf BranchGrid.Rows(i).Cells(5).Value = 0 And BranchGrid.Rows(i).Cells(3).Value = "trans2" Then
                    If TxtPasseord2.Text = "صندوق المكتب" Then
                        BranchGrid.Rows(i).Cells(5).Value = 1
                    End If
                End If

            ElseIf j = 6 Then
                If BranchGrid.Rows(i).Cells(2).Value <> Nothing Then
                    Dim linkOrPath As String = BranchGrid.Rows(i).Cells(2).Value.ToString()

                    Dim VIEW As New AttachView
                    VIEW.WebBrowser1.Navigate(linkOrPath)
                    VIEW.ShowDialog()
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    ' ==========================
    ' Upload Helpers
    ' ==========================
    Private Async Function UploadFilesAsync(nationalId As String, files As List(Of UploadFileItem)) As Task(Of SemouUploadResponse)
        ' TLS for .NET Framework
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls11

        Dim token As String = GenerateToken_Local(nationalId, 30) ' 30 minutes

        Using client As New HttpClient()
            client.Timeout = TimeSpan.FromMinutes(5)

            Using form As New MultipartFormDataContent()
                form.Add(New StringContent(token, Encoding.UTF8), "token")

                For Each f In files
                    Dim bytes = File.ReadAllBytes(f.LocalPath)
                    Dim content = New ByteArrayContent(bytes)
                    content.Headers.ContentType = New MediaTypeHeaderValue("application/octet-stream")

                    ' اسم الحقل لازم "files[]" عشان PHP يتعامل كمصفوفة
                    form.Add(content, "files[]", f.OriginalNameOnServer)
                Next

                Dim resp = Await client.PostAsync(UPLOAD_URL, form)
                Dim json = Await resp.Content.ReadAsStringAsync()

                If Not resp.IsSuccessStatusCode Then
                    ' غالباً بيرجع JSON error
                    Throw New Exception("Upload failed: " & json)
                End If

                Return JsonConvert.DeserializeObject(Of SemouUploadResponse)(json)
            End Using
        End Using
    End Function

    ' token = b64url( "nationalId|exp|sig" )
    ' sig = b64url( HMACSHA256( "nationalId|exp", ARC_SECRET ) )
    Private Function GenerateToken_Local(nationalId As String, minutesValid As Integer) As String
        Dim cleanId = New String(nationalId.Where(Function(c) Char.IsDigit(c)).ToArray())
        Dim exp As Long = DateTimeOffset.UtcNow.AddMinutes(minutesValid).ToUnixTimeSeconds()

        Dim payload As String = cleanId & "|" & exp.ToString()
        Dim sig As String = B64UrlEncode(HmacSha256(payload, ARC_SECRET))

        Dim tokenPlain As String = cleanId & "|" & exp.ToString() & "|" & sig
        Dim token As String = B64UrlEncode(Encoding.UTF8.GetBytes(tokenPlain))
        Return token
    End Function

    Private Function HmacSha256(text As String, key As String) As Byte()
        Using h As New HMACSHA256(Encoding.UTF8.GetBytes(key))
            Return h.ComputeHash(Encoding.UTF8.GetBytes(text))
        End Using
    End Function

    Private Function B64UrlEncode(data As Byte()) As String
        Dim s = Convert.ToBase64String(data)
        s = s.TrimEnd("="c).Replace("+"c, "-"c).Replace("/"c, "_"c)
        Return s
    End Function

End Class

' ==========================
' DTOs
' ==========================
Public Class SemouUploadResponse
    Public Property success As Boolean
    Public Property message As String
    Public Property root As String
    Public Property folder As String
    Public Property savedCount As Integer
    Public Property failedCount As Integer
    Public Property saved As List(Of SemouSavedItem)
    Public Property failed As List(Of SemouFailedItem)
End Class

Public Class SemouSavedItem
    Public Property origName As String
    Public Property fileName As String
    Public Property mime As String
    Public Property ext As String
    Public Property url As String
End Class

Public Class SemouFailedItem
    Public Property index As Integer
    Public Property name As String
    Public Property [error] As String
End Class

Public Class UploadFileItem
    Public Property RowIndex As Integer
    Public Property LocalPath As String
    Public Property OriginalNameOnServer As String
End Class
