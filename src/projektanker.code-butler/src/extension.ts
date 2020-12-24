// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
import * as child from 'child_process';
import { fs } from './fs';
import * as path from 'path';

// this method is called when your extension is activated
// your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {
	// Use the console to output diagnostic information (console.log) and errors (console.error)
	// This line of code will only be executed once when your extension is activated
	console.log('Congratulations, your extension "codeButler" is now active!');

	// The command has been defined in the package.json file
	// Now provide the implementation of the command with registerCommand
	// The commandId parameter must match the command field in package.json
	const disposable = vscode.commands.registerTextEditorCommand('extension.codeButler.cleanup', commandHandler);

	context.subscriptions.push(disposable);
}

const messagePrefix = "Code Butler: ";

async function commandHandler(textEditor: vscode.TextEditor, _: vscode.TextEditorEdit, ...args: any[]) {
	// The code you place here will be executed every time your command is executed
	try {
		const cleanerCliFile = 'CodeButler.Console.dll'
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
			showInformationMessage('Empty document. Nothing to cleanup.');
		}
		else {

			const result = await executeCli('dotnet', [cleanerCli], textEditor.document.getText());
			await replaceContent(textEditor, result);
			showInformationMessage('Done ✔');

		}
	} catch (error) {
		console.log(error)
		showErrorMessage(error);
	}
}

function replaceContent(textEditor: vscode.TextEditor, content: string): Promise<void> {
	const firstLine = textEditor.document.lineAt(0);
	const lastLine = textEditor.document.lineAt(textEditor.document.lineCount - 1);

	const range = new vscode.Range(
		firstLine.range.start,
		lastLine.range.end
	);

	return new Promise((resolve) => {
		textEditor.edit(edit => {
			edit.replace(range, content);
			resolve();
		})
	})
}

function executeCli(command: string, args: readonly string[], input: string): Promise<string> {

	const cli = child.spawn(command, args, { stdio: ['pipe'] });

	let stdOutData = "";
	let stdErrData = "";
	cli.stdout.setEncoding('utf8');
	cli.stdout.on('data', (data) => {
		stdOutData += data;
	});

	cli.stderr.setEncoding('utf8');
	cli.stderr.on('data', (data) => {
		stdErrData += data;
	});

	let promise = new Promise<string>((resolve, reject) => {
		cli.on('close', exitCode => {
			if (exitCode != 0) {
				reject(stdErrData);
			}
			else {
				resolve(stdOutData);
			}
		});
	})

	cli.stdin.end(input, 'utf-8');
	return promise;
}

function showInformationMessage(message: string) {
	vscode.window.showInformationMessage(messagePrefix + message);
}

function showErrorMessage(message: string) {
	vscode.window.showErrorMessage(messagePrefix + message);
}



