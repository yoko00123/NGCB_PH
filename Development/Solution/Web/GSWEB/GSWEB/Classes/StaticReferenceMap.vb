Imports System.Security.Cryptography
Imports System.Web
Imports System.Text

Public NotInheritable Class StaticReferenceMap
    Private Sub New()
    End Sub
    Public Const KeySize As Integer = 128
    'bits
    Public Const IvSize As Integer = 16
    'bytes
    Public Const OutputByteSize As Integer = KeySize / 8
    Private Shared ReadOnly Key As Byte()

    Shared Sub New()
        'pull 128 bit key in
        Key = {42, 1, 52, 67, 231, 13, 94, 101, 123, 6, 0, 12, 32, 91, 4, 111, 31, 70, 21, 141, 123, 142, 234, 82, 95, 129, 187, 162, 12, 55, 98, 23}
    End Sub


    Public Shared Function GetIndirectReferenceMap(value As String, salt As Byte()) As String
        'encode using UT8
        Dim directReferenceByteArray = Encoding.UTF8.GetBytes(value)

        'Encrypt and return URL safe Token string which is the indirect reference value
        Dim urlSafeToken = EncryptDirectReferenceValue(directReferenceByteArray, salt)
        Return urlSafeToken
    End Function

    Public Shared Function GetDirectReferenceMap(indirectReference As String) As String
        Dim indirectReferenceByteArray = HttpServerUtility.UrlTokenDecode(indirectReference)
        Return DecryptIndirectReferenceValue(indirectReferenceByteArray)
    End Function

    Private Shared Function EncryptDirectReferenceValue(directReferenceByteArray As Byte(), salt As Byte()) As String
        'IV needs to be a 16 byte cryptographic stength random value
        Dim iv = salt 'GetRandomValue()

        'We will store both the encrypted value and the IV used - IV is not a secret
        Dim indirectReferenceByteArray = New Byte(OutputByteSize + (IvSize - 1)) {}
        Using algorithm As SymmetricAlgorithm = GetAlgorithm()
            Dim encryptedByteArray = GetEncrptedByteArray(algorithm, iv, directReferenceByteArray)

            Buffer.BlockCopy(encryptedByteArray, 0, indirectReferenceByteArray, 0, OutputByteSize)
            Buffer.BlockCopy(iv, 0, indirectReferenceByteArray, OutputByteSize, IvSize)
        End Using
        Return HttpServerUtility.UrlTokenEncode(indirectReferenceByteArray)
    End Function

    Private Shared Function DecryptIndirectReferenceValue(indirectReferenceByteArray As Byte()) As String
        Dim decryptedByteArray As Byte()
        Using algorithm As SymmetricAlgorithm = GetAlgorithm()
            Dim encryptedByteArray = New Byte(OutputByteSize - 1) {}
            Dim iv = New Byte(IvSize - 1) {}

            'separate off the actual encrypted value and the IV from the byte array
            Buffer.BlockCopy(indirectReferenceByteArray, 0, encryptedByteArray, 0, OutputByteSize)

            Buffer.BlockCopy(indirectReferenceByteArray, encryptedByteArray.Length, iv, 0, IvSize)

            'decrypt the byte array using the IV that was stored with the value
            decryptedByteArray = GetDecryptedByteArray(algorithm, iv, encryptedByteArray)
        End Using
        'decode the UTF8 encoded byte array
        Return Encoding.UTF8.GetString(decryptedByteArray)
    End Function

    Private Shared Function GetDecryptedByteArray(algorithm As SymmetricAlgorithm, iv As Byte(), valueToBeDecrypted As Byte()) As Byte()
        Dim decryptor = algorithm.CreateDecryptor(Key, iv)
        Return decryptor.TransformFinalBlock(valueToBeDecrypted, 0, valueToBeDecrypted.Length)
    End Function

    Private Shared Function GetEncrptedByteArray(algorithm As SymmetricAlgorithm, iv As Byte(), valueToBeEncrypted As Byte()) As Byte()
        Dim encryptor = algorithm.CreateEncryptor(Key, iv)
        Return encryptor.TransformFinalBlock(valueToBeEncrypted, 0, valueToBeEncrypted.Length)
    End Function

    Private Shared Function GetAlgorithm() As AesManaged
        Dim aesManaged = New AesManaged() With { _
            .KeySize = KeySize, _
            .Mode = CipherMode.CBC, _
            .Padding = PaddingMode.PKCS7 _
        }
        Return aesManaged
    End Function

    Private Shared Function GetRandomValue() As Byte()
        Dim csprng = New RNGCryptoServiceProvider()
        Dim buffer = New [Byte](15) {}

        'generate the random indirect value
        csprng.GetBytes(buffer)
        Return buffer
    End Function

    Public Shared Function GetRandomSaltValue(UserID As Integer) As Byte()
        ' Dim csprng = New System.Security.Cryptography.RNGCryptoServiceProvider()
        Dim buffer = New [Byte](15) {}
        ''PAG NASA PROD. WAG NANG PALITAN
        Dim arr As Integer() = {24, 28, 123, 245, 124, 9, 100, 56, 198, 133, 78, 96, 42, 75, 32, 65}
        For x As Int64 = 0 To 15
            buffer(x) = CByte(((UserID + arr(x)) * 552) Mod 255)
        Next
        'For i As Integer = 0 To 15
        '    buffer(i) = CByte(i)
        'Next
        'generate the random indirect value
        '  csprng.GetBytes(buffer)
        'buffer = Encoding.ASCII.GetBytes(generateRand(15))
        Return buffer
    End Function
End Class