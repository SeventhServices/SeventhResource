using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Seventh.Resource.Common.Utilities;

namespace Seventh.Resource.Services
{
    public class SortService
    {
        private readonly ResourceLocation _location;

        public SortService(ResourceLocation location)
        {
            _location = location;
        }

        private readonly char _separator = Path.DirectorySeparatorChar;
        private readonly char _fileExtensionSeparator = '.';
        private readonly char _defaultNameSeparator = '_';
        private readonly Regex _numRegex = new Regex(@"^\d+$");
        private readonly Regex _needClassSortRegex = new Regex(@"_+\d{4,}");

        public ValueTask<string> SortAsync(string fileName)
        {
            return new ValueTask<string>(ProvideExtendPath(
                new StringBuilder(_location.PathOption.AssetPath.SortedAssetPath), fileName));
        }

        public ValueTask<string> SortAsync(string fileName, int revision)
        {
            return SortAsync(RenameForRevSpec(fileName, revision));
        }

        public string RenameForRevSpec(string fileName, int revision)
        {
            foreach (var (regex, expect) in _location.SortOption.RevSpecRules)
            {
                if (regex.IsMatch(fileName) == expect)
                {
                    var insetIndex = fileName.IndexOf(_fileExtensionSeparator);
                    fileName = fileName.Insert(insetIndex == -1
                            ? fileName.Length
                            : insetIndex,
                       $"_r{revision}");
                    break;
                }
            }
            return fileName;
        }

        public string ProvideExtendPath(StringBuilder pathBuilder, string fileName)
        {
            pathBuilder.Append(_separator);
            var result = TryAsRuleSort(pathBuilder, fileName);
            if (result)
            {
                return pathBuilder.ToString();
            }

            if (_needClassSortRegex.IsMatch(fileName))
            {
                return AsClassSort(pathBuilder, fileName);
            }

            return fileName.IndexOf(_defaultNameSeparator) == -1
                ? AsTypeSort(pathBuilder, fileName)
                : AsFirstClassSort(pathBuilder, fileName);
        }


        private static ReadOnlySpan<char> RemoveIf(ReadOnlySpan<char> text, char splitChar)
        {
            var index = text.IndexOf(splitChar);
            return index != -1 ? text.Slice(0, index) : text;
        }

        private static ReadOnlySpan<char> TakeIf(ReadOnlySpan<char> text, char splitChar)
        {
            var index = text.IndexOf(splitChar);
            return index != -1 ? text.Slice(index + 1) : text;
        }

        public bool TryAsRuleSort(StringBuilder pathBuilder, string fileName)
        {
            foreach (var (regex, path) in _location.SortOption.ConsumeRules)
            {
                if (!regex.IsMatch(fileName))
                {
                    continue;
                }
                pathBuilder.Append(path.Replace(':', _separator));
                AppendFileName(pathBuilder, fileName);
                return true;
            }
            return false;
        }



        public string AsTypeSort(StringBuilder pathBuilder, string fileName)
        {
            var extension = TakeIf(fileName, _fileExtensionSeparator);
            var pathPart = new Span<char>(new char[extension.Length]);
            extension.ToLowerInvariant(pathPart);
            pathBuilder.Append(pathPart);
            pathBuilder.Append(_separator);
            AppendFileName(pathBuilder, fileName);
            var path = pathBuilder.ToString();
            return path;
        }

        public string AsFirstClassSort(StringBuilder pathBuilder, string fileName)
        {
            var firstClass = RemoveIf(fileName, _defaultNameSeparator);
            var pathPart = new Span<char>(new char[firstClass.Length]);
            firstClass.ToLowerInvariant(pathPart);
            pathBuilder.Append(pathPart);
            pathBuilder.Append(_separator);
            AppendFileName(pathBuilder, fileName);
            var path = pathBuilder.ToString();
            return path;
        }

        public string AsClassSort(StringBuilder pathBuilder, string fileName)
        {
            var nameOnly = RemoveIf(fileName, _fileExtensionSeparator).ToString();
            var fileNameParts = nameOnly.Split(_defaultNameSeparator);
            foreach (var part in fileNameParts)
            {
                if (!_numRegex.IsMatch(part))
                {
                    pathBuilder.Append(part.ToLowerInvariant());
                    pathBuilder.Append(_separator);
                    continue;
                }
                AppendFileName(pathBuilder, fileName);
                break;
            }
            var path = pathBuilder.ToString();
            return path;
        }


        private static string AppendFileName(StringBuilder pathBuilder, string fileName)
        {
            CommonUtil.CreateNonexistentDirectory(pathBuilder.ToString());
            pathBuilder.Append(fileName);
            return pathBuilder.ToString();
        }

    }
}