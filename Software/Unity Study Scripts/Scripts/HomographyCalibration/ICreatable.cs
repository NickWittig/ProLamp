using System;

internal interface ICreatable
{
    public static ICreatable Create() => throw new NotImplementedException();

}