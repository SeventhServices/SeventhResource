using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Seventh.Resource.Common.Extensions;

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
        private readonly Regex _needClassSortRegex = new Regex(@"\d{4,}");


        public ValueTask<string> SortAsync(string fileName)
        {
            return new ValueTask<string>(ProvideExtendPath(
                new StringBuilder(_location.PathOption.AssetPath.SortedAssetPath),fileName));
        }

        public ValueTask<string> SortAsync(string fileName, int revision)
        {
            return SortAsync(RenameForRevSpec(fileName,revision));
        }

        public string RenameForRevSpec(string fileName,int revision)
        {
            foreach (var (regex,expect) in _location.SortOption.RevSpecRules)
            {
                if (regex.IsMatch(fileName) == expect)
                {
                     fileName = fileName.Insert(fileName.IndexOf(_fileExtensionSeparator),
                        $"_r{revision}");
                }
            }
            return fileName;
        }

        public string ProvideExtendPath(StringBuilder pathBuilder,string fileName)
        {
            pathBuilder.Append(_separator);
            var path = TryAsRuleSort(fileName);
            if (path != null)
            {
                return path;
            }

            if (_needClassSortRegex.IsMatch(fileName))
            {
                return AsClassSort(pathBuilder, fileName);
            }

            return fileName.IndexOf(_defaultNameSeparator) == -1 
                ? AsTypeSort(pathBuilder, fileName)
                : AsFirstClassSort(pathBuilder, fileName);
        }

        private static ReadOnlySpan<char> RemoveIf(ReadOnlySpan<char> text ,char splitChar)
        {
            var index = text.IndexOf(splitChar);
            return index != -1 ? text.Slice(0,index) : text;
        }

        private static ReadOnlySpan<char> TakeIf(ReadOnlySpan<char> text ,char splitChar)
        {
            var index = text.IndexOf(splitChar);
            return index != -1 ? text.Slice(index) : text;
        }

        public string TryAsRuleSort(string fileName)
        {
            var rules = _location.SortOption.Rules;
            return null;
        }

        public string AsTypeSort(StringBuilder pathBuilder,string fileName)
        {
            var type = Path.GetExtension(fileName);
            pathBuilder.Append(type);
            pathBuilder.Append(_separator);
            pathBuilder.Append(fileName);
            var path = pathBuilder.ToString();
            return path;
        }

        public string AsFirstClassSort(StringBuilder pathBuilder, string fileName)
        {
            var firstClass = RemoveIf(fileName,_defaultNameSeparator);
            pathBuilder.Append(firstClass);
            pathBuilder.Append(_separator);
            pathBuilder.Append(fileName);
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
                    pathBuilder.Append(part);
                    pathBuilder.Append(_separator);
                    continue;
                }
                pathBuilder.Append(fileName);
                break;
            }
            var path = pathBuilder.ToString();
            return path;
        }

    }
}