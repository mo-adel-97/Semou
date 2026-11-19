Imports System.Data.SqlClient
Imports System.IO

Public Class EncodingOrderDis
    Dim dbcontext As New SemouEntities
    Sub LOADDICOUNTTYPE()
        BranchGrid.Rows.Clear()
        Dim connection = dbcontext.Database.Connection
        Dim comm As New SqlCommand("ShowPreDisOrder", connection)
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.CommandType = CommandType.StoredProcedure
            If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        Dim adp As New SqlDataAdapter(comm)
        Dim ds As DataSet = New DataSet()
        adp.Fill(ds, "ShowPreDisOrder")
        Dim COUNT__ As Integer = 0
        If ds.Tables(0).Rows.Count > 0 Then
            BranchGrid.RowCount = ds.Tables(0).Rows.Count
            For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                BranchGrid.Rows(COUNT__).Cells(0).Value = Convert.ToDateTime(ds.Tables(0).Rows(i)(0)).ToShortDateString()
                BranchGrid.Rows(COUNT__).Cells(1).Value = ds.Tables(0).Rows(i)(1).ToString()
                BranchGrid.Rows(COUNT__).Cells(2).Value = ds.Tables(0).Rows(i)(2).ToString()
                BranchGrid.Rows(COUNT__).Cells(3).Value = ds.Tables(0).Rows(i)(3).ToString()
                BranchGrid.Rows(COUNT__).Cells(4).Value = ds.Tables(0).Rows(i)(4).ToString()
                COUNT__ = COUNT__ + 1
            Next
        End If
    End Sub
    Private Sub CmdExit_Click(sender As Object, e As EventArgs) Handles CmdExit.Click
        Me.Close()
    End Sub

    Private Sub CmdFind_Click(sender As Object, e As EventArgs) Handles CmdFind.Click
        Try
            Dim w As New DiscountTypeList
            w.ShowDialog()
            If w.IS_Yes = True Then
                Dim ii As Integer = w.BranchGrid.CurrentRow.Index
                TxtDiscountName.Text = w.BranchGrid.Rows(ii).Cells(1).Value.ToString()
                TxtDiscountVat.Text = w.BranchGrid.Rows(ii).Cells(2).Value.ToString()
                TxtDisGuid.Text = w.BranchGrid.Rows(ii).Cells(4).Value.ToString()
                ChkAttach.Checked = w.BranchGrid.Rows(ii).Cells(5).Value
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
    Function VALIDATE_DATA() As Boolean
        If TxtDisGuid.Text = Nothing Then
            MessageBox.Show("برجاء اختيار الخصم اولا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        ElseIf ChkAttach.Checked = True Then
            If TxtAttach1Path.Text = Nothing Then
                MessageBox.Show("نوع الخصم الذي قمت باختيارة يحتاج الى ملفات مرفقة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If
        End If
        Return True
    End Function
    Sub ADDDISCOUNTORDER()
        Dim connection = dbcontext.Database.Connection
        Dim comm = New SqlCommand("AddDiscountOrder", connection)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@AccountGuid", TxtAccountGuid.Text)
        comm.Parameters.AddWithValue("@DisGuid", TxtDisGuid.Text)
        comm.Parameters.AddWithValue("@UserGuid", AllFiles.USER_GUID____.ToString())
        comm.Parameters.AddWithValue("@IsUse", ChkStaut.Value)
        ' إضافة معامل إخراج لاستلام @Code
        Dim outputParam As New SqlParameter("@CodeOut", SqlDbType.Float)
        outputParam.Direction = ParameterDirection.Output
        comm.Parameters.Add(outputParam)
        If connection.State = ConnectionState.Open Then
            connection.Close()
        End If
        connection.Open()
        comm.ExecuteNonQuery()
        ' استرجاع القيمة الفعلية لمعامل الإخراج
        Dim RETURNCODE As Double = Convert.ToDouble(outputParam.Value)
        TxtCode.Text = RETURNCODE.ToString()
        comm.Parameters.Clear()
    End Sub

    Private Sub CmdSave_Click(sender As Object, e As EventArgs) Handles CmdSave.Click
        Try
            If VALIDATE_DATA() = False Then
                Return
            Else
                If TxtGuid.Text = Nothing Then
                    ADDDISCOUNTORDER()
                    If ChkAttach.Checked = True Then
                        AADATTACHFILE()
                    End If
                    MessageBox.Show("تم حفظ بيانات طلب الخصم بنجاح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    TxtGuid.Text = True
                End If
            End If
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub

    Private Sub CmdAttach1_Click(sender As Object, e As EventArgs) Handles CmdAttach1.Click
        Try
            OpenFileDialog1.Title = "برجاء اختيار مرفقات الخصم"
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
        Dim targetDirectory As String = "\\192.168.11.236\Arc\" & TxtNationalId.Text
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
        Dim dest_file As String = targetDirectory & "\" & "Discount" & "_" & TxtNationalId.Text & "_" & fileExtension1
        System.IO.File.Copy(SOURCE_FILE, dest_file, True)
    End Sub

    Private Sub EncodingOrderDis_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            LOADDICOUNTTYPE()
        Catch ex As Exception
            MessageBox.Show(ex.ToString)
        End Try
    End Sub
End Class