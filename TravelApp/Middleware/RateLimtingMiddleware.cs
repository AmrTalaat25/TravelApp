namespace WebApplicationAPI.Middleware
{
    public class RateLimtingMiddleware
    {
        private readonly RequestDelegate _next;
        private static int _counter = 0;
        private static DateTime _lasteRequestDate = DateTime.Now;
        public RateLimtingMiddleware( RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            _counter++;
            if(DateTime.Now.Subtract(_lasteRequestDate).Seconds > 10)
            {
                _counter = 1;
                _lasteRequestDate = DateTime.Now;
                await _next(context);
            }
            else
            {
                if (_counter >=3)
                {
                    _lasteRequestDate = DateTime.Now;
                    await context.Response.WriteAsync("Rate Limit Exceeded");

                }
                else
                {
                    _lasteRequestDate = DateTime.Now;
                    await _next(context);

                }

            }

        }

    }

}
