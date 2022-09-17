using System.Text.Json.Nodes;

namespace HBD.Web.Auth.Tests;

public static class TokenExtensions
{
    public static string UpdateWith(this string token, string key, string value)
    {
        var splits = token.Split(".");
        var header = splits[0];
        var body = splits[1];
        var footer = splits[2];

        //Update
        var decryptedBody = Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Decode(body);
        var root = JsonNode.Parse(decryptedBody)!.AsObject();
        root[key]= value;
        
        body =  Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Encode(root.ToJsonString());
        return $"{header}.{body}.{footer}";
    }
}