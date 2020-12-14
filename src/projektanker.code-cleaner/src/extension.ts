// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
import * as child from 'child_process';
import { fs } from './fs';
import * as path from 'path';

const messagePrefix = "Code Butler: ";

function showInformationMessage(message: string) {
	vscode.window.showInformationMessage(messagePrefix + message);
}

function showErrorMessage(message: string) {
	vscode.window.showErrorMessage(messagePrefix + message);
}

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {
	// Use the console to output diagnostic information (console.log) and errors (console.error)
	// This line of code will only be executed once when your extension is activated
	console.log('Congratulations, your extension "codeButler" is now active!');

	// The command has been defined in the package.json file
	// Now provide the implementation of the command with registerCommand
	// The commandId parameter must match the command field in package.json
	const disposable = vscode.commands.registerTextEditorCommand('extension.codeButler.reorganize', async (textEditor, edit, args) => {
		// The code you place here will be executed every time your command is executed


		const cleanerCliFile = 'CodeCleaner.Console.dll'
		const cleanerCli = path.resolve(__dirname, '..', 'publish', cleanerCliFile);

		if (!textEditor) {
			showErrorMessage('Editor not focused.');
		}
		else if (textEditor.document.languageId != 'csharp') {
			showErrorMessage('Language not supported. Currently supported language: C#');
		}
		else if (!(await fs.checkFileExists(cleanerCli))) {
			showErrorMessage(`${cleanerCliFile} not found.`);
		}
		else if (textEditor.document.lineCount <= 1) {
			showInformationMessage('Empty document. Nothing to reorganize.');
		}
		else {
			try {

				//const { stdout, stderr } = await execFileAsync(cleanerCli, [document.fileName, "-d"])
				const cleanerChild = child.spawn('dotnet', [cleanerCli], { stdio: ['pipe'] });

				let stdOutData = "";
				let stdErrData = "";
				cleanerChild.stdout.setEncoding('utf8');
				cleanerChild.stdout.on('data', (data) => {
					stdOutData += data;
				});

				cleanerChild.stderr.setEncoding('utf8');
				cleanerChild.stderr.on('data', (data) => {
					stdErrData += data;
				});

				cleanerChild.on('close', exitCode => {
					if (exitCode != 0) {
						showErrorMessage(stdErrData);
					} else {
						const firstLine = textEditor.document.lineAt(0);
						const lastLine = textEditor.document.lineAt(textEditor.document.lineCount - 1);

						textEditor.edit(editBuilder => {
							const range = new vscode.Range(
								firstLine.range.start,
								lastLine.range.end
							);
							editBuilder.replace(range, stdOutData);
							showInformationMessage('Reorganized ✔');
						})
					}
				})

				cleanerChild.stdin.end(textEditor.document.getText(), 'utf-8');

			} catch (error) {
				console.log(error)
				showErrorMessage(error);
			}
		}
	});

	context.subscriptions.push(disposable);
}
