// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
import * as child from 'child_process';
import { fs } from './fs';
import * as path from 'path';

interface CodeButlerConfiguration {
    cleanupOnSave: boolean;
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
    const disposable = vscode.commands.registerTextEditorCommand('extension.codeButler.cleanup', commandHandler);

    context.subscriptions.push(disposable);

    // get extension configuration
    let configuration = getConfiguration();
    vscode.workspace.onDidChangeConfiguration((e: vscode.ConfigurationChangeEvent) => {
        configuration = getConfiguration();
    });

    // perform cleanup on save
    vscode.workspace.onWillSaveTextDocument((e: vscode.TextDocumentWillSaveEvent) => {
        if (e.reason !== vscode.TextDocumentSaveReason.Manual ||
            !configuration.cleanupOnSave ||
            !e.document ||
            e.document.languageId !== "csharp") {
            return;
        }

        e.waitUntil(runCleanup(e.document, true));
    });
}

const messagePrefix = "Code Butler: ";

async function commandHandler(textEditor: vscode.TextEditor, _: vscode.TextEditorEdit, ...args: any[]) {
    // The code you place here will be executed every time your command is executed
    await runCleanup(textEditor.document, false);
}

async function runCleanup(document: vscode.TextDocument, silent: boolean) {
    try {
        const cleanerCliFile = 'CodeButler.Console.dll'
        const cleanerCli = path.resolve(__dirname, '..', 'publish', cleanerCliFile);

        if (!document) {
            showErrorMessage('No document specified.');
        }
        else if (document.languageId != 'csharp') {
            showErrorMessage('Only C# files are supported');
        }
        else if (!(await fs.checkFileExists(cleanerCli))) {
            showErrorMessage(`${cleanerCliFile} not found.`);
        }
        else if (document.lineCount <= 1 && !silent) {
            showInformationMessage('Empty document. Nothing to cleanup.');
        }
        else {
            const result = await executeCli('dotnet', [cleanerCli], document.getText());
            await replaceContent(document, result);
            const formatResult = await vscode.commands.executeCommand('editor.action.formatDocument');
            if (!silent) {
                showInformationMessage('Done ✔');
            }
        }
    } catch (error) {
        console.log(error)
        showErrorMessage(error);
    }
}

function replaceContent(document: vscode.TextDocument, content: string): Thenable<boolean> {
    const firstLine = document.lineAt(0);
    const lastLine = document.lineAt(document.lineCount - 1);

    const range = new vscode.Range(
        firstLine.range.start,
        lastLine.range.end
    );

    const edit = new vscode.WorkspaceEdit();
    edit.replace(document.uri, range, content);
    return vscode.workspace.applyEdit(edit);
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

function getConfiguration(): CodeButlerConfiguration {
    return vscode.workspace.getConfiguration("codeButler") as any;
}
