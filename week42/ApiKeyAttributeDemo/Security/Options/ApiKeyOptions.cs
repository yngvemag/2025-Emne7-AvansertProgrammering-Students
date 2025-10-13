namespace ApiKeyAttributeDemo.Security.Options;


/*
 *  "ApiKeyOptions" : {
    "HeaderName" : "X-API-KEY",
    "Keys" : [
      {"Key" : "abc_123_test", "appId" : "demo-app", "scopes" : ["products:read"]},
      {"Key" : "skj_sdf-fds", "appId" : "prod-client", "scopes" : ["products:read", "products:write"]}
    ]
  }
 */

public class ApiKeyItem
{
    public string Key { get; set; } = null!;
    public string AppId { get; set; } = null!;
    public List<string> Scopes { get; set; } = [];
}

public class ApiKeyOptions
{
    public const string SectionName = "ApiKeyOptions";
    public string HeaderName { get; set; } = "X-API-KEY";
    public List<ApiKeyItem> Keys { get; set; } = [];
}