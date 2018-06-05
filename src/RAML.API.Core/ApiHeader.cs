using System.Collections.Generic;
using System.Linq;
#if (PORTABLE || NETSTANDARD1_3 )
using System.Reflection;
#endif

namespace RAML.Api.Core
{
	public class ApiHeader
	{
		public IDictionary<string, string> Headers
		{
			get
			{
#if !(PORTABLE || NETSTANDARD1_3 )
                var properties = this.GetType().GetProperties().Where(p => p.Name != "Headers" && p.GetValue(this) != null);
#else
                var properties = this.GetType().GetTypeInfo().DeclaredProperties.Where(p => p.Name != "Headers" && p.GetValue(this) != null);
#endif
				return properties.ToDictionary(prop => prop.Name, prop => prop.GetValue(this).ToString());
			}
		}
	}
}