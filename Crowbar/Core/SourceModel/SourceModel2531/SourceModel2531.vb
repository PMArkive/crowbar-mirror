﻿Imports System.IO

Public Class SourceModel2531
	Inherits SourceModel10

#Region "Creation and Destruction"

	Public Sub New(ByVal mdlPathFileName As String)
		MyBase.New(mdlPathFileName)
	End Sub

#End Region

#Region "Properties"

	Public Overrides ReadOnly Property PhyFileIsUsed As Boolean
		Get
			Return True
		End Get
	End Property

	Public Overrides ReadOnly Property VtxFileIsUsed As Boolean
		Get
			Return True
		End Get
	End Property

	Public Overrides ReadOnly Property SequenceGroupMdlFilesAreUsed As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overrides ReadOnly Property TextureMdlFileIsUsed As Boolean
		Get
			Return False
		End Get
	End Property

	Public Overrides ReadOnly Property HasTextureData As Boolean
		Get
			Return Not Me.theMdlFileDataGeneric.theMdlFileOnlyHasAnimations AndAlso Me.theMdlFileData.theTextures IsNot Nothing AndAlso Me.theMdlFileData.theTextures.Count > 0
		End Get
	End Property

	Public Overrides ReadOnly Property HasMeshData As Boolean
		Get
			'TODO: [HasMeshData] Should check more than theBones.
			If Not Me.theMdlFileDataGeneric.theMdlFileOnlyHasAnimations _
					 AndAlso Me.theMdlFileData.theBones IsNot Nothing _
					 AndAlso Me.theMdlFileData.theBones.Count > 0 _
					 AndAlso Me.theVtxFileData IsNot Nothing Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasLodMeshData As Boolean
		Get
			If Not Me.theMdlFileData.theMdlFileOnlyHasAnimations _
					 AndAlso Me.theMdlFileData.theBones IsNot Nothing _
					 AndAlso Me.theMdlFileData.theBones.Count > 0 _
					 AndAlso Me.theVtxFileData IsNot Nothing _
					 AndAlso Me.theVtxFileData.lodCount > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasPhysicsMeshData As Boolean
		Get
			If Me.thePhyFileData IsNot Nothing _
			 AndAlso Me.thePhyFileData.theSourcePhyCollisionDatas IsNot Nothing _
			 AndAlso Not Me.theMdlFileData.theMdlFileOnlyHasAnimations _
			 AndAlso Me.theMdlFileData.theBones IsNot Nothing _
			 AndAlso Me.theMdlFileData.theBones.Count > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasBoneAnimationData As Boolean
		Get
			If Me.theMdlFileData.theSequences IsNot Nothing _
			 AndAlso Me.theMdlFileData.theSequences.Count > 0 _
			 AndAlso Me.theMdlFileData.theAnimationDescs IsNot Nothing _
			 AndAlso Me.theMdlFileData.theAnimationDescs.Count > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasVertexAnimationData As Boolean
		Get
			If Not Me.theMdlFileData.theMdlFileOnlyHasAnimations _
			 AndAlso Me.theMdlFileData.theFlexDescs IsNot Nothing _
			 AndAlso Me.theMdlFileData.theFlexDescs.Count > 0 Then
				Return True
			Else
				Return False
			End If
		End Get
	End Property

	Public Overrides ReadOnly Property HasTextureFileData As Boolean
		Get
			Return False
		End Get
	End Property

#End Region

#Region "Methods"

	Public Overrides Function CheckForRequiredFiles() As StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If Not Me.theMdlFileDataGeneric.theMdlFileOnlyHasAnimations Then
			Me.thePhyPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".phy")

			Me.theVtxPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".dx80.vtx")
			If Not File.Exists(Me.theVtxPathFileName) Then
				Me.theVtxPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".dx7_2bone.vtx")
				If Not File.Exists(Me.theVtxPathFileName) Then
					Me.theVtxPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".vtx")
					If Not File.Exists(Me.theVtxPathFileName) Then
						status = StatusMessage.ErrorRequiredVtxFileNotFound
					End If
				End If
			End If
		End If

		Return status
	End Function

	Public Overrides Function ReadPhyFile(ByVal mdlPathFileName As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If String.IsNullOrEmpty(Me.thePhyPathFileName) Then
			status = Me.CheckForRequiredFiles()
		End If

		If status = StatusMessage.Success Then
			Try
				Me.ReadFile(Me.thePhyPathFileName, AddressOf Me.ReadPhyFile)
				If Me.thePhyFileData.checksum <> Me.theMdlFileData.checksum Then
					'status = StatusMessage.WarningPhyChecksumDoesNotMatchMdl
					Me.NotifySourceModelProgress(ProgressOptions.WarningPhyFileChecksumDoesNotMatchMdlFileChecksum, "")
				End If
			Catch ex As Exception
				status = StatusMessage.Error
			End Try
		End If

		Return status
	End Function

	Public Overrides Function WritePhysicsMeshSmdFile(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim physicsMeshPathFileName As String
		physicsMeshPathFileName = Path.Combine(modelOutputPath, SourceFileNamesModule.GetPhysicsSmdFileName(Me.theName))
		Me.WriteTextFile(physicsMeshPathFileName, AddressOf Me.WritePhysicsMeshSmdFile)

		Return status
	End Function

	Public Overrides Function WriteReferenceMeshFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		status = Me.WriteMeshSmdFiles(modelOutputPath, 0, 0)

		Return status
	End Function

	Public Overrides Function WriteLodMeshFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		status = Me.WriteMeshSmdFiles(modelOutputPath, 1, Me.theVtxFileData.lodCount - 1)

		Return status
	End Function

	Public Overloads Function WriteMeshSmdFile(ByVal smdPathFileName As String, ByVal lodIndex As Integer, ByVal aVtxModel As SourceVtxModel107, ByVal aModel As SourceMdlModel2531, ByVal bodyPartVertexIndexStart As Integer) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Try
			Me.theOutputFileTextWriter = File.CreateText(smdPathFileName)
			Dim smdFile As New SourceSmdFile2531(Me.theOutputFileTextWriter, Me.theMdlFileData)

			smdFile.WriteHeaderComment()

			smdFile.WriteHeaderSection()
			smdFile.WriteNodesSection()
			smdFile.WriteSkeletonSection()
			smdFile.WriteTrianglesSection(lodIndex, aVtxModel, aModel, bodyPartVertexIndexStart)
		Catch ex As Exception
			Dim debug As Integer = 4242
		Finally
			If Me.theOutputFileTextWriter IsNot Nothing Then
				Me.theOutputFileTextWriter.Flush()
				Me.theOutputFileTextWriter.Close()
			End If
		End Try

		Return status
	End Function

	Public Overrides Function WriteBoneAnimationSmdFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim anAnimationDesc As SourceMdlAnimationDesc2531
		Dim smdPath As String
		Dim smdFileName As String
		Dim smdPathFileName As String

		For anAnimDescIndex As Integer = 0 To Me.theMdlFileData.theAnimationDescs.Count - 1
			Try
				anAnimationDesc = Me.theMdlFileData.theAnimationDescs(anAnimDescIndex)

				smdFileName = SourceFileNamesModule.GetAnimationSmdRelativePathFileName(Me.Name, anAnimationDesc.theName)
				smdPathFileName = Path.Combine(modelOutputPath, smdFileName)
				smdPath = FileManager.GetPath(smdPathFileName)
				Me.NotifySourceModelProgress(ProgressOptions.WritingSmdFileStarted, smdPathFileName)
				'NOTE: Check here in case writing is canceled in the above event.
				If Me.theWritingIsCanceled Then
					status = StatusMessage.Canceled
					Return status
				ElseIf Me.theWritingSingleFileIsCanceled Then
					Me.theWritingSingleFileIsCanceled = False
					Continue For
				End If

				Me.WriteBoneAnimationSmdFile(smdPathFileName, Nothing, anAnimationDesc)

				Me.NotifySourceModelProgress(ProgressOptions.WritingSmdFileFinished, smdPathFileName)
			Catch ex As Exception
				Dim debug As Integer = 4242
			End Try
		Next

		Return status
	End Function

	Public Overrides Function WriteVertexAnimationVtaFile(ByVal vtaPathFileName As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Me.WriteTextFile(vtaPathFileName, AddressOf Me.WriteVertexAnimationVtaFile)

		Return status
	End Function

	Public Overrides Function WriteAccessedBytesDebugFiles(ByVal debugPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim debugPathFileName As String

		If Me.theMdlFileData IsNot Nothing Then
			debugPathFileName = Path.Combine(debugPath, Me.theName + " " + My.Resources.Decompile_DebugMdlFileNameSuffix)
			Me.WriteAccessedBytesDebugFile(debugPathFileName, Me.theMdlFileData.theFileSeekLog)
		End If

		If Me.theVtxFileData IsNot Nothing Then
			debugPathFileName = Path.Combine(debugPath, Me.theName + " " + My.Resources.Decompile_DebugVtxFileNameSuffix)
			Me.WriteAccessedBytesDebugFile(debugPathFileName, Me.theVtxFileData.theFileSeekLog)
		End If

		Return status
	End Function

	Public Overrides Function GetTextureFolders() As List(Of String)
		Dim textureFolders As New List(Of String)()

		For i As Integer = 0 To Me.theMdlFileData.theTexturePaths.Count - 1
			Dim aTextureFolder As String
			aTextureFolder = Me.theMdlFileData.theTexturePaths(i)

			textureFolders.Add(aTextureFolder)
		Next

		Return textureFolders
	End Function

	Public Overrides Function GetTextureFileNames() As List(Of String)
		Dim textureFileNames As New List(Of String)()

		For i As Integer = 0 To Me.theMdlFileData.theTextures.Count - 1
			Dim aTexture As SourceMdlTexture2531
			aTexture = Me.theMdlFileData.theTextures(i)

			textureFileNames.Add(aTexture.theFileName)
		Next

		Return textureFileNames
	End Function

#End Region

#Region "Private Methods"

	Protected Overrides Sub ReadMdlFileHeader()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData2531()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim mdlFile As New SourceMdlFile2531(Me.theInputFileReader, Me.theMdlFileData)

		mdlFile.ReadMdlHeader()

		'If Me.theMdlFileData.fileSize <> Me.theMdlFileData.theActualFileSize Then
		'	status = StatusMessage.ErrorInvalidInternalMdlFileSize
		'End If
	End Sub

	Protected Overrides Sub ReadMdlFileForViewer()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData2531()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim mdlFile As New SourceMdlFile2531(Me.theInputFileReader, Me.theMdlFileData)

		mdlFile.ReadMdlHeader()

		mdlFile.ReadTexturePaths()
		mdlFile.ReadTextures()
	End Sub

	Protected Overrides Sub ReadMdlFile()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData2531()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim mdlFile As New SourceMdlFile2531(Me.theInputFileReader, Me.theMdlFileData)

		mdlFile.ReadMdlHeader()

		mdlFile.ReadBones()
		mdlFile.ReadBoneControllers()
		mdlFile.ReadAttachments()
		mdlFile.ReadHitboxSets()
		mdlFile.ReadSurfaceProp()

		mdlFile.ReadSequenceGroups()
		'NOTE: Must read sequences before reading animations.
		mdlFile.ReadSequences()
		'mdlFile.ReadTransitions()

		mdlFile.ReadLocalAnimationDescs()

		'NOTE: Read flex descs before body parts so that flexes (within body parts) can add info to flex descs.
		mdlFile.ReadFlexDescs()
		mdlFile.ReadBodyParts()
		mdlFile.ReadFlexControllers()
		'NOTE: This must be after flex descs are read so that flex desc usage can be saved in flex desc.
		mdlFile.ReadFlexRules()
		mdlFile.ReadPoseParamDescs()

		mdlFile.ReadTextures()
		mdlFile.ReadTexturePaths()
		mdlFile.ReadSkins()

		mdlFile.ReadIncludeModels()

		' Post-processing.
		mdlFile.CreateFlexFrameList()
	End Sub

	Protected Overrides Sub ReadPhyFile()
		If Me.thePhyFileData Is Nothing Then
			Me.thePhyFileData = New SourcePhyFileData2531()
		End If

		Dim phyFile As New SourcePhyFile2531(Me.theInputFileReader, Me.thePhyFileData)

		phyFile.ReadSourcePhyHeader()
		If Me.thePhyFileData.solidCount > 0 Then
			phyFile.ReadSourceCollisionData()
			phyFile.CalculateVertexNormals()
			phyFile.ReadSourcePhysCollisionModels()
			phyFile.ReadSourcePhyRagdollConstraintDescs()
			phyFile.ReadSourcePhyCollisionRules()
			phyFile.ReadSourcePhyEditParamsSection()
			phyFile.ReadCollisionTextSection()
		End If
	End Sub

	Protected Overrides Sub ReadVtxFile()
		If Me.theVtxFileData Is Nothing Then
			Me.theVtxFileData = New SourceVtxFileData107()
		End If

		Dim vtxFile As New SourceVtxFile107(Me.theInputFileReader, Me.theVtxFileData)

		vtxFile.ReadSourceVtxHeader()
		vtxFile.ReadSourceVtxBodyParts()
	End Sub

	Protected Overrides Sub WriteQcFile()
		Dim qcFile As New SourceQcFile2531(Me.theOutputFileTextWriter, Me.theQcPathFileName, Me.theMdlFileData, Me.theVtxFileData, Me.thePhyFileData, Me.theName)

		Try
			qcFile.WriteHeaderComment()

			qcFile.WriteModelNameCommand()

			qcFile.WriteBodyGroupCommand()
			qcFile.WriteLodCommand()

			qcFile.WriteStaticPropCommand()
			'qcFile.WriteFlagsCommand()
			qcFile.WriteEyePositionCommand()
			qcFile.WriteSurfacePropCommand()

			qcFile.WriteCdMaterialsCommand()
			qcFile.WriteTextureGroupCommand()
			'If TheApp.Settings.DecompileDebugInfoFilesIsChecked Then
			'	qcFile.WriteTextureFileNameComments()
			'End If

			qcFile.WriteAttachmentCommand()

			qcFile.WriteCBoxCommand()
			qcFile.WriteBBoxCommand()
			qcFile.WriteHBoxRelatedCommands()

			qcFile.WriteControllerCommand()

			qcFile.WriteSequenceGroupCommands()
			qcFile.WriteSequenceCommands()
			qcFile.WriteIncludeModelCommands()
		Catch ex As Exception
			Dim debug As Integer = 4242
		Finally
		End Try
	End Sub

	Protected Overrides Sub WritePhysicsMeshSmdFile()
		Dim physicsSmdFile As New SourceSmdFile2531(Me.theOutputFileTextWriter, Me.theMdlFileData, Me.thePhyFileData)

		Try
			physicsSmdFile.WriteHeaderComment()

			physicsSmdFile.WriteHeaderSection()
			physicsSmdFile.WriteNodesSection()
			physicsSmdFile.WriteSkeletonSection()
			physicsSmdFile.WriteTrianglesSectionForPhysics()
		Catch ex As Exception
			Dim debug As Integer = 4242
		Finally
		End Try
	End Sub

	Protected Overridable Function WriteMeshSmdFiles(ByVal modelOutputPath As String, ByVal lodStartIndex As Integer, ByVal lodStopIndex As Integer) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim smdFileName As String
		Dim smdPathFileName As String
		Dim aBodyPart As SourceVtxBodyPart107
		Dim aVtxModel As SourceVtxModel107
		Dim aModel As SourceMdlModel2531
		Dim bodyPartVertexIndexStart As Integer

		bodyPartVertexIndexStart = 0
		If Me.theVtxFileData.theVtxBodyParts IsNot Nothing AndAlso Me.theMdlFileData.theBodyParts IsNot Nothing Then
			For bodyPartIndex As Integer = 0 To Me.theVtxFileData.theVtxBodyParts.Count - 1
				aBodyPart = Me.theVtxFileData.theVtxBodyParts(bodyPartIndex)

				If aBodyPart.theVtxModels IsNot Nothing Then
					For modelIndex As Integer = 0 To aBodyPart.theVtxModels.Count - 1
						aVtxModel = aBodyPart.theVtxModels(modelIndex)

						If aVtxModel.theVtxModelLods IsNot Nothing Then
							aModel = Me.theMdlFileData.theBodyParts(bodyPartIndex).theModels(modelIndex)
							If aModel.name(0) = ChrW(0) AndAlso aVtxModel.theVtxModelLods(0).theVtxMeshes Is Nothing Then
								Continue For
							End If

							For lodIndex As Integer = lodStartIndex To lodStopIndex
								'TODO: Why would this count be different than the file header count?
								If lodIndex >= aVtxModel.theVtxModelLods.Count Then
									Exit For
								End If

								Try
									smdFileName = SourceModule2531.GetBodyGroupSmdFileName(bodyPartIndex, modelIndex, lodIndex, Me.theMdlFileData.theModelCommandIsUsed, Me.theName, Me.theMdlFileData.theBodyParts(bodyPartIndex).theModels(modelIndex).name, Me.theMdlFileData.theBodyParts.Count, Me.theMdlFileData.theBodyParts(bodyPartIndex).theModels.Count, Me.theMdlFileData.theSequenceGroups(0).theFileName)
									smdPathFileName = Path.Combine(modelOutputPath, smdFileName)

									Me.NotifySourceModelProgress(ProgressOptions.WritingSmdFileStarted, smdPathFileName)
									'NOTE: Check here in case writing is canceled in the above event.
									If Me.theWritingIsCanceled Then
										status = StatusMessage.Canceled
										Return status
									ElseIf Me.theWritingSingleFileIsCanceled Then
										Me.theWritingSingleFileIsCanceled = False
										Continue For
									End If

									Me.WriteMeshSmdFile(smdPathFileName, lodIndex, aVtxModel, aModel, bodyPartVertexIndexStart)

									Me.NotifySourceModelProgress(ProgressOptions.WritingSmdFileFinished, smdPathFileName)
								Catch ex As Exception
									Dim debug As Integer = 4242
								End Try
							Next

							bodyPartVertexIndexStart += aModel.vertexCount
						End If
					Next
				End If
			Next
		End If

		Return status
	End Function

	Protected Overrides Sub WriteBoneAnimationSmdFile(ByVal aSequenceDesc As SourceMdlSequenceDescBase, ByVal anAnimationDesc As SourceMdlAnimationDescBase)
		Dim smdFile As New SourceSmdFile2531(Me.theOutputFileTextWriter, Me.theMdlFileData)

		Try
			smdFile.WriteHeaderComment()

			smdFile.WriteHeaderSection()
			smdFile.WriteNodesSection()
			smdFile.WriteSkeletonSectionForAnimation(aSequenceDesc, anAnimationDesc)
		Catch ex As Exception
			Dim debug As Integer = 4242
		End Try
	End Sub

	Protected Overrides Sub WriteVertexAnimationVtaFile()
		Dim vertexAnimationVtaFile As New SourceVtaFile2531(Me.theOutputFileTextWriter, Me.theMdlFileData)

		Try
			vertexAnimationVtaFile.WriteHeaderComment()

			vertexAnimationVtaFile.WriteHeaderSection()
			vertexAnimationVtaFile.WriteNodesSection()
			vertexAnimationVtaFile.WriteSkeletonSectionForVertexAnimation()
			vertexAnimationVtaFile.WriteVertexAnimationSection()
		Catch ex As Exception
			Dim debug As Integer = 4242
		Finally
		End Try
	End Sub

	Protected Overrides Sub WriteMdlFileNameToMdlFile(ByVal internalMdlFileName As String)
		Dim mdlFile As New SourceMdlFile2531(Me.theOutputFileBinaryWriter, Me.theMdlFileData)

		mdlFile.WriteInternalMdlFileName(internalMdlFileName)
	End Sub

#End Region

#Region "Constants"

	''#define MAX_NUM_BONES_PER_VERT 4
	''#define MAX_NUM_BONES_PER_TRI ( MAX_NUM_BONES_PER_VERT * 3 )
	''#define MAX_NUM_BONES_PER_STRIP 16
	'Public Shared MAX_NUM_BONES_PER_VERT As Integer = 4
	'Public Shared MAX_NUM_BONES_PER_TRI As Integer = MAX_NUM_BONES_PER_VERT * 3
	'Public Shared MAX_NUM_BONES_PER_STRIP As Integer = 16
	'------
	'FROM: VAMPTools-master\MDLConverter\inc\external\studio.h
	'#define MAX_NUM_BONES_PER_VERT 3
	Public Shared MAX_NUM_BONES_PER_VERT As Integer = 3

#End Region

#Region "Data"

	Private theMdlFileData As SourceMdlFileData2531
	Private thePhyFileData As SourcePhyFileData2531
	Private theVtxFileData As SourceVtxFileData107

#End Region

End Class