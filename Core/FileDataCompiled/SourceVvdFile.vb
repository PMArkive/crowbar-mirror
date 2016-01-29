Imports System.IO
Imports System.Text

Public Class SourceVvdFile

#Region "Methods"

	Public Sub ReadFile(ByVal inputPathFileName As String, ByVal aSourceEngineModel As SourceModel)
		'Dim inputPathFileName As String

		'inputPathFileName = Path.ChangeExtension(pathFileName, ".vvd")
		'If Not File.Exists(inputPathFileName) Then
		'	Return
		'End If

		Dim inputFileStream As FileStream = Nothing
		Me.theInputFileReader = Nothing
		Try
			inputFileStream = New FileStream(inputPathFileName, FileMode.Open)
			If inputFileStream IsNot Nothing Then
				Try
					Me.theInputFileReader = New BinaryReader(inputFileStream)

					Me.theSourceEngineModel = aSourceEngineModel

					Me.ReadSourceVvdHeader()
					'If Me.theSourceEngineModel.theVtxFileHeader.lodCount > 0 Then
					'	Me.ReadSourceVtxBodyParts()
					'End If
				Catch
				Finally
					If Me.theInputFileReader IsNot Nothing Then
						Me.theInputFileReader.Close()
					End If
				End Try
			End If
		Catch
		Finally
			If inputFileStream IsNot Nothing Then
				inputFileStream.Close()
			End If
		End Try
	End Sub

#End Region

#Region "Private Methods"

	Private Sub ReadSourceVvdHeader()
		Me.theSourceEngineModel.theVvdFileHeader = New SourceVvdFileHeader()

		Me.theSourceEngineModel.theVvdFileHeader.id = Me.theInputFileReader.ReadChars(4)
		Me.theSourceEngineModel.theVvdFileHeader.version = Me.theInputFileReader.ReadInt32()
		Me.theSourceEngineModel.theVvdFileHeader.checksum = Me.theInputFileReader.ReadInt32()
		Me.theSourceEngineModel.theVvdFileHeader.lodCount = Me.theInputFileReader.ReadInt32()
		For i As Integer = 0 To MAX_NUM_LODS - 1
			Me.theSourceEngineModel.theVvdFileHeader.lodVertexCount(i) = Me.theInputFileReader.ReadInt32()
		Next
		Me.theSourceEngineModel.theVvdFileHeader.fixupCount = Me.theInputFileReader.ReadInt32()
		Me.theSourceEngineModel.theVvdFileHeader.fixupTableOffset = Me.theInputFileReader.ReadInt32()
		Me.theSourceEngineModel.theVvdFileHeader.vertexDataOffset = Me.theInputFileReader.ReadInt32()
		Me.theSourceEngineModel.theVvdFileHeader.tangentDataOffset = Me.theInputFileReader.ReadInt32()

		If Me.theSourceEngineModel.theVvdFileHeader.lodCount > 0 Then
			Me.theInputFileReader.BaseStream.Seek(Me.theSourceEngineModel.theVvdFileHeader.vertexDataOffset, SeekOrigin.Begin)

			Me.ReadVertexes()
		End If
		If Me.theSourceEngineModel.theVvdFileHeader.fixupCount > 0 Then
			Me.theInputFileReader.BaseStream.Seek(Me.theSourceEngineModel.theVvdFileHeader.fixupTableOffset, SeekOrigin.Begin)

			Me.theSourceEngineModel.theVvdFileHeader.theFixups = New List(Of SourceVvdFixup)(Me.theSourceEngineModel.theVvdFileHeader.fixupCount)
			For fixupIndex As Integer = 0 To Me.theSourceEngineModel.theVvdFileHeader.fixupCount - 1
				Dim aFixup As New SourceVvdFixup()

				aFixup.lodIndex = Me.theInputFileReader.ReadInt32()
				aFixup.vertexIndex = Me.theInputFileReader.ReadInt32()
				aFixup.vertexCount = Me.theInputFileReader.ReadInt32()
				Me.theSourceEngineModel.theVvdFileHeader.theFixups.Add(aFixup)
			Next
			If Me.theSourceEngineModel.theVvdFileHeader.lodCount > 0 Then
				Me.theInputFileReader.BaseStream.Seek(Me.theSourceEngineModel.theVvdFileHeader.vertexDataOffset, SeekOrigin.Begin)

				For lodIndex As Integer = 0 To Me.theSourceEngineModel.theVvdFileHeader.lodCount - 1
					Me.SetupFixedVertexes(lodIndex)
				Next
				Dim i As Integer = 0
			End If
		End If
	End Sub

	Private Sub ReadVertexes()
		'Dim boneWeightingIsIncorrect As Boolean
		Dim weight As Single
		Dim boneIndex As Byte

		Dim vertexCount As Integer
		vertexCount = Me.theSourceEngineModel.theVvdFileHeader.lodVertexCount(0)
		Me.theSourceEngineModel.theVvdFileHeader.theVertexes = New List(Of SourceVertex)(vertexCount)
		For j As Integer = 0 To vertexCount - 1
			Dim aStudioVertex As New SourceVertex()

			Dim boneWeight As New SourceBoneWeight()
			'boneWeightingIsIncorrect = False
			For x As Integer = 0 To MAX_NUM_BONES_PER_VERT - 1
				weight = Me.theInputFileReader.ReadSingle()
				boneWeight.weight(x) = weight
				'If weight > 1 Then
				'	boneWeightingIsIncorrect = True
				'End If
			Next
			For x As Integer = 0 To MAX_NUM_BONES_PER_VERT - 1
				boneIndex = Me.theInputFileReader.ReadByte()
				boneWeight.bone(x) = boneIndex
				'If boneIndex > 127 Then
				'	boneWeightingIsIncorrect = True
				'End If
			Next
			boneWeight.boneCount = Me.theInputFileReader.ReadByte()
			''TODO: ReadVertexes() -- boneWeight.boneCount > MAX_NUM_BONES_PER_VERT, which seems like incorrect vvd format 
			'If boneWeight.boneCount > MAX_NUM_BONES_PER_VERT Then
			'	boneWeight.boneCount = CByte(MAX_NUM_BONES_PER_VERT)
			'End If
			'If boneWeightingIsIncorrect Then
			'	boneWeight.boneCount = 0
			'End If
			aStudioVertex.boneWeight = boneWeight

			aStudioVertex.positionX = Me.theInputFileReader.ReadSingle()
			aStudioVertex.positionY = Me.theInputFileReader.ReadSingle()
			aStudioVertex.positionZ = Me.theInputFileReader.ReadSingle()
			aStudioVertex.normalX = Me.theInputFileReader.ReadSingle()
			aStudioVertex.normalY = Me.theInputFileReader.ReadSingle()
			aStudioVertex.normalZ = Me.theInputFileReader.ReadSingle()
			aStudioVertex.texCoordX = Me.theInputFileReader.ReadSingle()
			aStudioVertex.texCoordY = Me.theInputFileReader.ReadSingle()
			Me.theSourceEngineModel.theVvdFileHeader.theVertexes.Add(aStudioVertex)
		Next
	End Sub

	Private Sub SetupFixedVertexes(ByVal lodIndex As Integer)
		Dim aFixup As SourceVvdFixup
		Dim aStudioVertex As SourceVertex

		Me.theSourceEngineModel.theVvdFileHeader.theFixedVertexesByLod(lodIndex) = New List(Of SourceVertex)
		For fixupIndex As Integer = 0 To Me.theSourceEngineModel.theVvdFileHeader.theFixups.Count - 1
			aFixup = Me.theSourceEngineModel.theVvdFileHeader.theFixups(fixupIndex)

			If aFixup.lodIndex >= lodIndex Then
				For j As Integer = 0 To aFixup.vertexCount - 1
					aStudioVertex = Me.theSourceEngineModel.theVvdFileHeader.theVertexes(aFixup.vertexIndex + j)
					Me.theSourceEngineModel.theVvdFileHeader.theFixedVertexesByLod(lodIndex).Add(aStudioVertex)
				Next
			End If
		Next
	End Sub

#End Region

#Region "Data"

	Private theSourceEngineModel As SourceModel
	Private theInputFileReader As BinaryReader

#End Region

End Class
