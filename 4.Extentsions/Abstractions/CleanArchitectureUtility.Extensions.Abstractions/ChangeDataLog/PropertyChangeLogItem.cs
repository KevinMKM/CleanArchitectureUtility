﻿namespace CleanArchitectureUtility.Extensions.Abstractions.ChangeDataLog;

public class PropertyChangeLogItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ChangeInterceptorItemId { get; set; }
    public string PropertyName { get; set; }
    public string Value { get; set; }
}