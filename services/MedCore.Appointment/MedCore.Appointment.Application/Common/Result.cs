using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Appointment.Application.Common
{
    public class Result<T>
    {
        public T? Value { get; set; }
        public string? Error { get; set; }
        public bool IsSuccess => Error is null;

        public static Result<T> Ok(T value) => new() { Value = value };
        public static Result<T> Fail(string err) => new() { Error = err };
    }
}
