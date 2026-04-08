using Microsoft.Extensions.Options;

namespace Sufficit.Gateway.ReceitaNet.IntegrationTests;

internal sealed class FixedOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
    where TOptions : class
{
    public FixedOptionsMonitor(TOptions currentValue)
    {
        CurrentValue = currentValue;
    }

    public TOptions CurrentValue { get; }

    public TOptions Get(string? name) => CurrentValue;

    public IDisposable? OnChange(Action<TOptions, string?> listener) => null;
}