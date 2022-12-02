namespace WebApplication1
{
	public class CompanyMiddleware
	{
		readonly RequestDelegate next;
		private static UID uid = new UID();
		public CompanyMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext ctx)
		{
			var resp = ctx.Response;
			var req = ctx.Request;

			var method = req.Method;

			var CompanyName = req.Query["name"];
			var TypeOfCompany = req.Query["type"];

			if (method == "GET")
			{
				if (req.Path == "/Company")
				{
					if (string.IsNullOrWhiteSpace(CompanyName) && string.IsNullOrWhiteSpace(TypeOfCompany))
					{
						resp.StatusCode = 403;
						return;
					}
					var company = new Company(CompanyName, uid.Id, TypeOfCompany);
					await resp.WriteAsync(company.ToString());
				}
				else
				{
					resp.StatusCode = 404;
				}
			}
			else
			{
				resp.StatusCode = 403;
			}
		}
	}

	public class UID {
		private int id = 0;
		public UID(){}
		public int Id { 
			get {
				return id++;
			}
		}
	}

	public class Company
	{
		public Company(string name, int uniqueId, string typeOfCompany)
		{
			this.Name = name;
			this.UniqueId = uniqueId;
			this.TypeOfCompany = typeOfCompany;
		}

        public override string ToString()
        {
            return $"UID: {UniqueId}";
        }

        public string Name { get; set; }

		public int UniqueId { get; set; }

		public string TypeOfCompany { get; set; }
	}
}
