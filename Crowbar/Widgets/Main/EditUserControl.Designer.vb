<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EditUserControl
	Inherits System.Windows.Forms.UserControl

	'UserControl overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.ApplyRightHandFixCheckBox = New System.Windows.Forms.CheckBox
		Me.Panel1 = New System.Windows.Forms.Panel
		Me.UseInCompileButton = New System.Windows.Forms.Button
		Me.GotoQcButton = New System.Windows.Forms.Button
		Me.Label6 = New System.Windows.Forms.Label
		Me.QcPathFileNameTextBox = New System.Windows.Forms.TextBox
		Me.BrowseForQcPathFolderOrFileNameButton = New System.Windows.Forms.Button
		Me.Panel1.SuspendLayout()
		Me.SuspendLayout()
		'
		'ApplyRightHandFixCheckBox
		'
		Me.ApplyRightHandFixCheckBox.AutoSize = True
		Me.ApplyRightHandFixCheckBox.Location = New System.Drawing.Point(3, 31)
		Me.ApplyRightHandFixCheckBox.Name = "ApplyRightHandFixCheckBox"
		Me.ApplyRightHandFixCheckBox.Size = New System.Drawing.Size(263, 17)
		Me.ApplyRightHandFixCheckBox.TabIndex = 15
		Me.ApplyRightHandFixCheckBox.Text = "Apply ""Right-Hand Fix"" (only for survivors in L4D2)"
		Me.ApplyRightHandFixCheckBox.UseVisualStyleBackColor = True
		'
		'Panel1
		'
		Me.Panel1.Controls.Add(Me.GotoQcButton)
		Me.Panel1.Controls.Add(Me.Label6)
		Me.Panel1.Controls.Add(Me.QcPathFileNameTextBox)
		Me.Panel1.Controls.Add(Me.BrowseForQcPathFolderOrFileNameButton)
		Me.Panel1.Controls.Add(Me.UseInCompileButton)
		Me.Panel1.Controls.Add(Me.ApplyRightHandFixCheckBox)
		Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Panel1.Location = New System.Drawing.Point(0, 0)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(784, 547)
		Me.Panel1.TabIndex = 16
		'
		'UseInCompileButton
		'
		Me.UseInCompileButton.Enabled = False
		Me.UseInCompileButton.Location = New System.Drawing.Point(3, 54)
		Me.UseInCompileButton.Name = "UseInCompileButton"
		Me.UseInCompileButton.Size = New System.Drawing.Size(90, 23)
		Me.UseInCompileButton.TabIndex = 25
		Me.UseInCompileButton.Text = "Use in Compile"
		Me.UseInCompileButton.UseVisualStyleBackColor = True
		'
		'GotoQcButton
		'
		Me.GotoQcButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.GotoQcButton.Location = New System.Drawing.Point(741, 3)
		Me.GotoQcButton.Name = "GotoQcButton"
		Me.GotoQcButton.Size = New System.Drawing.Size(40, 23)
		Me.GotoQcButton.TabIndex = 29
		Me.GotoQcButton.Text = "Goto"
		Me.GotoQcButton.UseVisualStyleBackColor = True
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(3, 8)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(82, 13)
		Me.Label6.TabIndex = 26
		Me.Label6.Text = "QC file or folder:"
		'
		'QcPathFileNameTextBox
		'
		Me.QcPathFileNameTextBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
					Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.QcPathFileNameTextBox.Location = New System.Drawing.Point(91, 5)
		Me.QcPathFileNameTextBox.Name = "QcPathFileNameTextBox"
		Me.QcPathFileNameTextBox.Size = New System.Drawing.Size(563, 20)
		Me.QcPathFileNameTextBox.TabIndex = 27
		'
		'BrowseForQcPathFolderOrFileNameButton
		'
		Me.BrowseForQcPathFolderOrFileNameButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.BrowseForQcPathFolderOrFileNameButton.Location = New System.Drawing.Point(660, 3)
		Me.BrowseForQcPathFolderOrFileNameButton.Name = "BrowseForQcPathFolderOrFileNameButton"
		Me.BrowseForQcPathFolderOrFileNameButton.Size = New System.Drawing.Size(75, 23)
		Me.BrowseForQcPathFolderOrFileNameButton.TabIndex = 28
		Me.BrowseForQcPathFolderOrFileNameButton.Text = "Browse..."
		Me.BrowseForQcPathFolderOrFileNameButton.UseVisualStyleBackColor = True
		'
		'EditUserControl
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Controls.Add(Me.Panel1)
		Me.Name = "EditUserControl"
		Me.Size = New System.Drawing.Size(784, 547)
		Me.Panel1.ResumeLayout(False)
		Me.Panel1.PerformLayout()
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents ApplyRightHandFixCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
	Friend WithEvents UseInCompileButton As System.Windows.Forms.Button
	Friend WithEvents GotoQcButton As System.Windows.Forms.Button
	Friend WithEvents Label6 As System.Windows.Forms.Label
	Friend WithEvents QcPathFileNameTextBox As System.Windows.Forms.TextBox
	Friend WithEvents BrowseForQcPathFolderOrFileNameButton As System.Windows.Forms.Button

End Class
