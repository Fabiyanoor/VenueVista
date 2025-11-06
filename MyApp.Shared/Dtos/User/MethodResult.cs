using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Shared.Dtos.User;

// A record type to represent the result of a method that 
// does NOT return data, only success/failure info.
public record MethodResult(bool IsSuccess, string? Error)
{
    // Factory method that creates a successful result
    public static MethodResult Ok() => new(true, null);

    // Factory method that creates a failed result with an error message
    public static MethodResult Fail(string error) => new(false, error);
}

// A generic record type to represent the result of a method that
// returns data along with success/failure info.
public record MethodResult<TData>(bool IsSuccess, TData Data, string? Error)
{
    // Factory method that creates a successful result with data
    public static MethodResult<TData> Ok(TData data) => new(true, data, null);

    // Factory method that creates a failed result with an error message
    // and sets Data to default (e.g., null for reference types, 0 for int, etc.)
    public static MethodResult<TData> Fail(string error) => new(false, default!, error);
}
