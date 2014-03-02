namespace sticky_notes_wp8.Services
{
    using Newtonsoft.Json.Serialization;
    using System.Text.RegularExpressions;

    public class CamelCaseToUnderscorePropertyNamesContractResolver : DefaultContractResolver
    {
        public CamelCaseToUnderscorePropertyNamesContractResolver() : base(false) { }

        protected override string ResolvePropertyName(string propertyName)
        {
            return Regex.Replace(propertyName, "([A-Z])", "_$1", RegexOptions.Compiled).Trim(new char[]{'_'});
        }
    }
}
