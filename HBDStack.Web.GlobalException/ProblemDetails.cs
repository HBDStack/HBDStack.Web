using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using HBDStack.StatusGeneric;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace HBDStack.Web.GlobalException;

public class ProblemDetails
{
    private HttpStatusCode _status;

    public ProblemDetails(string? errorMessage = null)
    {
        Status = HttpStatusCode.BadRequest;
        ErrorMessage = errorMessage;
    }

    [JsonConverter(typeof(StatusCodeEnumJsonConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public HttpStatusCode Status
    {
        get => _status;
        set
        {
            _status = value;
            ErrorCode = _status.ToString();
        }
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TraceId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorCode { get; private set; }

    public string? ErrorMessage { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ProblemResultCollection ErrorDetails { get; set; } = new();
}

public class ProblemResult
{
    public ProblemResult()
    {
    }

    public ProblemResult(string errorMessage) : this(null, errorMessage)
    {
    }

    public ProblemResult(string? errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorCode { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IDictionary<string, object> References { get; set; } = new Dictionary<string, object>();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ErrorMessage { get; set; }
}

public class ProblemResultCollection : Dictionary<string, ICollection<ProblemResult>>
{
    public ProblemResultCollection()
    {
    }

    public ProblemResultCollection(IEnumerable<GenericValidationResult> errors) => AddRange(errors);

    public ProblemResultCollection(IEnumerable<ValidationResult> errors) => AddRange(errors);

    public void Add(GenericValidationResult result)
    {
        var rs = result.ToProblemResult();

        if (result.MemberNames?.Any() == true)
            foreach (var memberName in result.MemberNames)
            {
                if (ContainsKey(memberName))
                    base[memberName].Add(rs);
                else Add(memberName, new List<ProblemResult>(new[] { rs }));
            }
        else
        {
            if (ContainsKey(string.Empty))
                base[string.Empty].Add(rs);
            else Add(string.Empty, new List<ProblemResult>(new[] { rs }));
        }
    }

    public void AddRange(IEnumerable<GenericValidationResult> results)
    {
        foreach (var rs in results) Add(rs);
    }

    public void Add(ValidationResult result) => Add(new GenericValidationResult(result));

    public void AddRange(IEnumerable<ValidationResult> results)
    {
        foreach (var rs in results) Add(rs);
    }

    public void Add(string memberName, params string[] errorMessages)
    {
        foreach (var message in errorMessages)
            Add(new ProblemResult(message));
    }

    public void Add(ProblemResult problem)
    {
        var code = problem.ErrorCode ?? string.Empty;
        
        if (!ContainsKey(code))
            Add(code, new List<ProblemResult>());
        
        this[code].Add(problem);
    }
}

public sealed class StatusCodeEnumJsonConverter : JsonConverter<HttpStatusCode>
{
    public override HttpStatusCode Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        HttpStatusCode result;
        if (reader.TokenType == JsonTokenType.Number)
            Enum.TryParse(reader.GetInt32().ToString(), out result);
        else
            Enum.TryParse(reader.GetString(), out result);

        return result;
    }

    public override void Write(Utf8JsonWriter writer, HttpStatusCode value, JsonSerializerOptions options)
        => writer.WriteNumberValue((int)value);
}