namespace manage_my_hairsaloon.ViewModels
{
    /// <summary>
    /// Parameters for the _DatePicker partial view.
    /// </summary>
    public class DatePickerViewModel
    {
        /// <summary>HTML input name (and key for the value in the submitted form).</summary>
        public string Name { get; init; } = "date";

        /// <summary>Pre-selected value. Use "yyyy-MM-dd" for date mode, "yyyy-MM-ddTHH:mm" for datetime mode.</summary>
        public string? Value { get; init; }

        /// <summary>Minimum selectable date in "yyyy-MM-dd" format.</summary>
        public string? Min { get; init; }

        /// <summary>Maximum selectable date in "yyyy-MM-dd" format.</summary>
        public string? Max { get; init; }

        /// <summary>"date" (default) or "datetime".</summary>
        public string Mode { get; init; } = "date";

        /// <summary>When true, automatically submits the closest form after a date is selected.</summary>
        public bool AutoSubmit { get; init; } = false;

        /// <summary>Marks the hidden input as required for browser validation.</summary>
        public bool Required { get; init; } = false;

        /// <summary>Optional label rendered above the trigger button.</summary>
        public string? Label { get; init; }

        /// <summary>
        /// Name of a global JS function to call after every commit (date selected / OK clicked).
        /// Receives the committed ISO value as the first argument.
        /// </summary>
        public string? OnCommit { get; init; }
    }
}
