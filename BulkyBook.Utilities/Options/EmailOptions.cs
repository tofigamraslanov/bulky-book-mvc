﻿namespace BulkyBook.Utilities.Options
{
    public class EmailOptions
    {
        public const string Email = "Email";

        public string SendGridKey { get; set; }
        public string SendGridUser { get; set; }
    }
}