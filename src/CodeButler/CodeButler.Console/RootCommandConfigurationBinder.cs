using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

namespace CodeButler;

public class RootCommandConfigurationBinder : BinderBase<RootCommandConfiguration>
{
    private readonly Option<bool> _noSortMemberByAlphabet;
    private readonly Argument<FileInfo?> _file;

    public RootCommandConfigurationBinder(
        Option<bool> noSortMemberByAlphabet,
        Argument<FileInfo?> file
    )
    {
        _noSortMemberByAlphabet = noSortMemberByAlphabet;
        _file = file;
    }

    protected override RootCommandConfiguration GetBoundValue(BindingContext bindingContext)
    {
        var parseResult = bindingContext.ParseResult;

        var file = parseResult.GetValueForArgument(_file);
        var mode = file is null ? InputOutputMode.Console : InputOutputMode.File;

        return new RootCommandConfiguration
        {
            SortMemberByAlphabet = !bindingContext.ParseResult.GetValueForOption(
                _noSortMemberByAlphabet
            ),
            Mode = mode,
            File = file,
            Console = bindingContext.Console,
        };
    }
}
