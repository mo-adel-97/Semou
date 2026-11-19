Imports System.Data.SqlClient
Imports System.IO

Public Class ContractHome
    Dim dbcontext As New SemouEntities
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub
    Sub SHOWRECEPITINFO()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowContractInfo", connection)
        comm.Parameters.AddWithValue("@NationalId", TxtFind2.Text)
        comm.CommandType = CommandType.StoredProcedure
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowContractInfo")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                TxtStudentName.Text = ds.Tables(0).Rows(i)(0).ToString()
                TxtTel.Text = ds.Tables(0).Rows(i)(1).ToString()
                TxtBranchName.Text = ds.Tables(0).Rows(i)(2).ToString()
                TxtSellerName.Text = ds.Tables(0).Rows(i)(3).ToString()
                TxtCode.Text = ds.Tables(0).Rows(i)(4).ToString()
                TxtDayDate.Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(5)).ToShortDateString()
                TxtTime.Text = ds.Tables(0).Rows(i)(6).ToString()
                ChkContract.Checked = ds.Tables(0).Rows(i)(7)
                TxtGuid.Text = ds.Tables(0).Rows(i)(8).ToString()
                COUNT__ = COUNT__ + 1
            Next
        Else
            MessageBox.Show("لا توجد اى استمارات مسجلة للطالب برجاء مراجعه كشف الحساب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            TxtStudentName.Text = Nothing
            TxtTel.Text = Nothing
            TxtBranchName.Text = Nothing
            TxtSellerName.Text = Nothing
            TxtCode.Text = Nothing
            TxtDayDate.Value = Convert.ToDateTime(DateTime.Now).ToShortDateString()
            TxtTime.Text = Nothing
            ChkContract.Checked = Nothing
            TxtGuid.Text = Nothing
        End If
    End Sub

    Private Sub TxtFind2_TextChanged(sender As Object, e As EventArgs) Handles TxtFind2.TextChanged

    End Sub

    Private Sub TxtFind2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtFind2.KeyPress
        Try
            If e.KeyChar = Chr(13) Then
                SHOWRECEPITINFO()
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub UPDATESTAUT()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("ChangeContractStaut", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@Guid", TxtGuid.Text)
        comm.Parameters.AddWithValue("@Staut", ChkContract.Checked)
        If ChkContract.Checked = True Then
            comm.Parameters.AddWithValue("@Time", 0)
        Else
            comm.Parameters.AddWithValue("@Time", 30)
        End If
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        comm.Parameters.Clear()
    End Sub
    Private Sub CmdOk_Click(sender As Object, e As EventArgs) Handles CmdOk.Click
        Try
            If TxtGuid.Text <> Nothing Then
                If AllFiles.FORSAVE("contract") = False Then
                    Return
                Else
                    If ChkContract.Checked = True And TxtAttach1Path.Text = Nothing Then
                        MessageBox.Show("برجاء اختيار ملف الاتفاقية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    Else
                        UPDATESTAUT()
                        AADATTACHFILE()
                        MessageBox.Show("تم تعديل حالة الاتفاقية بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End If
            Else
                MessageBox.Show("برجاء اختيار طالب اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdAttach1_Click(sender As Object, e As EventArgs) Handles CmdAttach1.Click
        Try
            OpenFileDialog1.Title = "برجاء اختيار اتفاقية التدريب"
            'OpenFileDialog1.InitialDirectory = "c:"
            OpenFileDialog1.FileName = Nothing
            OpenFileDialog1.ShowDialog()
            TxtAttach1Path.Text = OpenFileDialog1.FileName.ToString
            Dim fileExtension As String = Path.GetExtension(TxtAttach1Path.Text)
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Sub AADATTACHFILE()
        AllFiles.SHOWSYSTEMINFO()
        Dim baseDirectory As String = AllFiles.UPDATEURL
        Dim targetDirectory As String = baseDirectory & TxtFind2.Text
        Dim fileExtension1 As String = Path.GetExtension(TxtAttach1Path.Text)
        If Not Directory.Exists(targetDirectory) Then
            Try
                Directory.CreateDirectory(targetDirectory)
            Catch ex As Exception
                MessageBox.Show("حدث خطأ أثناء إنشاء المجلد: " + ex.Message)
                Exit Sub
            End Try
        End If
        Dim SOURCE_FILE = TxtAttach1Path.Text
        Dim dest_file As String = targetDirectory & "\" & "Contract" & "_" & TxtFind2.Text & "_" & fileExtension1
        System.IO.File.Copy(SOURCE_FILE, dest_file, True)
    End Sub

    Private Sub ContractHome_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class