using ApiKeyAttributeDemo.Security.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ApiKeyAttributeDemo.Security.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute() 
    : TypeFilterAttribute(typeof(ApiKeyAuthorizationFilter));