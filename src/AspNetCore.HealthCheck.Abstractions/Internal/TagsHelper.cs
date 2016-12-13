using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public static class TagsHelper
    {
        private static readonly IEnumerable<string> _defaultTags = new string[0];

        public static IEnumerable<string> DefaultTags => _defaultTags;

        public static IEnumerable<string> FromTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return _defaultTags;
            }

            return new[] { tag };
        }

        public static IEnumerable<string> FromTag(IEnumerable<string> tags)
        {
            if (tags == null)
            {
                return _defaultTags;
            }

            return tags;
        }
    }
}