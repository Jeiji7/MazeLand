using System;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class CommandAttribute : Attribute
{
    public string Description { get; }

    public CommandAttribute(string description = "")
    {
        Description = description;
    }
}
