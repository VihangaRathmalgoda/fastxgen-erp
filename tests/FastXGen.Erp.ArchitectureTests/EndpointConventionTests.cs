using System.Reflection;
using FastXGen.Erp.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace FastXGen.Erp.ArchitectureTests;

/// <summary>
/// Every endpoint must state its access rule explicitly and live under the versioned route.
/// </summary>
public sealed class EndpointConventionTests
{
    private static readonly Type[] Controllers = typeof(ApiRoutes).Assembly.GetTypes()
        .Where(type => type.IsClass && !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
        .ToArray();

    [Fact]
    public void Every_action_declares_a_permission_policy_or_allow_anonymous()
    {
        var violations = Controllers
            .SelectMany(controller => controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .Where(action => action.GetCustomAttributes<HttpMethodAttribute>().Any())
            .Where(action => !HasAccessRule(action))
            .Select(action => $"{action.DeclaringType!.Name}.{action.Name}")
            .ToList();

        Assert.True(violations.Count == 0, "Actions without [Authorize(Policy = ...)] or [AllowAnonymous]: " + string.Join(", ", violations));
    }

    [Fact]
    public void Every_controller_route_is_versioned()
    {
        var violations = Controllers
            .Where(controller => controller.GetCustomAttribute<RouteAttribute>()?.Template.StartsWith(ApiRoutes.V1 + "/", StringComparison.Ordinal) != true)
            .Select(controller => controller.Name)
            .ToList();

        Assert.True(violations.Count == 0, $"Controllers not routed under '{ApiRoutes.V1}/': " + string.Join(", ", violations));
    }

    private static bool HasAccessRule(MethodInfo action)
    {
        if (action.GetCustomAttribute<AllowAnonymousAttribute>() is not null)
        {
            return true;
        }

        var policies = action.GetCustomAttributes<AuthorizeAttribute>()
            .Concat(action.DeclaringType!.GetCustomAttributes<AuthorizeAttribute>());

        return policies.Any(attribute => !string.IsNullOrWhiteSpace(attribute.Policy));
    }
}
