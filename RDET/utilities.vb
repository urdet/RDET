Imports System
Imports System.Data
Imports MySql.Data.MySqlClient
Public Class utilities
    Public Shared consq As MySqlConnection = Class1.conectionscr
    Public Shared condmc As MySqlConnection = Class1.conectiondmc
    Public Shared conreg As MySqlConnection = Class1.conectionreg
    Public Shared Sub EXCUTESomeQueriesSq(stringQuery As String, pr() As String, id() As Object)
        If consq.State = ConnectionState.Open Then
        Else
            consq.Open()
        End If
        Dim query As String = stringQuery
        Using command As New MySqlCommand(query, consq)
            For i = 0 To pr.Length - 1
                command.Parameters.AddWithValue(pr(i), id(i))
            Next
            command.ExecuteNonQuery()
        End Using
        consq.Close()
    End Sub
    Public Shared Function checkValiditi(first As String, id As Integer) As String
        If consq.State = ConnectionState.Open Then
        Else
            consq.Open()
        End If

        Dim command As New MySqlCommand($"SELECT * FROM users where ID={id} AND First='{first}'", consq)

        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)
        If table.Rows.Count = 0 Then
            Return Nothing
            consq.Close()
        Else
            Return table.Rows(0)(9).ToString
            consq.Close()
        End If
    End Function
    Public Shared Function checksld(id As Integer, mtn As Decimal)
        If consq.State = ConnectionState.Open Then
        Else
            consq.Open()
        End If

        Dim command As New MySqlCommand($"SELECT * FROM hisabat where ID={id}", consq)

        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)
        If table.Rows.Count = 0 Then
            Return "Error"
            consq.Close()
        Else
            Dim a As Decimal = table.Rows(0)(2) - mtn
            If a >= 0 Then
                Return True
            Else
                Return False
            End If

            consq.Close()
        End If
    End Function
    Public Shared Function getsld(id As Integer)
        If consq.State = ConnectionState.Open Then
        Else
            consq.Open()
        End If
        Try
            Dim command As New MySqlCommand($"SELECT * FROM hisabat where ID={id}", consq)

            Dim adapter As New MySqlDataAdapter(command)
            Dim table As New DataTable()

            adapter.Fill(table)
            If table.Rows.Count > 0 AndAlso table.Columns.Count > 2 Then
                ' Check if the value is NOT NULL
                If Not IsDBNull(table(0)(2)) Then
                    Return table(0)(2)
                End If
            End If

        Catch ex As Exception

        End Try


        consq.Close()
    End Function
    Public Shared Function getmtn(id As Integer)
        If condmc.State = ConnectionState.Open Then
        Else
            condmc.Open()
        End If

        Dim command As New MySqlCommand($"SELECT * FROM tansactionscomptes where ID={id}", condmc)

        Dim adapter As New MySqlDataAdapter(command)
        Dim table As New DataTable()

        adapter.Fill(table)
        Return table(0)(3)

        condmc.Close()
    End Function
    Public Shared Function CheckIfIdExistsdmc(idToCheck As Integer, tablename As String) As Boolean
        Dim idExists As Boolean = False
        If condmc.State = ConnectionState.Open Then
        Else
            condmc.Open()
        End If

        Dim query As String = $"SELECT COUNT(*) FROM {tablename} WHERE ID = @IdToCheck"
        Using command As New MySqlCommand(query, condmc)
            command.Parameters.AddWithValue("@IdToCheck", idToCheck)

            Dim count As Integer = CInt(command.ExecuteScalar())

            ' If count is greater than 0, the ID exists
            idExists = (count > 0)
        End Using
        condmc.Close()
        Return idExists
    End Function

    Public Shared Sub UpdateSomeValuesdmc(stringQuery As String, id As Object, pr As String)
        If condmc.State = ConnectionState.Open Then
        Else
            condmc.Open()
        End If
        Dim query As String = $"{stringQuery} WHERE {pr} = {id};"
        Using command As New MySqlCommand(query, condmc)
            command.ExecuteNonQuery()
        End Using
        condmc.Close()
    End Sub

    Public Shared Function detailbankVersment(ty As Integer)
        If condmc.State = ConnectionState.Open Then
        Else
            condmc.Open()
        End If
        Dim com As New MySqlCommand
        com.CommandType = CommandType.StoredProcedure
        com.CommandText = "GetBankDetail"
        com.Connection = condmc
        com.Parameters.AddWithValue("Typ", ty)
        Dim adapter As New MySqlDataAdapter(com)
        Dim table As New DataTable()

        adapter.Fill(table)
        Return table

        condmc.Close()
    End Function
    Public Shared Function GetSomeValuesdmc(stringQuery As String, id As Object, pr As String)
        If condmc.State = ConnectionState.Open Then
        Else
            condmc.Open()
        End If
        Dim query As String = $"{stringQuery} WHERE {pr} = {id}"
        Using command As New MySqlCommand(query, condmc)
            Dim adapter As New MySqlDataAdapter(command)
            Dim table As New DataTable()

            adapter.Fill(table)
            Return table
        End Using
        condmc.Close()
    End Function
    Public Shared Function GetSomeValuesreg(stringQuery As String, id As Object, pr As String)
        If conreg.State = ConnectionState.Open Then
        Else
            conreg.Open()
        End If
        Dim query As String = $"{stringQuery} WHERE {pr} = {id}"
        Using command As New MySqlCommand(query, conreg)
            Dim adapter As New MySqlDataAdapter(command)
            Dim table As New DataTable()

            adapter.Fill(table)
            Return table
        End Using
        conreg.Close()
    End Function
    Public Shared Function GetSomeValuesscr(stringQuery As String, id As Object, pr As String)
        If consq.State = ConnectionState.Open Then
        Else
            consq.Open()
        End If
        Dim query As String = $"{stringQuery} WHERE {pr} = {id}"
        Using command As New MySqlCommand(query, consq)
            Dim adapter As New MySqlDataAdapter(command)
            Dim table As New DataTable()

            adapter.Fill(table)
            Return table
        End Using
        consq.Close()
    End Function

    Public Shared Sub AddTransServices(isDouble As Boolean, TypeOfTrans As String, serviceID As Integer, serviceName As String, mtn As Decimal, Description As String, Datex As DateTime, uUser As String, Frais As Decimal)
        If condmc.State = ConnectionState.Closed Then
            condmc.Open()
        End If
        ' MsgBox($"isdouble: {isDouble}, TypeOfTrans: {TypeOfTrans}, serviceID: {serviceID}, ServiceName: {serviceName}, mtn: {mtn}, date:{Datex},uuser : {uUser}, frais: {Frais}")
        If isDouble Then
            Dim para1 As String
            'Para1
            Dim dt As DataTable = GetSomeValuesdmc("SELECT * FROM services", serviceID, "ID")
            para1 = dt.Rows(0)(4)
            'Compare bettwen para1 and typ of service
            If para1 = TypeOfTrans Then
                'From = para2
                Dim fromCompte As Integer = dt.Rows(0)(5)
                'To = para3
                Dim ToCompte As Integer = dt.Rows(0)(6)
                'MsgBox($"from :{fromCompte}, To: {ToCompte}")
                'Execute 
                Dim com As New MySqlCommand
                com.CommandType = CommandType.StoredProcedure
                com.CommandText = "trans"
                com.Connection = condmc
                com.Parameters.AddWithValue("ty", 1)
                com.Parameters.AddWithValue("FCompte", fromCompte)
                com.Parameters.AddWithValue("TCompte", ToCompte)
                com.Parameters.AddWithValue("IDr", Nothing)
                com.Parameters.AddWithValue("Serv", serviceName)
                com.Parameters.AddWithValue("Mtn", mtn)
                com.Parameters.AddWithValue("descr", Description)
                com.Parameters.AddWithValue("Datex", Datex)
                com.Parameters.AddWithValue("uUser", uUser)
                com.Parameters.AddWithValue("TypServ", TypeOfTrans)
                com.Parameters.AddWithValue("Frais", Frais)
                com.ExecuteNonQuery()
            Else
                'From = para3
                Dim fromCompte As Integer = dt.Rows(0)(6)
                'To = para2
                Dim ToCompte As Integer = dt.Rows(0)(5)

                'MsgBox($"from :{fromCompte}, To: {ToCompte}")
                'Execute
                Dim com As New MySqlCommand
                com.CommandType = CommandType.StoredProcedure
                com.CommandText = "trans"
                com.Connection = condmc
                com.Parameters.AddWithValue("ty", 1)
                com.Parameters.AddWithValue("FCompte", fromCompte)
                com.Parameters.AddWithValue("TCompte", ToCompte)
                com.Parameters.AddWithValue("IDr", Nothing)
                com.Parameters.AddWithValue("Serv", serviceName)
                com.Parameters.AddWithValue("Mtn", mtn)
                com.Parameters.AddWithValue("descr", Description)
                com.Parameters.AddWithValue("Datex", Datex)
                com.Parameters.AddWithValue("uUser", uUser)
                com.Parameters.AddWithValue("TypServ", TypeOfTrans)
                com.Parameters.AddWithValue("Frais", Frais)
                com.ExecuteNonQuery()
            End If
        Else
            Dim dt As DataTable = GetSomeValuesdmc("SELECT * FROM services", serviceID, "ID")
            Dim fromCompte As Integer = dt.Rows(0)(5)
            Dim ToCompte As Integer = dt.Rows(0)(6)
            Dim com As New MySqlCommand
            'MsgBox($"from :{fromCompte}, To: {ToCompte}")
            com.CommandType = CommandType.StoredProcedure
            com.CommandText = "trans"
            com.Connection = condmc
            com.Parameters.AddWithValue("ty", 1)
            com.Parameters.AddWithValue("FCompte", fromCompte)
            com.Parameters.AddWithValue("TCompte", ToCompte)
            com.Parameters.AddWithValue("IDr", Nothing)
            com.Parameters.AddWithValue("Serv", serviceName)
            com.Parameters.AddWithValue("Mtn", mtn)
            com.Parameters.AddWithValue("descr", Description)
            com.Parameters.AddWithValue("Datex", Datex)
            com.Parameters.AddWithValue("uUser", uUser)
            com.Parameters.AddWithValue("TypServ", TypeOfTrans)
            com.Parameters.AddWithValue("Frais", Frais)
            com.ExecuteNonQuery()
        End If
        AutoOperations.UComptes1(Datex)
        condmc.Close()
    End Sub


    Public Shared Sub Deletetrans(isDouble As Boolean, ID As Integer, TypeOfTrans As String, serviceID As Integer, serviceName As String, mtn As Decimal, Description As String, Datex As DateTime, uUser As String, Frais As Decimal)
        If condmc.State = ConnectionState.Closed Then
            condmc.Open()
        End If
        If isDouble Then
            Dim para1 As String
            'Para1
            Dim dt As DataTable = GetSomeValuesdmc("SELECT * FROM services", serviceID, "ID")
            para1 = dt.Rows(0)(4)
            'Compare bettwen para1 and typ of service
            If para1 = TypeOfTrans Then
                'From = para2
                Dim fromCompte As Integer = dt.Rows(0)(5)
                'To = para3
                Dim ToCompte As Integer = dt.Rows(0)(6)

                'Execute 
                Dim com As New MySqlCommand
                com.CommandType = CommandType.StoredProcedure
                com.CommandText = "trans"
                com.Connection = condmc
                com.Parameters.AddWithValue("ty", 2)
                com.Parameters.AddWithValue("FCompte", fromCompte)
                com.Parameters.AddWithValue("TCompte", ToCompte)
                com.Parameters.AddWithValue("IDr", ID)
                com.Parameters.AddWithValue("Serv", serviceName)
                com.Parameters.AddWithValue("Mtn", mtn)
                com.Parameters.AddWithValue("descr", Description)
                com.Parameters.AddWithValue("Datex", Datex)
                com.Parameters.AddWithValue("uUser", uUser)
                com.Parameters.AddWithValue("TypServ", TypeOfTrans)
                com.Parameters.AddWithValue("Frais", Frais)
                com.ExecuteNonQuery()
            Else
                'From = para3
                Dim fromCompte As Integer = dt.Rows(0)(6)
                'To = para2
                Dim ToCompte As Integer = dt.Rows(0)(5)
                ' CAR PARA1 = <> type os trans
                Dim com As New MySqlCommand
                com.CommandType = CommandType.StoredProcedure
                com.CommandText = "trans"
                com.Connection = condmc
                com.Parameters.AddWithValue("ty", 2)
                com.Parameters.AddWithValue("FCompte", fromCompte)
                com.Parameters.AddWithValue("TCompte", ToCompte)
                com.Parameters.AddWithValue("IDr", ID)
                com.Parameters.AddWithValue("Serv", serviceName)
                com.Parameters.AddWithValue("Mtn", mtn)
                com.Parameters.AddWithValue("descr", Description)
                com.Parameters.AddWithValue("Datex", Datex)
                com.Parameters.AddWithValue("uUser", uUser)
                com.Parameters.AddWithValue("TypServ", TypeOfTrans)
                com.Parameters.AddWithValue("Frais", Frais)
                com.ExecuteNonQuery()
            End If
        Else
            Dim dt As DataTable = GetSomeValuesdmc("SELECT * FROM services", serviceID, "ID")
            Dim fromCompte = dt.Rows(0)(5)
            Dim ToCompte = dt.Rows(0)(6)
            Dim com As New MySqlCommand
            com.CommandType = CommandType.StoredProcedure
            com.CommandText = "trans"
            com.Connection = condmc
            com.Parameters.AddWithValue("ty", 2)
            com.Parameters.AddWithValue("FCompte", fromCompte)
            com.Parameters.AddWithValue("TCompte", ToCompte)
            com.Parameters.AddWithValue("IDr", ID)
            com.Parameters.AddWithValue("Serv", serviceName)
            com.Parameters.AddWithValue("Mtn", mtn)
            com.Parameters.AddWithValue("descr", Description)
            com.Parameters.AddWithValue("Datex", Datex)
            com.Parameters.AddWithValue("uUser", uUser)
            com.Parameters.AddWithValue("TypServ", TypeOfTrans)
            com.Parameters.AddWithValue("Frais", Frais)
            com.ExecuteNonQuery()
        End If
        AutoOperations.UComptes1(Datex)
        condmc.Close()
    End Sub



    Public Shared Sub Updatetrans(isDouble As Boolean, ID As Integer, TypeOfTrans As String, serviceID As Integer, serviceName As String, mtn As Decimal, Description As String, Datex As DateTime, uUser As String, NewFrais As Decimal)
        If condmc.State = ConnectionState.Closed Then
            condmc.Open()
        End If
        If isDouble Then
            Dim para1 As String
            'Para1
            Dim dt As DataTable = GetSomeValuesdmc("SELECT * FROM services", serviceID, "ID")
            para1 = dt.Rows(0)(4)
            'Compare bettwen para1 and typ of service
            If para1 = TypeOfTrans Then
                'From = para2
                Dim fromCompte As Integer = dt.Rows(0)(5)
                'To = para3
                Dim ToCompte As Integer = dt.Rows(0)(6)
                'Execute 
                Dim com As New MySqlCommand
                com.CommandType = CommandType.StoredProcedure
                com.CommandText = "UpTans"
                com.Connection = condmc
                com.Parameters.AddWithValue("typ", 3) 'typIDr FComp TComp Serv Mtn1 descr Datex uUser
                com.Parameters.AddWithValue("FComp", fromCompte)
                com.Parameters.AddWithValue("TComp", ToCompte)
                com.Parameters.AddWithValue("IDr", ID)
                com.Parameters.AddWithValue("Serv", serviceName)
                com.Parameters.AddWithValue("Mtn1", mtn)
                com.Parameters.AddWithValue("descr", Description)
                com.Parameters.AddWithValue("Datex", Datex)
                com.Parameters.AddWithValue("uUser", uUser)
                com.Parameters.AddWithValue("NewFrais", NewFrais)
                com.ExecuteNonQuery()
            Else
                'From = para2
                Dim fromCompte As Integer = dt.Rows(0)(6)
                'To = para3
                Dim ToCompte As Integer = dt.Rows(0)(5)
                'Execute
                Dim com As New MySqlCommand
                com.CommandType = CommandType.StoredProcedure
                com.CommandText = "UpTans"
                com.Connection = condmc
                com.Parameters.AddWithValue("typ", 3) 'typIDr FComp TComp Serv Mtn1 descr Datex uUser
                com.Parameters.AddWithValue("FComp", fromCompte)
                com.Parameters.AddWithValue("TComp", ToCompte)
                com.Parameters.AddWithValue("IDr", ID)
                com.Parameters.AddWithValue("Serv", serviceName)
                com.Parameters.AddWithValue("Mtn1", mtn)
                com.Parameters.AddWithValue("descr", Description)
                com.Parameters.AddWithValue("Datex", Datex)
                com.Parameters.AddWithValue("uUser", uUser)
                com.Parameters.AddWithValue("NewFrais", NewFrais)
                com.ExecuteNonQuery()
            End If
        Else
            Dim dt As DataTable = GetSomeValuesdmc("SELECT * FROM services", serviceID, "ID")
            Dim fromCompte = dt.Rows(0)(5)
            Dim ToCompte = dt.Rows(0)(6)
            Dim com As New MySqlCommand
            com.CommandType = CommandType.StoredProcedure
            com.CommandText = "UpTans"
            com.Connection = condmc
            com.Parameters.AddWithValue("typ", 3) 'typIDr FComp TComp Serv Mtn1 descr Datex uUser
            com.Parameters.AddWithValue("FComp", fromCompte)
            com.Parameters.AddWithValue("TComp", ToCompte)
            com.Parameters.AddWithValue("IDr", ID)
            com.Parameters.AddWithValue("Serv", serviceName)
            com.Parameters.AddWithValue("Mtn1", mtn)
            com.Parameters.AddWithValue("descr", Description)
            com.Parameters.AddWithValue("Datex", Datex)
            com.Parameters.AddWithValue("uUser", uUser)
            com.Parameters.AddWithValue("NewFrais", NewFrais)
            com.ExecuteNonQuery()
        End If
        AutoOperations.UComptes1(Datex)
        condmc.Close()
    End Sub




    Public Shared Function Getfrais(serviceid As String, mtn As Decimal, Typ As String) As Decimal
        Dim dt As DataTable = GetSomeValuesdmc("SELECT * FROM fraisargumenttable ", serviceid, $"ServiceType = '[{Typ}]' AND ServiceID")
                If dt.Rows.Count > 0 Then
            For i = 0 To dt.Rows.Count - 1
                'MsgBox("1")
                Dim parts() As String = dt.Rows(i)(3).ToString.Split("][")
                Dim str As String = parts(0)
                Dim str1 As String = str.Replace("[", "")
                Dim str2 As String = str1.Replace("]", "")
                '
                Dim sr As String = parts(1)
                Dim sr1 As String = sr.Replace("[", "")
                Dim sr2 As String = sr1.Replace("]", "")
                '
                'MsgBox($"str2 = {CDec(str2)}, sr2 = {CDec(sr2)}, mtn = {mtn}")
                If CDec(str2) <= mtn And mtn <= CDec(sr2) Then
                    Return dt.Rows(i)(4)
                    Exit Function
                End If
            Next
            Return 0
        Else
            'MsgBox("2")
            Return 0
        End If

    End Function
    Public Shared Function FundexOperation() As Integer
        Dim dt As DataTable = GetSomeValuesscr("SELECT Solde FROM hisabat ", 2, "ID")
        Dim sld As Decimal = dt.Rows(0)(0)
        Dim sl As Integer = Int(sld / 100) * 100 - 10000
        Return sl
    End Function
    Public Shared Function getRelationOfCompte(id As Integer)
        Dim dt As DataTable = GetSomeValuesdmc("SELECT Type FROM relationsvompte", id, $"Para1 = {id} OR Para2")
        If dt.Rows.Count > 0 Then
            Dim dt1 As DataTable = GetSomeValuesdmc("SELECT Description FROM relationtypes", dt.Rows(0)(0).ToString, $"Type")
            If dt1.Rows.Count Then
                Return dt1.Rows(0)(0).ToString
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function
    Public Shared Function CalculeBalance()
        Dim sld1 As Decimal = getsld(1)
        Dim sld2 As Decimal = getsld(2)
        Dim sld3 As Decimal = getsld(3)
        Dim sld4 As Decimal = getsld(4)
        Dim sld5 As Decimal = getsld(5)
        Dim sldCapital = getsld(9)
        Return (sld1 + sld2 + sld3 + sld4 + sld5) - sldCapital
    End Function
    Public Shared Sub DeleteTranslation(Mtn As Decimal, Frais As Decimal, datex As DateTime, idr As Integer, TypeOfTrans As String, ServID As Integer)
        Try

            Dim para1 As String
            'Para1
            Dim dt As DataTable = GetSomeValuesdmc("SELECT * FROM services", ServID, "ID")
            para1 = dt.Rows(0)(4)
            'Compare bettwen para1 and typ of service
            If IsDBNull(para1) = True Or para1 = "" Then
                Dim FCompte As Integer = dt.Rows(0)(5)
                'To = para3
                Dim TCompte As Integer = dt.Rows(0)(6)
                deleteTransactionoperation(FCompte, TCompte, Mtn, Frais, idr, datex)
            Else
                If para1 = TypeOfTrans Then
                    'From = para2
                    Dim FCompte As Integer = dt.Rows(0)(5)
                    'To = para3
                    Dim TCompte As Integer = dt.Rows(0)(6)

                    deleteTransactionoperation(FCompte, TCompte, Mtn, Frais, idr, datex)
                Else
                    'From = para3
                    Dim FCompte As Integer = dt.Rows(0)(6)
                    'To = para2
                    Dim TCompte As Integer = dt.Rows(0)(5)
                    deleteTransactionoperation(FCompte, TCompte, Mtn, Frais, idr, datex)
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub
    Public Shared Sub deleteTransactionoperation(FCompte As Integer, TCompte As Integer, Mtn As Decimal, Frais As Decimal, IDr As Integer, datex As DateTime)
        Dim Fsld As Decimal = getsld(FCompte)
        '' Calcule
        Dim stringa As String = "UPDATE `security_db`.`hisabat` SET Solde = @Mant WHERE ID = @FCompte;"
        Dim prm(1) As String
        prm(0) = "@Mant"
        prm(1) = "@FCompte"
        Dim ids(1) As Object
        ids(0) = (Fsld + Mtn)
        ids(1) = FCompte
        EXCUTESomeQueriesSq(stringa, prm, ids)

        Dim Tsld As Decimal = getsld(TCompte)

        Dim stringa1 As String = "UPDATE `security_db`.`hisabat` SET Solde = @Mant WHERE ID = @FCompte;"
        Dim prm1(1) As String
        prm1(0) = "@Mant"
        prm1(1) = "@FCompte"
        Dim ids1(1) As Object
        ids1(0) = Tsld - Mtn
        ids1(1) = TCompte
        EXCUTESomeQueriesSq(stringa1, prm1, ids1)
        Dim Csld As Decimal = getsld(5)

        Dim stringa2 As String = "UPDATE `security_db`.`hisabat` SET Solde = @Mant WHERE ID = @FCompte;"
        Dim prm2(1) As String
        prm2(0) = "@Mant"
        prm2(1) = "@FCompte"
        Dim ids2(1) As Object
        ids2(0) = Csld - Frais
        ids2(1) = 5
        EXCUTESomeQueriesSq(stringa2, prm2, ids2)
        Fsld = getsld(FCompte)

        Dim stringa3 As String = "UPDATE `security_db`.`hisabat` SET Solde = @Mant WHERE ID = @FCompte;"
        Dim prm3(1) As String
        prm3(0) = "@Mant"
        prm3(1) = "@FCompte"
        Dim ids3(1) As Object
        ids3(0) = Fsld + Frais
        ids3(1) = FCompte

        EXCUTESomeQueriesSq(stringa3, prm3, ids3)
        ''Enregistement `damanecash`.`tansactionscomptes`

        Dim stringa4 As String = "DELETE FROM `damanecash`.`TransactionsServices` WHERE ID = @IDr;"
        Dim prm4(1) As String
        'prm4(0) = "@Mant"
        prm4(1) = "@IDr"
        Dim ids4(1) As Object
        'ids4(0) = Fsld + Frais
        ids4(1) = IDr
        EXCUTESomeQueriesSq(stringa4, prm4, ids4)

        Dim stringa5 As String = $"INSERT INTO `damanecash`.`tansactionscomptes` (`FROM`,`TO`,`Mtn`,`Desc`,`datex`) VALUES (@FCompte, @TCompte, @Mtn, 'Translaction Delete with frais = {Frais}', @Datex); "
        Dim prm5(3) As String
        prm5(0) = "@FCompte"
        prm5(1) = "@TCompte"
        prm5(2) = "@Mtn"
        prm5(3) = "@Datex"
        Dim ids5(3) As Object
        ids5(0) = FCompte
        ids5(1) = TCompte
        ids5(2) = Mtn
        ids5(3) = datex

        EXCUTESomeQueriesSq(stringa5, prm5, ids5)
    End Sub
End Class

