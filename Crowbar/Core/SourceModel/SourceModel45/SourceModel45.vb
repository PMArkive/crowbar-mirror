﻿Imports System.IO

Public Class SourceModel45
	Inherits SourceModel44

#Region "Creation and Destruction"

	Public Sub New(ByVal mdlPathFileName As String, ByVal mdlVersion As Integer)
		MyBase.New(mdlPathFileName, mdlVersion)
	End Sub

#End Region

#Region "Properties"

	Public Overrides ReadOnly Property VtxFileIsUsed As Boolean
		Get
			Return Not String.IsNullOrEmpty(Me.theVtxPathFileName) AndAlso File.Exists(Me.theVtxPathFileName)
		End Get
	End Property

	Public Overrides ReadOnly Property AniFileIsUsed As Boolean
		Get
			Return Not String.IsNullOrEmpty(Me.theAniPathFileName) AndAlso File.Exists(Me.theAniPathFileName)
		End Get
	End Property

	Public Overrides ReadOnly Property VvdFileIsUsed As Boolean
		Get
			Return Not String.IsNullOrEmpty(Me.theVvdPathFileName) AndAlso File.Exists(Me.theVvdPathFileName)
		End Get
	End Property

	Public Overrides ReadOnly Property HasTextureData As Boolean
		Get
			Return Not Me.theMdlFileDataGeneric.theMdlFileOnlyHasAnimations AndAlso Me.theMdlFileData.theTextures IsNot Nothing AndAlso Me.theMdlFileData.theTextures.Count > 0
		End Get
	End Property

	Public Overrides ReadOnly Property HasMeshData As Boolean
		Get
			If Not Me.theMdlFileData.theMdlFileOnlyHasAnimations _
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

	Public Overrides ReadOnly Property HasProceduralBonesData As Boolean
		Get
			If Me.theMdlFileData IsNot Nothing _
			 AndAlso Me.theMdlFileData.theProceduralBonesCommandIsUsed _
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
			If Me.theMdlFileData.theAnimationDescs IsNot Nothing _
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

#End Region

#Region "Methods"

	Public Overrides Function CheckForRequiredFiles() As StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If Me.theMdlFileData.animBlockCount > 0 Then
			Me.theAniPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".ani")
			If Not File.Exists(Me.theAniPathFileName) Then
				status = StatusMessage.ErrorRequiredAniFileNotFound
			End If
		End If

		If Not Me.theMdlFileDataGeneric.theMdlFileOnlyHasAnimations Then
			Me.thePhyPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".phy")

			Me.theVtxPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".dx11.vtx")
			If Not File.Exists(Me.theVtxPathFileName) Then
				Me.theVtxPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".dx90.vtx")
				If Not File.Exists(Me.theVtxPathFileName) Then
					Me.theVtxPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".dx80.vtx")
					If Not File.Exists(Me.theVtxPathFileName) Then
						Me.theVtxPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".sw.vtx")
						If Not File.Exists(Me.theVtxPathFileName) Then
							Me.theVtxPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".vtx")
							If Not File.Exists(Me.theVtxPathFileName) Then
								status = StatusMessage.ErrorRequiredVtxFileNotFound
							End If
						End If
					End If
				End If
			End If

			Me.theVvdPathFileName = Path.ChangeExtension(Me.theMdlPathFileName, ".vvd")
			If Not File.Exists(Me.theVvdPathFileName) Then
				status = StatusMessage.ErrorRequiredVvdFileNotFound
			End If
		End If

		Return status
	End Function

	Public Overrides Function ReadPhyFile() As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		If String.IsNullOrEmpty(Me.thePhyPathFileName) Then
			status = Me.CheckForRequiredFiles()
		End If

		If Not String.IsNullOrEmpty(Me.thePhyPathFileName) Then
			If status = StatusMessage.Success Then
				Try
					Me.ReadFile(Me.thePhyPathFileName, AddressOf Me.ReadPhyFile_Internal)
					If Me.thePhyFileData.checksum <> Me.theMdlFileData.checksum Then
						'status = StatusMessage.WarningPhyChecksumDoesNotMatchMdl
						Me.NotifySourceModelProgress(ProgressOptions.WarningPhyFileChecksumDoesNotMatchMdlFileChecksum, "")
					End If
				Catch ex As Exception
					status = StatusMessage.Error
				End Try
			End If
		End If

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

	Public Overrides Function WriteBoneAnimationSmdFiles(ByVal modelOutputPath As String) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim anAnimationDesc As SourceMdlAnimationDesc45
		Dim smdPath As String
		Dim smdFileName As String
		Dim smdPathFileName As String

		Try
			For anAnimDescIndex As Integer = 0 To Me.theMdlFileData.theAnimationDescs.Count - 1
				anAnimationDesc = Me.theMdlFileData.theAnimationDescs(anAnimDescIndex)

				smdFileName = SourceFileNamesModule.GetAnimationSmdRelativePathFileName(Me.Name, anAnimationDesc.theName)
				smdPathFileName = Path.Combine(modelOutputPath, smdFileName)
				smdPath = FileManager.GetPath(smdPathFileName)
				If FileManager.OutputPathIsUsable(smdPath) Then
					Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, smdPathFileName)
					'NOTE: Check here in case writing is canceled in the above event.
					If Me.theWritingIsCanceled Then
						status = StatusMessage.Canceled
						Return status
					ElseIf Me.theWritingSingleFileIsCanceled Then
						Me.theWritingSingleFileIsCanceled = False
						Continue For
					End If

					Me.WriteBoneAnimationSmdFile(smdPathFileName, Nothing, anAnimationDesc)

					Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, smdPathFileName)
				End If
			Next
		Catch ex As Exception
			Dim debug As Integer = 4242
		End Try

		Return status
	End Function

	'Public Overrides Function WriteVrdFile(ByVal vrdPathFileName As String) As AppEnums.StatusMessage
	'	Dim status As AppEnums.StatusMessage = StatusMessage.Success

	'	Me.WriteTextFile(vrdPathFileName, AddressOf Me.WriteVrdFile)

	'	Return status
	'End Function

	'Public Overrides Function WriteAccessedBytesDebugFiles(ByVal debugPath As String) As AppEnums.StatusMessage
	'	Dim status As AppEnums.StatusMessage = StatusMessage.Success

	'	Dim debugPathFileName As String

	'	If Me.theMdlFileData48 IsNot Nothing Then
	'		debugPathFileName = Path.Combine(debugPath, Me.theName + " " + My.Resources.Decompile_DebugMdlFileNameSuffix)
	'		Me.WriteAccessedBytesDebugFile(debugPathFileName, Me.theMdlFileData48.theFileSeekLog)
	'	End If

	'	If Me.theAniFileData48 IsNot Nothing Then
	'		debugPathFileName = Path.Combine(debugPath, Me.theName + " " + My.Resources.Decompile_DebugAniFileNameSuffix)
	'		Me.WriteAccessedBytesDebugFile(debugPathFileName, Me.theAniFileData48.theFileSeekLog)
	'	End If

	'	Return status
	'End Function

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
			Dim aTexture As SourceMdlTexture
			aTexture = Me.theMdlFileData.theTextures(i)

			textureFileNames.Add(aTexture.thePathFileName)
		Next

		Return textureFileNames
	End Function

#End Region

#Region "Private Methods"

	Protected Overrides Sub ReadAniFile_Internal()
		If Me.theAniFileData Is Nothing Then
			Me.theAniFileData = New SourceAniFileData45()
			Me.theAniFileDataGeneric = Me.theAniFileData
		End If

		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData45()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim aniFile As New SourceAniFile45(Me.theInputFileReader, Me.theAniFileData, Me.theMdlFileData)

		aniFile.ReadMdlHeader00("ANI File Header 00")
		aniFile.ReadMdlHeader01("ANI File Header 01")

		aniFile.ReadAniBlocks()
	End Sub

	Protected Overrides Sub ReadMdlFile_Internal()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData45()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim mdlFile As New SourceMdlFile45(Me.theInputFileReader, Me.theMdlFileData)

		Me.theMdlFileData.theSectionFrameCount = 0
		Me.theMdlFileData.theModelCommandIsUsed = False
		Me.theMdlFileData.theProceduralBonesCommandIsUsed = False

		mdlFile.ReadMdlHeader00("MDL File Header 00")
		mdlFile.ReadMdlHeader01("MDL File Header 01")
		If Me.theMdlFileData.studioHeader2Offset > 0 Then
			mdlFile.ReadMdlHeader02("MDL File Header 02")
		End If

		' Read what WriteBoneInfo() writes.
		mdlFile.ReadBones()
		mdlFile.ReadBoneControllers()
		mdlFile.ReadAttachments()

		mdlFile.ReadHitboxSets()

		mdlFile.ReadBoneTableByName()

		' Read what WriteAnimations() writes.
		If Me.theMdlFileData.localAnimationCount > 0 Then
			Try
				mdlFile.ReadLocalAnimationDescs()
			Catch
			End Try
		End If

		' Read what WriteSequenceInfo() writes.
		If Me.theMdlFileData.localSequenceCount > 0 Then
			Try
				mdlFile.ReadSequenceDescs()
			Catch
			End Try
		End If
		mdlFile.ReadLocalNodeNames()
		mdlFile.ReadLocalNodes()

		' Read what WriteModel() writes.
		'Me.theCurrentFrameIndex = 0
		'NOTE: Read flex descs before body parts so that flexes (within body parts) can add info to flex descs.
		mdlFile.ReadFlexDescs()
		mdlFile.ReadBodyParts()
		mdlFile.ReadFlexControllers()
		'NOTE: This must be after flex descs are read so that flex desc usage can be saved in flex desc.
		mdlFile.ReadFlexRules()
		mdlFile.ReadIkChains()
		mdlFile.ReadIkLocks()
		mdlFile.ReadMouths()
		mdlFile.ReadPoseParamDescs()
		mdlFile.ReadModelGroups()
		'TODO: Me.ReadAnimBlocks()
		'TODO: Me.ReadAnimBlockName()

		' Read what WriteTextures() writes.
		mdlFile.ReadTexturePaths()
		'NOTE: ReadTextures must be after ReadTexturePaths(), so it can compare with the texture paths.
		mdlFile.ReadTextures()
		mdlFile.ReadSkinFamilies()

		' Read what WriteKeyValues() writes.
		mdlFile.ReadKeyValues()

		' Read what WriteBoneTransforms() writes.
		mdlFile.ReadBoneTransforms()
		mdlFile.ReadLinearBoneTable()

		'TODO: ReadLocalIkAutoPlayLocks()
		mdlFile.ReadFlexControllerUis()

		mdlFile.ReadFinalBytesAlignment()
		mdlFile.ReadUnknownValues(Me.theMdlFileData.theFileSeekLog)

		' Post-processing.
		mdlFile.CreateFlexFrameList()
		Common.ProcessTexturePaths(Me.theMdlFileData.theTexturePaths, Me.theMdlFileData.theTextures, Me.theMdlFileData.theModifiedTexturePaths, Me.theMdlFileData.theModifiedTextureFileNames)
	End Sub

	Protected Overrides Sub ReadMdlFileHeader_Internal()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData45()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim mdlFile As New SourceMdlFile45(Me.theInputFileReader, Me.theMdlFileData)

		mdlFile.ReadMdlHeader00("MDL File Header 00")
		mdlFile.ReadMdlHeader01("MDL File Header 01")
		If Me.theMdlFileData.studioHeader2Offset > 0 Then
			mdlFile.ReadMdlHeader02("MDL File Header 02")
		End If

		'If Me.theMdlFileData.fileSize <> Me.theMdlFileData.theActualFileSize Then
		'	status = StatusMessage.ErrorInvalidInternalMdlFileSize
		'End If
	End Sub

	Protected Overrides Sub ReadMdlFileForViewer_Internal()
		If Me.theMdlFileData Is Nothing Then
			Me.theMdlFileData = New SourceMdlFileData45()
			Me.theMdlFileDataGeneric = Me.theMdlFileData
		End If

		Dim mdlFile As New SourceMdlFile45(Me.theInputFileReader, Me.theMdlFileData)

		mdlFile.ReadMdlHeader00("MDL File Header 00")
		mdlFile.ReadMdlHeader01("MDL File Header 01")
		If Me.theMdlFileData.studioHeader2Offset > 0 Then
			mdlFile.ReadMdlHeader02("MDL File Header 02")
		End If

		mdlFile.ReadTexturePaths()
		mdlFile.ReadTextures()
	End Sub

	Protected Overrides Sub ReadPhyFile_Internal()
		If Me.thePhyFileData Is Nothing Then
			Me.thePhyFileData = New SourcePhyFileData45()
		End If

		Dim phyFile As New SourcePhyFile45(Me.theInputFileReader, Me.thePhyFileData)

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

	Protected Overrides Sub ReadVtxFile_Internal()
		If Me.theVtxFileData Is Nothing Then
			Me.theVtxFileData = New SourceVtxFileData45()
		End If

		Dim vtxFile As New SourceVtxFile45(Me.theInputFileReader, Me.theVtxFileData)

		vtxFile.ReadSourceVtxHeader()
		If Me.theVtxFileData.lodCount > 0 Then
			vtxFile.ReadSourceVtxBodyParts()
		End If
	End Sub

	Protected Overrides Sub ReadVvdFile_Internal()
		If Me.theVvdFileData Is Nothing Then
			Me.theVvdFileData = New SourceVvdFileData45()
		End If

		Dim vvdFile As New SourceVvdFile45(Me.theInputFileReader, Me.theVvdFileData)

		vvdFile.ReadSourceVvdHeader()
		vvdFile.ReadVertexes()
		vvdFile.ReadFixups()
	End Sub

	Protected Overrides Sub WriteQcFile()
		Dim qcFile As New SourceQcFile45(Me.theOutputFileTextWriter, Me.theQcPathFileName, Me.theMdlFileData, Me.theVtxFileData, Me.thePhyFileData, Me.theAniFileData, Me.theName)

		Try
			qcFile.WriteHeaderComment()

			qcFile.WriteModelNameCommand()

			qcFile.WriteStaticPropCommand()
			qcFile.WriteConstDirectionalLightCommand()

			If Me.theMdlFileData.theModelCommandIsUsed Then
				qcFile.WriteModelCommand()
				qcFile.WriteBodyGroupCommand(1)
			Else
				qcFile.WriteBodyGroupCommand(0)
			End If
			qcFile.WriteGroup("lod", AddressOf qcFile.WriteGroupLod, False, False)

			qcFile.WriteSurfacePropCommand()
			qcFile.WriteJointSurfacePropCommand()
			qcFile.WriteContentsCommand()
			qcFile.WriteJointContentsCommand()
			If TheApp.Settings.DecompileDebugInfoFilesIsChecked Then
				qcFile.WriteIllumPositionCommand()
			End If

			qcFile.WriteEyePositionCommand()
			qcFile.WriteMaxEyeDeflectionCommand()
			qcFile.WriteNoForcedFadeCommand()
			qcFile.WriteForcePhonemeCrossfadeCommand()

			qcFile.WriteAmbientBoostCommand()
			qcFile.WriteOpaqueCommand()
			qcFile.WriteObsoleteCommand()
			qcFile.WriteCdMaterialsCommand()
			qcFile.WriteTextureGroupCommand()
			If TheApp.Settings.DecompileDebugInfoFilesIsChecked Then
				qcFile.WriteTextureFileNameComments()
			End If

			qcFile.WriteAttachmentCommand()

			qcFile.WriteGroup("box", AddressOf qcFile.WriteGroupBox, True, False)

			qcFile.WriteControllerCommand()
			qcFile.WriteScreenAlignCommand()

			qcFile.WriteGroup("bone", AddressOf qcFile.WriteGroupBone, False, False)

			qcFile.WriteGroup("animation", AddressOf qcFile.WriteGroupAnimation, False, False)

			qcFile.WriteGroup("collision", AddressOf qcFile.WriteGroupCollision, False, False)

			qcFile.WriteKeyValues(Me.theMdlFileData.theKeyValuesText, "$KeyValues")
		Catch ex As Exception
			Dim debug As Integer = 4242
		Finally
		End Try
	End Sub

	Protected Overrides Function WriteMeshSmdFiles(ByVal modelOutputPath As String, ByVal lodStartIndex As Integer, ByVal lodStopIndex As Integer) As AppEnums.StatusMessage
		Dim status As AppEnums.StatusMessage = StatusMessage.Success

		Dim smdFileName As String
		Dim smdPathFileName As String
		Dim aBodyPart As SourceVtxBodyPart
		Dim aVtxModel As SourceVtxModel
		Dim aModel As SourceMdlModel
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

								smdFileName = SourceFileNamesModule.GetBodyGroupSmdFileName(bodyPartIndex, modelIndex, lodIndex, Me.theMdlFileData.theModelCommandIsUsed, Me.theName, Me.theMdlFileData.theBodyParts(bodyPartIndex).theModels(modelIndex).name, Me.theMdlFileData.theBodyParts.Count, Me.theMdlFileData.theBodyParts(bodyPartIndex).theModels.Count)
								smdPathFileName = Path.Combine(modelOutputPath, smdFileName)

								Me.NotifySourceModelProgress(ProgressOptions.WritingFileStarted, smdPathFileName)
								'NOTE: Check here in case writing is canceled in the above event.
								If Me.theWritingIsCanceled Then
									status = StatusMessage.Canceled
									Return status
								ElseIf Me.theWritingSingleFileIsCanceled Then
									Me.theWritingSingleFileIsCanceled = False
									Continue For
								End If

								Me.WriteMeshSmdFile(smdPathFileName, lodIndex, aVtxModel, aModel, bodyPartVertexIndexStart)

								Me.NotifySourceModelProgress(ProgressOptions.WritingFileFinished, smdPathFileName)
							Next

							bodyPartVertexIndexStart += aModel.vertexCount
						End If
					Next
				End If
			Next
		End If

		Return status
	End Function

	Protected Overrides Sub WriteMeshSmdFile(ByVal lodIndex As Integer, ByVal aVtxModel As SourceVtxModel, ByVal aModel As SourceMdlModel, ByVal bodyPartVertexIndexStart As Integer)
		Dim smdFile As New SourceSmdFile45(Me.theOutputFileTextWriter, Me.theMdlFileData, Me.theVvdFileData)

		Try
			smdFile.WriteHeaderComment()

			smdFile.WriteHeaderSection()
			smdFile.WriteNodesSection(lodIndex)
			smdFile.WriteSkeletonSection(lodIndex)
			smdFile.WriteTrianglesSection(aVtxModel, lodIndex, aModel, bodyPartVertexIndexStart)
		Catch ex As Exception
			Dim debug As Integer = 4242
		End Try
	End Sub

	Protected Overrides Sub WritePhysicsMeshSmdFile()
		Dim physicsMeshSmdFile As New SourceSmdFile45(Me.theOutputFileTextWriter, Me.theMdlFileData, Me.thePhyFileData)

		Try
			physicsMeshSmdFile.WriteHeaderComment()

			physicsMeshSmdFile.WriteHeaderSection()
			physicsMeshSmdFile.WriteNodesSection(-1)
			physicsMeshSmdFile.WriteSkeletonSection(-1)
			physicsMeshSmdFile.WriteTrianglesSectionForPhysics()
		Catch ex As Exception
			Dim debug As Integer = 4242
		Finally
		End Try
	End Sub

	Protected Overrides Sub WriteVrdFile()
		Dim vrdFile As New SourceVrdFile45(Me.theOutputFileTextWriter, Me.theMdlFileData)

		Try
			vrdFile.WriteHeaderComment()
			vrdFile.WriteCommands()
		Catch ex As Exception
			Dim debug As Integer = 4242
		Finally
		End Try
	End Sub

	Protected Overrides Sub WriteDeclareSequenceQciFile()
		Dim qciFile As New SourceQcFile45(Me.theOutputFileTextWriter, Me.theMdlFileData, Me.theName)

		Try
			qciFile.WriteHeaderComment()

			qciFile.WriteQciDeclareSequenceLines()
		Catch ex As Exception
			Dim debug As Integer = 4242
		End Try
	End Sub

	Protected Overrides Sub WriteBoneAnimationSmdFile(ByVal aSequenceDesc As SourceMdlSequenceDescBase, ByVal anAnimationDesc As SourceMdlAnimationDescBase)
		Dim smdFile As New SourceSmdFile45(Me.theOutputFileTextWriter, Me.theMdlFileData)

		Try
			smdFile.WriteHeaderComment()

			smdFile.WriteHeaderSection()
			smdFile.WriteNodesSection(-1)
			'If anAnimationDesc.animBlock > 0 AndAlso Me.theSourceEngineModel.MdlFileHeader.version >= 49 AndAlso Me.theSourceEngineModel.MdlFileHeader.version <> 2531 Then
			'	smdFile.WriteSkeletonSectionForAnimationAni_VERSION49(aSequenceDesc, anAnimationDesc)
			'Else
			'End If
			smdFile.WriteSkeletonSectionForAnimation(aSequenceDesc, anAnimationDesc)
		Catch ex As Exception
			Dim debug As Integer = 4242
		End Try
	End Sub

	Protected Overrides Sub WriteVertexAnimationVtaFile()
		Dim vertexAnimationVtaFile As New SourceVtaFile45(Me.theOutputFileTextWriter, Me.theMdlFileData, Me.theVvdFileData)

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
		Dim mdlFile As New SourceMdlFile45(Me.theOutputFileBinaryWriter, Me.theMdlFileData)

		mdlFile.WriteInternalMdlFileName(internalMdlFileName)
	End Sub

	Protected Overrides Sub WriteAniFileNameToMdlFile(ByVal internalAniFileName As String)
		Dim mdlFile As New SourceMdlFile45(Me.theOutputFileBinaryWriter, Me.theMdlFileData)

		mdlFile.WriteInternalAniFileName(internalAniFileName)
	End Sub

#End Region

#Region "Data"

	Private theAniFileData As SourceAniFileData45
	Private theMdlFileData As SourceMdlFileData45
	Private thePhyFileData As SourcePhyFileData45
	Private theVtxFileData As SourceVtxFileData45
	Private theVvdFileData As SourceVvdFileData45

#End Region

End Class