using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server.Validation
{
    class ValidationResult
    {
        public bool IsValid { get; }
        public IEnumerable<string> Errors { get; set; }

        public ValidationResult(IEnumerable<string> errors)
        {
            var errorArray = errors.ToArray();
            IsValid = errorArray.Length == 0;
            Errors = errorArray;                
        }
        public ValidationResult(string result) : this(new[] { result })
        {
        }
        public ValidationResult(IEnumerable<ValidationResult> results) : this(results.SelectMany(s=> s.Errors))
        {
        }

        public static ValidationResult CreateValid() => new ValidationResult(Enumerable.Empty<string>());
    }
}
