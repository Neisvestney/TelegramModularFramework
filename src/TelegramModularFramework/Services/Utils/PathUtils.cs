using System.Text;
using System.Text.RegularExpressions;

namespace TelegramModularFramework.Services.Utils;

public static class PathUtils
{
    public static IEnumerable<PathPart> ParsePath(string path)
    {
        return path
            .Split('/')
            .Skip(1) // Starts from '/'
            .Select(p => new
            {
                Match = Regex.Match(p, @"\{([^()]+)\}"),
                Path = p
            })
            .Select(p => new
            {
                p.Match,
                p.Path,
                Split = p.Match.Success ? p.Match.Value.Remove(0, 1).Remove(p.Match.Value.Length - 2, 1).Split(':') : null
            })
            .Select(p => new PathPart()
            {
                Dynamic = p.Match.Success,
                Name = p.Split?[0] ?? p.Path,
                Template = p.Match.Success ? (p.Split?.Length > 0 ? string.Join(":", p.Split.Skip(1)) : "*") : p.Path
            });
        
        // return Regex
        //     .Matches(path, @"\{([^()]+)\}")
        //     .Select(m => m.Value.Split(':'))
        //     .Select(m => new PathPart()
        //     {
        //         Name = m[0],
        //         Template = m.Length > 0 ? string.Join(':', m.Skip(1)) : "*"
        //     });
    }
    
    public static string PatternFromPath(string path)
    {
        // /sample/{data:*} => /sample/*

        return "/" + string.Join("/", ParsePath(path).Select(p => p.Template));
    }

    public static string InsertParametersIntoPath(string path, object? parameters)
    {
        if (parameters == null) return path;
        
        var parts = ParsePath(path);
        var dictionary = parameters.ToDictionary();
        var usedParams = new List<string>();
        var result = new StringBuilder();
        foreach (var part in parts)
        {
            result.Append("/");
            if (part.Dynamic)
            {
                if (!dictionary.TryGetValue(part.Name, out var value))
                    throw new ArgumentException($"{part.Name} was not present", nameof(parameters));

                usedParams.Add(part.Name);
                result.Append(value);
            }
            else
            {
                result.Append(part.Name);
            }
        }

        var queryParams = dictionary.Where(e => !usedParams.Contains(e.Key)).ToDictionary(x => x.Key, x => x.Value);
        result.Append(queryParams.ToQueryString());
        
        return result.ToString();
    }
}