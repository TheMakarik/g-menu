using System;
using System.Collections.Generic;
using System.IO;

namespace GMenu.DesktopFiles.Core;

public interface IDesktopFilesReader<out T> where T : class
{
    public T ReadOne(string pathToFile);
    public IReadOnlyCollection<T> ReadAll(string pathToDirectory);
    public IEnumerable<T> EnumerateAll(string pathToDirectory);
    public IReadOnlyCollection<T> ReadAll(params Span<string> pathTsoDirectory);
    public IEnumerable<T> EnumerateAll(params Span<string> pathsToDirectory);
    public IReadOnlyCollection<T> ReadAll(EnumerationOptions enumerationOptions, params Span<string> pathTsoDirectory);
    public IEnumerable<T> EnumerateAll(EnumerationOptions enumerationOptions, params Span<string> pathsToDirectory);
}