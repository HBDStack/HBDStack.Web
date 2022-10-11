using System.Diagnostics;
using System.Net;
using System.Text;
using HBDStack.StatusGeneric;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Serialization;

namespace HBDStack.Web.GlobalException;

public static class TransformModelStateExtensions
{
    internal static IActionResult TransformModelState(ModelStateDictionary errorModelState, NamingStrategy namingStrategy)
    {
        var problemDetails = new ProblemDetails("One or more validation errors occurred.")
        {
            TraceId = Activity.Current?.RootId ?? Activity.Current?.TraceId.ToString() ?? string.Empty,
            Status = HttpStatusCode.BadRequest,
            ErrorDetails = new ProblemResultCollection()
        };

        foreach (var (s, value) in errorModelState)
        {
            var key = namingStrategy != null ? namingStrategy.GetPropertyName(s, false) : s;

            problemDetails.ErrorDetails.Add(
                new GenericValidationResult(value?.Errors?.FirstOrDefault()?.ErrorMessage, key));
        }

        return new BadRequestObjectResult(problemDetails);
    }

    private enum SeparatedCaseState
    {
        Start,
        Lower,
        Upper,
        NewWord,
        DotChar
    }

    //TODO: using this and migrate Newtonsoft.Json to System.Text.Json
    public static string ToSnakeCase(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        const char separator = '_';
        const char dotChar = '.';

        var sb = new StringBuilder();
        var state = SeparatedCaseState.Start;

        for (var i = 0; i < s.Length; i++)
        {
            switch (s[i])
            {
                case ' ':
                {
                    if (state != SeparatedCaseState.Start)
                    {
                        state = SeparatedCaseState.NewWord;
                    }

                    break;
                }
                case dotChar:
                    sb.Append(dotChar);
                    state = SeparatedCaseState.DotChar;
                    break;
                default:
                {
                    if (char.IsUpper(s[i]))
                    {
                        switch (state)
                        {
                            case SeparatedCaseState.Upper:
                                var hasNext = (i + 1 < s.Length);
                                if (i > 0 && hasNext)
                                {
                                    var nextChar = s[i + 1];
                                    if (!char.IsUpper(nextChar) && nextChar != separator)
                                    {
                                        sb.Append(separator);
                                    }
                                }

                                break;
                            case SeparatedCaseState.Lower:
                            case SeparatedCaseState.NewWord:
                                sb.Append(separator);
                                break;
                            case SeparatedCaseState.Start:
                                break;
                            case SeparatedCaseState.DotChar:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        var c = char.ToLowerInvariant(s[i]);

                        sb.Append(c);
                        state = SeparatedCaseState.Upper;
                    }
                    else if (s[i] == separator)
                    {
                        sb.Append(separator);
                        state = SeparatedCaseState.Start;
                    }
                    else
                    {
                        if (state == SeparatedCaseState.NewWord)
                        {
                            sb.Append(separator);
                        }

                        sb.Append(s[i]);
                        state = SeparatedCaseState.Lower;
                    }

                    break;
                }
            }
        }

        return sb.ToString();
    }
}