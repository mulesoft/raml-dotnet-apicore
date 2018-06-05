using System.Linq;
using System.Net.Http.Headers;
using Raml.Common;
#if (PORTABLE || NETSTANDARD1_3 )
using System.Reflection;
#endif

namespace RAML.Api.Core
{
	public class ApiResponseHeader
	{
		public void SetProperties(HttpResponseHeaders headers)
		{
#if !(PORTABLE || NETSTANDARD1_3 )
            var properties = this.GetType().GetProperties().Where(p => p.GetValue(this) != null);
#else
            var properties = this.GetType().GetTypeInfo().DeclaredProperties.Where(p => p.GetValue(this) != null);
#endif
			foreach (var prop in properties.Where(prop => headers.Any(h => h.Key == prop.Name)))
			{
				prop.SetValue(this, headers.First(h => NetNamingMapper.GetPropertyName(h.Key) == prop.Name));
			}
		}
	}
}